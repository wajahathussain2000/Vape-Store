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
    public partial class Categories : Form
    {
        private CategoryRepository _categoryRepository;
        private List<Category> _categories;
        private bool isEditMode = false;
        private int selectedCategoryId = -1;

        public Categories()
        {
            InitializeComponent();
            _categoryRepository = new CategoryRepository();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCategories();
        }
        
        private void InitializeDataGridView()
        {
            // Configure DataGridView columns
            dgvCategories.AutoGenerateColumns = false;
            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.AllowUserToDeleteRows = false;
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
            dgvCategories.ReadOnly = true;
            
            // Clear existing columns
            dgvCategories.Columns.Clear();
            
            // Add columns
            dgvCategories.Columns.Add("CategoryID", "ID");
            dgvCategories.Columns.Add("CategoryName", "Category Name");
            dgvCategories.Columns.Add("Description", "Description");
            dgvCategories.Columns.Add("CreatedDate", "Created Date");
            dgvCategories.Columns.Add("IsActive", "Status");
            
            // Configure column properties
            dgvCategories.Columns["CategoryID"].Width = 80;
            dgvCategories.Columns["CategoryName"].Width = 200;
            dgvCategories.Columns["Description"].Width = 300;
            dgvCategories.Columns["CreatedDate"].Width = 150;
            dgvCategories.Columns["IsActive"].Width = 100;
            
            // Format date column
            dgvCategories.Columns["CreatedDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
            
            // Format status column
            dgvCategories.Columns["IsActive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            addCategoryBtn.Click += AddCategoryBtn_Click;
            Savebtn.Click += SaveBtn_Click;
            Deletebtn.Click += DeleteBtn_Click;
            
            // DataGridView event handlers
            dgvCategories.CellDoubleClick += DgvCategories_CellDoubleClick;
            dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvCategories.Rows.Clear();
                
                foreach (var category in _categories)
                {
                    dgvCategories.Rows.Add(
                        category.CategoryID,
                        category.CategoryName,
                        category.Description,
                        category.CreatedDate,
                        category.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddCategoryBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            SetEditMode(false);
            txtCategoryName.Focus();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveCategory();
        }

        private void SaveCategory()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    ShowMessage("Please enter a category name.", "Validation Error", MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                // Check for duplicate category name
                var existingCategory = _categories.FirstOrDefault(c => 
                    c.CategoryName.ToLower() == txtCategoryName.Text.ToLower() && 
                    c.CategoryID != selectedCategoryId);

                if (existingCategory != null)
                {
                    ShowMessage("A category with this name already exists.", "Duplicate Error", MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                if (isEditMode)
                {
                    // Update existing category
                    var category = new Category
                    {
                        CategoryID = selectedCategoryId,
                        CategoryName = txtCategoryName.Text.Trim(),
                        Description = txtCategoryDesc.Text.Trim(),
                        IsActive = true,
                        CreatedDate = _categories.First(c => c.CategoryID == selectedCategoryId).CreatedDate
                    };

                    bool success = _categoryRepository.UpdateCategory(category);
                    
                    if (success)
                    {
                        ShowMessage("Category updated successfully!", "Success", MessageBoxIcon.Information);
                        LoadCategories();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to update category.", "Error", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Add new category
                    var category = new Category
                    {
                        CategoryName = txtCategoryName.Text.Trim(),
                        Description = txtCategoryDesc.Text.Trim(),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    bool success = _categoryRepository.AddCategory(category);
                    
                    if (success)
                    {
                        ShowMessage("Category added successfully!", "Success", MessageBoxIcon.Information);
                        LoadCategories();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to add category.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            DeleteCategory();
        }

        private void DeleteCategory()
        {
            try
            {
                if (selectedCategoryId == -1)
                {
                    ShowMessage("Please select a category to delete.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var category = _categories.FirstOrDefault(c => c.CategoryID == selectedCategoryId);
                if (category == null)
                {
                    ShowMessage("Category not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the category '{category.CategoryName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _categoryRepository.DeleteCategory(selectedCategoryId);
                    
                    if (success)
                    {
                        ShowMessage("Category deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadCategories();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to delete category.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvCategories.Rows.Count)
            {
                EditSelectedCategory();
            }
        }

        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                int rowIndex = dgvCategories.SelectedRows[0].Index;
                if (rowIndex >= 0 && rowIndex < _categories.Count)
                {
                    selectedCategoryId = _categories[rowIndex].CategoryID;
                }
            }
        }

        private void EditSelectedCategory()
        {
            try
            {
                if (selectedCategoryId == -1)
                {
                    ShowMessage("Please select a category to edit.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var category = _categories.FirstOrDefault(c => c.CategoryID == selectedCategoryId);
                if (category == null)
                {
                    ShowMessage("Category not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate form with category data
                txtCategoryName.Text = category.CategoryName;
                txtCategoryDesc.Text = category.Description;
                
                SetEditMode(true);
                txtCategoryName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error editing category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterCategories();
        }

        private void FilterCategories()
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RefreshDataGridView();
                    return;
                }

                var filteredCategories = _categories.Where(c => 
                    c.CategoryName.ToLower().Contains(searchText) ||
                    c.Description.ToLower().Contains(searchText)).ToList();

                dgvCategories.Rows.Clear();
                
                foreach (var category in filteredCategories)
                {
                    dgvCategories.Rows.Add(
                        category.CategoryID,
                        category.CategoryName,
                        category.Description,
                        category.CreatedDate,
                        category.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtCategoryName.Clear();
            txtCategoryDesc.Clear();
            selectedCategoryId = -1;
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            if (editMode)
            {
                addCategoryBtn.Text = "New Category";
                Savebtn.Text = "Update";
                Deletebtn.Enabled = true;
                categoryInputGroup.Text = "Edit Category";
            }
            else
            {
                addCategoryBtn.Text = "Add Category";
                Savebtn.Text = "Save";
                Deletebtn.Enabled = false;
                categoryInputGroup.Text = "Add New Category";
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void Categories_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetEditMode(false);
            txtSearch.Focus();
        }

        private void addCategoryBtn_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            ClearForm();
            txtCategoryName.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // This appears to be a generic button click handler
            // Implementation depends on which button this refers to
            MessageBox.Show("Button clicked", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
