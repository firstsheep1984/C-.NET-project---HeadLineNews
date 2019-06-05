using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeadLineNews
{
    class User
    {

        public int User_id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                //Format Exception?
                if (!Regex.IsMatch(value, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z]{2,})$"))
                {

                    throw new FormatException("Invalid Email");

                }
                _email = value;
            }
        }
    }
}
