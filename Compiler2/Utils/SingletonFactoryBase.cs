using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Utils
{
    abstract class SingletonFactoryBase<T> where T : class, new()
    {
        private static T s_instance;

        public static T Instance
        {
            get
            {
                return s_instance ?? (s_instance = new T());
            }
        }
    }
}
