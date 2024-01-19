using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyChat
{
    /// <summary>

    /// 单例模式

    /// </summary>

    /// <typeparam name="T">//new()，new不支持非公共的无参构造函数 </typeparam>

    // Token: 0x020001AC RID: 428

    public abstract class SingletonUtils<T> where T : class

    {

        /// <summary>

        /// 创建获取实例

        /// </summary>

        /// <returns></returns>

        // Token: 0x06000F6D RID: 3949 RVA: 0x0004202C File Offset: 0x0004022C

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


        // Token: 0x040007E0 RID: 2016

        private static T sInstance;

    }
}
