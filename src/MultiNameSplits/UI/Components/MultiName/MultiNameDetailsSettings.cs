using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using Fetze.WinFormsColor;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public partial class MultiNameDetailsSettings : UserControl
{
    protected LiveSplitState CurrentState { get; set; }
    protected IList<MultiNameDetailsSettings> DetailsList { get; set; }
    private SplitsSettings splitsSettings { get; set; }

    public bool IsVisible { get; set; }
    public float DisplayTime { get; set; }
    public Color TextColor { get; set; }
    public Font TextFont { get ; set; }

    public bool OverrideDisplayTime = false;
    public bool OverrideTextColor = false;
    public bool OverrideTextFont = false;

    public string TextFontString
    {
        get
        {
            string str = OverrideTextFont ? string.Format("{0} {1}", TextFont.FontFamily.Name, TextFont.Style) : "(choose font)";
            bool isLongStr = Encoding.UTF8.GetByteCount(str) > 24;
            btnTextFont.Font = new Font(btnTextFont.Font.FontFamily, isLongStr ? 8F : 9F);
            btnTextFont.Padding = new System.Windows.Forms.Padding(0, isLongStr ? 2: 0, 0, 0);
            return str;
        }
    }

    private float defaultDisplayTime => splitsSettings.MultiNameDisplayTime;
    private readonly Color defaultTextColor = Color.FromArgb(0, 200, 200, 200);
    private readonly Font defaultTextFont = new ("Segoe UI", 16, FontStyle.Regular, GraphicsUnit.Pixel);

    protected int Index => DetailsList.IndexOf(this);
    protected int RowsCount => DetailsList.Count;
    
    public event EventHandler MovedUp;
    public event EventHandler MovedDown;

    public MultiNameDetailsSettings(
        LiveSplitState state, 
        SplitsSettings splitsSettings, 
        IList<MultiNameDetailsSettings> detailsList, 
        XmlNode node)
    {
        CurrentState = state;
        this.splitsSettings = splitsSettings;
        DetailsList = detailsList;

        InitializeComponent();

        Margin = new System.Windows.Forms.Padding(0);
        Padding = new System.Windows.Forms.Padding(0);
        Location = new System.Drawing.Point(0, 0);

        if (node != null)
        {
            XmlElement element = (XmlElement)node;
            IsVisible = SettingsHelper.ParseBool(element["IsVisible"]);
            DisplayTime = SettingsHelper.ParseFloat(element["DisplayTime"]);
            TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            TextFont = SettingsHelper.GetFontFromElement(element["TextFont"]);

            OverrideDisplayTime = SettingsHelper.ParseBool(element["OverrideDisplayTime"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            OverrideTextFont = SettingsHelper.ParseBool(element["OverrideTextFont"]);

            setOverrideDisplayTime(OverrideDisplayTime);
            setOverrideTextColor(OverrideTextColor);
            setOverrideTextFont(OverrideTextFont);
        }
        else
        {
            init();
        }
    }

    private void init()
    {
        chkIsVisible.Checked = true;
        IsVisible = true;
        DisplayTime = defaultDisplayTime;
        TextColor = defaultTextColor;
        TextFont = defaultTextFont;
        setOverrideDisplayTime(false);
        setOverrideTextColor(false);
        setOverrideTextFont(false);
    }

    private void MultiNameDetailsSettings_Load(object sender, EventArgs e)
    {
        chkIsVisible.DataBindings.Clear();
        dmnDisplayTime.DataBindings.Clear();
        btnTextColor.DataBindings.Clear();
        btnTextFont.DataBindings.Clear();

        chkIsVisible.DataBindings.Add("Checked", this, "IsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
        dmnDisplayTime.DataBindings.Add("Value", this, "DisplayTime", false, DataSourceUpdateMode.OnPropertyChanged);
        btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
        btnTextFont.DataBindings.Add("Text", this, "TextFontString", false, DataSourceUpdateMode.OnPropertyChanged);
    }

    public int CreateSettingsNode(XmlDocument document, XmlElement parent)
    {
        return SettingsHelper.CreateSetting(document, parent, "IsVisible", IsVisible) ^
        SettingsHelper.CreateSetting(document, parent, "DisplayTime", DisplayTime) ^
        SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
        SettingsHelper.CreateSetting(document, parent, "TextFont", TextFont) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideDisplayTime", OverrideDisplayTime) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
        SettingsHelper.CreateSetting(document, parent, "OverrideTextFont", OverrideTextFont);
    }

    private void chkIsVisible_CheckedChanged(object sender, EventArgs e)
    {
        dmnDisplayTime.Enabled = chkIsVisible.Checked;
        btnTextColor.Enabled = chkIsVisible.Checked;
        btnTextFont.Enabled = chkIsVisible.Checked;
        btnTextColor.FlatAppearance.BorderSize  = chkIsVisible.Checked ? 1 : 0;

        IsVisible = chkIsVisible.Checked;

        splitsSettings.SetMultiNameDetailsIsVisibleEnabled();
        splitsSettings.MultiNameDisplayController.ResetVisibleNames();
    }

    private void dmnDisplayTime_ValueChanged(object sender, EventArgs e)
    {
        if ((float)dmnDisplayTime.Value != defaultDisplayTime)
        {
            setOverrideDisplayTime(true);
        }

        DisplayTime = (float)dmnDisplayTime.Value;

        splitsSettings.MultiNameDisplayController.ResetInfo();
        splitsSettings.MultiNameTime_Notice();
    }

    private void btnTextColor_Click(object sender, EventArgs e)
    {
        Color initialColor = OverrideTextColor ? btnTextColor.BackColor : Color.FromArgb(255, 0, 0, 0);
        
        var picker = new ColorPickerDialog();
        picker.SelectedColorChanged += (s, x) => initialColor = picker.SelectedColor;
        picker.SelectedColor = picker.OldColor = initialColor;
        
        if (picker.ShowDialog(this) == DialogResult.OK)
        {
            setOverrideTextColor(true);
            btnTextColor.BackColor = picker.SelectedColor;
            splitsSettings.MultiNameDisplayController.ResetInfo();
        }
    }

    private void btnTextFont_Click(object sender, EventArgs e)
    {
        CustomFontDialog.FontDialog dialog = SettingsHelper.GetFontDialog(TextFont, 11, 26);
        dialog.FontChanged += (s, ev) => TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            setOverrideTextFont(true);
            splitsSettings.MultiNameDisplayController.ResetInfo();
        }

        btnTextFont.Text = TextFontString;
    }

    private void btnMoveUp_Click(object sender, EventArgs e)
    {
        MovedUp?.Invoke(this, null);
    }

    private void btnMoveDown_Click(object sender, EventArgs e)
    {
        MovedDown?.Invoke(this, null);
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(
            "Are you sure you want to clear the settings for the " 
            + lblNth.Text 
            + " item?", "Confirm Removal", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes
        )
        {
            init();
            splitsSettings.MultiNameDisplayController.ResetInfo();
        }
    }

    private void setOverrideDisplayTime(bool value)
    {
        OverrideDisplayTime = value;
        dmnDisplayTime.ForeColor = OverrideDisplayTime ? Color.Black : Color.Gray;

         if (!OverrideDisplayTime)
        {
            dmnDisplayTime.Value = (decimal)defaultDisplayTime;
        }
    }

    private void setOverrideTextColor(bool value)
    {
        OverrideTextColor = value;
        btnTextColor.FlatAppearance.BorderColor = OverrideTextColor ? Color.Black : Color.Gray;

        if (!OverrideTextColor)
        {
            btnTextColor.BackColor = defaultTextColor;
        }
    }

    private void setOverrideTextFont(bool value)
    {
        OverrideTextFont = value;
        btnTextFont.FlatAppearance.BorderColor = OverrideTextFont ? Color.Black : Color.Gray;
        btnTextFont.ForeColor = OverrideTextFont ? Color.Black : Color.Gray;

        if (!OverrideTextFont)
        {
            TextFont = defaultTextFont;
            btnTextFont.Text = TextFontString;
        }
    }

    public void SetNth(int nth)
    {
        static string toOrdinal(int number)
        {
            if (number <= 0)
            {
                return number.ToString();
            }

            int lastTwoDigits = number % 100;
            int lastDigit = number % 10;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
            {
                return number + "th";
            }

            switch (lastDigit)
            {
                case 1: return number + "st";
                case 2: return number + "nd";
                case 3: return number + "rd";
                default: return number + "th";
            }
        }
        
        lblNth.Text = toOrdinal(nth);

        btnMoveUp.Enabled = nth != 1;
        btnMoveDown.Enabled = nth != DetailsList.Count;
    }

    public System.Windows.Forms.CheckBox GetChkIsVisible()
    {
        return chkIsVisible;
    }

    public System.Windows.Forms.NumericUpDown GetDmnDisplayTime()
    {
        return dmnDisplayTime;
    }

    public void SetDefaultDisplayTime(decimal value)
    {
        if (!OverrideDisplayTime)
        {
            dmnDisplayTime.Value = value;
        }
    }

    public void UpdateEnabledButtons()
    {
        btnMoveDown.Enabled = Index < RowsCount - 1;
        btnMoveUp.Enabled = Index > 0;
    }
}
