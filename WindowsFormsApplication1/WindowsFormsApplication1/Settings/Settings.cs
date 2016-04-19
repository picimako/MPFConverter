using System.Collections.Generic;

namespace MPFConverterApp
{
    class Settings
    {
        private static Settings SETTINGS;
        private string gqOn;
        private string gqOff;
        private string applicationStartupPath;
        public List<KeyValuePair<string, string>> MachineBaseTargetFolders { get; set; }

        private Settings()
        {
            MachineBaseTargetFolders = new List<KeyValuePair<string, string>>();
        }

        public static Settings settings
        {
            get
            {
                if (SETTINGS == null)
                {
                    SETTINGS = new Settings();
                }
                return SETTINGS;
            }
        }

        public void setGQOn(string onValue)
        {
            gqOn = onValue;
        }

        public string getGQOn()
        {
            return gqOn;
        }

        public void setGQOff(string offValue)
        {
            gqOff = offValue;
        }

        public string getGQOff()
        {
            return gqOff;
        }

        public void setApplicationStartupPath(string path)
        {
            this.applicationStartupPath = path;
        }

        public string getApplicationStartupPath()
        {
            return applicationStartupPath;
        }
    }
}
