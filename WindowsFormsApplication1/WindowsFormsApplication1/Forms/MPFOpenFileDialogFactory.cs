using System.Windows.Forms;

namespace MPFConverterApp.Forms
{
    class MPFOpenFileDialogFactory
    {
        public static OpenFileDialog CreateDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"D:\NC\";
            ofd.Filter = "Mpf fájlok (*.mpf)|*.mpf";
            return ofd;
        }
    }
}
