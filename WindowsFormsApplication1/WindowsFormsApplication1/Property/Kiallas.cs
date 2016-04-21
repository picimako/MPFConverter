namespace MPFConverterApp
{
    class Kiallas
    {
        public bool Enabled { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }

        public Kiallas()
        {
            SetVariables(false, string.Empty, string.Empty, string.Empty);
        }

        public Kiallas(bool enabled, string xValue, string yValue, string zValue)
        {
            SetVariables(enabled, xValue, yValue, zValue);
        }

        private void SetVariables(bool enabled, string xValue, string yValue, string zValue)
        {
            Enabled = enabled;
            X = xValue;
            Y = yValue;
            Z = zValue;
        }
    }
}
