using System.Diagnostics;
using System.IO;
using static System.Windows.Forms.MessageBox;

namespace MPFConverterApp.Util
{
    class FileOpener
    {
        public static void OpenLogFile()
        {
            if (File.Exists(Logger.getLogFilePath()))
            {
                Process.Start(Logger.getLogFilePath());
            }
            else
            {
                Show("A naplófájl nem található.");
            }
        }
    }
}
