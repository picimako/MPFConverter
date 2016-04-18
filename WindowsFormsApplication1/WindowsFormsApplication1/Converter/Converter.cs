using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MPFConverterApp
{
    class Converter
    {
        private Label doneLabel;
        private CheckBox gqCheckBox;
        private Logger logger;
        private const string mpfFolder = @"D:\MPF\";
        private const string nctFolder = @"D:\NCT\";
        private const string G30VALUE = "G30ZI0P4";
        private string networkTargetFolder;

        public Converter(Label doneLabel, CheckBox gqCheckBox)
        {
            this.doneLabel = doneLabel;
            this.gqCheckBox = gqCheckBox;
        }

        public void convertFromMpfToNct(string mpfFileParam, string nctFileParam, NCTConfiguration config)
        {
            this.networkTargetFolder = config.NetworkTargetFolder;
            logger = Logger.logger;
            logger.logComment("Program azonosító: " + config.ProgramId);
            char[] lineAsCharacters = null;
            string mpfFile = mpfFileParam;
            FolderUtil.createDirectoryIfNotExists(mpfFolder, logger);
            //köztes fájl
            string nctFile = mpfFolder + Path.GetFileName(nctFileParam);

            using (StreamReader reader = new StreamReader(mpfFile))
            {
                String line;
                TextWriter writer = new StreamWriter(nctFile, false);
                writer.WriteLine(String.Format("%O{0} ({1})", config.ProgramId, config.Comment));
                writeOsztofejValue(writer, config.Osztofej, config.INeeded);
                writeGQOn(writer);
                //while ((line = reader.ReadLine()) != null)
                while (!"M30".Equals(line = reader.ReadLine()))
                {
                    string final = putSemicolonedPartOfRowsIntoBrackets(lineAsCharacters, line);
                    writeGQOffAtFileEnd(writer, final);
                    writeXYZ(writer, config.Kiallas, final);
                    writer.WriteLine(final);
                }
                writeG30BeforeM30(writer, config.G30Needed);
                writer.WriteLine("M30");
                writer.Write("%");
                writer.Close();

                convertNctToNewNct(nctFile);
            }
        }

        private string putSemicolonedPartOfRowsIntoBrackets(char[] lineAsCharacters, String line)
        {
            string final = "";
            lineAsCharacters = line.ToCharArray();
            //A ; karakter indexe
            int indexOfFirstBracket = line.LastIndexOf(";");
            if (indexOfFirstBracket != -1)
            {
                logger.logComment("Aktuális sor zárójelezése: " + line);
                lineAsCharacters[indexOfFirstBracket] = '('; //a ; kicserélése (-re
                final = new string(lineAsCharacters);
                final += ")"; //a sor végleges változata egy )-et kap a sor végére
                logger.logComment("Aktuális sor zárójelekkel: " + final);
            }
            else
            {
                final = new string(lineAsCharacters);
            }
            return final;
        }

        private void writeOsztofejValue(TextWriter writer, Osztofej osztofej, bool INeeded)
        {
            if (osztofej.Enabled)
            {
                logger.logComment("Osztófej érték: A" + osztofej.Value);
                writer.WriteLine("A" + (INeeded ? "I" : "") + osztofej.Value);
            }
        }

        private void writeGQOn(TextWriter writer)
        {
            if (gqCheckBox.Checked)
            {
                logger.logComment("GQ kezdőérték kiírása: " + Settings.settings.getGQOn());
                writer.WriteLine(Settings.settings.getGQOn());
            }
        }

        private void writeGQOffAtFileEnd(TextWriter writer, string final)
        {
            if (gqCheckBox.Checked && "M30".Equals(final))
            {
                logger.logComment("GQ záróérték kiírása: " + Settings.settings.getGQOff());
                writer.WriteLine(Settings.settings.getGQOff());
            }
        }

        private void writeXYZ(TextWriter writer, Kiallas kiallas, string final)
        {
            if (kiallas.Enabled && "M30".Equals(final))
            {
                logger.logComment(String.Format("XYZ értékek kiírása. X: {0}, Y: {1}, Z: {2}", kiallas.X, kiallas.Y, kiallas.Z));
                writer.WriteLine(String.Format("G0 Z{0}", kiallas.Z));
                writer.WriteLine(String.Format("G0 X{0} Y{1}", kiallas.X, kiallas.Y));
            }
        }

        private void writeG30BeforeM30(TextWriter writer, bool g30Checked)
        {
            if (g30Checked)
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
         ///6. G után újrakezdi a keresés, és ha több TMSG mintát talál, akkor mindre megcsinálja az átalakítást.
         ///T, M6, S keresése; T értékének lementése
         ///S után mindig G0-s mondat lesz
         ///A G0-s mondatban megkeresni a Z koordinátát. A Zxxx kerüljön új sorba. Az új Z-s sor elejére "G43 H<a T utáni szám>"
         ///"G43 H<a T utáni szám>" után lehet, de nem feltétlen szükségeltetik szóköz a Zxx.xx elé
         ///A T után szerepelhet 2 jegyű szám is.
        ///</summary>
        private void convertNctToNewNct(string initialNctFile)
        {
            string newNctFile = nctFolder + Path.GetFileNameWithoutExtension(initialNctFile) + (gqCheckBox.Checked ? "_f" : "") + Path.GetExtension(initialNctFile);
            bool joMinta = false;
            String nextSignToLookFor = "T";
            int tErteke = -1;

            using (StreamReader reader = new StreamReader(initialNctFile))
            {
                string line;
                FolderUtil.createDirectoryIfNotExists(nctFolder, logger);
                logger.logComment("A végleges fájl ide kerül létrehozásra: " + newNctFile);
                TextWriter writer = new StreamWriter(newNctFile, false);

                while ((line = reader.ReadLine()) != null)
                {
                    if (!joMinta)
                    {
                        if (nextSignToLookFor.Equals("A"))
                        {
                            writer.WriteLine(line);
                            nextSignToLookFor = "T";
                            continue;
                        }

                        if (line.Contains("T"))
                        {
                            joMinta = true;
                            Int32.TryParse(line.Substring(line.LastIndexOf("T") + 1), out tErteke);
                            nextSignToLookFor = "M6";
                        }
                        writer.WriteLine(line);
                    }
                    else
                    {
                        switch (nextSignToLookFor)
                        {
                            case "M6":
                                if (isLineContainLetter("M6", ref nextSignToLookFor, line, ref joMinta, writer))
                                {
                                    continue;
                                }
                                break;
                            case "S":
                                if (isLineContainLetter("S", ref nextSignToLookFor, line, ref joMinta, writer))
                                {
                                    continue;
                                }
                                break;
                            case "G":
                                writeValuesAccordingToLineContainsG(ref nextSignToLookFor, line, ref joMinta, tErteke, writer);
                                break;
                        }
                    }
                }
                writer.Close();
                doneLabel.Visible = true;
            }
            finalization(initialNctFile, newNctFile);
        }

        private bool isLineContainLetter(string letter, ref string nextSignToLookFor, string line, ref bool joMinta, TextWriter writer)
        {
            writer.WriteLine(line);
            if (line.Contains(letter))
            {
                nextSignToLookFor = "M6".Equals(letter) ? "S" : "G";
                return true;
            }
            return joMinta = false;
        }

        private void writeValuesAccordingToLineContainsG(ref string nextSignToLookFor, string line, ref bool joMinta, int tErteke, TextWriter writer)
        {
            if (line.Contains("G"))
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

        private void finalization(string initialNctFile, string newNctFile)
        {
            logger.logComment("Köztes fájl törlése.");
            File.Delete(initialNctFile);
            FileCopier.copyNCTFileToNetworkFolder(@networkTargetFolder, newNctFile, logger);
            logger.closeLogger();
        }
    }
}
