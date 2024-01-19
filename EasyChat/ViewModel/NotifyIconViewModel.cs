using System.Windows;
using System.Windows.Input;

namespace EasyChat.ViewModel
{
    /// <summary>
    /// 窗口相关
    /// </summary>
    public class NotifyIconViewModel:BaseViewModel
    {
        /// <summary>
        /// 如果窗口没显示，就显示窗口
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    return true;
                }, _ =>
                {
                    MainWindow old = null;
                    foreach (var w in Application.Current.Windows)
                    {
                        if (w is MainWindow mw)
                        {
                            old = mw;
                        }
                    }
                    if (old == null)
                    {
                        Application.Current.MainWindow = old = new MainWindow();
                    }
                    old.Show();
                });
            }
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new RelayCommand(_ => {
                    return Application.Current.MainWindow != null;
                }, _ =>
                {
                    Application.Current.MainWindow.Close();
                });
            }
        }


        /// <summary>
        /// 关闭软件
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(_ => {
                    return true;
                }, _ =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
