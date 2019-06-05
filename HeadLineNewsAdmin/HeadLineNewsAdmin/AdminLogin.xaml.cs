using System;
using System.Collections.Generic;
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

namespace HeadLineNewsAdmin
{
    /// <summary>
    /// Interaction logic for AdminLogin.xaml
    /// </summary>
    public partial class AdminLogin : Window
    {

        String message;
        string title;
        string pswMd5;
        public AdminLogin(Admin admin = null)
        {

            InitializeComponent();
            if (admin == null)
            {
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
            else
            {
                tbAdminUserName.Text = admin.admin_email;
                btnComfirmDelete.Content = "Comfirme";
                tbAdminUserName.IsEnabled = false;
                lbMessage.Content = "Please entre your password to confoirme DELETE!";

            }

        }

        private void login_ButtonClick(object sender, RoutedEventArgs e)
        {
            string password = pwdAdminPsw.Password;
            string adminEmail = tbAdminUserName.Text;
            // ligin window
            if (Globals.ChannelToDelete == null)
            {
                if (adminEmail.Length == 0 || password.Length == 0)
                {
                    message = "Please entre all the information";
                    title = "Information not complet";
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!Regex.IsMatch(tbAdminUserName.Text, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z]{2,})$"))
                {
                    message = "Please check email format";
                    title = "Information not complet";
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {
                    Globals.admin = new Admin();
                    Globals.admin.admin_email = tbAdminUserName.Text;
                }
            }

            if(Globals.admin!=null)
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    pswMd5 = Utils.GetMd5Hash(md5Hash, password);
                }

                if (pswMd5 == Globals.Db.FindAdminPassword(adminEmail))
                {

                    if (Globals.ChannelToDelete != null)
                    {
                        //delete comfirm
                        Globals.Db.deleteChannel(Globals.ChannelToDelete.Ch_id);

                    }
                    else
                    { Globals.admin.admin_password = pswMd5; }

                    DialogResult = true;
                }
                else
                {
                    if (Globals.ChannelToDelete == null)
                    {
                        message = "Wrong user email or password \n Try again";
                        title = "HeadLine News Admin Login";
                    }
                    else
                    {
                        message = "Password not right, can not delete CHANNEL";
                        title = "Confire Password";
                    }

                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }




        private void Window_Closed(object sender, EventArgs e)
        {
            if (Globals.admin == null)
            { Application.Current.Shutdown(); }


        }
    }
}

