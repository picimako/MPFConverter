using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.MessageBox;

namespace MPFConverterApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Application.StartupPath + @"\settings.txt"))
                {
                    string line = reader.ReadLine();
                    Settings.Instance.GQOn = Split(line)[0];
                    Settings.Instance.GQOff = Split(line)[1];
                }
                Settings.Instance.ApplicationStartupPath = Application.StartupPath;
            }
            catch (FileNotFoundException e)
            {
                Show(String.Format("A settings.txt nem található a következő helyen:\n{0}", e.FileName));
                return;
            }
            catch (IOException e)
            {
                Show(String.Format("Hiba történt a settings.txt beolvasása közben.\n\n{0}", e.StackTrace));
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static string[] Split(string line)
        {
            return line.Split("#".ToCharArray());
        }
    }
}
