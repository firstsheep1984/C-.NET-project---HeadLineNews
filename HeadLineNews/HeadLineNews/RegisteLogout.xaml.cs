using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// RegisteLogout.xaml 的交互逻辑
    /// </summary>
    public partial class RegisteLogout : Window
    {
        public RegisteLogout()
        {
            InitializeComponent();
            if (Globals.currUser != null)
            {
                tbUserName.Text = Globals.currUser.Username;
                tbUserName.IsEnabled = false;
                tbUserEmail.Text = Globals.currUser.Email;
                tbUserEmail.IsEnabled = false;

            }
            else
            {

                btnLogout.Content = "Login";
                btnUpdate.Content = "Sign Up";

            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {

            string pswMd5=null;
            if (pwdPswOne.Password.Length == 0 || pwdPswTwo.Password.Length == 0)
            {
                MessageBox.Show("Please entre new password",
                        "Update error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (pwdPswOne.Password != pwdPswTwo.Password)
            {

                MessageBox.Show("Password not match, please check again",
                        "Update error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            else {
                using (MD5 md5Hash = MD5.Create())
                {
                    pswMd5 = Utiles.GetMd5Hash(md5Hash, pwdPswOne.Password);
                }


                if (Globals.currUser != null)
                { Globals.Db.UpdateUserAccount(Globals.currUser, pswMd5);
                    return;
                }

            }

            if (!Regex.IsMatch(tbUserEmail.Text, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z]{2,})$"))
            {

                MessageBox.Show("Email format not correct",
                          "Sign up error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            else
            {
                Globals.Db = new Database();
                if (Globals.Db.LoginVerification(tbUserEmail.Text) != null)
                {
                    MessageBox.Show("User exist, please login in",
                            "Sign up error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                else
                {
                    User newUser = new User() { Username = tbUserName.Text, Email = tbUserEmail.Text, Password = pswMd5 };

                  newUser.User_id=   Globals.Db.NewUserSignUp(newUser);
                    Globals.currUser = newUser;
                    DialogResult = true;
                }
            }

        }

        private void Logout_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Globals.currUser != null)
            {
                try
                {

                    Utiles.AddOrUpdateAppSettings("userEmail", "");
                    Utiles.AddOrUpdateAppSettings("password", "");
                    Utiles.AddOrUpdateAppSettings("isRemember", "false");
                    Globals.currUser = null;
                    DialogResult = true;
                }
                catch (ConfigurationErrorsException ex)
                {
                    MessageBox.Show("Cannot remember you login set", "login set error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else {

                UserLogin login = new UserLogin();
                login.ShowDialog();

                if (login.DialogResult == true)
                {
                    Close();
                }
            }
        }
    }
}
