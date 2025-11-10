using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Vape_Store.Helpers
{
    /// <summary>
    /// Helper class to make ComboBox controls searchable with full-word matching,
    /// keyboard navigation (up/down arrows), and Enter key selection
    /// </summary>
    public static class SearchableComboBoxHelper
    {
        private static Dictionary<ComboBox, object> _originalDataSources = new Dictionary<ComboBox, object>();
        private static Dictionary<ComboBox, string> _displayMembers = new Dictionary<ComboBox, string>();
        private static Dictionary<ComboBox, string> _valueMembers = new Dictionary<ComboBox, string>();
        private static Dictionary<ComboBox, bool> _isSearchable = new Dictionary<ComboBox, bool>();
        private static Dictionary<ComboBox, bool> _isProcessing = new Dictionary<ComboBox, bool>(); // Prevent recursive calls

        /// <summary>
        /// Makes a ComboBox searchable with full-word matching, arrow key navigation, and Enter key selection
        /// </summary>
        /// <param name="comboBox">The ComboBox to make searchable</param>
        /// <param name="dataSource">The data source (List, Array, etc.)</param>
        /// <param name="displayMember">Property name to display (for objects) or null for simple strings</param>
        /// <param name="valueMember">Property name for value (for objects) or null for simple strings</param>
        /// <param name="filterProperty">Property name to use for filtering (defaults to displayMember)</param>
        public static void MakeSearchable(ComboBox comboBox, object dataSource, string displayMember = null, string valueMember = null, string filterProperty = null)
        {
            if (comboBox == null) return;

            try
            {
                // Store original settings
                if (!_originalDataSources.ContainsKey(comboBox))
                {
                    _originalDataSources[comboBox] = dataSource;
                    _displayMembers[comboBox] = displayMember;
                    _valueMembers[comboBox] = valueMember;
                    _isSearchable[comboBox] = true;
                }

                // Configure ComboBox for searchable dropdown
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.None; // Disable default autocomplete
                comboBox.AutoCompleteSource = AutoCompleteSource.None;

                // Set initial data source
                comboBox.DataSource = dataSource;
                if (!string.IsNullOrEmpty(displayMember))
                {
                    comboBox.DisplayMember = displayMember;
                }
                if (!string.IsNullOrEmpty(valueMember))
                {
                    comboBox.ValueMember = valueMember;
                }

                // Remove existing event handlers to prevent duplicates
                comboBox.TextChanged -= ComboBox_TextChanged;
                comboBox.KeyDown -= ComboBox_KeyDown;
                comboBox.KeyPress -= ComboBox_KeyPress;
                comboBox.DropDown -= ComboBox_DropDown;
                comboBox.Leave -= ComboBox_Leave;
                comboBox.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;
                comboBox.Enter -= ComboBox_Enter;
                comboBox.MouseClick -= ComboBox_MouseClick;

                // Add event handlers
                comboBox.TextChanged += ComboBox_TextChanged;
                comboBox.KeyDown += ComboBox_KeyDown;
                comboBox.KeyPress += ComboBox_KeyPress;
                comboBox.DropDown += ComboBox_DropDown;
                comboBox.Leave += ComboBox_Leave;
                comboBox.SelectionChangeCommitted += ComboBox_SelectionChangeCommitted;
                comboBox.Enter += ComboBox_Enter;
                comboBox.MouseClick += ComboBox_MouseClick;

                // Store filter property
                if (string.IsNullOrEmpty(filterProperty))
                {
                    filterProperty = displayMember;
                }
                if (!_displayMembers.ContainsKey(comboBox))
                {
                    _displayMembers[comboBox] = filterProperty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error making ComboBox searchable: {ex.Message}");
            }
        }

        /// <summary>
        /// Makes a ComboBox searchable using a simple list of strings
        /// </summary>
        public static void MakeSearchable(ComboBox comboBox, List<string> items)
        {
            if (comboBox == null || items == null) return;
            MakeSearchable(comboBox, items, null, null, null);
        }

        /// <summary>
        /// Refreshes the data source for a searchable ComboBox
        /// </summary>
        public static void RefreshDataSource(ComboBox comboBox, object newDataSource)
        {
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                _originalDataSources[comboBox] = newDataSource;
                comboBox.DataSource = newDataSource;
                
                if (_displayMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_displayMembers[comboBox]))
                {
                    comboBox.DisplayMember = _displayMembers[comboBox];
                }
                if (_valueMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_valueMembers[comboBox]))
                {
                    comboBox.ValueMember = _valueMembers[comboBox];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing ComboBox data source: {ex.Message}");
            }
        }

        private static void ComboBox_TextChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;
            
            // Prevent recursive calls
            if (_isProcessing.ContainsKey(comboBox) && _isProcessing[comboBox]) return;
            
            try
            {
                _isProcessing[comboBox] = true;
                
                string currentText = comboBox.Text?.Trim() ?? "";
                
                // If text is cleared, restore all items immediately
                if (string.IsNullOrWhiteSpace(currentText))
                {
                    RestoreAllItems(comboBox);
                    return;
                }
                
                // Clear selection when user starts typing (if text doesn't match current selection)
                if (comboBox.SelectedIndex >= 0)
                {
                    string selectedText = GetDisplayText(comboBox.SelectedItem, _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null);
                    
                    // If typed text doesn't match selected item, clear selection
                    if (!string.Equals(currentText, selectedText, StringComparison.OrdinalIgnoreCase))
                    {
                        comboBox.SelectedIndex = -1;
                    }
                }
                
                // Filter whenever text changes - this enables search while typing
                // The dropdown will be opened by KeyPress event when user starts typing
                FilterComboBox(comboBox);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_TextChanged: {ex.Message}");
            }
            finally
            {
                _isProcessing[comboBox] = false;
            }
        }

        private static void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Handle Enter key to select current item
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    // If dropdown is open, select the highlighted item
                    if (comboBox.DroppedDown && comboBox.Items.Count > 0)
                    {
                        object itemToSelect = null;
                        string displayMember = _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null;
                        string valueMember = _valueMembers.ContainsKey(comboBox) ? _valueMembers[comboBox] : null;
                        
                        // Always use the currently selected/highlighted item
                        if (comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < comboBox.Items.Count)
                        {
                            itemToSelect = comboBox.SelectedItem;
                        }
                        else if (comboBox.Items.Count > 0)
                        {
                            // If nothing is selected, select the first item (shouldn't happen, but safety check)
                            comboBox.SelectedIndex = 0;
                            itemToSelect = comboBox.SelectedItem;
                        }
                        
                        if (itemToSelect != null)
                        {
                            // Get display text and value (ID) of selected item
                            string selectedDisplayText = GetDisplayText(itemToSelect, displayMember);
                            object selectedValue = null;
                            
                            // Get the value (ID) from the selected item - this is the key to finding the exact match
                            if (!string.IsNullOrEmpty(valueMember))
                            {
                                try
                                {
                                    var valueProperty = itemToSelect.GetType().GetProperty(valueMember);
                                    if (valueProperty != null)
                                    {
                                        selectedValue = valueProperty.GetValue(itemToSelect);
                                    }
                                }
                                catch { }
                            }
                            
                            if (!string.IsNullOrEmpty(selectedDisplayText))
                            {
                                // Find the matching item in the original data source
                                object originalDataSource = _originalDataSources[comboBox];
                                object matchingItemFromOriginal = null;
                                
                                if (originalDataSource is System.Collections.IEnumerable enumerable)
                                {
                                    // First, try to match by ValueMember (ID) - this ensures we get the exact item
                                    if (selectedValue != null && !string.IsNullOrEmpty(valueMember))
                                    {
                                        foreach (var item in enumerable)
                                        {
                                            if (item == null) continue;
                                            
                                            try
                                            {
                                                var valueProperty = item.GetType().GetProperty(valueMember);
                                                if (valueProperty != null)
                                                {
                                                    object itemValue = valueProperty.GetValue(item);
                                                    if (itemValue != null)
                                                    {
                                                        // Use robust comparison that handles type conversions
                                                        if (CompareValues(itemValue, selectedValue))
                                                        {
                                                            matchingItemFromOriginal = item;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    
                                    // If not found by ID, fall back to display text matching
                                    if (matchingItemFromOriginal == null)
                                    {
                                        foreach (var item in enumerable)
                                        {
                                            if (item == null) continue;
                                            string itemDisplayText = GetDisplayText(item, displayMember);
                                            if (string.Equals(itemDisplayText, selectedDisplayText, StringComparison.OrdinalIgnoreCase))
                                            {
                                                matchingItemFromOriginal = item;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                // Restore full list and select from original source
                                if (matchingItemFromOriginal != null)
                                {
                                    // Store the item to select and its value before restoring
                                    object itemToSelectEnter = matchingItemFromOriginal;
                                    string textToSetEnter = selectedDisplayText;
                                    object valueToSelectEnter = selectedValue;
                                    
                                    // If we don't have the value yet, get it from the matched item
                                    if (valueToSelectEnter == null && !string.IsNullOrEmpty(valueMember))
                                    {
                                        try
                                        {
                                            var valueProperty = itemToSelectEnter.GetType().GetProperty(valueMember);
                                            if (valueProperty != null)
                                            {
                                                valueToSelectEnter = valueProperty.GetValue(itemToSelectEnter);
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Restore all items and immediately select
                                    RestoreAllItemsWithSelection(comboBox, itemToSelectEnter, valueToSelectEnter, textToSetEnter);
                                }
                            }
                        }
                        
                        comboBox.DroppedDown = false;
                    }
                    else
                    {
                        // If dropdown is closed, try to find and select matching item
                        // This allows Enter key to work even when dropdown is not open
                        if (comboBox.Items.Count > 0)
                        {
                            // If we have filtered items, select the first one
                            if (comboBox.SelectedIndex < 0 && comboBox.Items.Count > 0)
                            {
                                comboBox.SelectedIndex = 0;
                            }
                            
                            // Get the selected item and commit the selection
                            if (comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < comboBox.Items.Count)
                            {
                                object itemToSelect = comboBox.SelectedItem;
                                string displayMember = _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null;
                                string valueMember = _valueMembers.ContainsKey(comboBox) ? _valueMembers[comboBox] : null;
                                string selectedDisplayText = GetDisplayText(itemToSelect, displayMember);
                                object selectedValue = null;
                                
                                if (!string.IsNullOrEmpty(valueMember))
                                {
                                    try
                                    {
                                        var valueProperty = itemToSelect.GetType().GetProperty(valueMember);
                                        if (valueProperty != null)
                                        {
                                            selectedValue = valueProperty.GetValue(itemToSelect);
                                        }
                                    }
                                    catch { }
                                }
                                
                                // Find matching item in original source and select it
                                object originalDataSource = _originalDataSources[comboBox];
                                if (originalDataSource is System.Collections.IEnumerable enumerable)
                                {
                                    object matchingItemFromOriginal = null;
                                    
                                    // Try to match by value first
                                    if (selectedValue != null && !string.IsNullOrEmpty(valueMember))
                                    {
                                        foreach (var item in enumerable)
                                        {
                                            if (item == null) continue;
                                            try
                                            {
                                                var valueProperty = item.GetType().GetProperty(valueMember);
                                                if (valueProperty != null)
                                                {
                                                    object itemValue = valueProperty.GetValue(item);
                                                    if (itemValue != null && CompareValues(itemValue, selectedValue))
                                                    {
                                                        matchingItemFromOriginal = item;
                                                        break;
                                                    }
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    
                                    // Fall back to display text matching
                                    if (matchingItemFromOriginal == null)
                                    {
                                        foreach (var item in enumerable)
                                        {
                                            if (item == null) continue;
                                            string itemDisplayText = GetDisplayText(item, displayMember);
                                            if (string.Equals(itemDisplayText, selectedDisplayText, StringComparison.OrdinalIgnoreCase))
                                            {
                                                matchingItemFromOriginal = item;
                                                break;
                                            }
                                        }
                                    }
                                    
                                    if (matchingItemFromOriginal != null)
                                    {
                                        RestoreAllItemsWithSelection(comboBox, matchingItemFromOriginal, selectedValue, selectedDisplayText);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // No filtered items, try to find match in original source
                            SelectMatchingItem(comboBox);
                        }
                    }
                }
                // Handle Escape to close dropdown
                else if (e.KeyCode == Keys.Escape)
                {
                    if (comboBox.DroppedDown)
                    {
                        comboBox.DroppedDown = false;
                        e.Handled = true;
                    }
                }
                // Up/Down arrows are handled by default ComboBox behavior
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_KeyDown: {ex.Message}");
            }
        }

        private static void ComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Clear selection when user starts typing a new character
                if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
                {
                    // If there's a current selection, clear it when user starts typing
                    if (comboBox.SelectedIndex >= 0)
                    {
                        comboBox.SelectedIndex = -1;
                    }
                    
                    // Open dropdown immediately when user starts typing
                    // This provides instant visual feedback
                    if (!comboBox.DroppedDown)
                    {
                        comboBox.DroppedDown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_KeyPress: {ex.Message}");
            }
        }

        private static void ComboBox_DropDown(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Don't interfere with dropdown opening - let it open normally
                // Filtering is handled in TextChanged event, so items should already be correct
                // If text is empty, items should already be the full list
                // If text has content, items should already be filtered
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_DropDown: {ex.Message}");
            }
        }

        private static void ComboBox_Enter(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Don't interfere when ComboBox gains focus
                // Items should already be correct based on current text state
                // Restoring items here can interfere with dropdown opening
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_Enter: {ex.Message}");
            }
        }

        private static void ComboBox_MouseClick(object sender, MouseEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Don't interfere with normal dropdown opening
                // The DropDown event will handle restoring items if needed
                // This handler is kept for potential future enhancements but doesn't block dropdown
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_MouseClick: {ex.Message}");
            }
        }

        private static void ComboBox_Leave(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;

            try
            {
                // Try to select matching item when focus leaves
                SelectMatchingItem(comboBox);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_Leave: {ex.Message}");
            }
        }

        private static void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null || !_isSearchable.ContainsKey(comboBox) || !_isSearchable[comboBox]) return;
            
            // Prevent recursive calls
            if (_isProcessing.ContainsKey(comboBox) && _isProcessing[comboBox]) return;

            try
            {
                _isProcessing[comboBox] = true;
                
                // When user explicitly selects an item (mouse click), we need to:
                // 1. Get the selected item from filtered list
                // 2. Find the matching item in the original data source using ID (ValueMember) for exact match
                // 3. Restore full list and select the item from original source
                // This ensures SelectedItem and SelectedValue work correctly
                
                if (comboBox.SelectedItem != null && comboBox.SelectedIndex >= 0)
                {
                    // Get display text and value (ID) of selected item
                    string displayMember = _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null;
                    string valueMember = _valueMembers.ContainsKey(comboBox) ? _valueMembers[comboBox] : null;
                    string selectedDisplayText = GetDisplayText(comboBox.SelectedItem, displayMember);
                    object selectedValue = null;
                    
                    // Get the value (ID) from the selected item - this is the key to finding the exact match
                    if (!string.IsNullOrEmpty(valueMember))
                    {
                        try
                        {
                            var valueProperty = comboBox.SelectedItem.GetType().GetProperty(valueMember);
                            if (valueProperty != null)
                            {
                                selectedValue = valueProperty.GetValue(comboBox.SelectedItem);
                            }
                        }
                        catch { }
                    }
                    
                    if (string.IsNullOrEmpty(selectedDisplayText)) return;
                    
                    // Find the matching item in the original data source
                    object originalDataSource = _originalDataSources[comboBox];
                    object matchingItemFromOriginal = null;
                    
                    if (originalDataSource is System.Collections.IEnumerable enumerable)
                    {
                        // First, try to match by ValueMember (ID) - this ensures we get the exact item
                        if (selectedValue != null && !string.IsNullOrEmpty(valueMember))
                        {
                            foreach (var item in enumerable)
                            {
                                if (item == null) continue;
                                
                                try
                                {
                                    var valueProperty = item.GetType().GetProperty(valueMember);
                                    if (valueProperty != null)
                                    {
                                        object itemValue = valueProperty.GetValue(item);
                                        if (itemValue != null)
                                        {
                                            // Use robust comparison that handles type conversions
                                            if (CompareValues(itemValue, selectedValue))
                                            {
                                                matchingItemFromOriginal = item;
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        
                        // If not found by ID, fall back to display text matching
                        if (matchingItemFromOriginal == null)
                        {
                            foreach (var item in enumerable)
                            {
                                if (item == null) continue;
                                
                                string itemDisplayText = GetDisplayText(item, displayMember);
                                
                                // Find exact match in original data source
                                if (string.Equals(itemDisplayText, selectedDisplayText, StringComparison.OrdinalIgnoreCase))
                                {
                                    matchingItemFromOriginal = item;
                                    break;
                                }
                            }
                        }
                    }
                    
                    // Restore full list and select the matching item from original source
                    if (matchingItemFromOriginal != null)
                    {
                        // Store the item to select and its value before restoring
                        object itemToSelect = matchingItemFromOriginal;
                        string textToSet = selectedDisplayText;
                        object valueToSelect = selectedValue;
                        
                        // If we don't have the value yet, get it from the matched item
                        if (valueToSelect == null && !string.IsNullOrEmpty(valueMember))
                        {
                            try
                            {
                                var valueProperty = itemToSelect.GetType().GetProperty(valueMember);
                                if (valueProperty != null)
                                {
                                    valueToSelect = valueProperty.GetValue(itemToSelect);
                                }
                            }
                            catch { }
                        }
                        
                        // Restore all items and immediately select
                        RestoreAllItemsWithSelection(comboBox, itemToSelect, valueToSelect, textToSet);
                    }
                    else
                    {
                        // If we can't find it in original, just update the text
                        comboBox.Text = selectedDisplayText;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ComboBox_SelectionChangeCommitted: {ex.Message}");
            }
            finally
            {
                _isProcessing[comboBox] = false;
            }
        }

        private static void FilterComboBox(ComboBox comboBox)
        {
            if (comboBox == null || !_originalDataSources.ContainsKey(comboBox)) return;
            
            // Prevent recursive calls
            if (_isProcessing.ContainsKey(comboBox) && _isProcessing[comboBox]) return;

            try
            {
                _isProcessing[comboBox] = true;
                
                // Store current text to preserve it
                string currentText = comboBox.Text;
                
                string searchText = comboBox.Text?.Trim() ?? "";
                object originalDataSource = _originalDataSources[comboBox];
                string displayMember = _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null;

                // If search text is empty, show all items
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RestoreAllItems(comboBox);
                    // Restore text after restoring items
                    if (!string.IsNullOrEmpty(currentText))
                    {
                        comboBox.Text = currentText;
                    }
                    return;
                }

                // Filter items based on search text (full-word matching, case-insensitive)
                // Use a list with match priority for sorting
                List<Tuple<object, int, string>> filteredItemsWithPriority = new List<Tuple<object, int, string>>();

                if (originalDataSource is System.Collections.IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;

                        string displayText = GetDisplayText(item, displayMember);
                        
                        if (string.IsNullOrEmpty(displayText)) continue;
                        
                        // Check if search text appears in the display text
                        int index = displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            // Calculate priority: 
                            // 0 = exact match (case-insensitive)
                            // 1 = starts with search text
                            // 2 = word boundary match (starts a word)
                            // 3 = contains search text anywhere
                            int priority = 3;
                            if (string.Equals(displayText, searchText, StringComparison.OrdinalIgnoreCase))
                            {
                                priority = 0; // Exact match - highest priority
                            }
                            else if (displayText.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                            {
                                priority = 1; // Starts with - second priority
                            }
                            else if (index > 0)
                            {
                                // Check if it's at a word boundary (after space, dash, etc.)
                                char charBefore = displayText[index - 1];
                                if (char.IsWhiteSpace(charBefore) || charBefore == '-' || charBefore == '_')
                                {
                                    priority = 2; // Word boundary match - third priority
                                }
                            }
                            
                            filteredItemsWithPriority.Add(new Tuple<object, int, string>(item, priority, displayText));
                        }
                    }
                }
                
                // Sort by priority (0 = exact, 1 = starts with, 2 = contains), then alphabetically
                var filteredItems = filteredItemsWithPriority
                    .OrderBy(x => x.Item2) // Sort by priority first
                    .ThenBy(x => x.Item3, StringComparer.OrdinalIgnoreCase) // Then alphabetically
                    .Select(x => x.Item1)
                    .ToList();

                // Update ComboBox with filtered items
                if (filteredItems.Count > 0)
                {
                    bool wasDroppedDown = comboBox.DroppedDown;
                    
                    comboBox.BeginUpdate();
                    try
                    {
                        comboBox.DataSource = null;
                        comboBox.Items.Clear();
                        comboBox.DataSource = filteredItems;
                        
                        if (!string.IsNullOrEmpty(displayMember))
                        {
                            comboBox.DisplayMember = displayMember;
                        }
                        if (_valueMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_valueMembers[comboBox]))
                        {
                            comboBox.ValueMember = _valueMembers[comboBox];
                        }
                    }
                    finally
                    {
                        comboBox.EndUpdate();
                    }
                    
                    // After EndUpdate, ensure DataSource is fully loaded
                    System.Windows.Forms.Application.DoEvents();
                    
                    // Restore the text the user typed
                    comboBox.Text = currentText;
                    
                    // Automatically select and highlight the first matching item
                    // This makes it easy to press Enter to select it
                    if (filteredItems.Count > 0)
                    {
                        // Select first item (highest priority match) - this highlights it
                        comboBox.SelectedIndex = 0;
                        
                        // Ensure dropdown is open when filtering
                        if (!wasDroppedDown && !comboBox.DroppedDown)
                        {
                            comboBox.DroppedDown = true;
                        }
                        
                        // Position cursor at end of typed text for better UX
                        comboBox.SelectionStart = currentText.Length;
                        comboBox.SelectionLength = 0;
                        
                        // Ensure the selected item is visible in the dropdown
                        if (comboBox.DroppedDown && comboBox.Items.Count > 0)
                        {
                            // Force the dropdown to scroll to show the selected item
                            // This is handled automatically by Windows Forms when SelectedIndex is set
                        }
                    }
                    else
                    {
                        comboBox.SelectedIndex = -1;
                    }
                }
                else
                {
                    // No matches found - keep current text but clear selection
                    comboBox.BeginUpdate();
                    try
                    {
                        comboBox.DataSource = null;
                        comboBox.Items.Clear();
                        comboBox.Text = currentText; // Restore user's typed text
                        comboBox.SelectedIndex = -1;
                    }
                    finally
                    {
                        comboBox.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering ComboBox: {ex.Message}");
            }
            finally
            {
                _isProcessing[comboBox] = false;
            }
        }

        private static void RestoreAllItems(ComboBox comboBox)
        {
            if (comboBox == null || !_originalDataSources.ContainsKey(comboBox)) return;
            
            // Prevent recursive calls
            if (_isProcessing.ContainsKey(comboBox) && _isProcessing[comboBox]) return;

            try
            {
                _isProcessing[comboBox] = true;
                
                // Store current text
                string currentText = comboBox.Text;
                
                object originalDataSource = _originalDataSources[comboBox];
                
                comboBox.BeginUpdate();
                try
                {
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();
                    comboBox.DataSource = originalDataSource;

                    if (_displayMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_displayMembers[comboBox]))
                    {
                        comboBox.DisplayMember = _displayMembers[comboBox];
                    }
                    if (_valueMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_valueMembers[comboBox]))
                    {
                        comboBox.ValueMember = _valueMembers[comboBox];
                    }
                    
                    // Restore text and clear selection to prevent TextChanged loop
                    comboBox.Text = currentText;
                    comboBox.SelectedIndex = -1;
                }
                finally
                {
                    comboBox.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring all items: {ex.Message}");
            }
            finally
            {
                _isProcessing[comboBox] = false;
            }
        }

        private static void RestoreAllItemsWithSelection(ComboBox comboBox, object itemToSelect, object valueToSelect, string textToSet)
        {
            if (comboBox == null || !_originalDataSources.ContainsKey(comboBox)) return;
            
            // Prevent recursive calls
            if (_isProcessing.ContainsKey(comboBox) && _isProcessing[comboBox]) return;

            try
            {
                _isProcessing[comboBox] = true;
                
                object originalDataSource = _originalDataSources[comboBox];
                
                comboBox.BeginUpdate();
                try
                {
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();
                    comboBox.DataSource = originalDataSource;

                    if (_displayMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_displayMembers[comboBox]))
                    {
                        comboBox.DisplayMember = _displayMembers[comboBox];
                    }
                    if (_valueMembers.ContainsKey(comboBox) && !string.IsNullOrEmpty(_valueMembers[comboBox]))
                    {
                        comboBox.ValueMember = _valueMembers[comboBox];
                    }
                }
                finally
                {
                    comboBox.EndUpdate();
                }
                
                // After EndUpdate, the DataSource is fully loaded, so we can select
                // Ensure the DataSource is fully populated by processing events
                System.Windows.Forms.Application.DoEvents();
                
                // Try to select by value first (most reliable method)
                if (valueToSelect != null)
                {
                    try
                    {
                        comboBox.SelectedValue = valueToSelect;
                        // Verify the selection worked
                        if (comboBox.SelectedIndex >= 0)
                        {
                            // Selection successful, ensure text is set
                            if (!string.IsNullOrEmpty(textToSet))
                            {
                                comboBox.Text = textToSet;
                            }
                            return; // Success, exit early
                        }
                    }
                    catch { }
                }
                
                // If value selection didn't work, try by finding the item in the restored list
                if (comboBox.SelectedIndex < 0 && itemToSelect != null)
                {
                    try
                    {
                        // Find the item in the restored DataSource by comparing values
                        string valueMember = _valueMembers.ContainsKey(comboBox) ? _valueMembers[comboBox] : null;
                        if (!string.IsNullOrEmpty(valueMember) && valueToSelect != null)
                        {
                            // Try to find by iterating through items
                            for (int i = 0; i < comboBox.Items.Count; i++)
                            {
                                var item = comboBox.Items[i];
                                if (item == null) continue;
                                
                                try
                                {
                                    var valueProperty = item.GetType().GetProperty(valueMember);
                                    if (valueProperty != null)
                                    {
                                        object itemValue = valueProperty.GetValue(item);
                                        if (itemValue != null && CompareValues(itemValue, valueToSelect))
                                        {
                                            comboBox.SelectedIndex = i;
                                            if (!string.IsNullOrEmpty(textToSet))
                                            {
                                                comboBox.Text = textToSet;
                                            }
                                            return; // Success
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        
                        // Last resort: try SelectedItem directly
                        comboBox.SelectedItem = itemToSelect;
                    }
                    catch { }
                }
                
                // Ensure text matches if selection succeeded
                if (comboBox.SelectedIndex >= 0 && !string.IsNullOrEmpty(textToSet))
                {
                    comboBox.Text = textToSet;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring all items with selection: {ex.Message}");
            }
            finally
            {
                _isProcessing[comboBox] = false;
            }
        }

        private static void SelectMatchingItem(ComboBox comboBox)
        {
            if (comboBox == null || !_originalDataSources.ContainsKey(comboBox)) return;

            try
            {
                string searchText = comboBox.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(searchText)) return;

                object originalDataSource = _originalDataSources[comboBox];
                string displayMember = _displayMembers.ContainsKey(comboBox) ? _displayMembers[comboBox] : null;

                // Try to find exact or partial match
                if (originalDataSource is System.Collections.IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;

                        string displayText = GetDisplayText(item, displayMember);
                        
                        // Check for exact match first
                        if (string.Equals(displayText, searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            comboBox.SelectedItem = item;
                            return;
                        }
                    }

                    // If no exact match, try partial match
                    foreach (var item in enumerable)
                    {
                        if (item == null) continue;

                        string displayText = GetDisplayText(item, displayMember);
                        
                        if (!string.IsNullOrEmpty(displayText) && 
                            displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            comboBox.SelectedItem = item;
                            comboBox.Text = displayText;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting matching item: {ex.Message}");
            }
        }

        /// <summary>
        /// Compares two values with robust type handling (handles int/long/int? conversions, etc.)
        /// </summary>
        private static bool CompareValues(object value1, object value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;
            
            // If types match exactly, use Equals
            if (value1.GetType() == value2.GetType())
            {
                return value1.Equals(value2);
            }
            
            // Try to convert to string and compare
            try
            {
                string str1 = value1.ToString();
                string str2 = value2.ToString();
                if (str1 == str2) return true;
            }
            catch { }
            
            // Try numeric comparison for numeric types
            try
            {
                // Convert both to decimal for comparison (handles int, long, float, double, etc.)
                decimal dec1 = Convert.ToDecimal(value1);
                decimal dec2 = Convert.ToDecimal(value2);
                return dec1 == dec2;
            }
            catch { }
            
            // Fallback to Equals
            return value1.Equals(value2);
        }

        private static string GetDisplayText(object item, string displayMember)
        {
            if (item == null) return string.Empty;

            try
            {
                // If it's a string, return it directly
                if (item is string str)
                {
                    return str;
                }

                // If displayMember is specified, get property value
                if (!string.IsNullOrEmpty(displayMember))
                {
                    var property = item.GetType().GetProperty(displayMember);
                    if (property != null)
                    {
                        var value = property.GetValue(item);
                        return value?.ToString() ?? string.Empty;
                    }
                }

                // Fallback to ToString()
                return item.ToString();
            }
            catch
            {
                return item.ToString();
            }
        }

        /// <summary>
        /// Cleans up event handlers when ComboBox is disposed
        /// </summary>
        public static void Cleanup(ComboBox comboBox)
        {
            if (comboBox == null) return;

            try
            {
                comboBox.TextChanged -= ComboBox_TextChanged;
                comboBox.KeyDown -= ComboBox_KeyDown;
                comboBox.KeyPress -= ComboBox_KeyPress;
                comboBox.DropDown -= ComboBox_DropDown;
                comboBox.Leave -= ComboBox_Leave;
                comboBox.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;
                comboBox.Enter -= ComboBox_Enter;
                comboBox.MouseClick -= ComboBox_MouseClick;

                _originalDataSources.Remove(comboBox);
                _displayMembers.Remove(comboBox);
                _valueMembers.Remove(comboBox);
                _isSearchable.Remove(comboBox);
                _isProcessing.Remove(comboBox);
            }
            catch { }
        }
    }
}

