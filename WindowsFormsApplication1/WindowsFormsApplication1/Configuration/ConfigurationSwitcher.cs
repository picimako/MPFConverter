using System;
using MPFConverterApp.Configuration;

namespace MPFConverterApp
{
    class ConfigurationSwitcher
    {
        private ConfigurationControls controls;

        public ConfigurationSwitcher(ConfigurationControls NCTConfigControls)
        {
            controls = NCTConfigControls;
        }

        public void SaveConfigTo(NCTConfiguration configuration, string networkTargetFolder)
        {
            configuration.Comment = controls.Comment.Text;
            configuration.GQHSHPNeeded = controls.GQ.Checked;
            configuration.INeeded = controls.INeeded.Checked;
            configuration.Osztofej = new Osztofej(controls.Osztofej.Checked, controls.OsztofejValue.Text);
            SaveId(configuration);
            configuration.NetworkTargetFolder = networkTargetFolder;
            configuration.Kiallas = new Kiallas(controls.Kiallas.Checked,
                controls.KiallasX.Text, controls.KiallasY.Text, controls.KiallasZ.Text);
            configuration.G650Needed = controls.G650.Checked;
        }

        private void SaveId(NCTConfiguration configuration)
        {
            int programId;
            Int32.TryParse(controls.ID.Text, out programId);
            configuration.ProgramId = programId;
        }

        public void ReadConfigFrom(NCTConfiguration configuration)
        {
            controls.Comment.Text = configuration.Comment;
            controls.GQ.Checked = configuration.GQHSHPNeeded;
            controls.INeeded.Checked = configuration.INeeded;
            controls.ID.Text = configuration.ProgramId.ToString();
            ReadControlsForOsztofej(configuration);
            ReadControlsForKiallas(configuration);
            controls.G650.Checked = configuration.G650Needed;
        }

        private void ReadControlsForOsztofej(NCTConfiguration configuration)
        {
            controls.Osztofej.Checked = configuration.Osztofej.Enabled;
            controls.OsztofejValue.Text = configuration.Osztofej.Value;
        }

        private void ReadControlsForKiallas(NCTConfiguration configuration)
        {
            controls.Kiallas.Checked = configuration.Kiallas.Enabled;
            controls.KiallasX.Text = configuration.Kiallas.X;
            controls.KiallasY.Text = configuration.Kiallas.Y;
            controls.KiallasZ.Text = configuration.Kiallas.Z;
        }
    }
}
