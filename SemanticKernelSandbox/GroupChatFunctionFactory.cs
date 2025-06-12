using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelSandbox;

public static class GroupChatFunctionFactory
{
    private static readonly OpenAIPromptExecutionSettings JsonSettings = new()
    {
        ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonObjectFormat()
    };
    
    public static KernelFunction CreateTerminationFunction()
    {
        var terminationFunctionInstructions =
            """
            Determine if the user's request has been completed successfully. Provide the reason of your decision as well.

            Respond in JSON format.  The JSON schema can include only:
            {
                "isApproved": "bool (true if the user request has been successfully completed, false otherwise)",
                "reason": "string (the reason for your determination)"
            }

            History:
            {{$history}}
            """;
        
        return KernelFunctionFactory.CreateFromPrompt(terminationFunctionInstructions, JsonSettings);
    }

    public static KernelFunction CreateSelectionFunction()
    {
        var selectionFunctionInstructions =
            """
               Determine which participant takes the next turn in a conversation based on the the most recent participant. 
               State only the name of the participant to take the next turn. 
               No participant should take more than one turn in a row.

               Choose only from these participants in the following order:
               - SupervisorAgent
               - SchedulingAgent
               - PricingAgent
               - BillingAgent
               - QuoteAgent

               Always follow these rules when selecting the next participant:        
               - The SupervisorAgent always takes the first turn.
               - If request is related to scheduling, the SchedulingAgent should take the next turn.
               - If request is related to pricing, the PricingAgent should take the next turn.
               - If request is related to quotes, the QuoteAgent should take the next turn.

               Based upon the feedback of SupervisorAgent's feedback, select the appropriate agent for the response.

               Respond in JSON format.  The JSON schema can include only:
               {
                   "name": "string (the name of the agent selected for the next turn)",
                   "reason": "string (the reason for the participant was selected)"
               }

               History:
               {{$history}}
               """;
        
        return KernelFunctionFactory.CreateFromPrompt(selectionFunctionInstructions, JsonSettings);
    }
}