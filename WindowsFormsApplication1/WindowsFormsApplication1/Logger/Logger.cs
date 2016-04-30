using System.IO;

namespace MPFConverterApp
{
    class Logger
    {
        private static Logger INSTANCE;
        private static string logFilePath = Settings.Instance.ApplicationStartupPath + @"\log.txt";
        private static TextWriter writer;

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    INSTANCE = new Logger();
                }
                return INSTANCE;
            }
        }

        public void Open()
        {
            writer = new StreamWriter(logFilePath, false);
        }

        public void LogComment(string comment)
        {
            writer.WriteLine(comment);
        }

        public void Close()
        {
            writer.Close();
        }

        public static string GetLogFilePath()
        {
            return logFilePath;
        }
    }
}
