using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeadLineNewsAdmin
{
    public class Admin
    {
        public string admin_id { get; set; }
        public string admin_username { get; set; }
        public string admin_password { get; set; }
        private string _admin_email;
        public string admin_email
        {
            get
            {
                return _admin_email;
            }
            set
            {
                //Format Exception?
                if (!Regex.IsMatch(value, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z]{2,})$"))
                {
                   
                     throw new FormatException("Invalid Email");
                                      
                }
                _admin_email = value;
            }
        }
    }
}
