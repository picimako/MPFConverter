using System.Windows.Forms;

namespace MPFConverterApp
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
