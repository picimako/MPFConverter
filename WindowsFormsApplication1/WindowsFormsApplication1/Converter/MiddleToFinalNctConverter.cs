using System;
using System.IO;
using System.Windows.Forms;

namespace MPFConverterApp
{
    class MiddleToFinalNctConverter
    {
        private const string NCT_FOLDER = @"D:\NCT\";
        private const string T = "T";
        private const string M6 = "M6";
        private const string S = "S";
        private const string G = "G";
        
        private Label doneLabel;
        private CheckBox gqCheckBox;
        private Logger logger = Logger.Instance;

        public string NetworkTargetFolder { get; set; }

        public MiddleToFinalNctConverter(Label doneLabel, CheckBox gqCheckBox)
        {
            this.doneLabel = doneLabel;
            this.gqCheckBox = gqCheckBox;
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
        public void ConvertMiddleNctToFinalNct(string middleNctFile)
        {
            string finalNctFile = NCT_FOLDER + Path.GetFileNameWithoutExtension(middleNctFile)
                + (gqCheckBox.Checked ? "_f" : "") + Path.GetExtension(middleNctFile);
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
            FileCopier.CopyNCTFileToNetworkFolder(NetworkTargetFolder, newNctFile);
            logger.Close();
        }
    }
}
