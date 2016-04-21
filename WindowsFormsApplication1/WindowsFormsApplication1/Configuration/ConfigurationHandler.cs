using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MPFConverterApp.Configuration
{
    class ConfigurationHandler
    {
        public Dictionary<NetworkMachineConfiguration, NCTConfiguration> Configurations { get; set; }

        public ConfigurationHandler()
        {
            this.Configurations = new Dictionary<NetworkMachineConfiguration, NCTConfiguration>();
        }

        public NetworkMachineConfiguration GetNetworkConfigForSelectedNetwork()
        {
            return Configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton.Checked).FirstOrDefault();
        }

        public NetworkMachineConfiguration GetNetworkConfigForRadioButton(RadioButton radioButton)
        {
            return Configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton == radioButton).FirstOrDefault();
        }

        public NCTConfiguration GetNCTConfigForSelectedNetwork()
        {
            return Configurations[GetNetworkConfigForSelectedNetwork()];
        }

        public NCTConfiguration GetNCTConfigForRadioButton(RadioButton radioButton)
        {
            return Configurations[GetNetworkConfigForRadioButton(radioButton)];
        }
    }
}
