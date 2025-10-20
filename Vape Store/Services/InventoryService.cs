using System;
using System.Collections.Generic;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store.Services
{
    public class InventoryService
    {
        private readonly ProductRepository _productRepository;
        
        public InventoryService()
        {
            _productRepository = new ProductRepository();
        }
        
        public bool UpdateStock(int productID, int quantityChange)
        {
            try
            {
                return _productRepository.UpdateStock(productID, quantityChange);
            }
            catch (Exception ex)
            {
                throw new Exception($"Stock update failed: {ex.Message}");
            }
        }
        
        public bool UpdateStock(int productID, int quantityChange, int bonus)
        {
            try
            {
                return _productRepository.UpdateStock(productID, quantityChange, bonus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Stock update failed: {ex.Message}");
            }
        }
        
        public List<Product> GetLowStockProducts()
        {
            try
            {
                return _productRepository.GetLowStockProducts();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get low stock products: {ex.Message}");
            }
        }
        
        public bool CheckStockAvailability(int productID, int requestedQuantity)
        {
            try
            {
                var product = _productRepository.GetProductById(productID);
                return product != null && product.StockQuantity >= requestedQuantity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Stock check failed: {ex.Message}");
            }
        }
        
        public int GetAvailableStock(int productID)
        {
            try
            {
                var product = _productRepository.GetProductById(productID);
                return product?.StockQuantity ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get available stock: {ex.Message}");
            }
        }
        
        public bool ProcessStockAdjustment(int productID, int adjustmentQuantity, string reason)
        {
            try
            {
                // Update stock
                bool stockUpdated = _productRepository.UpdateStock(productID, adjustmentQuantity);
                
                if (stockUpdated)
                {
                    // Log the adjustment (you can extend this to add to an audit table)
                    // For now, we'll just return true
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Stock adjustment failed: {ex.Message}");
            }
        }
        
        public List<Product> GetProductsByCategory(int categoryID)
        {
            try
            {
                var allProducts = _productRepository.GetAllProducts();
                return allProducts.FindAll(p => p.CategoryID == categoryID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get products by category: {ex.Message}");
            }
        }
        
        public List<Product> GetProductsByBrand(int brandID)
        {
            try
            {
                var allProducts = _productRepository.GetAllProducts();
                return allProducts.FindAll(p => p.BrandID == brandID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get products by brand: {ex.Message}");
            }
        }
        
        public List<Product> SearchProducts(string searchTerm)
        {
            try
            {
                var allProducts = _productRepository.GetAllProducts();
                return allProducts.FindAll(p => 
                    p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.ProductCode.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Description.ToLower().Contains(searchTerm.ToLower()) ||
                    p.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.BrandName.ToLower().Contains(searchTerm.ToLower())
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Product search failed: {ex.Message}");
            }
        }
        
        public decimal GetTotalInventoryValue()
        {
            try
            {
                var products = _productRepository.GetAllProducts();
                decimal totalValue = 0;
                
                foreach (var product in products)
                {
                    totalValue += product.StockQuantity * product.PurchasePrice;
                }
                
                return totalValue;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to calculate inventory value: {ex.Message}");
            }
        }
        
        public int GetTotalProductsCount()
        {
            try
            {
                var products = _productRepository.GetAllProducts();
                return products.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get products count: {ex.Message}");
            }
        }
        
        public int GetTotalStockQuantity()
        {
            try
            {
                var products = _productRepository.GetAllProducts();
                int totalStock = 0;
                
                foreach (var product in products)
                {
                    totalStock += product.StockQuantity;
                }
                
                return totalStock;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to calculate total stock: {ex.Message}");
            }
        }
    }
}
