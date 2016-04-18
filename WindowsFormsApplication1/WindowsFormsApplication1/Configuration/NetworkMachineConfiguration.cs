using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
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
