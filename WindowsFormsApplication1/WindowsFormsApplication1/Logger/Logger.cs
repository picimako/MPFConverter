using System.IO;

namespace MPFConverterApp
{
    class Logger
    {
        private static Logger LOGGER;
        private static string logFilePath = Settings.settings.getApplicationStartupPath() + @"\log.txt";
        private static TextWriter writer;

        private Logger() { }

        public static Logger logger
        {
            get
            {
                if (LOGGER == null)
                {
                    LOGGER = new Logger();
                }
                writer = new StreamWriter(logFilePath, false);
                return LOGGER;
            }
        }

        public void logComment(string comment)
        {
            writer.WriteLine(comment);
        }

        public void closeLogger()
        {
            writer.Close();
        }

        public static string getLogFilePath()
        {
            return logFilePath;
        }
    }
}
