using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class List
    {
        public static string Join<T>(this List<T> items) 
        {
            string delimiter = ",";
            var strs = String.Join(delimiter, items);
            return strs;
        }
    }
}
