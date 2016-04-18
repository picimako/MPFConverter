using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    class Kiallas
    {
        public bool Enabled { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }

        public Kiallas()
        {
            setVariables(false, String.Empty, String.Empty, String.Empty);
        }

        public Kiallas(bool enabled, String xValue, String yValue, String zValue)
        {
            setVariables(enabled, xValue, yValue, zValue);
        }

        private void setVariables(bool enabled, String xValue, String yValue, String zValue)
        {
            Enabled = enabled;
            X = xValue;
            Y = yValue;
            Z = zValue;
        }
    }
}
