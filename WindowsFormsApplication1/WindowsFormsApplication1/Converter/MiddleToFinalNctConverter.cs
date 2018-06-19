using System;
using System.IO;
using System.Windows.Forms;
using MPFConverterApp.Configuration;

namespace MPFConverterApp
{
    class MiddleToFinalNctConverter
    {
        private const string NCT_FOLDER = @"D:\NCT\";
        private const string NEW_PATTERN = "A";
        private const string T = "T";
        private const string M6 = "M6";
        private const string S = "S";
        private const string G = "G";
        private const string M8 = "M8";

        private int valueOfT;
        private string nextSignToLookFor;
        private bool isPatternCorrect;

        private Label doneLabel;
        private TextWriter writer;
        private Logger logger = Logger.Instance;

        public NCTConfiguration NCTConfiguration { get; set; }

        public MiddleToFinalNctConverter(Label doneLabel)
        {
            this.doneLabel = doneLabel;
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
        ///
        /// Az összes minta után új sorba még bekerül egy M8, ha ki van választva a checkbox.
        ///</summary>
        public void ConvertMiddleNctToFinalNct(string middleNctFile)
        {
            string finalNctFile = NCT_FOLDER + Path.GetFileNameWithoutExtension(middleNctFile)
                + FinalFilePostFix() + Path.GetExtension(middleNctFile);
            valueOfT = -1;
            isPatternCorrect = false;
            nextSignToLookFor = T;

            using (StreamReader reader = new StreamReader(middleNctFile))
            {
                string line;
                FolderUtil.CreateDirectoryIfNotExists(NCT_FOLDER);
                logger.LogComment("A végleges fájl ide kerül létrehozásra: " + finalNctFile);
                writer = new StreamWriter(finalNctFile, false);

                while ((line = reader.ReadLine()) != null)
                {
                    if (!isPatternCorrect)
                    {
                        if (nextSignToLookFor.Equals(NEW_PATTERN))
                        {
                            writer.WriteLine(line);
                            nextSignToLookFor = T;
                            continue;
                        }

                        if (line.Contains(T))
                        {
                            isPatternCorrect = true;
                            Int32.TryParse(line.Substring(line.LastIndexOf(T) + 1), out valueOfT);
                            nextSignToLookFor = M6;
                        }
                        writer.WriteLine(line);
                    }
                    else
                    {
                        switch (nextSignToLookFor)
                        {
                            case M6:
                                if (IsLineContainLetter(M6, line))
                                {
                                    continue;
                                }
                                break;
                            case S:
                                if (IsLineContainLetter(S, line))
                                {
                                    continue;
                                }
                                break;
                            case G:
                                WriteValuesAccordingToLineContainsG(line);
                                break;
                        }
                    }
                }
                writer.Close();
                doneLabel.Visible = true;
            }
            Finalize(middleNctFile, finalNctFile);
        }

        private string FinalFilePostFix()
        {
            return NCTConfiguration.GQHSHPNeeded ? "_f" : "";
        }

        private bool IsLineContainLetter(string letter, string line)
        {
            writer.WriteLine(line);
            if (line.Contains(letter))
            {
                nextSignToLookFor = M6.Equals(letter) ? S : G;
                return true;
            }
            return isPatternCorrect = false;
        }

        private void WriteValuesAccordingToLineContainsG(string line)
        {
            if (line.Contains(G))
            {
                int lastIndexOfZ = line.LastIndexOf("Z");
                string line1, line2;
                line2 = String.Format("G43 H{0} {1}", valueOfT, line.Substring(lastIndexOfZ));
                line1 = line.Remove(lastIndexOfZ);
                logger.LogComment("T-M6-S-G minta észlelve.");
                logger.LogComment(line1 + " és " + line2 + " kiírása.");
                writer.WriteLine(line1);
                writer.WriteLine(line2);
                WriteM8AfterTM6SGPattern();
                nextSignToLookFor = NEW_PATTERN;
            }
            else
            {
                writer.WriteLine(line);
            }
            isPatternCorrect = false;
        }

        //M8 (hűtővíz bekapcsolás) is needed right after the G43 row. It is applied after each TM6SG match.
        private void WriteM8AfterTM6SGPattern()
        {
            if (NCTConfiguration.M8Needed)
            {
                logger.LogComment("M8 kiírása.");
                writer.WriteLine(M8);
            }
        }

        private void Finalize(string middleNctFile, string newNctFile)
        {
            logger.LogComment("Köztes fájl törlése.");
            File.Delete(middleNctFile);
            logger.Close();
        }
    }
}
