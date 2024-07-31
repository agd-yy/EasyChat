using System;

namespace EasyChat
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T">//new()，new不支持非公共的无参构造函数 </typeparam>
    public abstract class SingletonUtils<T> where T : class
    {
        /// <summary>
        /// 创建获取实例
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            Type typeFromHandle = typeof(T);
            lock (typeFromHandle)
            {
                bool flag2 = SingletonUtils<T>.sInstance == null;
                if (flag2)
                {
                    SingletonUtils<T>.sInstance = (Activator.CreateInstance(typeof(T), true) as T);
                }
            }
            return SingletonUtils<T>.sInstance;
        }
        private static T sInstance;

    }
}
