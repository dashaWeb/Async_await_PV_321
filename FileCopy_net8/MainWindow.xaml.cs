using IOExtensions;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileCopy_net8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel model;
        public MainWindow()
        {
            InitializeComponent();
            model = new ViewModel();
            model.Progress = 0;
            this.DataContext = model;
        }
        private void OpenSource_btn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                model.Source = dialog.FileName;
               // srcTextBox.Text = model.Source;
            }
        }

        private void OpenDest_btn(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                model.Destination = dialog.FileName;
                //destTextBox.Text = model.Destination;
            }
        }

        private async void CopyFile_btn(object sender, RoutedEventArgs e)
        {
            model.Progress = 0;
            string fileName = System.IO.Path.GetFileName(model.Source);
            string destPath = System.IO.Path.Combine(model.Destination, fileName);
            //MessageBox.Show(destPath);
            //File.Copy(Source, destPath, true);
            CopyProcesseInfo info = new CopyProcesseInfo(fileName);
            model.AddProcess(info);
            await CopyFileAsync(model.Source, destPath,info);
            info.Percentage = 100;
            MessageBox.Show("Complete!!!!");
        }
        private Task CopyFileAsync(string src, string dest, CopyProcesseInfo info)
        {
            return FileTransferManager.CopyWithProgressAsync(src, dest, (process) => {
                model.Progress = process.Percentage;
                info.Percentage = process.Percentage;
                info.BytesPerSecond = process.BytesPerSecond;
            
            },false);

            /*return Task.Run(() =>
            {
                using (FileStream srcStream = new FileStream(src, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream destStream = new FileStream(dest, FileMode.Create, FileAccess.Write))
                    {

                        byte[] buffer = new byte[1024 * 8];
                        int bytes = 0;
                        do
                        {
                            bytes = srcStream.Read(buffer, 0, buffer.Length);
                            destStream.Write(buffer, 0, bytes);
                            model.Progress = destStream.Length / (srcStream.Length / 100);

                        } while (bytes > 0);
                    }
                }
            });*/
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class ViewModel
    {
        private ObservableCollection<CopyProcesseInfo> processes;
        public string Source { get; set; }
        public string Destination { get; set; }
        public double Progress { get; set; }
        public bool IsWaiting => Progress == 0;
        public ViewModel()
        {
            processes = new ObservableCollection<CopyProcesseInfo>();
        }
        public IEnumerable<CopyProcesseInfo> Processes => processes; // get
        public void AddProcess(CopyProcesseInfo info)
        {
            processes.Add(info);
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class CopyProcesseInfo
    {
        public string FileName { get; set; }
        public double Percentage { get; set; }
        public int PercentageInt => (int)Percentage;
        public double BytesPerSecond { get; set; }
        public double MegaBytesPerSecond => Math.Round(BytesPerSecond / 1024 / 1024, 1);
        public CopyProcesseInfo(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
