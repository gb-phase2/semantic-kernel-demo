using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.SemanticKernel.Extensions;
using SemanticKernelSandbox;
using SemanticKernelSandbox.Configuration;
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");
    
var config = builder.Build();
var keys = new ApiKeysConfig();
config.GetSection("keys").Bind(keys);

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatCompletion("gpt-4o", keys.OpenAi, serviceId: "gpt-4-service");

var kernel = kernelBuilder.Build();

var schedulingAgent = OfficeAgentFactory.CreateSchedulingAgent(kernel);
var billingAgent = OfficeAgentFactory.CreateBillingAgent(kernel);
var pricingAgent = OfficeAgentFactory.CreatePricingAgent(kernel);
var supervisorAgent = OfficeAgentFactory.CreateSupervisorAgent(kernel);
var quoteAgent = OfficeAgentFactory.CreateQuoteAgent(kernel);

// Limit history used for selection and termination to the most recent message.
var strategyReducer = new ChatHistoryTruncationReducer(10);

var chat = new AgentGroupChat(supervisorAgent, schedulingAgent, billingAgent, pricingAgent, quoteAgent)
{
    ExecutionSettings = new AgentGroupChatSettings
    {
        SelectionStrategy =
            new KernelFunctionSelectionStrategy(GroupChatFunctionFactory.CreateSelectionFunction(), kernel)
            {
                // Always start with the supervisor agent
                InitialAgent = supervisorAgent,
                // Returns the entire result value as a string.
                ResultParser = (result) =>
                {
                    var jsonResult = JsonResultTranslator.Translate<AgentSelectionResult>(result.GetValue<string>());
                    var agentName = string.IsNullOrWhiteSpace(jsonResult?.name) ? null : jsonResult.name;
                    agentName ??= supervisorAgent.Name;
                    return agentName!;
                },
                // The prompt variable name for the history argument.
                HistoryVariableName = "history",
                // Save tokens by not including the entire history in the prompt
                HistoryReducer = strategyReducer
            },
        TerminationStrategy =
            new KernelFunctionTerminationStrategy(GroupChatFunctionFactory.CreateTerminationFunction(), kernel)
            {
                // Only the concierge agent may approve.
                Agents = [supervisorAgent],
                // Result parser to determine if the response is "yes"
                ResultParser =
                    (result) =>
                    {
                        var jsonResult = JsonResultTranslator.Translate<OuterTerminationResult>(result.GetValue<string>());
                        return jsonResult?.isApproved ?? false;
                    },
                // The prompt variable name for the history argument.
                HistoryVariableName = "history",
                // Limit total number of turns
                MaximumIterations = 20,
                // Save tokens by not including the entire history in the prompt
                HistoryReducer = strategyReducer
            }
    }
};

// Invoke chat and display messages.
var input = new ChatMessageContent(AuthorRole.User, 
    "I need to get a quote for a new project to build a new website. " +
           "The website should have a modern design, be mobile-friendly, and include an e-commerce section. " +
           "Please provide a detailed quote including estimated costs and timelines and schedule the work. " +
           "Also, I need to know the available time slots for the project.");

chat.AddChatMessage(input);

await foreach (var response in chat.InvokeAsync())
{
    // Include ChatMessageContent.AuthorName in output, if present.
    var authorExpression = response.Role == AuthorRole.User ? string.Empty : $" - {response.AuthorName ?? "*"}";
    // Include TextContent (via ChatMessageContent.Content), if present.
    var contentExpression = string.IsNullOrWhiteSpace(response.Content) ? string.Empty : response.Content;
    var isCode = response.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false;
    var codeMarker = isCode ? "\n  [CODE]\n" : " ";
    Console.WriteLine($"\n# {response.Role}{authorExpression}:{codeMarker}{contentExpression}");
}

Console.WriteLine($"\n[IS COMPLETED: {chat.IsComplete}]");