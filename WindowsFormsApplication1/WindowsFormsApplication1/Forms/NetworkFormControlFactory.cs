using System.Drawing;
using System.Windows.Forms;
using MPFConverterApp.Configuration;

namespace MPFConverterApp.Forms
{
    class NetworkFormControlFactory
    {
        private const int radioX = 13, radioY = 17, radioYDiff = 32;
        private const int baseTargetFolderX = 130, baseTargetFolderY = 33, baseTargetFolderDiff = 33;
        private const int targetFolderX = 293, targetFolderY = 33, targetFolderDiff = 33;
        private ConfigurationHandler configHandler;
        private ConfigurationSwitcher configSwitcher;

        public NetworkFormControlFactory(ConfigurationHandler configHandler, ConfigurationSwitcher configSwitcher)
        {
            this.configHandler = configHandler;
            this.configSwitcher = configSwitcher;
        }

        public RadioButton CreateRadioButtonFor(int currentFolderNumber)
        {
            RadioButton radioButton = new RadioButton();
            radioButton.Location = new Point(radioX, radioY + currentFolderNumber * radioYDiff);
            radioButton.Text = Settings.Instance.MachineBaseTargetFolders[currentFolderNumber].Key;
            radioButton.CheckedChanged += (sender, e) =>
            {
                configHandler.GetNetworkConfigForRadioButton(radioButton).TargetFolder.Enabled = radioButton.Checked;

                /*A CheckedChanged kétszer fut le. Először arra a RadioButton-re, amiből ki lett szedve a pötty,
                * majd utána arra, amelyikbe be lett téve a pötty.*/
                if (radioButton.Checked)
                {
                    configSwitcher.ReadConfigFrom(configHandler.GetNCTConfigForSelectedNetwork());
                    configHandler.GetNetworkConfigForRadioButton(radioButton).TargetFolder.Text = configHandler.GetNCTConfigForSelectedNetwork().NetworkTargetFolder;
                }
                else
                {
                    configSwitcher.SaveConfigTo(configHandler.GetNCTConfigForRadioButton(radioButton), configHandler.GetNetworkConfigForRadioButton(radioButton).TargetFolder.Text);
                }
            };

            return radioButton;
        }

        public TextBox CreateBaseTargetFolderTextBoxFor(int currentFolderNumber)
        {
            TextBox baseTargetFolder = new TextBox();
            baseTargetFolder.Size = new Size(157, 20);
            baseTargetFolder.Location = new Point(baseTargetFolderX, baseTargetFolderY + currentFolderNumber * baseTargetFolderDiff);
            baseTargetFolder.Text = Settings.Instance.MachineBaseTargetFolders[currentFolderNumber].Value;
            baseTargetFolder.Enabled = false;

            return baseTargetFolder;
        }

        public TextBox CreateTargetFolderTextBoxFor(int currentFolderNumber)
        {
            TextBox targetFolder = new TextBox();
            targetFolder.Size = new Size(100, 20);
            targetFolder.Location = new Point(targetFolderX, targetFolderY + currentFolderNumber * targetFolderDiff);
            targetFolder.TextChanged += (sender, e) =>
            {
                configHandler.GetNCTConfigForSelectedNetwork().NetworkTargetFolder =
                    configHandler.GetNetworkConfigForSelectedNetwork().BaseTargetFolder +
                    configHandler.GetNetworkConfigForSelectedNetwork().TargetFolder.Text;
            };

            return targetFolder;
        }
    }
}
