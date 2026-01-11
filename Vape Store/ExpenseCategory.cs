using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store
{
    public partial class ExpenseCategoryForm : Form
    {
        private ExpenseCategoryRepository _expenseCategoryRepository;
        
        private List<ExpenseCategory> _expenseCategories;
        private ExpenseCategory _currentCategory;
        
        private bool isEditMode = false;
        private int selectedCategoryId = -1;

        public ExpenseCategoryForm()
        {
            InitializeComponent();
            _expenseCategoryRepository = new ExpenseCategoryRepository();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadExpenseCategories();
            GenerateCategoryCode();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            
            // DataGridView event handlers
            dgvCategories.CellDoubleClick += DgvCategories_CellDoubleClick;
            dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += ExpenseCategory_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvCategories.AutoGenerateColumns = false;
                dgvCategories.AllowUserToAddRows = false;
                dgvCategories.AllowUserToDeleteRows = false;
                dgvCategories.ReadOnly = true;
                dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCategories.MultiSelect = false;

                // Define columns
                dgvCategories.Columns.Clear();
                
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryCode",
                    HeaderText = "Code",
                    DataPropertyName = "CategoryCode",
                    Width = 80
                });
                
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Category Name",
                    DataPropertyName = "CategoryName",
                    Width = 200
                });
                
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 250
                });
                
                dgvCategories.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Active",
                    DataPropertyName = "IsActive",
                    Width = 60
                });
                
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CreatedDate",
                    HeaderText = "Created Date",
                    DataPropertyName = "CreatedDate",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" },
                    Width = 100
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadExpenseCategories()
        {
            try
            {
                _expenseCategories = _expenseCategoryRepository.GetAllExpenseCategories();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvCategories.DataSource = null;
                dgvCategories.DataSource = _expenseCategories;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateCategoryCode()
        {
            try
            {
                txtCategoryCode.Text = _expenseCategoryRepository.GetNextCategoryCode();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating category code: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            chkIsActive.Checked = true;
            
            isEditMode = false;
            selectedCategoryId = -1;
            _currentCategory = null;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveExpenseCategory();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteExpenseCategory();
        }

        private void SaveExpenseCategory()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                var category = new ExpenseCategory
                {
                    CategoryCode = txtCategoryCode.Text.Trim(),
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = chkIsActive.Checked,
                    UserID = UserSession.CurrentUser?.UserID ?? 1, // Get from current user session
                    CreatedDate = DateTime.Now
                };

                bool success;
                if (isEditMode)
                {
                    category.CategoryID = selectedCategoryId;
                    success = _expenseCategoryRepository.UpdateExpenseCategory(category);
                }
                else
                {
                    success = _expenseCategoryRepository.AddExpenseCategory(category);
                }
                
                if (success)
                {
                    ShowMessage("Expense category saved successfully!", "Success", MessageBoxIcon.Information);
                    LoadExpenseCategories();
                    ClearForm();
                }
                else
                {
                    ShowMessage("Failed to save expense category.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving expense category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteExpenseCategory()
        {
            try
            {
                if (!isEditMode)
                {
                    ShowMessage("Please select a category to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this expense category?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _expenseCategoryRepository.DeleteExpenseCategory(selectedCategoryId);
                    
                    if (success)
                    {
                        ShowMessage("Expense category deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadExpenseCategories();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to delete expense category.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting expense category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                ShowMessage("Please enter a category name.", "Validation Error", MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }

            // Check for duplicate category name
            try
            {
                bool nameExists = _expenseCategoryRepository.IsCategoryNameExists(
                    txtCategoryName.Text.Trim(), 
                    isEditMode ? selectedCategoryId : 0);
                
                if (nameExists)
                {
                    ShowMessage("A category with this name already exists.", "Validation Error", MessageBoxIcon.Warning);
                    txtCategoryName.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating category name: {ex.Message}", "Error", MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            GenerateCategoryCode();
            txtCategoryName.Clear();
            txtDescription.Clear();
            chkIsActive.Checked = true;
            
            isEditMode = false;
            selectedCategoryId = -1;
            _currentCategory = null;
        }

        private void DgvCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var category = _expenseCategories[e.RowIndex];
                    LoadCategoryForEdit(category);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading category for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCategories.SelectedRows.Count > 0)
                {
                    var selectedCategory = (ExpenseCategory)dgvCategories.SelectedRows[0].DataBoundItem;
                    selectedCategoryId = selectedCategory.CategoryID;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling selection change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCategoryForEdit(ExpenseCategory category)
        {
            try
            {
                _currentCategory = category;
                isEditMode = true;
                selectedCategoryId = category.CategoryID;
                
                txtCategoryCode.Text = category.CategoryCode;
                txtCategoryName.Text = category.CategoryName;
                txtDescription.Text = category.Description;
                chkIsActive.Checked = category.IsActive;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading category for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadExpenseCategories();
                }
                else
                {
                    _expenseCategories = _expenseCategoryRepository.SearchExpenseCategories(txtSearch.Text);
                    RefreshDataGridView();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching expense categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void ExpenseCategory_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
        }
    }
}
