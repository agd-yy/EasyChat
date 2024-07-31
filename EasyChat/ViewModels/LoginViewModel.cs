using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyChat.ViewModels
{
    public class LoginViewModel : SingletonUtils<LoginViewModel>, INotifyPropertyChanged
    {
        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { UpdateProperty(ref userName, value); }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {

            get { return password; }
            set { UpdateProperty(ref password, value); }
        }

        private string ipAddr;
        /// <summary>
        /// IP
        /// </summary>
        public string IpAddr
        {
            get { return ipAddr; }
            set { UpdateProperty(ref ipAddr, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            //对比新旧值，如果相等就不更新
            if (object.Equals(properValue, newValue))
                return false;
            properValue = newValue;
            NotifyPropertyChanged(properName);
            return true;
        }

        /// <summary>
        /// 最基础的方式
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
