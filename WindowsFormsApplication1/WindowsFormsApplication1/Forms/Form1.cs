using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using MPFConverterApp.Forms;
using MPFConverterApp.Util;

namespace MPFConverterApp
{
    public partial class MainForm : Form
    {
        private Dictionary<NetworkMachineConfiguration, NCTConfiguration> configurations;
        private ConfigurationSwitcher switcher;
        private ConfigurationControls controls;
        private FormValidator validator;

        public MainForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            setUpConfigurationControls();
            configurations = new Dictionary<NetworkMachineConfiguration, NCTConfiguration>(Settings.instance.MachineBaseTargetFolders.Count);
            setNetworkMachines();
            switcher = new ConfigurationSwitcher(controls);
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
            NetworkFormControlFactory factory = new NetworkFormControlFactory();

            for (int currentFolder = 0; currentFolder < Settings.instance.MachineBaseTargetFolders.Count; currentFolder++)
            {
                RadioButton radioButton = factory.CreateRadioButtonFor(currentFolder);
                //TODO: move this delegate as well to the factory
                radioButton.CheckedChanged += (sender, e) =>
                {
                    getNetworkConfigForRadioButton(radioButton).TargetFolder.Enabled = radioButton.Checked;
                    
                    /*A CheckedChanged kétszer fut le. Először arra a RadioButton-re, amiből ki lett szedve a pötty,
                    * majd utána arra, amelyikbe be lett téve a pötty.*/
                    if (radioButton.Checked)
                    {
                        switcher.readConfigFrom(getNCTConfigForSelectedNetwork());
                        getNetworkConfigForRadioButton(radioButton).TargetFolder.Text = getNCTConfigForSelectedNetwork().NetworkTargetFolder;
                    }
                    else
                    {
                        switcher.saveConfigTo(getNCTConfigForRadioButton(radioButton), getNetworkConfigForRadioButton(radioButton).TargetFolder.Text);
                    }
                };

                TextBox baseTargetFolder = factory.CreateBaseTargetFolderTextBoxFor(currentFolder);

                TextBox targetFolder = factory.CreateTargetFolderTextBoxFor(currentFolder);
                //TODO: move this delegate as well to the factory
                targetFolder.TextChanged += (sender, e) =>
                {
                    getNCTConfigForSelectedNetwork().NetworkTargetFolder = getNetworkConfigForSelectedNetwork().BaseTargetFolder +
                        getNetworkConfigForSelectedNetwork().TargetFolder.Text;
                };

                if (currentFolder > 0)
                {
                    targetFolder.Enabled = false;
                }

                if (currentFolder > 1)
                {
                    this.Height += 37;
                    groupBox1.Height += 37;
                    groupBox2.Height += 37;
                }
                groupBox2.Controls.Add(radioButton);
                groupBox1.Controls.Add(baseTargetFolder);
                groupBox1.Controls.Add(targetFolder);
                configurations.Add(
                    new NetworkMachineConfiguration(radioButton, baseTargetFolder.Text, targetFolder),
                    new NCTConfiguration());
            }
        }

        private void setEventHandlers()
        {
            mitTextBox.TextChanged += (sender, e) => { setKeszitButton(); };
            idBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().ProgramId = validator.ProgramId;
            };
            osztofejValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().Osztofej.Value = osztofejValueBox.Text;
            };
            osztofejBox.CheckedChanged += (sender, e) =>
            {
                setKeszitButton();
                osztofejValueBox.Enabled = !osztofejValueBox.Enabled;
                osztofejValueBox.Text = String.Empty;
                iNeededCheckbox.Enabled = osztofejBox.Checked;
                getNCTConfigForSelectedNetwork().Osztofej.Enabled = osztofejBox.Checked;
            };
            commentTextBox.TextChanged += (sender, e) => { getNCTConfigForSelectedNetwork().Comment = commentTextBox.Text; };

            iNeededCheckbox.CheckedChanged += (sender, e) => { getNCTConfigForSelectedNetwork().INeeded = iNeededCheckbox.Checked; };

            gqCheckBox.CheckedChanged += (sender, e) => { getNCTConfigForSelectedNetwork().GQHSHPNeeded = gqCheckBox.Checked; };

            kiallasBox.CheckedChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().Kiallas.Enabled = kiallasBox.Checked;
                xValueBox.Enabled = yValueBox.Enabled = zValueBox.Enabled = !xValueBox.Enabled;
                xValueBox.Text = yValueBox.Text = zValueBox.Text = String.Empty;
            };
            xValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().Kiallas.X = xValueBox.Text;
            };
            yValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().Kiallas.Y = yValueBox.Text;
            };
            zValueBox.TextChanged += (sender, e) =>
            {
                setKeszitButton();
                getNCTConfigForSelectedNetwork().Kiallas.Z = zValueBox.Text;
            };
            g30CheckBox.CheckedChanged += (sender, e) => { getNCTConfigForSelectedNetwork().G30Needed = g30CheckBox.Checked; };
        }

        private void setFormItemsStatus()
        {
            ControlUtil.DisableControls(keszitButton, iNeededCheckbox, hovaTextBox, osztofejValueBox, xValueBox, yValueBox, zValueBox);
            doneLabel.Visible = false;
            configurations.First<KeyValuePair<NetworkMachineConfiguration, NCTConfiguration>>().Key.RadioButton.Checked = true;
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
                validator.isProgramIdValid()
                && validator.isOsztofejAngleValid()
                && validator.areKiallasValuesValid(xValueBox, yValueBox, zValueBox)
                && validator.isPathNotEmpty(mitTextBox)
                && File.Exists(mitTextBox.Text)) ? true : false;
        }

        private void keszitButton_Click(object sender, EventArgs e)
        {
            Osztofej osztofej = new Osztofej(osztofejBox.Checked, osztofejValueBox.Text);
            string networkTargetFolder =
                getNetworkConfigForSelectedNetwork().BaseTargetFolder +
                getNetworkConfigForSelectedNetwork().TargetFolder.Text;
            Converter converter = new Converter(doneLabel, gqCheckBox);
            converter.convertFromMpfToNct(mitTextBox.Text, hovaTextBox.Text, getNCTConfigForSelectedNetwork());
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
            OpenFileDialog tallozDialog = MPFOpenFileDialogFactory.CreateDialog();
            if (tallozDialog.ShowDialog() == DialogResult.OK)
            {
                mitTextBox.Text = tallozDialog.FileName;
                hovaTextBox.Text = @"D:\NCT\" + Path.GetFileNameWithoutExtension(tallozDialog.FileName) + ".nct";
            }
        }

        private void networkFolderBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog networkFolderBrowserDialog = new FolderBrowserDialog();
            networkFolderBrowserDialog.Description = "Válassz célmappát:";
            string baseSelectedPath = getNetworkConfigForSelectedNetwork().BaseTargetFolder;
            try
            {
                networkFolderBrowserDialog.SelectedPath = FolderUtil.getFirstChildFolderIfExist(baseSelectedPath);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("A megadott hálózati elérési útvonal nem létezik.", "Érvénytelen útvonal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (networkFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                int startCharacterIndexOfFolder = networkFolderBrowserDialog.SelectedPath.LastIndexOf(baseSelectedPath) + baseSelectedPath.Length;
                string targetFolderText = Path.GetDirectoryName(networkFolderBrowserDialog.SelectedPath + "\\").Substring(startCharacterIndexOfFolder);
                getNetworkConfigForSelectedNetwork().TargetFolder.Text = targetFolderText;
            }
        }

        private void openLogFileButton_Click(object sender, EventArgs e)
        {
            FileOpener.OpenLogFile();
        }

        private NetworkMachineConfiguration getNetworkConfigForSelectedNetwork()
        {
            return configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton.Checked).FirstOrDefault();
        }
        
        private NetworkMachineConfiguration getNetworkConfigForRadioButton(RadioButton radioButton)
        {
            return configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton == radioButton).FirstOrDefault();
        }

        private NCTConfiguration getNCTConfigForSelectedNetwork()
        {
            return configurations[getNetworkConfigForSelectedNetwork()];
        }

        private NCTConfiguration getNCTConfigForRadioButton(RadioButton radioButton)
        {
            return configurations[getNetworkConfigForRadioButton(radioButton)];
        }
    }
}
