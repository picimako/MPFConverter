using System.Windows.Forms;

namespace MPFConverterApp.Forms
{
    class MPFOpenFileDialogCreator
    {
        public static OpenFileDialog createDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"D:\NC\";
            ofd.Filter = "Mpf fájlok (*.mpf)|*.mpf";
            return ofd;
        }
    }
}
