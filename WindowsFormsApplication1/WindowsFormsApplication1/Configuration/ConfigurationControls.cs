﻿using System.Windows.Forms;

namespace MPFConverterApp
{
    class ConfigurationControls
    {
        public TextBox Comment { get; set; }
        public NumericUpDown ID { get; set; }
        public CheckBox GQ { get; set; }
        public CheckBox INeeded { get; set; }
        public CheckBox Osztofej { get; set; }
        public TextBox OsztofejValue { get; set; }
        public CheckBox Kiallas { get; set; }
        public TextBox KiallasX{ get; set; }
        public TextBox KiallasY { get; set; }
        public TextBox KiallasZ { get; set; }
        public CheckBox G650 { get; set; }
        public CheckBox M8 { get; set; }
    }
}
