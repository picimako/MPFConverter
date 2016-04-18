using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
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
                    Settings.settings.setGQOn(splittedLine(line)[0]);
                    Settings.settings.setGQOff(splittedLine(line)[1]);

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lineElements = splittedLine(line);
                        Settings.settings.MachineBaseTargetFolders.Add(new KeyValuePair<string ,string>(lineElements[0], lineElements[1]));
                    }
                    //line = reader.ReadLine();
                    //Settings.settings.setMachineOneBaseTargetFolder(line);

                    //line = reader.ReadLine();
                    //Settings.settings.setMachineTwoBaseTargetFolder(line);
                }
                Settings.settings.setApplicationStartupPath(Application.StartupPath);
            }
            catch (FileNotFoundException fnfe)
            {
                MessageBox.Show(String.Format("A settings.txt nem található a következő helyen:\n{0}", fnfe.FileName));
                return;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(String.Format("Hiba történt a settings.txt beolvasása közben.\n\n{0}", ioe.StackTrace));
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static string[] splittedLine(string line)
        {
            return line.Split("#".ToCharArray());
        }
    }
}
