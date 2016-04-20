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
        public bool G30Needed { get; set; }

        public NCTConfiguration()
        {
            setVariables(1000, string.Empty, new Osztofej(), false, false, new Kiallas());
        }

        private void setVariables(int programId, string comment, Osztofej osztofej, bool iNeeded, bool gqHshpNeeded, Kiallas kiallas)
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
