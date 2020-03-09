using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FluffMuff.Stores
{
    class WhitelistStore
    {
        private readonly List<string> WhitelistCollection;

        public static string pattern = @"[@'>]";

        public WhitelistStore()
        {
            WhitelistCollection = new List<string>()
            {
                "^You also see .+$",
                "^You (get a|pick up) .+$",
            };
        }

        public List<String> GetWhitelist()
        {
            return WhitelistCollection;
        }
    }
}
