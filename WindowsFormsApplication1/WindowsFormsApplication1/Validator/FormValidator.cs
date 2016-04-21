using System;
using System.Linq;
using System.Windows.Forms;

namespace MPFConverterApp
{
    class FormValidator
    {
        private ConfigurationControls controls;
        public int ProgramId;

        public FormValidator(ConfigurationControls configurationControls)
        {
            controls = configurationControls;
        }

        public bool IsPathNotEmpty(TextBox sourceBox)
        {
            return !sourceBox.Text.Equals(String.Empty);
        }

        public bool IsProgramIdValid()
        {
            bool programIdValid = false;
            programIdValid = (!String.Empty.Equals(controls.ID.Text)
                && !controls.ID.Text.StartsWith("0")
                && !controls.ID.Text.StartsWith("-")
                && controls.ID.Text.Length == 4
                && IsIdContainNumbersOnly()) ? true : false;
            return programIdValid;
        }

        public bool IsIdContainNumbersOnly()
        {
            return Int32.TryParse(controls.ID.Text, out ProgramId);
        }

        public bool IsOsztofejAngleValid()
        {
            double osztofejAngle;
            if (!controls.Osztofej.Checked
                || controls.Osztofej.Checked
                    && controls.OsztofejValue.Text != String.Empty
                    && Double.TryParse(controls.OsztofejValue.Text.Replace(".", ","), out osztofejAngle)
                    && Math.Abs(osztofejAngle) <= 360)
            {
                return true;
            }
            return false;
        }

        public bool AreKiallasValuesValid(params TextBox[] valueBoxes)
        {
            bool isValueValid = true;
            valueBoxes.ToList().ForEach(valueBox => isValueValid &= IsKiallasValueValidFor(valueBox));
            return isValueValid;
        }

        public bool IsKiallasValueValidFor(TextBox valueBox)
        {
            double commonKiallasValue;
            if (!controls.Kiallas.Checked
                || controls.Kiallas.Checked
                    && valueBox.Text != String.Empty
                    && Double.TryParse(valueBox.Text.Replace(".", ","), out commonKiallasValue))
            {
                return true;
            }
            return false;
        }
    }
}
