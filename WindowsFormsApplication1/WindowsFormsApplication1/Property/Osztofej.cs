namespace MPFConverterApp
{
    class Osztofej
    {
        public bool Enabled { get; set; }
        public string Value { get; set; }

        public Osztofej()
        {
            Enabled = false;
            Value = string.Empty;
        }

        public Osztofej(bool enabled, string value)
        {
            Enabled = enabled;
            Value = value;
        }
    }
}
