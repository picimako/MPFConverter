﻿using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.MessageBox;

namespace MPFConverterApp
{
    class FileCopier
    {
        public static void CopyNCTFileToNetworkFolder(string networkTargetFolder, string newNctFile, Logger logger)
        {
            if (!Directory.Exists(networkTargetFolder))
            {
                Show("A hálózaton levő mappa nem létezik vagy a számítógép nem elérhető!");
                return;
            }
            string finalNetworkTarget = networkTargetFolder + "\\" + Path.GetFileName(newNctFile);
            logger.LogComment("Az elkészített NCT fájl [" + finalNetworkTarget + "] másolásának megkezdése a hálózatra.");

            if (File.Exists(finalNetworkTarget))
            {
                DialogResult result = Show("A fájl már létezik a célhelyen. Felül szeretnéd írni?", "Értesítés.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    File.Delete(finalNetworkTarget);
                }
                else if (result == DialogResult.No)
                {
                    return;
                }
            }
            File.Copy(newNctFile, finalNetworkTarget);
            logger.LogComment("Az elkészített NCT fájl sikeresen átmásolva a megadott hálózati helyre.");
        }
    }
}
