using EasyChat.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EasyChat.Views.SubControls
{
    /// <summary>
    /// FileSendPreviewUc.xaml 的交互逻辑
    /// </summary>
    public partial class FileSendPreviewUc : Window
    {
        public FileSendPreviewUc(List<FileModel> fileList)
        {
            InitializeComponent();
            FileList = new ObservableCollection<FileModel>(fileList);
            DataContext = this;
        }
        public ObservableCollection<FileModel> FileList { get; set; }
        private void OnSendClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
