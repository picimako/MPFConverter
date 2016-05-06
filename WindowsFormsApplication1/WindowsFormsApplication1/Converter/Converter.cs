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
        private const string G30VALUE = "G30ZI0P4"; //to be changed to G650
        private const string M30 = "M30";

        private Label doneLabel;
        private CheckBox gqCheckBox;
        private Logger logger = Logger.Instance;
        private Regex semiColonedRowPattern = new Regex(@"(\w*)(;)(.*)");

        public NCTConfiguration NCTConfiguration { get; set; }

        public Converter(Label doneLabel, CheckBox gqCheckBox)
        {
            this.doneLabel = doneLabel;
            this.gqCheckBox = gqCheckBox;
        }

        public void ConvertFromMpfToNct(string mpfFile, string nctFile)
        {
            logger.Open();
            FolderUtil.CreateDirectoryIfNotExists(MPF_FOLDER);
            string middleNctFile = MPF_FOLDER + Path.GetFileName(nctFile);

            using (StreamReader reader = new StreamReader(mpfFile))
            {
                string line;
                TextWriter writer = new StreamWriter(middleNctFile, false);
                WriteProgramIdAndComment(writer);
                WriteOsztofejValue(writer);
                WriteGQOn(writer);
                //while ((line = reader.ReadLine()) != null)
                while (!M30.Equals(line = reader.ReadLine()))
                {
                    string final = PutSemicolonedPartOfRowsIntoBrackets(line);
                    WriteGQOffAtFileEnd(writer, final);
                    WriteXYZ(writer, final);
                    writer.WriteLine(final);
                }
                WriteG30BeforeM30(writer);
                WriteFileClosing(writer);
                writer.Close();

                MiddleToFinalNctConverter finalNctConverter = new MiddleToFinalNctConverter(doneLabel, gqCheckBox);
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

        private void WriteProgramIdAndComment(TextWriter writer)
        {
            logger.LogComment("Program azonosító: " + NCTConfiguration.ProgramId);
            writer.WriteLine(String.Format("%O{0} ({1})", NCTConfiguration.ProgramId, NCTConfiguration.Comment));
        }

        private void WriteOsztofejValue(TextWriter writer)
        {
            Osztofej osztofej = NCTConfiguration.Osztofej;
            if (osztofej.Enabled)
            {
                logger.LogComment("Osztófej érték: A" + osztofej.Value);
                writer.WriteLine(String.Format("A{0}{1}", NCTConfiguration.INeeded ? "I" : "", osztofej.Value));
            }
        }

        private void WriteGQOn(TextWriter writer)
        {
            if (gqCheckBox.Checked)
            {
                logger.LogComment("GQ kezdőérték: " + Settings.Instance.GQOn);
                writer.WriteLine(Settings.Instance.GQOn);
            }
        }

        //TODO: this is currently not working. Will be changed in a later version.
        //Np problem if missing because M30 will set this one to default as well.
        //Before M30
        private void WriteGQOffAtFileEnd(TextWriter writer, string final)
        {
            if (gqCheckBox.Checked && M30.Equals(final))
            {
                logger.LogComment("GQ záróérték: " + Settings.Instance.GQOff);
                writer.WriteLine(Settings.Instance.GQOff);
            }
        }

        //TODO: this is currently not working. Beszélni Apával.
        //Közvetlenül M30 elé.
        //Vagy ez (G0) vagy a G650 kell, hogy bekerüljön
        //Ha az egyiket bepipálom, akkor a másik inaktív legyen
        private void WriteXYZ(TextWriter writer, string final)
        {
            Kiallas kiallas = NCTConfiguration.Kiallas;
            if (kiallas.Enabled && M30.Equals(final))
            {
                logger.LogComment(String.Format("XYZ értékek - X: {0}, Y: {1}, Z: {2}", kiallas.X, kiallas.Y, kiallas.Z));
                writer.WriteLine(String.Format("G0 Z{0}", kiallas.Z));
                writer.WriteLine(String.Format("G0 X{0} Y{1}", kiallas.X, kiallas.Y));
            }
        }

        private void WriteG30BeforeM30(TextWriter writer)
        {
            if (NCTConfiguration.G30Needed)
            {
                writer.WriteLine(G30VALUE);
            }
        }

        private void WriteFileClosing(TextWriter writer)
        {
            writer.WriteLine(M30);
            writer.Write("%");
        }
    }
}
