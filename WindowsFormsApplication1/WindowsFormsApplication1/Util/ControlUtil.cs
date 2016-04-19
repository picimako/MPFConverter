using System.Linq;
using System.Windows.Forms;

namespace MPFConverterApp
{
    class ControlUtil
    {
        public static void DisableControls(params Control[] controls)
        {
            controls.ToList().ForEach(control => control.Enabled = false);
        }
    }
}
