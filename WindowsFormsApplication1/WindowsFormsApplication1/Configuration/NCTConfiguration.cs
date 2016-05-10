namespace MPFConverterApp.Configuration
{
    class NCTConfiguration
    {
        public int ProgramId { get; set; }
        public string Comment { get; set; }
        public Osztofej Osztofej { get; set; }
        public string NetworkTargetFolder { get; set; }
        public bool INeeded { get; set; }
        public bool GQHSHPNeeded { get; set; }
        public Kiallas Kiallas { get; set; }
        public bool G650Needed { get; set; }
        public bool M8Needed { get; set; }

        public NCTConfiguration()
        {
            SetVariables(1000, string.Empty, new Osztofej(), false, false, new Kiallas());
        }

        private void SetVariables(int programId, string comment, Osztofej osztofej, bool iNeeded, bool gqHshpNeeded, Kiallas kiallas)
        {
            ProgramId = programId;
            Comment = comment;
            Osztofej = osztofej;
            NetworkTargetFolder = string.Empty;
            INeeded = iNeeded;
            GQHSHPNeeded = gqHshpNeeded;
            Kiallas = kiallas;
        }
    }
}
