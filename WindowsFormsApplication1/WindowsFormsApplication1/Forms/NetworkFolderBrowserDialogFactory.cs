using System.Windows.Forms;
using System.IO;

namespace MPFConverterApp.Forms
{
    class NetworkFolderBrowserDialogFactory
    {
        public FolderBrowserDialog CreateDialogFor(string baseSelectedPath)
        {
            FolderBrowserDialog networkFolderBrowserDialog = new FolderBrowserDialog();
            networkFolderBrowserDialog.Description = "Válassz célmappát:";
            try
            {
                networkFolderBrowserDialog.SelectedPath = FolderUtil.GetFirstChildFolderIfExist(baseSelectedPath);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("A megadott hálózati elérési útvonal nem létezik.", "Érvénytelen útvonal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return EmptyDialogForNonExistentFolder();
            }

            return networkFolderBrowserDialog;
        }

        private FolderBrowserDialog EmptyDialogForNonExistentFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Tag = "NETWORK_FOLDER_NOT_FOUND";
            return dialog;
        }
    }
}
