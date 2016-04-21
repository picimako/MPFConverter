using System.IO;

namespace MPFConverterApp
{
    class FolderUtil
    {
        public static void CreateDirectoryIfNotExists(string path, Logger logger)
        {
            logger.logComment("A következő útvonal elérhetőségének vizsgálata: " + path);
            if (!Directory.Exists(path))
            {
                logger.logComment("A következő mappa létrehozása:" + path);
                Directory.CreateDirectory(path);
            }
            else
            {
                logger.logComment("A mappa már létezik.");
            }
        }

        public static string GetFirstChildFolderIfExist(string folder)
        {
            DirectoryInfo sub = new DirectoryInfo(folder);
            if (sub.GetDirectories().Length > 0)
            {
                sub = sub.GetDirectories()[0];
            }
            return sub.FullName;
        }
    }
}
