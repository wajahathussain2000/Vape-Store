using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Services;
using Vape_Store.Repositories;

namespace Vape_Store.Examples
{
    /// <summary>
    /// Example of how to integrate backend services with Products form
    /// This shows how to use the ProductRepository and InventoryService
    /// </summary>
    public partial class ProductsFormExample : Form
    {
        private ProductRepository _productRepository;
        private InventoryService _inventoryService;
        private List<Product> _products;
        
        public ProductsFormExample()
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
            _inventoryService = new InventoryService();
            LoadProducts();
        }
        
        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                PopulateDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void PopulateDataGridView()
        {
            // Example of populating a DataGridView with products
            // This would be connected to your actual DataGridView control
            /*
            dgvProducts.DataSource = _products;
            dgvProducts.Columns["ProductID"].Visible = false;
            dgvProducts.Columns["CategoryID"].Visible = false;
            dgvProducts.Columns["BrandID"].Visible = false;
            dgvProducts.Columns["IsActive"].Visible = false;
            dgvProducts.Columns["CreatedDate"].Visible = false;
            */
        }
        
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                var product = new Product
                {
                    ProductCode = GenerateProductCode(),
                    ProductName = txtProductName.Text,
                    Description = txtDescription.Text,
                    CategoryID = Convert.ToInt32(cmbCategory.SelectedValue),
                    BrandID = Convert.ToInt32(cmbBrand.SelectedValue),
                    PurchasePrice = Convert.ToDecimal(txtPurchasePrice.Text),
                    RetailPrice = Convert.ToDecimal(txtRetailPrice.Text),
                    StockQuantity = Convert.ToInt32(txtStockQuantity.Text),
                    ReorderLevel = Convert.ToInt32(txtReorderLevel.Text),
                    Barcode = txtBarcode.Text,
                    IsActive = true
                };
                
                if (_productRepository.AddProduct(product))
                {
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts(); // Refresh the list
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to add product!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                    
                    selectedProduct.ProductName = txtProductName.Text;
                    selectedProduct.Description = txtDescription.Text;
                    selectedProduct.CategoryID = Convert.ToInt32(cmbCategory.SelectedValue);
                    selectedProduct.BrandID = Convert.ToInt32(cmbBrand.SelectedValue);
                    selectedProduct.PurchasePrice = Convert.ToDecimal(txtPurchasePrice.Text);
                    selectedProduct.RetailPrice = Convert.ToDecimal(txtRetailPrice.Text);
                    selectedProduct.StockQuantity = Convert.ToInt32(txtStockQuantity.Text);
                    selectedProduct.ReorderLevel = Convert.ToInt32(txtReorderLevel.Text);
                    selectedProduct.Barcode = txtBarcode.Text;
                    
                    if (_productRepository.UpdateProduct(selectedProduct))
                    {
                        MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts(); // Refresh the list
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update product!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                    
                    if (MessageBox.Show($"Are you sure you want to delete {selectedProduct.ProductName}?", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (_productRepository.DeleteProduct(selectedProduct.ProductID))
                        {
                            MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadProducts(); // Refresh the list
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete product!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchResults = _inventoryService.SearchProducts(searchTerm);
                    dgvProducts.DataSource = searchResults;
                }
                else
                {
                    LoadProducts(); // Show all products
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCheckLowStock_Click(object sender, EventArgs e)
        {
            try
            {
                var lowStockProducts = _inventoryService.GetLowStockProducts();
                
                if (lowStockProducts.Count > 0)
                {
                    string message = "Low Stock Products:\n\n";
                    foreach (var product in lowStockProducts)
                    {
                        message += $"{product.ProductName} - Stock: {product.StockQuantity} (Reorder Level: {product.ReorderLevel})\n";
                    }
                    
                    MessageBox.Show(message, "Low Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("All products have sufficient stock!", "Stock Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnUpdateStock_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                    int adjustment = Convert.ToInt32(txtStockAdjustment.Text);
                    
                    if (_inventoryService.ProcessStockAdjustment(selectedProduct.ProductID, adjustment, "Manual adjustment"))
                    {
                        MessageBox.Show("Stock updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts(); // Refresh the list
                    }
                    else
                    {
                        MessageBox.Show("Failed to update stock!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string GenerateProductCode()
        {
            // Simple product code generation
            return $"PROD{DateTime.Now:yyyyMMddHHmmss}";
        }
        
        private void ClearForm()
        {
            txtProductName.Clear();
            txtDescription.Clear();
            txtPurchasePrice.Clear();
            txtRetailPrice.Clear();
            txtStockQuantity.Clear();
            txtReorderLevel.Clear();
            txtBarcode.Clear();
        }
        
        // Example of how to get inventory statistics
        private void ShowInventoryStats()
        {
            try
            {
                int totalProducts = _inventoryService.GetTotalProductsCount();
                int totalStock = _inventoryService.GetTotalStockQuantity();
                decimal totalValue = _inventoryService.GetTotalInventoryValue();
                
                string stats = $"Inventory Statistics:\n\n" +
                              $"Total Products: {totalProducts}\n" +
                              $"Total Stock Quantity: {totalStock}\n" +
                              $"Total Inventory Value: ${totalValue:F2}";
                
                MessageBox.Show(stats, "Inventory Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting inventory stats: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
