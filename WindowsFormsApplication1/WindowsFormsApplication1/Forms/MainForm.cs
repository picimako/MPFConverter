using System;
using System.Windows.Forms;
using System.IO;
using MPFConverterApp.Forms;

namespace MPFConverterApp
{
    public partial class MainForm : Form
    {
        private ConfigurationSwitcher configSwitcher;
        private ConfigurationControls controls;
        private FormValidator validator;

        public MainForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetUpConfigurationControls();
            configSwitcher = new ConfigurationSwitcher(controls);
            validator = new FormValidator(controls);
            SetToolTips();
            SetFormItemsStatus();
            SetFormItemValues();
            SetEventHandlers();
        }

        private void SetUpConfigurationControls()
        {
            controls = new ConfigurationControls();
            controls.Comment = commentTextBox;
            controls.GQ = gqCheckBox;
            controls.ID = idBox;
            controls.INeeded = iNeededCheckbox;
            controls.Kiallas = kiallasBox;
            controls.KiallasX = xValueBox;
            controls.KiallasY = yValueBox;
            controls.KiallasZ = zValueBox;
            controls.Osztofej = osztofejBox;
            controls.OsztofejValue = osztofejValueBox;
            controls.G650 = g650CheckBox;
            controls.M8 = m8CheckBox;
        }

        private void SetEventHandlers()
        {
            mitTextBox.TextChanged += (sender, e) => { SetKeszitButton(); };
            idBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
            };
            osztofejValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
            };
            osztofejBox.CheckedChanged += (sender, e) =>
            {
                SetKeszitButton();
                osztofejValueBox.Enabled = !osztofejValueBox.Enabled;
                osztofejValueBox.Text = String.Empty;
                iNeededCheckbox.Enabled = osztofejBox.Checked;
            };

            kiallasBox.CheckedChanged += (sender, e) =>
            {
                SetKeszitButton();
                xValueBox.Enabled = yValueBox.Enabled = zValueBox.Enabled = !xValueBox.Enabled;
                xValueBox.Text = yValueBox.Text = zValueBox.Text = String.Empty;
                g650CheckBox.Enabled = !kiallasBox.Checked;
            };
            xValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
            };
            yValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
            };
            zValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
            };
            g650CheckBox.CheckedChanged += (sender, e) => 
            {
                kiallasBox.Enabled = !g650CheckBox.Checked;
            };
        }

        private void SetFormItemsStatus()
        {
            ControlUtil.DisableControls(keszitButton, iNeededCheckbox, hovaTextBox, osztofejValueBox, xValueBox, yValueBox, zValueBox);
            doneLabel.Visible = false;
        }

        private void SetFormItemValues()
        {
            doneLabel.Text = "Az NCT fájl készítése véget ért.";
            idBox.Minimum = 1000;
            idBox.Maximum = 9999;
        }

        private void SetToolTips()
        {
            ToolTipInitializer.SetToolTipForGqCheckbox(gqCheckBox);
            ToolTipInitializer.SetToolTipForHovaLabel(hovaLabel);
            ToolTipInitializer.SetToolTipForIdBox(idBox);
            ToolTipInitializer.SetToolTipForCommentTextBox(commentTextBox);
            ToolTipInitializer.SetToolTipForM8CheckBox(m8CheckBox);
        }

        private void SetKeszitButton()
        {
            keszitButton.Enabled = (
                validator.IsProgramIdValid()
                && validator.IsOsztofejAngleValid()
                && validator.AreKiallasValuesValid(xValueBox, yValueBox, zValueBox)
                && !validator.IsEmpty(mitTextBox.Text)
                && File.Exists(mitTextBox.Text)) ? true : false;
        }

        private void keszitButton_Click(object sender, EventArgs e)
        {
            Converter converter = new Converter(doneLabel);
            converter.ConvertFromMpfToNct(mitTextBox.Text, hovaTextBox.Text);

            DoAfterCreation();
        }

        private void DoAfterCreation()
        {
            if (++validator.ProgramId > (Int32)idBox.Maximum)
            {
                validator.ProgramId = (Int32)idBox.Minimum;
            }
            idBox.Text = validator.ProgramId.ToString();
            mitTextBox.Text = String.Empty;
        }

        private void tallozButton_Click(object sender, EventArgs e)
        {
            doneLabel.Visible = false;
            OpenFileDialog tallozDialog = new MPFOpenFileDialogFactory().CreateDialog();
            if (tallozDialog.ShowDialog() == DialogResult.OK)
            {
                mitTextBox.Text = tallozDialog.FileName;
                hovaTextBox.Text = @"D:\NCT\" + Path.GetFileNameWithoutExtension(tallozDialog.FileName) + ".nct";
            }
        }
    }
}
