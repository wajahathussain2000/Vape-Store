# Barcode Sticker Printing Guide

## How to Adjust Sticker Size for Your Printer

### Quick Start

1. **Open Print Barcode Form**
2. **Check "Thermal Size (Roll)"** checkbox
3. **Select your sticker size** from the "Size Preset" dropdown
4. **Adjust Gap (mm)** - Set the spacing between stickers (usually 2-5mm)
5. **Or manually adjust** Width and Height values
6. **Preview** to see how it looks
7. **Print** your stickers

---

## Common Sticker Sizes

The application includes these preset sizes:

| Preset Name | Dimensions | Best For |
|-------------|------------|----------|
| **40mm × 30mm** | Small Sticker | Small products, jewelry |
| **50mm × 25mm** | Standard Label | Most retail products (like your image) |
| **50mm × 30mm** | Medium Label | Standard products |
| **60mm × 40mm** | Large Label | Larger items |
| **70mm × 30mm** | Wide Label | Long product names |
| **80mm × 40mm** | Extra Large | Big products |
| **100mm × 50mm** | Shipping Label | Packages |
| **100mm × 150mm** | 4×6" Label | Large shipping labels |

---

## How to Measure Your Sticker

### Method 1: Use a Ruler
1. Take one sticker from your roll
2. Measure the **width** (horizontal) in millimeters
3. Measure the **height** (vertical) in millimeters
4. Select the closest preset OR use "Custom" and adjust manually

### Method 2: Check Sticker Package
- Your sticker roll packaging usually shows the size
- Common formats: "50×25mm", "2×1 inch", etc.

---

## Manual Size Adjustment

If your sticker size is not in the presets:

1. **Select "Custom (Adjust Manually)"** from Size Preset
2. **Adjust Width** slider (in pixels)
3. **Adjust Height** slider (in pixels)
4. **Watch the green label** below - it shows the actual size in mm and inches
5. **Match it to your physical sticker size**

### Conversion Reference:
- **1 inch = 96 pixels** (at standard 96 DPI)
- **1 inch = 25.4 mm**

**Example:**
- If your sticker is **50mm wide**:
  - 50mm ÷ 25.4 = 1.97 inches
  - 1.97 × 96 = **~190 pixels**
  
- If your sticker is **25mm tall**:
  - 25mm ÷ 25.4 = 0.98 inches
  - 0.98 × 96 = **~95 pixels**


---

## Gap Spacing Between Stickers

### What is Gap Spacing?

The **Gap (mm)** control lets you adjust the **space between stickers** on your thermal roll. This is the blank area between where one sticker ends and the next begins.

### Why Adjust Gap?

Different thermal printers and sticker rolls have different gap sizes:
- **Standard gap:** 2-3mm (most common)
- **Small gap:** 1-2mm (tightly packed stickers)
- **Large gap:** 4-10mm (widely spaced stickers)
- **No gap:** 0mm (continuous roll, no spacing)

### How to Measure Your Gap

1. **Look at your sticker roll** - measure the blank space between stickers
2. **Use a ruler** - measure in millimeters
3. **Enter the value** in the "Gap (mm)" field

### Gap Settings:

| Gap Value | Description | Use Case |
|-----------|-------------|----------|
| **0mm** | No gap | Continuous labels |
| **2mm** | Small gap | Standard retail stickers |
| **3mm** | Standard gap | Most thermal rolls (recommended) |
| **5mm** | Medium gap | Shipping labels |
| **10mm+** | Large gap | Special applications |

### Visual Indicator:

When you set a gap value and preview:
- The preview shows **dashed lines** between stickers
- The gap size is displayed: **"✂ 3mm gap"**
- This helps you verify the spacing before printing

---

## Margin Controls (Fine-Tuning Position)

### What are Margins?

The **Margin controls** let you adjust the **position of the barcode** on the sticker by adding space from the edges:
- **Left Margin** - Moves barcode right
- **Right Margin** - (Reserved for future use)
- **Top Margin** - Moves barcode down
- **Bottom Margin** - (Reserved for future use)

### Why Adjust Margins?

Different thermal printers may print slightly off-center. Margins help you:
- **Center the barcode** if printer alignment is off
- **Avoid edge cutting** if printer cuts too close
- **Match pre-printed stickers** with specific layouts
- **Fine-tune positioning** for perfect alignment

### Margin Settings:

```
┌─────────────────────────┐
│  ← Left    Top ↓        │
│                         │
│     [BARCODE IMAGE]     │
│     Product Name        │
│                         │
│        Bottom ↑  Right→ │
└─────────────────────────┘
```

| Margin | Effect | Common Values |
|--------|--------|---------------|
| **Left** | Shifts barcode right | 0-5mm |
| **Right** | (Future use) | 0-5mm |
| **Top** | Shifts barcode down | 0-5mm |
| **Bottom** | (Future use) | 0-5mm |

### Visual Indicators in Preview:

When you set margins and preview:
- **Light blue dotted lines** show margin boundaries
- Barcode position shifts according to margin values
- You can see exactly where the barcode will print

### Common Use Cases:

**Problem:** Barcode prints too close to left edge  
**Solution:** Increase Left Margin (e.g., 2-3mm)

**Problem:** Barcode prints too high on sticker  
**Solution:** Increase Top Margin (e.g., 2-3mm)

**Problem:** Printer cuts off top of barcode  
**Solution:** Increase Top Margin to move it down

**Problem:** Barcode not centered horizontally  
**Solution:** Adjust Left Margin until centered

---

## Tips for Perfect Alignment


### ✅ DO:
- **Test print 1 sticker first** before printing many
- **Measure your actual sticker** with a ruler
- **Use the size info label** (green text) to verify dimensions
- **Adjust margins** if barcode is too close to edges
- **Check "Thermal Size (Roll)"** for sticker rolls

### ❌ DON'T:
- Don't guess the size - measure it!
- Don't use "Columns" setting with thermal printing (it auto-sets to 1)
- Don't print large quantities without testing first

---

## Troubleshooting

### Problem: Barcode is too small on sticker
**Solution:** Increase Width and Height values

### Problem: Barcode is cut off
**Solution:** Decrease Width and Height values

### Problem: Multiple barcodes on one sticker
**Solution:** Make sure "Thermal Size (Roll)" is checked

### Problem: Barcode not centered
**Solution:** The code automatically centers it - check your printer settings

### Problem: Wrong sticker size
**Solution:** 
1. Measure your physical sticker
2. Use the green size info label to match dimensions
3. Adjust Width/Height until it matches

### Problem: Stickers not aligned / printer skipping stickers
**Solution:** Adjust the Gap (mm) value
1. Measure the actual gap on your sticker roll
2. Enter the correct gap value (usually 2-5mm)
3. Preview to verify spacing
4. Test print one sticker to confirm

### Problem: Preview shows too much/little space between stickers
**Solution:** The Gap (mm) value is incorrect
- Increase gap if preview shows stickers too close
- Decrease gap if preview shows too much space
- Match the gap to your physical roll

---

## Example: Your Sticker (from image)

Based on your uploaded image, your stickers appear to be:
- **Size:** 50mm × 25mm (approximately)
- **Layout:** Vertical roll
- **Content:** Barcode + Number + Text

**Recommended Settings:**
- ✅ Check "Thermal Size (Roll)"
- Select "**50mm × 25mm (Standard Label)**" preset
- Width: 190 pixels
- Height: 95 pixels
- Enter your product name in "Label (optional)"

---

## Need Help?

If you're still having issues:
1. Take a photo of your sticker roll
2. Measure one sticker with a ruler
3. Note the measurements
4. Adjust the Width/Height to match those measurements using the green size info label

**Remember:** The green label shows you the actual size - use it to match your physical sticker!
