using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class FormValidator
    {
        private ConfigurationControls controls;
        public Int32 ProgramId;

        public FormValidator(ConfigurationControls configurationControls)
        {
            controls = configurationControls;
        }

        public bool isPathNotEmpty(TextBox sourceBox)
        {
            return !sourceBox.Text.Equals(String.Empty);
        }

        public bool isProgramIdValid()
        {
            bool programIdValid = false;
            programIdValid = (!String.Empty.Equals(controls.ID.Text)
                && !controls.ID.Text.StartsWith("0")
                && !controls.ID.Text.StartsWith("-")
                && controls.ID.Text.Length == 4
                && isIdContainNumbersOnly()) ? true : false;
            return programIdValid;
        }

        public bool isIdContainNumbersOnly()
        {
            return Int32.TryParse(controls.ID.Text, out ProgramId);
        }

        public bool isOsztofejAngleValid()
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

        public bool areKiallasValuesValid(params TextBox[] valueBoxes)
        {
            bool isValueValid = true;
            foreach (TextBox valueBox in valueBoxes.ToList<TextBox>())
            {
                isValueValid = isValueValid & isKiallasValueValidFor(valueBox);
            }
            return isValueValid;
        }

        public bool isKiallasValueValidFor(TextBox valueBox)
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
