using EasyChat.ViewModels;
using System.Windows;

namespace EasyChat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = MainViewModel.Instance();
            InitializeComponent();
        }
    }
}
