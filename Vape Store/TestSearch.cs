using System;
using System.Collections.Generic;
using System.Linq;

namespace Vape_Store.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
    }
}

class Program
{
    private static string GetDisplayText(object item, string displayMember)
    {
        if (item == null) return string.Empty;

        try
        {
            if (item is string str) return str;
            if (!string.IsNullOrEmpty(displayMember))
            {
                var property = item.GetType().GetProperty(displayMember);
                if (property != null)
                {
                    var value = property.GetValue(item);
                    return value?.ToString() ?? string.Empty;
                }
            }
            return item.ToString();
        }
        catch
        {
            return item.ToString();
        }
    }

    static void Main()
    {
        var products = new List<Vape_Store.Models.Product>
        {
            new Vape_Store.Models.Product { ProductName = "apple", ProductCode = null, Barcode = null },
            new Vape_Store.Models.Product { ProductName = "mango", ProductCode = null, Barcode = null },
            new Vape_Store.Models.Product { ProductName = "Laptop", ProductCode = null, Barcode = null },
            new Vape_Store.Models.Product { ProductName = "iPhone 17 pro", ProductCode = null, Barcode = null },
        };

        string searchText = "pro";
        string[] keywords = searchText.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        string[] filterProperties = "ProductName|ProductCode|Barcode".Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in products)
        {
            string combinedFilterValue = string.Join(" ", filterProperties.Select(prop => GetDisplayText(item, prop.Trim())));
            bool anyKeywordMatch = false;

            foreach (var keyword in keywords)
            {
                int index = combinedFilterValue.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    anyKeywordMatch = true;
                }
            }

            Console.WriteLine($"{item.ProductName} combined='{combinedFilterValue}' match={anyKeywordMatch}");
        }
    }
}
