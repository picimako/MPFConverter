using System.Collections.Generic;

namespace MPFConverterApp
{
    class Settings
    {
        private static Settings INSTANCE;
        public string GQOn { get; set; }
        public string GQOff { get; set; }
        public string ApplicationStartupPath { get; set; }
        public List<KeyValuePair<string, string>> MachineBaseTargetFolders { get; set; }

        private Settings()
        {
            MachineBaseTargetFolders = new List<KeyValuePair<string, string>>();
        }

        public static Settings Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    INSTANCE = new Settings();
                }
                return INSTANCE;
            }
        }
    }
}
