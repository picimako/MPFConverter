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

        public NetworkMachineConfiguration getNetworkConfigForSelectedNetwork()
        {
            return Configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton.Checked).FirstOrDefault();
        }

        public NetworkMachineConfiguration getNetworkConfigForRadioButton(RadioButton radioButton)
        {
            return Configurations.Keys.ToList().Where(netConfig => netConfig.RadioButton == radioButton).FirstOrDefault();
        }

        public NCTConfiguration getNCTConfigForSelectedNetwork()
        {
            return Configurations[getNetworkConfigForSelectedNetwork()];
        }

        public NCTConfiguration getNCTConfigForRadioButton(RadioButton radioButton)
        {
            return Configurations[getNetworkConfigForRadioButton(radioButton)];
        }
    }
}
