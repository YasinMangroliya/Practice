using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Services
{
    public static class Common
    {
        public static Int64[] ConvertStringToInt64Array(string strArray)
        {
            return strArray.Split(',').Select(x => Int64.Parse(x)).ToArray();
        }
    }
}
