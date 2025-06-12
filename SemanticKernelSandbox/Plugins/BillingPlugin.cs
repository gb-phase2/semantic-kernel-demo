using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelSandbox.Plugins;

public class BillingPlugin
{
    [KernelFunction("create_invoice"), Description("Creates an invoice based from a quote and assigns it to the customer.")]
    public string CreateInvoice(
        [Description("The quote to create an invoice from.")]
        Quote quote,
        [Description("The customer to assign the invoice to.")]
        string customerName)
    {
        // Logic to create an invoice from the quote and assign it to the customer
        return $"Invoice created for {customerName} with total amount {quote.Total:C}.";
    }
}