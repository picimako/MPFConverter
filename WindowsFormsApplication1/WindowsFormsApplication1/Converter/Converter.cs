using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using MPFConverterApp.Configuration;

namespace MPFConverterApp
{
    class Converter
    {
        private const string MPF_FOLDER = @"D:\MPF\";
        private const string G650VALUE = "G650";
        private const string M30 = "M30";

        private Label doneLabel;
        private TextWriter writer;
        private Logger logger = Logger.Instance;
        private Regex semiColonedRowPattern = new Regex(@"(\w*)(;)(.*)");

        public NCTConfiguration NCTConfiguration { get; set; }

        public Converter(Label doneLabel)
        {
            this.doneLabel = doneLabel;
        }

        public void ConvertFromMpfToNct(string mpfFile, string nctFile)
        {
            logger.Open();
            FolderUtil.CreateDirectoryIfNotExists(MPF_FOLDER);
            string middleNctFile = MPF_FOLDER + Path.GetFileName(nctFile);

            using (StreamReader reader = new StreamReader(mpfFile))
            {
                string line;
                writer = new StreamWriter(middleNctFile, false);
                WriteProgramIdAndComment();
                WriteOsztofejValue();
                WriteGQOn();
                //while ((line = reader.ReadLine()) != null)
                while (!M30.Equals(line = reader.ReadLine()))
                {
                    string final = PutSemicolonedPartOfRowsIntoBrackets(line);
                    WriteGQOffAtFileEnd(final);
                    WriteXYZ(final);
                    writer.WriteLine(final);
                }
                WriteG650BeforeM30();
                WriteFileClosing();
                writer.Close();

                MiddleToFinalNctConverter finalNctConverter = new MiddleToFinalNctConverter(doneLabel);
                finalNctConverter.NCTConfiguration = NCTConfiguration;
                finalNctConverter.NetworkTargetFolder = @NCTConfiguration.NetworkTargetFolder;
                finalNctConverter.ConvertMiddleNctToFinalNct(middleNctFile);
            }
        }

        private string PutSemicolonedPartOfRowsIntoBrackets(string line)
        {
            string final = line;
            Match match = semiColonedRowPattern.Match(line);
            if (match.Success)
            {
                logger.LogComment("Aktuális sor zárójelezése: " + line);
                final = String.Format("{0}{1}", match.Groups[1], "(" + match.Groups[3] + ")");
                logger.LogComment("Aktuális sor zárójelekkel: " + final);
            }
            return final;
        }

        private void WriteProgramIdAndComment()
        {
            logger.LogComment("Program azonosító: " + NCTConfiguration.ProgramId);
            writer.WriteLine(String.Format("%O{0} ({1})", NCTConfiguration.ProgramId, NCTConfiguration.Comment));
        }

        private void WriteOsztofejValue()
        {
            Osztofej osztofej = NCTConfiguration.Osztofej;
            if (osztofej.Enabled)
            {
                logger.LogComment("Osztófej érték: A" + osztofej.Value);
                writer.WriteLine(String.Format("A{0}{1}", NCTConfiguration.INeeded ? "I" : "", osztofej.Value));
            }
        }

        private void WriteGQOn()
        {
            if (NCTConfiguration.GQHSHPNeeded)
            {
                logger.LogComment("GQ kezdőérték: " + Settings.Instance.GQOn);
                writer.WriteLine(Settings.Instance.GQOn);
            }
        }

        //TODO: this is currently not working. Will be changed in a later version.
        //Np problem if missing because M30 will set this one to default as well.
        //Before M30
        private void WriteGQOffAtFileEnd(string final)
        {
            if (NCTConfiguration.GQHSHPNeeded && M30.Equals(final))
            {
                logger.LogComment("GQ záróérték: " + Settings.Instance.GQOff);
                writer.WriteLine(Settings.Instance.GQOff);
            }
        }

        //TODO: this is currently not working. Beszélni Apával.
        //Közvetlenül M30 elé.
        //Vagy ez (G0) vagy a G650 kell, hogy bekerüljön
        //Ha az egyiket bepipálom, akkor a másik inaktív legyen
        private void WriteXYZ(string final)
        {
            Kiallas kiallas = NCTConfiguration.Kiallas;
            if (kiallas.Enabled && M30.Equals(final))
            {
                logger.LogComment(String.Format("XYZ értékek - X: {0}, Y: {1}, Z: {2}", kiallas.X, kiallas.Y, kiallas.Z));
                writer.WriteLine(String.Format("G0 Z{0}", kiallas.Z));
                writer.WriteLine(String.Format("G0 X{0} Y{1}", kiallas.X, kiallas.Y));
            }
        }

        private void WriteG650BeforeM30()
        {
            if (NCTConfiguration.G650Needed)
            {
                writer.WriteLine(G650VALUE);
            }
        }

        private void WriteFileClosing()
        {
            writer.WriteLine(M30);
            writer.Write("%");
        }
    }
}
