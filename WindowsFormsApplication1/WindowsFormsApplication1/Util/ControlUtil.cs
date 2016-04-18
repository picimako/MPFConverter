using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class ControlUtil
    {
        public static void EnableControls(params Control[] controls)
        {
            foreach (Control control in controls)
                control.Enabled = true;
        }

        public static void DisableControls(params Control[] controls)
        {
            foreach (Control control in controls)
                control.Enabled = false;
        }
    }
}
