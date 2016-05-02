using System;
using System.Windows.Forms;
using System.IO;
using MPFConverterApp.Configuration;

namespace MPFConverterApp
{
    class Converter
    {
        private const string MPF_FOLDER = @"D:\MPF\";
        private const string NCT_FOLDER = @"D:\NCT\";
        private const string G30VALUE = "G30ZI0P4"; //to be changed to G650
        private const string M30 = "M30";
        private const string T = "T";
        private const string M6 = "M6";
        private const string S = "S";
        private const string G = "G";

        private Label doneLabel;
        private CheckBox gqCheckBox;
        private Logger logger = Logger.Instance;        

        public NCTConfiguration NCTConfiguration { get; set; }

        public Converter(Label doneLabel, CheckBox gqCheckBox)
        {
            this.doneLabel = doneLabel;
            this.gqCheckBox = gqCheckBox;
        }

        public void ConvertFromMpfToNct(string mpfFileParam, string nctFileParam)
        {
            logger.Open();
            logger.LogComment("Program azonosító: " + NCTConfiguration.ProgramId);
            char[] lineAsCharacters = null;
            FolderUtil.CreateDirectoryIfNotExists(MPF_FOLDER);
            string middleNctFile = MPF_FOLDER + Path.GetFileName(nctFileParam);

            using (StreamReader reader = new StreamReader(mpfFileParam))
            {
                string line;
                TextWriter writer = new StreamWriter(middleNctFile, false);
                writer.WriteLine(String.Format("%O{0} ({1})", NCTConfiguration.ProgramId, NCTConfiguration.Comment));
                WriteOsztofejValue(writer);
                WriteGQOn(writer);
                //while ((line = reader.ReadLine()) != null)
                while (!M30.Equals(line = reader.ReadLine()))
                {
                    string final = PutSemicolonedPartOfRowsIntoBrackets(lineAsCharacters, line);
                    WriteGQOffAtFileEnd(writer, final);
                    WriteXYZ(writer, final);
                    writer.WriteLine(final);
                }
                WriteG30BeforeM30(writer);
                writer.WriteLine(M30);
                writer.Write("%");
                writer.Close();

                ConvertMiddleNctToFinalNct(middleNctFile);
            }
        }

        private string PutSemicolonedPartOfRowsIntoBrackets(char[] lineAsCharacters, string line)
        {
            string final = "";
            lineAsCharacters = line.ToCharArray();
            //A ; karakter indexe
            int indexOfFirstBracket = line.LastIndexOf(";");
            if (indexOfFirstBracket != -1)
            {
                logger.LogComment("Aktuális sor zárójelezése: " + line);
                lineAsCharacters[indexOfFirstBracket] = '('; //a ; kicserélése (-re
                final = new string(lineAsCharacters);
                final += ")"; //a sor végleges változata egy )-et kap a sor végére
                logger.LogComment("Aktuális sor zárójelekkel: " + final);
            }
            else
            {
                final = new string(lineAsCharacters);
            }
            return final;
        }

        private void WriteOsztofejValue(TextWriter writer)
        {
            Osztofej osztofej = NCTConfiguration.Osztofej;
            if (osztofej.Enabled)
            {
                logger.LogComment("Osztófej érték: A" + osztofej.Value);
                writer.WriteLine("A" + (NCTConfiguration.INeeded ? "I" : "") + osztofej.Value);
            }
        }

        private void WriteGQOn(TextWriter writer)
        {
            if (gqCheckBox.Checked)
            {
                logger.LogComment("GQ kezdőérték kiírása: " + Settings.Instance.GQOn);
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
                logger.LogComment("GQ záróérték kiírása: " + Settings.Instance.GQOff);
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
                logger.LogComment(String.Format("XYZ értékek kiírása. X: {0}, Y: {1}, Z: {2}", kiallas.X, kiallas.Y, kiallas.Z));
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

         ///<summary>
         ///1. Soronként beolvasom a fájlt.
         ///(A T-s sor biztosan az után a sor után van, amelyik az utolsó ;-s sor, legalábbis a fájl elején.)
         ///2. Ha a sorban van T, akkor elmentem az utána levő értéket. Ha nem sikerült számmá alakítani, akkor ez nem jó T.
             ///(ehhez esetleg lehet egy bool változó)
         ///3. Ha volt T, akkor megnézem, hogy van-e M6. Ha nincs, akkor megnézem, hogy van-e T. -> 2.
         ///4. Ha volt M6, akkor megnézem, hogy van-e S. Ha nincs, akkor a következő sorra ugrok. -> 2.
         ///5. Ha volt S, akkor megnézem, hogy van-e G. Ha van, akkor a szükséges műveletek végrehajtása.
             ///Ha nincs, akkor a következő sorra ugrok. -> 2.
         ///6. G után újrakezdi a keresést, és ha több TMSG mintát talál, akkor mindre megcsinálja az átalakítást.
         ///T, M6, S keresése; T értékének lementése
         ///S után mindig G0-s mondat lesz
         ///A G0-s mondatban megkeresni a Z koordinátát. A Zxxx kerüljön új sorba. Az új Z-s sor elejére "G43 H<a T utáni szám>"
         ///"G43 H<a T utáni szám>" után lehet, de nem feltétlen szükségeltetik szóköz a Zxx.xx elé
         ///A T után szerepelhet 2 jegyű szám is.
        ///</summary>
        private void ConvertMiddleNctToFinalNct(string middleNctFile)
        {
            string finalNctFile = NCT_FOLDER + Path.GetFileNameWithoutExtension(middleNctFile) + (gqCheckBox.Checked ? "_f" : "") + Path.GetExtension(middleNctFile);
            bool joMinta = false;
            string nextSignToLookFor = T;
            int tErteke = -1;

            using (StreamReader reader = new StreamReader(middleNctFile))
            {
                string line;
                FolderUtil.CreateDirectoryIfNotExists(NCT_FOLDER);
                logger.LogComment("A végleges fájl ide kerül létrehozásra: " + finalNctFile);
                TextWriter writer = new StreamWriter(finalNctFile, false);

                while ((line = reader.ReadLine()) != null)
                {
                    if (!joMinta)
                    {
                        if (nextSignToLookFor.Equals("A"))
                        {
                            writer.WriteLine(line);
                            nextSignToLookFor = T;
                            continue;
                        }

                        if (line.Contains(T))
                        {
                            joMinta = true;
                            Int32.TryParse(line.Substring(line.LastIndexOf(T) + 1), out tErteke);
                            nextSignToLookFor = M6;
                        }
                        writer.WriteLine(line);
                    }
                    else
                    {
                        switch (nextSignToLookFor)
                        {
                            case M6:
                                if (IsLineContainLetter(M6, ref nextSignToLookFor, line, ref joMinta, writer))
                                {
                                    continue;
                                }
                                break;
                            case S:
                                if (IsLineContainLetter(S, ref nextSignToLookFor, line, ref joMinta, writer))
                                {
                                    continue;
                                }
                                break;
                            case G:
                                WriteValuesAccordingToLineContainsG(ref nextSignToLookFor, line, ref joMinta, tErteke, writer);
                                break;
                        }
                    }
                }
                writer.Close();
                doneLabel.Visible = true;
            }
            Finalize(middleNctFile, finalNctFile);
        }

        private bool IsLineContainLetter(string letter, ref string nextSignToLookFor, string line, ref bool joMinta, TextWriter writer)
        {
            writer.WriteLine(line);
            if (line.Contains(letter))
            {
                nextSignToLookFor = M6.Equals(letter) ? S : G;
                return true;
            }
            return joMinta = false;
        }

        //TODO: A G43-as sor után közvetlenül egy M8-at (hűtővíz bekapcsolás) beilleszteni (checkbox-ból választható legyen). Az összes TM6SG előfordulás esetén
        private void WriteValuesAccordingToLineContainsG(ref string nextSignToLookFor, string line, ref bool joMinta, int tErteke, TextWriter writer)
        {
            if (line.Contains(G))
            {
                int lastIndexOfZ = line.LastIndexOf("Z");
                string sor1, sor2;
                sor2 = "G43 H" + tErteke + " " + line.Substring(lastIndexOfZ);
                sor1 = line.Remove(lastIndexOfZ);
                writer.WriteLine(sor1);
                writer.WriteLine(sor2);
                nextSignToLookFor = "A";
            }
            else
            {
                writer.WriteLine(line);
            }
            joMinta = false;
        }

        private void Finalize(string middleNctFile, string newNctFile)
        {
            logger.LogComment("Köztes fájl törlése.");
            File.Delete(middleNctFile);
            FileCopier.CopyNCTFileToNetworkFolder(@NCTConfiguration.NetworkTargetFolder, newNctFile);
            logger.Close();
        }
    }
}
