using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyChat
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbar = (TaskbarIcon)FindResource("Taskbar");
            base.OnStartup(e);
            _taskbar.Visibility = Visibility.Collapsed;
        }

        public static TaskbarIcon _taskbar
        {
            get;
            private set;
        }
    }
}
