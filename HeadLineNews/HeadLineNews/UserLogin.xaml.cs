using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HeadLineNews
{
    /// <summary>
    /// Interaction logic for UserLogin.xaml
    /// </summary>
    public partial class UserLogin : Window
    {
        string message;
        string title;
        string email;
        string password;
        bool isRemember;
        string pswMd5;

        public UserLogin()
        {
            InitializeComponent();

            try
            {
                Globals.Db = new Database();

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Fatal error: unable to connext to database\n" + ex.Message,
                    "HeadLine News Database", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UserLogin_ButtonClick(object sender, RoutedEventArgs e)
        {

            email = tbUserEmail.Text;
            password = pwdUserPsw.Password;
            isRemember = (bool)cbRemenberMe.IsChecked;
            
            

            if (email.Length == 0 || password.Length == 0)
            {
                message = "Please entre all the information";
                title = "Information not complet";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Regex.IsMatch(email, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z]{2,})$"))
            {
                message = "Please check email format";
                title = "Information not complet";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {

                Globals.currUser = new User();
               
                
          

            using (MD5 md5Hash = MD5.Create())
            {
                pswMd5 = Utiles.GetMd5Hash(md5Hash, password);
            }

          User user = Globals.Db.LoginVerification(email);

            if (pswMd5 == user.Password)
            {
                Globals.currUser = user;
                Globals.currUserSub = Globals.Db.GetSubscriptChannels(Globals.currUser);
                //write to confi
                if (isRemember)
                {
                    try
                    {

                        Utiles.AddOrUpdateAppSettings("userEmail", email);
                        Utiles.AddOrUpdateAppSettings("password", pswMd5);
                        Utiles.AddOrUpdateAppSettings("isRemember", "true");
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        MessageBox.Show("Cannot remember you login set", "login set error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }


                DialogResult = true;
            }
            else
            {
               
                  message = "Wrong user email or password \n Try again";
                  title = "HeadLine News Login";
               

                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            }


        }

        private void BtnCancel_ButtenClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
