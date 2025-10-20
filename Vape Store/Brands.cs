using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Repositories;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class Brands : Form
    {
        private BrandRepository _brandRepository;
        private List<Brand> _brands;
        private bool isEditMode = false;
        private int selectedBrandId = -1;

        public Brands()
        {
            InitializeComponent();
            _brandRepository = new BrandRepository();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadBrands();
        }
        
        private void InitializeDataGridView()
        {
            // Configure DataGridView columns
            dgvBrands.AutoGenerateColumns = false;
            dgvBrands.AllowUserToAddRows = false;
            dgvBrands.AllowUserToDeleteRows = false;
            dgvBrands.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBrands.MultiSelect = false;
            dgvBrands.ReadOnly = true;
            
            // Clear existing columns
            dgvBrands.Columns.Clear();
            
            // Add columns
            dgvBrands.Columns.Add("BrandID", "ID");
            dgvBrands.Columns.Add("BrandName", "Brand Name");
            dgvBrands.Columns.Add("Description", "Description");
            dgvBrands.Columns.Add("CreatedDate", "Created Date");
            dgvBrands.Columns.Add("IsActive", "Status");
            
            // Configure column properties
            dgvBrands.Columns["BrandID"].Width = 80;
            dgvBrands.Columns["BrandName"].Width = 200;
            dgvBrands.Columns["Description"].Width = 300;
            dgvBrands.Columns["CreatedDate"].Width = 150;
            dgvBrands.Columns["IsActive"].Width = 100;
            
            // Format date column
            dgvBrands.Columns["CreatedDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
            
            // Format status column
            dgvBrands.Columns["IsActive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            addCategoryBtn.Click += AddBrandBtn_Click;
            Savebtn.Click += SaveBtn_Click;
            Deletebtn.Click += DeleteBtn_Click;
            
            // DataGridView event handlers
            dgvBrands.CellDoubleClick += DgvBrands_CellDoubleClick;
            dgvBrands.SelectionChanged += DgvBrands_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadBrands()
        {
            try
            {
                _brands = _brandRepository.GetAllBrands();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvBrands.Rows.Clear();
                
                foreach (var brand in _brands)
                {
                    dgvBrands.Rows.Add(
                        brand.BrandID,
                        brand.BrandName,
                        brand.Description,
                        brand.CreatedDate,
                        brand.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddBrandBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            SetEditMode(false);
            txtCategoryName.Focus();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveBrand();
        }

        private void SaveBrand()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    ShowMessage("Please enter a brand name.", "Validation Error", MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                // Check for duplicate brand name
                var existingBrand = _brands.FirstOrDefault(b => 
                    b.BrandName.ToLower() == txtCategoryName.Text.ToLower() && 
                    b.BrandID != selectedBrandId);

                if (existingBrand != null)
                {
                    ShowMessage("A brand with this name already exists.", "Duplicate Error", MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                if (isEditMode)
                {
                    // Update existing brand
                    var brand = new Brand
                    {
                        BrandID = selectedBrandId,
                        BrandName = txtCategoryName.Text.Trim(),
                        Description = txtCategoryDesc.Text.Trim(),
                        IsActive = true,
                        CreatedDate = _brands.First(b => b.BrandID == selectedBrandId).CreatedDate
                    };

                    bool success = _brandRepository.UpdateBrand(brand);
                    
                    if (success)
                    {
                        ShowMessage("Brand updated successfully!", "Success", MessageBoxIcon.Information);
                        LoadBrands();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to update brand.", "Error", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Add new brand
                    var brand = new Brand
                    {
                        BrandName = txtCategoryName.Text.Trim(),
                        Description = txtCategoryDesc.Text.Trim(),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    bool success = _brandRepository.AddBrand(brand);
                    
                    if (success)
                    {
                        ShowMessage("Brand added successfully!", "Success", MessageBoxIcon.Information);
                        LoadBrands();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to add brand.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving brand: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            DeleteBrand();
        }

        private void DeleteBrand()
        {
            try
            {
                if (selectedBrandId == -1)
                {
                    ShowMessage("Please select a brand to delete.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var brand = _brands.FirstOrDefault(b => b.BrandID == selectedBrandId);
                if (brand == null)
                {
                    ShowMessage("Brand not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the brand '{brand.BrandName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _brandRepository.DeleteBrand(selectedBrandId);
                    
                    if (success)
                    {
                        ShowMessage("Brand deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadBrands();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to delete brand.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting brand: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvBrands_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvBrands.Rows.Count)
            {
                EditSelectedBrand();
            }
        }

        private void DgvBrands_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBrands.SelectedRows.Count > 0)
            {
                int rowIndex = dgvBrands.SelectedRows[0].Index;
                if (rowIndex >= 0 && rowIndex < _brands.Count)
                {
                    selectedBrandId = _brands[rowIndex].BrandID;
                }
            }
        }

        private void EditSelectedBrand()
        {
            try
            {
                if (selectedBrandId == -1)
                {
                    ShowMessage("Please select a brand to edit.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var brand = _brands.FirstOrDefault(b => b.BrandID == selectedBrandId);
                if (brand == null)
                {
                    ShowMessage("Brand not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate form with brand data
                txtCategoryName.Text = brand.BrandName;
                txtCategoryDesc.Text = brand.Description;
                
                SetEditMode(true);
                txtCategoryName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error editing brand: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterBrands();
        }

        private void FilterBrands()
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RefreshDataGridView();
                    return;
                }

                var filteredBrands = _brands.Where(b => 
                    b.BrandName.ToLower().Contains(searchText) ||
                    b.Description.ToLower().Contains(searchText)).ToList();

                dgvBrands.Rows.Clear();
                
                foreach (var brand in filteredBrands)
                {
                    dgvBrands.Rows.Add(
                        brand.BrandID,
                        brand.BrandName,
                        brand.Description,
                        brand.CreatedDate,
                        brand.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtCategoryName.Clear();
            txtCategoryDesc.Clear();
            selectedBrandId = -1;
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            if (editMode)
            {
                addCategoryBtn.Text = "New Brand";
                Savebtn.Text = "Update";
                Deletebtn.Enabled = true;
                categoryInputGroup.Text = "Edit Brand";
            }
            else
            {
                addCategoryBtn.Text = "Add Brand";
                Savebtn.Text = "Save";
                Deletebtn.Enabled = false;
                categoryInputGroup.Text = "Add New Brand";
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }
}
