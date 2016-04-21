using System.Diagnostics;
using System.IO;
using static System.Windows.Forms.MessageBox;

namespace MPFConverterApp.Util
{
    class FileOpener
    {
        public static void OpenLogFile()
        {
            if (File.Exists(Logger.GetLogFilePath()))
            {
                Process.Start(Logger.GetLogFilePath());
            }
            else
            {
                Show("A naplófájl nem található.");
            }
        }
    }
}
