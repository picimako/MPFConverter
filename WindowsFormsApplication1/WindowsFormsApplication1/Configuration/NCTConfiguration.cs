using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class NCTConfiguration
    {
        public int ProgramId { get; set; }
        public String Comment { get; set; }
        public Osztofej Osztofej { get; set; }
        public String NetworkTargetFolder { get; set; }
        public bool INeeded { get; set; }
        public bool GQHSHPNeeded { get; set; }
        public Kiallas Kiallas { get; set; }
        public bool G30Needed { get; set; }

        //TODO: innen ki lehetne szedni a paramétert. Elvileg nem szükséges.
        public NCTConfiguration()
        {
            setVariables(1000, String.Empty, new Osztofej(), false, false, new Kiallas());
        }

        public NCTConfiguration(int programId, String comment, Osztofej osztofej, bool iNeeded, bool gqHshpNeeded, Kiallas kiallas)
        {
            setVariables(programId, comment, osztofej, iNeeded, gqHshpNeeded, kiallas);
        }

        private void setVariables(int programId, String comment, Osztofej osztofej, bool iNeeded, bool gqHshpNeeded, Kiallas kiallas)
        {
            ProgramId = programId;
            Comment = comment;
            Osztofej = osztofej;
            NetworkTargetFolder = String.Empty;
            INeeded = iNeeded;
            GQHSHPNeeded = gqHshpNeeded;
            Kiallas = kiallas;
        }
    }
}
