using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadLineNewsAdmin
{
    class Globals
    {
        public static Database Db;

        public static Admin admin;

        public static Channel ChannelToDelete;

        public  const  string api_head = "https://newsapi.org/v2/top-headlines?sources=";

        public static string[] api_keys = {
   "44b2de67fa3f4329b591e1076b6e8bf5",
   "bda7be050323422bade474011e626a84",
    "8ad6060a7ef843ea8e70cc5498180f80",
   "10032b5cbc084cf5b6e2ff55320cab7f",
   "fe216b327bce47d4838009e05f0874ae"
};

        

    } 
}
