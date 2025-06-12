using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelSandbox.Plugins;

public class PricingPlugin
{
    [KernelFunction("get_pricing"), Description("Retrieves the pricing for a given product or service.")]
    public string GetPricing(
        [Description("The name of the product or service to retrieve pricing for.")]
        string productName,
        [Description("The quantity of the product or service.")]
        int quantity = 1)
    {
        // Simulated pricing logic
        var pricePerUnit = 19.99m; // Example price per unit
        var totalPrice = pricePerUnit * quantity;

        return $"The price for {quantity} unit(s) of {productName} is {totalPrice:C}.";
    }
}