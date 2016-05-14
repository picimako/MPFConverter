using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using MPFConverterApp.Forms;
using MPFConverterApp.Util;
using MPFConverterApp.Configuration;

namespace MPFConverterApp
{
    public partial class MainForm : Form
    {
        private const int NETWORK_FOLDER_HEIGHT = 37;

        private ConfigurationSwitcher configSwitcher;
        private ConfigurationControls controls;
        private ConfigurationHandler configHandler;
        private FormValidator validator;

        public MainForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetUpConfigurationControls();
            configHandler = new ConfigurationHandler();
            configSwitcher = new ConfigurationSwitcher(controls);
            SetNetworkMachines();
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

        private void SetNetworkMachines()
        {
            NetworkFormControlFactory factory = new NetworkFormControlFactory(configHandler, configSwitcher);

            for (int currentFolder = 0; currentFolder < Settings.Instance.MachineBaseTargetFolders.Count; currentFolder++)
            {
                RadioButton radioButton = factory.CreateRadioButtonFor(currentFolder);
                TextBox baseTargetFolder = factory.CreateBaseTargetFolderTextBoxFor(currentFolder);
                TextBox targetFolder = factory.CreateTargetFolderTextBoxFor(currentFolder);            

                if (currentFolder > 0)
                {
                    targetFolder.Enabled = false;
                }

                if (currentFolder > 1)
                {
                    this.Height += NETWORK_FOLDER_HEIGHT;
                    networkMachinesGroupBox.Height += NETWORK_FOLDER_HEIGHT;
                    networkMachineNamesGroupBox.Height += NETWORK_FOLDER_HEIGHT;
                }
                networkMachineNamesGroupBox.Controls.Add(radioButton);
                networkMachinesGroupBox.Controls.Add(baseTargetFolder);
                networkMachinesGroupBox.Controls.Add(targetFolder);
                configHandler.Configurations.Add(
                    new NetworkMachineConfiguration(radioButton, baseTargetFolder, targetFolder),
                    new NCTConfiguration());
            }
        }

        private void SetEventHandlers()
        {
            mitTextBox.TextChanged += (sender, e) => { SetKeszitButton(); };
            idBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().ProgramId = validator.ProgramId;
            };
            osztofejValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Osztofej.Value = osztofejValueBox.Text;
            };
            osztofejBox.CheckedChanged += (sender, e) =>
            {
                SetKeszitButton();
                osztofejValueBox.Enabled = !osztofejValueBox.Enabled;
                osztofejValueBox.Text = String.Empty;
                iNeededCheckbox.Enabled = osztofejBox.Checked;
                configHandler.GetNCTConfigForSelectedNetwork().Osztofej.Enabled = osztofejBox.Checked;
            };
            commentTextBox.TextChanged += (sender, e) => { configHandler.GetNCTConfigForSelectedNetwork().Comment = commentTextBox.Text; };

            iNeededCheckbox.CheckedChanged += (sender, e) => { configHandler.GetNCTConfigForSelectedNetwork().INeeded = iNeededCheckbox.Checked; };

            gqCheckBox.CheckedChanged += (sender, e) => { configHandler.GetNCTConfigForSelectedNetwork().GQHSHPNeeded = gqCheckBox.Checked; };

            kiallasBox.CheckedChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Enabled = kiallasBox.Checked;
                xValueBox.Enabled = yValueBox.Enabled = zValueBox.Enabled = !xValueBox.Enabled;
                xValueBox.Text = yValueBox.Text = zValueBox.Text = String.Empty;
                g650CheckBox.Enabled = !kiallasBox.Checked;
            };
            xValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.X = xValueBox.Text;
            };
            yValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Y = yValueBox.Text;
            };
            zValueBox.TextChanged += (sender, e) =>
            {
                SetKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Z = zValueBox.Text;
            };
            g650CheckBox.CheckedChanged += (sender, e) => 
            {
                configHandler.GetNCTConfigForSelectedNetwork().G650Needed = g650CheckBox.Checked;
                kiallasBox.Enabled = !g650CheckBox.Checked;
            };

            m8CheckBox.CheckedChanged += (sender, e) => { configHandler.GetNCTConfigForSelectedNetwork().M8Needed = m8CheckBox.Checked; };
        }

        private void SetFormItemsStatus()
        {
            ControlUtil.DisableControls(keszitButton, iNeededCheckbox, hovaTextBox, osztofejValueBox, xValueBox, yValueBox, zValueBox);
            doneLabel.Visible = false;
            configHandler.Configurations.First().Key.RadioButton.Checked = true;
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
            string networkTargetFolder =
                configHandler.GetNetworkConfigForSelectedNetwork().BaseTargetFolder +
                configHandler.GetNetworkConfigForSelectedNetwork().TargetFolder.Text;

            Converter converter = new Converter(doneLabel);
            converter.NCTConfiguration = configHandler.GetNCTConfigForSelectedNetwork();
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

        private void networkFolderBrowseButton_Click(object sender, EventArgs e)
        {
            string baseSelectedPath = configHandler.GetNetworkConfigForSelectedNetwork().BaseTargetFolder;
            FolderBrowserDialog networkDialog = new NetworkFolderBrowserDialogFactory().CreateDialogFor(baseSelectedPath);
            if (!"NETWORK_FOLDER_NOT_FOUND".Equals(networkDialog.Tag) && networkDialog.ShowDialog() == DialogResult.OK)
            {
                int startCharacterIndexOfFolder = networkDialog.SelectedPath.LastIndexOf(baseSelectedPath) + baseSelectedPath.Length;
                string targetFolderText = Path.GetDirectoryName(networkDialog.SelectedPath + "\\").Substring(startCharacterIndexOfFolder);
                configHandler.GetNetworkConfigForSelectedNetwork().TargetFolder.Text = targetFolderText;
            }
        }

        private void openLogFileButton_Click(object sender, EventArgs e)
        {
            FileOpener.OpenLogFile();
        }
    }
}
