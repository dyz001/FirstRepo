using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class Class3
    {
        public static List<Class1> mList = new List<Class1>();

        public static T GetClass<T>() where T : Class1
        {
            foreach(var item in mList)
            {
                if (item is T)
                {
                    return (T)item;
                }
            }
            return null;
        }
        
    }
}
