using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace HeadLineNewsAdmin
{
    public static class Utils
    {
        public static string GetMd5Hash(MD5 md5Hash, string passwordBeforMd5)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordBeforMd5));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static bool VerifyMd5Hash(MD5 md5Hash, string passwordMd5, string passwordInDatabase)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(passwordMd5, passwordInDatabase))
            {
                return true;
            }
            else
            {
                return false;
            }

        }



    }


}
