using System.IO;

namespace MPFConverterApp
{
    class Logger
    {
        private static Logger LOGGER;
        private static string logFilePath = Settings.Instance.ApplicationStartupPath + @"\log.txt";
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

        public void LogComment(string comment)
        {
            writer.WriteLine(comment);
        }

        public void CloseLogger()
        {
            writer.Close();
        }

        public static string GetLogFilePath()
        {
            return logFilePath;
        }
    }
}
