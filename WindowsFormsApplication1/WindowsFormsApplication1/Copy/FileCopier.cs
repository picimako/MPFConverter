using System.IO;
using System.Windows.Forms;

namespace MPFConverterApp
{
    class FileCopier
    {
        public static void copyNCTFileToNetworkFolder(string networkTargetFolder, string newNctFile, Logger logger)
        {
            if (!Directory.Exists(networkTargetFolder))
            {
                MessageBox.Show("A hálózaton levő mappa nem létezik vagy a számítógép nem elérhető!");
                return;
            }
            string finalNetworkTarget = networkTargetFolder + "\\" + Path.GetFileName(newNctFile);
            logger.logComment("Az elkészített NCT fájl [" + finalNetworkTarget + "] másolásának megkezdése a hálózatra.");

            if (File.Exists(finalNetworkTarget))
            {
                DialogResult result = MessageBox.Show("A fájl már létezik a célhelyen. Felül szeretnéd írni?", "Értesítés.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
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
            logger.logComment("Az elkészített NCT fájl sikeresen átmásolva a megadott hálózati helyre.");
        }
    }
}
