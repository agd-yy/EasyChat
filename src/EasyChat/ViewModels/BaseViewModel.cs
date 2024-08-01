using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyChat.ViewModels
{
    public class BaseViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properValue"></param>
        /// <param name="newValue"></param>
        /// <param name="properName"></param>
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
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
