using System;
using System.IO;
using System.Windows.Forms;
using Tbasic.Runtime;

namespace texecute {
    public class Program {
        
        [STAThread]
        public static void Main(string[] args) {
            string file = null;
            if (args.Length > 0 && File.Exists(args[0])) {
                file = args[0];
            }
            if (file == null) {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Open";
                dialog.FileName = "";
                dialog.Multiselect = false;
                dialog.Filter = "Tbasic 2.0 Script (*.tba)|*.tba|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    file = dialog.FileName;
                }
                else {
                    return;
                }
            }

            try {
                Executer exec = new Executer();
                exec.Global.LoadStandardLibrary();
                exec.Execute(File.ReadAllLines(file));
            }
            catch (Exception ex) {
                MessageBox.Show("An error occoured\n" + ex.Message, "Tbasic Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
