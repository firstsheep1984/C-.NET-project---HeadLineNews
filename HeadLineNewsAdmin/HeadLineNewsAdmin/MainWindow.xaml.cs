using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeadLineNewsAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Channel> ChannelsList = new List<Channel>();
        public MainWindow()
        {
            
            InitializeComponent();
            
            AdminLogin adminLogin = new AdminLogin();
            adminLogin.ShowDialog();
            
            lvChannel.ItemsSource = ChannelsList;
            ReloadList();
          
        }

        private void ReloadList()
        {
            try
            {
                List<Channel> listOfChannel = Globals.Db.GetAllChannels();
                ChannelsList.Clear();
                foreach (Channel ch in listOfChannel)
                {
                    ChannelsList.Add(ch);
                }
                lvChannel.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL Query : \n" + ex.Message,
                   "HeadLine News Database", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void AddEditChannel_MenuClick(object sender, RoutedEventArgs e)
        {
            AddchannelDialog addChannelDialog = new AddchannelDialog(this);
            if (addChannelDialog.ShowDialog() == true)
            {
                ReloadList();
            }

        }

        private void LvChannel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Channel currChannel = lvChannel.SelectedItem as Channel;
            if (currChannel == null) return;

            AddchannelDialog editDialog = new AddchannelDialog(this, currChannel);
            if (editDialog.ShowDialog() == true)
            {
                ReloadList();
            }

        }

        private void Edit_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            LvChannel_MouseDoubleClick(sender, null);
        }

        private void Delete_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Channel channelToDelete = lvChannel.SelectedItem as Channel;
            Globals.ChannelToDelete = new Channel();
            Globals.ChannelToDelete = channelToDelete;
           AdminLogin confirmDelete = new AdminLogin(Globals.admin);
            confirmDelete.ShowDialog();

            if (confirmDelete.ShowDialog() == true)
            {
                ReloadList();
            }
       //     else
         

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
