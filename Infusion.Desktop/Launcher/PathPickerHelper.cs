using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infusion.Desktop.Launcher
{
    internal static class PathPickerHelper
    {
        public static string SelectPath(string initialPath, string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = filter,
                FileName = initialPath
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return initialPath;
        }

    }
}
