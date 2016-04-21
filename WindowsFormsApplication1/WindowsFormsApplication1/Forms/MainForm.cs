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
            setUpConfigurationControls();
            configHandler = new ConfigurationHandler();
            configSwitcher = new ConfigurationSwitcher(controls);
            setNetworkMachines();
            validator = new FormValidator(controls);
            setToolTips();
            setFormItemsStatus();
            setFormItemValues();
            setEventHandlers();
        }

        private void setUpConfigurationControls()
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
            controls.G30 = g30CheckBox;
        }

        private void setNetworkMachines()
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
                    groupBox1.Height += NETWORK_FOLDER_HEIGHT;
                    groupBox2.Height += NETWORK_FOLDER_HEIGHT;
                }
                groupBox2.Controls.Add(radioButton);
                groupBox1.Controls.Add(baseTargetFolder);
                groupBox1.Controls.Add(targetFolder);
                configHandler.Configurations.Add(
                    new NetworkMachineConfiguration(radioButton, baseTargetFolder, targetFolder),
                    new NCTConfiguration());
            }
        }

        private void setEventHandlers()
        {
            mitTextBox.TextChanged += (sender, e) => { setKeszitButton(); };
            idBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().ProgramId = validator.ProgramId;
            };
            osztofejValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Osztofej.Value = osztofejValueBox.Text;
            };
            osztofejBox.CheckedChanged += (sender, e) =>
            {
                setKeszitButton();
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
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Enabled = kiallasBox.Checked;
                xValueBox.Enabled = yValueBox.Enabled = zValueBox.Enabled = !xValueBox.Enabled;
                xValueBox.Text = yValueBox.Text = zValueBox.Text = String.Empty;
            };
            xValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.X = xValueBox.Text;
            };
            yValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Y = yValueBox.Text;
            };
            zValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                configHandler.GetNCTConfigForSelectedNetwork().Kiallas.Z = zValueBox.Text;
            };
            g30CheckBox.CheckedChanged += (sender, e) => { configHandler.GetNCTConfigForSelectedNetwork().G30Needed = g30CheckBox.Checked; };
        }

        private void setFormItemsStatus()
        {
            ControlUtil.DisableControls(keszitButton, iNeededCheckbox, hovaTextBox, osztofejValueBox, xValueBox, yValueBox, zValueBox);
            doneLabel.Visible = false;
            configHandler.Configurations.First().Key.RadioButton.Checked = true;
        }

        private void setFormItemValues()
        {
            doneLabel.Text = "Az NCT fájl készítése véget ért.";
            idBox.Minimum = 1000;
            idBox.Maximum = 9999;
        }

        private void setToolTips()
        {
            new ToolTip().SetToolTip(this.gqCheckBox, "Pipa: a nagypontosságú pályakövetés aktív (M30-ig).\nNincs pipa: kikapcsolt állapot.");

            new ToolTip().SetToolTip(this.hovaLabel, "A végeredmény fájl fixen a D:\\NCT\\ mappába fog kerülni.");

            new ToolTip().SetToolTip(this.idBox,
                "Az azonosító formátuma:\n"
                    + "- nem kezdődhet 0-val,\n"
                    + "- pontosan 4 számjegyből kell, hogy álljon,\n"
                    + "- csak szám szerepelhet benne,\n"
                    + "- nem lehet negatív szám.");

            new ToolTip().SetToolTip(this.commentTextBox, "Az azonosító után kerül bele zárójelbe.");
        }

        private void setKeszitButton()
        {
            keszitButton.Enabled = (
                validator.IsProgramIdValid()
                && validator.IsOsztofejAngleValid()
                && validator.AreKiallasValuesValid(xValueBox, yValueBox, zValueBox)
                && validator.IsPathNotEmpty(mitTextBox)
                && File.Exists(mitTextBox.Text)) ? true : false;
        }

        private void keszitButton_Click(object sender, EventArgs e)
        {
            //TODO: check why this is there, and whether this is necessary
            Osztofej osztofej = new Osztofej(osztofejBox.Checked, osztofejValueBox.Text);
            string networkTargetFolder =
                configHandler.GetNetworkConfigForSelectedNetwork().BaseTargetFolder +
                configHandler.GetNetworkConfigForSelectedNetwork().TargetFolder.Text;
            new Converter(doneLabel, gqCheckBox).ConvertFromMpfToNct(mitTextBox.Text, hovaTextBox.Text, configHandler.GetNCTConfigForSelectedNetwork());
            doAfterCreation();
        }

        private void doAfterCreation()
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
