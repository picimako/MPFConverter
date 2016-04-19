using System.Windows.Forms;

namespace MPFConverterApp
{
    class NetworkMachineConfiguration
    {
        public RadioButton RadioButton { get; set; }
        public string BaseTargetFolder { get; set; }
        public TextBox TargetFolder { get; set; }

        public NetworkMachineConfiguration(RadioButton radioButton, string baseTargetFolder, TextBox targetFolder)
        {
            RadioButton = radioButton;
            BaseTargetFolder = baseTargetFolder;
            TargetFolder = targetFolder;
        }
    }
}
