using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    public class EnumConvert<T> where T : struct
    {
        public static T Parse(string value)
        {
            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Invalid {typeof(T).Name}: {value}");
        }
    }
}
