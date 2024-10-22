using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Common
{
    public class Constants
    {
        public static class Operators
        {
            public const string LessThan = "lt";
            public const string GreaterThan = "gt";
            public const string LessThanOrEquals = "lte";
            public const string GreaterThanOrEquals = "gte";
            public const string Equal = "eq";

        }
        public static class CacheKeys
        {
            public static string NGOConnectionString => "NGOConnectionString";
        }
    }
}
