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
    public partial class ExpenseEntry : Form
    {
        private ExpenseRepository _expenseRepository;
        private ExpenseCategoryRepository _expenseCategoryRepository;
        
        private List<Expense> _expenses;
        private List<ExpenseCategory> _expenseCategories;
        private Expense _currentExpense;
        
        private bool isEditMode = false;
        private int selectedExpenseId = -1;

        public ExpenseEntry()
        {
            InitializeComponent();
            _expenseRepository = new ExpenseRepository();
            _expenseCategoryRepository = new ExpenseCategoryRepository();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadExpenseCategories();
            LoadExpenses();
            GenerateExpenseCode();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSubmit.Click += BtnSubmit_Click;
            btnSaveDraft.Click += BtnSaveDraft_Click;
            btnClear.Click += BtnClear_Click;
            btncategory.Click += Btncategory_Click;
            
            // Add delete functionality if delete button exists
            if (this.Controls.Find("btnDelete", true).FirstOrDefault() is Button btnDelete)
            {
                btnDelete.Click += BtnDelete_Click;
            }
            
            // DataGridView event handlers
            dgvExpenses.CellDoubleClick += DgvExpenses_CellDoubleClick;
            dgvExpenses.SelectionChanged += DgvExpenses_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += ExpenseEntry_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvExpenses.AutoGenerateColumns = false;
                dgvExpenses.AllowUserToAddRows = false;
                dgvExpenses.AllowUserToDeleteRows = false;
                dgvExpenses.ReadOnly = true;
                dgvExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvExpenses.MultiSelect = false;

                // Define columns
                dgvExpenses.Columns.Clear();
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ExpenseCode",
                    HeaderText = "Expense Code",
                    DataPropertyName = "ExpenseCode",
                    Width = 120
                });
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Category",
                    DataPropertyName = "CategoryName",
                    Width = 150
                });
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 200
                });
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" },
                    Width = 100
                });
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ExpenseDate",
                    HeaderText = "Date",
                    DataPropertyName = "ExpenseDate",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" },
                    Width = 100
                });
                
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Status",
                    DataPropertyName = "Status",
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
                cmbCategory.DataSource = _expenseCategories;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadExpenses()
        {
            try
            {
                _expenses = _expenseRepository.GetAllExpenses();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expenses: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvExpenses.DataSource = null;
                dgvExpenses.DataSource = _expenses;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateExpenseCode()
        {
            try
            {
                txtExpenseCode.Text = _expenseRepository.GetNextExpenseCode();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating expense code: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dtpExpenseDate.Value = DateTime.Now;
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            txtAmount.Clear();
            cmbPaymentMethod.SelectedIndex = -1;
            txtReferenceNumber.Clear();
            txtRemarks.Clear();
            
            isEditMode = false;
            selectedExpenseId = -1;
            _currentExpense = null;
        }

        private void InitializePaymentMethods()
        {
            try
            {
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Credit Card", "Debit Card", "Bank Transfer", "Check", "Other" });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing payment methods: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SaveExpense("Submitted");
        }

        private void BtnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveExpense("Draft");
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void Btncategory_Click(object sender, EventArgs e)
        {
            try
            {
                ExpenseCategoryForm expenseCategory = new ExpenseCategoryForm();
                expenseCategory.ShowDialog();
                LoadExpenseCategories(); // Refresh categories after closing
            }
            catch (Exception ex)
            {
                ShowMessage($"Error opening expense category form: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SaveExpense(string status)
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                var expense = new Expense
                {
                    ExpenseCode = txtExpenseCode.Text.Trim(),
                    CategoryID = cmbCategory.SelectedItem != null ? ((ExpenseCategory)cmbCategory.SelectedItem).CategoryID : 0,
                    Description = txtDescription.Text.Trim(),
                    Amount = ParseDecimal(txtAmount.Text),
                    ExpenseDate = dtpExpenseDate.Value,
                    PaymentMethod = cmbPaymentMethod.SelectedItem?.ToString(),
                    ReferenceNumber = txtReferenceNumber.Text.Trim(),
                    Remarks = txtRemarks.Text.Trim(),
                    Status = status,
                    UserID = 1, // TODO: Get from current user session
                    CreatedDate = DateTime.Now
                };

                bool success;
                if (isEditMode)
                {
                    expense.ExpenseID = selectedExpenseId;
                    success = _expenseRepository.UpdateExpense(expense);
                }
                else
                {
                    success = _expenseRepository.AddExpense(expense);
                }
                
                if (success)
                {
                    ShowMessage($"Expense {status.ToLower()} successfully!", "Success", MessageBoxIcon.Information);
                    LoadExpenses();
                    ClearForm();
                }
                else
                {
                    ShowMessage($"Failed to {status.ToLower()} expense.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving expense: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (cmbCategory.SelectedValue == null)
            {
                ShowMessage("Please select an expense category.", "Validation Error", MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                ShowMessage("Please enter a description.", "Validation Error", MessageBoxIcon.Warning);
                txtDescription.Focus();
                return false;
            }

            decimal amount = ParseDecimal(txtAmount.Text);
            if (amount <= 0)
            {
                ShowMessage("Please enter a valid amount.", "Validation Error", MessageBoxIcon.Warning);
                txtAmount.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            GenerateExpenseCode();
            dtpExpenseDate.Value = DateTime.Now;
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            txtAmount.Clear();
            cmbPaymentMethod.SelectedIndex = -1;
            txtReferenceNumber.Clear();
            txtRemarks.Clear();
            
            isEditMode = false;
            selectedExpenseId = -1;
            _currentExpense = null;
        }

        private void DgvExpenses_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var expense = _expenses[e.RowIndex];
                    LoadExpenseForEdit(expense);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvExpenses_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvExpenses.SelectedRows.Count > 0)
                {
                    var selectedExpense = (Expense)dgvExpenses.SelectedRows[0].DataBoundItem;
                    selectedExpenseId = selectedExpense.ExpenseID;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling selection change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadExpenseForEdit(Expense expense)
        {
            try
            {
                _currentExpense = expense;
                isEditMode = true;
                selectedExpenseId = expense.ExpenseID;
                
                txtExpenseCode.Text = expense.ExpenseCode;
                cmbCategory.SelectedValue = expense.CategoryID;
                txtDescription.Text = expense.Description;
                txtAmount.Text = expense.Amount.ToString("F2");
                dtpExpenseDate.Value = expense.ExpenseDate;
                
                if (!string.IsNullOrEmpty(expense.PaymentMethod))
                {
                    cmbPaymentMethod.SelectedItem = expense.PaymentMethod;
                }
                
                txtReferenceNumber.Text = expense.ReferenceNumber;
                txtRemarks.Text = expense.Remarks;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    RefreshDataGridView();
                }
                else
                {
                    _expenses = _expenseRepository.SearchExpenses(txtSearch.Text);
                    RefreshDataGridView();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching expenses: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void ExpenseEntry_Load(object sender, EventArgs e)
        {
            InitializePaymentMethods();
        }

        // Event handler for expense category button (already defined in designer)
        private void btnexpensecategory_Click(object sender, EventArgs e)
        {
            Btncategory_Click(sender, e);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedExpenseId <= 0)
                {
                    ShowMessage("Please select an expense to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _expenseRepository.DeleteExpense(selectedExpenseId);
                    
                    if (success)
                    {
                        ShowMessage("Expense deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadExpenses();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to delete expense.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting expense: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            
            // Remove any non-numeric characters except decimal point and minus sign
            string cleanValue = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d.-]", "");
            
            if (string.IsNullOrWhiteSpace(cleanValue))
                return 0;
            
            if (decimal.TryParse(cleanValue, out decimal result))
                return result;
            
            return 0;
        }
    }
}
