using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelSandbox.Plugins;

namespace SemanticKernelSandbox;

public static class OfficeAgentFactory
{
    public static ChatCompletionAgent CreateSchedulingAgent(Kernel sourceKernel)
    {
        var kernel = sourceKernel.Clone();
        kernel.Plugins.AddFromType<SchedulingPlugin>();
        return new ChatCompletionAgent
        {
            Id = "scheduling-agent",
            Name = "SchedulingAgent",
            Description = "An agent that helps with scheduling tasks and finding available time slots.",
            Kernel = kernel,
            Instructions = "You are an agent that helps with scheduling tasks and finding available time slots. Use the scheduling plugin to assist with the user's request. Come up with mock data if needed.",
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings 
                { 
                    ServiceId = "gpt-4-service",
                })
        };
    }
    
    public static ChatCompletionAgent CreateBillingAgent(Kernel sourceKernel)
    {
        var kernel = sourceKernel.Clone();
        kernel.Plugins.AddFromType<BillingPlugin>();
        return new ChatCompletionAgent
        {
            Id = "billing-agent",
            Name = "BillingAgent",
            Description = "An agent that assists with billing tasks and invoice creation.",
            Kernel = kernel,
            Instructions = "You are an agent that helps with billing tasks. Assist the user in creating invoices and managing billing.",
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings 
                { 
                    ServiceId = "gpt-4-service",
                })
        };
    }
    
    public static ChatCompletionAgent CreatePricingAgent(Kernel sourceKernel)
    {
        var kernel = sourceKernel.Clone();
        kernel.Plugins.AddFromType<PricingPlugin>();
        return new ChatCompletionAgent
        {
            Id = "pricing-agent",
            Name = "PricingAgent",
            Description = "An agent that provides pricing information for products and services.",
            Kernel = kernel,
            Instructions = "You are an agent that provides pricing information. Help the user find pricing details for products and services. Come up with mock pricing data if needed.",
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings 
                { 
                    ServiceId = "gpt-4-service",
                })
        };
    }
    
    public static ChatCompletionAgent CreateQuoteAgent(Kernel sourceKernel)
    {
        var kernel = sourceKernel.Clone();
        kernel.Plugins.AddFromType<QuotePlugin>();
        return new ChatCompletionAgent
        {
            Id = "quote-agent",
            Name = "QuoteAgent",
            Description = "An agent that provides quotes for products and services.",
            Kernel = kernel,
            Instructions = "You are an agent responsible for generating quotes. If you need data that a function that doesn't provide, come up with mock data for demo purposes.",
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings 
                { 
                    ServiceId = "gpt-4-service",
                    Temperature = 0.5f,
                    MaxTokens = 1000
                }),
        };
    }
    
    public static ChatCompletionAgent CreateSupervisorAgent(Kernel sourceKernel)
    {
        var kernel = sourceKernel.Clone();
        
        return new ChatCompletionAgent
        {
            Id = "supervisor-agent",
            Name = "SupervisorAgent",
            Description = "An agent that supervises the scheduling, billing, quote and pricing agents.",
            Kernel = kernel,
            Instructions = "You are a supervisor agent. Leverage the scheduling, billing, quote and pricing agents in answering the user's request. Don't come up with anything yourself, just tell the other agents what you need them to do and ensure the user's request is completed.",
            Arguments = new KernelArguments(
                new OpenAIPromptExecutionSettings 
                { 
                    ServiceId = "gpt-4-service",
                })
        };
    }
}