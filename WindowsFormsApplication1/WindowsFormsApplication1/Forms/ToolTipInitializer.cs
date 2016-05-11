using System.Windows.Forms;

namespace MPFConverterApp
{
    class ToolTipInitializer
    {
        public static void SetToolTipForGqCheckbox(CheckBox gqCheckBox)
        {
            new ToolTip().SetToolTip(gqCheckBox, "Pipa: a nagypontosságú pályakövetés aktív (M30-ig).\nNincs pipa: kikapcsolt állapot.");
        }

        public static void SetToolTipForHovaLabel(Label hovaLabel)
        {
            new ToolTip().SetToolTip(hovaLabel, "A végeredmény fájl fixen a D:\\NCT\\ mappába fog kerülni.");
        }

        public static void SetToolTipForIdBox(NumericUpDown idBox)
        {
            new ToolTip().SetToolTip(idBox,
                "Az azonosító formátuma:\n"
                    + "- nem kezdődhet 0-val,\n"
                    + "- pontosan 4 számjegyből kell, hogy álljon,\n"
                    + "- csak szám szerepelhet benne,\n"
                    + "- nem lehet negatív szám.");
        }

        public static void SetToolTipForCommentTextBox(TextBox commentTextBox)
        {
            new ToolTip().SetToolTip(commentTextBox, "Az azonosító után kerül bele zárójelbe.");
        }

        public static void SetToolTipForM8CheckBox(CheckBox m8CheckBox)
        {
            new ToolTip().SetToolTip(m8CheckBox, "Hűtővíz be/kikapcsolása");
        }
    }
}
