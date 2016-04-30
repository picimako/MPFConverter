using System;
using System.Linq;
using System.Windows.Forms;

namespace MPFConverterApp
{
    class FormValidator
    {
        private ConfigurationControls controls;
        private int programId;
        public int ProgramId
        {
            get
            {
                return programId;
            }
            set
            {
                programId = value;
            }
        }

        public FormValidator(ConfigurationControls configurationControls)
        {
            controls = configurationControls;
        }

        public bool IsProgramIdValid()
        {
            bool programIdValid = false;
            programIdValid =
                (!IsEmpty(controls.ID.Text)
                && !controls.ID.Text.StartsWith("0")
                && !controls.ID.Text.StartsWith("-")
                && controls.ID.Text.Length == 4
                && IsIdInteger()) ? true : false;
            return programIdValid;
        }

        public bool IsIdInteger()
        {
            return Int32.TryParse(controls.ID.Text, out programId);
        }

        public bool IsOsztofejAngleValid()
        {
            double osztofejAngle;
            bool osztofejAngleValid = false;
            osztofejAngleValid =(!controls.Osztofej.Checked
                || controls.Osztofej.Checked
                    && !IsEmpty(controls.OsztofejValue.Text)
                    && IsDoubleTypeNumber(controls.OsztofejValue.Text, out osztofejAngle)
                    && Math.Abs(osztofejAngle) <= 360) ? true : false;
            return osztofejAngleValid;
        }

        public bool AreKiallasValuesValid(params TextBox[] valueBoxes)
        {
            bool isValueValid = true;
            valueBoxes.ToList().ForEach(valueBox => isValueValid &= IsKiallasValueValidFor(valueBox));
            return isValueValid;
        }

        public bool IsKiallasValueValidFor(TextBox valueBox)
        {
            bool commonKiallasValueValid = false;
            commonKiallasValueValid = (!controls.Kiallas.Checked
                || controls.Kiallas.Checked
                    && !IsEmpty(valueBox.Text)
                    && IsDoubleTypeNumber(valueBox.Text)) ? true : false;
            return commonKiallasValueValid;
        }

        public bool IsEmpty(string text)
        {
            return String.Empty.Equals(text);
        }

        private bool IsDoubleTypeNumber(string text, out double value)
        {
            return Double.TryParse(text.Replace(".", ","), out value);
        }

        private bool IsDoubleTypeNumber(string text)
        {
            double value;
            return Double.TryParse(text.Replace(".", ","), out value);
        }
    }
}
