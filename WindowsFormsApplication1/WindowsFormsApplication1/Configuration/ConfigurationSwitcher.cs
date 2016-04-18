using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPFConverterApp
{
    class ConfigurationSwitcher
    {
        private ConfigurationControls controls;

        public ConfigurationSwitcher(ConfigurationControls NCTConfigControls)
        {
            controls = NCTConfigControls;
        }

        public void saveConfigTo(NCTConfiguration configuration, string networkTargetFolder)
        {
            configuration.Comment = controls.Comment.Text;
            configuration.GQHSHPNeeded = controls.GQ.Checked;
            configuration.INeeded = controls.INeeded.Checked;
            configuration.Osztofej = new Osztofej(controls.Osztofej.Checked, controls.OsztofejValue.Text);
            saveId(configuration);
            configuration.NetworkTargetFolder = networkTargetFolder;
            configuration.Kiallas = new Kiallas(controls.Kiallas.Checked,
                controls.KiallasX.Text, controls.KiallasY.Text, controls.KiallasZ.Text);
            configuration.G30Needed = controls.G30.Checked;
        }

        private void saveId(NCTConfiguration configuration)
        {
            int programId;
            Int32.TryParse(controls.ID.Text, out programId);
            configuration.ProgramId = programId;
        }

        public void readConfigFrom(NCTConfiguration configuration)
        {
            controls.Comment.Text = configuration.Comment;
            controls.GQ.Checked = configuration.GQHSHPNeeded;
            controls.INeeded.Checked = configuration.INeeded;
            controls.ID.Text = configuration.ProgramId.ToString();
            readControlsForOsztofej(configuration);
            readControlsForKiallas(configuration);
            controls.G30.Checked = configuration.G30Needed;
        }

        private void readControlsForOsztofej(NCTConfiguration configuration)
        {
            controls.Osztofej.Checked = configuration.Osztofej.Enabled;
            controls.OsztofejValue.Text = configuration.Osztofej.Value;
        }

        private void readControlsForKiallas(NCTConfiguration configuration)
        {
            controls.Kiallas.Checked = configuration.Kiallas.Enabled;
            controls.KiallasX.Text = configuration.Kiallas.X;
            controls.KiallasY.Text = configuration.Kiallas.Y;
            controls.KiallasZ.Text = configuration.Kiallas.Z;
        }
    }
}
