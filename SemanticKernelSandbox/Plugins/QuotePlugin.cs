using System.ComponentModel;
using System.Text;
using Microsoft.SemanticKernel;

namespace SemanticKernelSandbox.Plugins;

public class Quote
{
    public float Total { get; set; }
    public List<QuoteLineItems> LineItems { get; set; } = new();
}

public class QuoteLineItems
{
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public int Quantity { get; set; }
    public float Total => Price * Quantity;
}

public class QuotePlugin
{
    [KernelFunction("generate_quote"), Description("Generates a quote based on the provided line items.")]
    public Quote GenerateQuote(
        [Description("The line items for the quote. Each item should include a description, price, and quantity.")]
        List<QuoteLineItems> lineItems)
    {
        var quote = new Quote
        {
            LineItems = lineItems,
            Total = lineItems.Sum(item => item.Total)
        };

        return quote;
    }
    
    [KernelFunction("format_quote"), Description("Formats the quote into a string representation.")]
    public string FormatQuote(
        [Description("The quote to format.")]
        Quote quote)
    {
        var formattedQuote = new StringBuilder();
        formattedQuote.AppendLine("Quote Summary:");
        foreach (var item in quote.LineItems)
        {
            formattedQuote.AppendLine($"{item.Quantity} x {item.Description} @ {item.Price:C} each = {item.Total:C}");
        }
        formattedQuote.AppendLine($"Total: {quote.Total:C}");
        
        return formattedQuote.ToString();
    }
}