
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1.Util
{
    class FileOpener
    {
        public static void openLogFile()
        {
            if (File.Exists(Logger.getLogFilePath()))
            {
                Process.Start(Logger.getLogFilePath());
            }
            else
            {
                MessageBox.Show("A naplófájl nem található.");
            }
        }
    }
}
