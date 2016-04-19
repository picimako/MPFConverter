using System.Drawing;
using System.Windows.Forms;

namespace MPFConverterApp.Forms
{
    class NetworkFormControlFactory
    {
        const int radioX = 13, radioY = 17, radioYDiff = 32;
        const int baseTargetFolderX = 130, baseTargetFolderY = 33, baseTargetFolderDiff = 33;
        const int targetFolderX = 293, targetFolderY = 33, targetFolderDiff = 33;

        public RadioButton CreateRadioButtonFor(int currentFolderNumber)
        {
            RadioButton radioButton = new RadioButton();
            radioButton.Location = new Point(radioX, radioY + currentFolderNumber * radioYDiff);
            radioButton.Text = Settings.settings.MachineBaseTargetFolders[currentFolderNumber].Key;
            return radioButton;
        }

        public TextBox CreateBaseTargetFolderTextBoxFor(int currentFolderNumber)
        {
            TextBox baseTargetFolder = new TextBox();
            baseTargetFolder.Size = new Size(157, 20);
            baseTargetFolder.Location = new Point(baseTargetFolderX, baseTargetFolderY + currentFolderNumber * baseTargetFolderDiff);
            baseTargetFolder.Text = Settings.settings.MachineBaseTargetFolders[currentFolderNumber].Value;
            baseTargetFolder.Enabled = false;

            return baseTargetFolder;
        }

        public TextBox CreateTargetFolderTextBoxFor(int currentFolderNumber)
        {
            TextBox targetFolder = new TextBox();
            targetFolder.Size = new Size(100, 20);
            targetFolder.Location = new Point(targetFolderX, targetFolderY + currentFolderNumber * targetFolderDiff);

            return targetFolder;
        }
    }
}
