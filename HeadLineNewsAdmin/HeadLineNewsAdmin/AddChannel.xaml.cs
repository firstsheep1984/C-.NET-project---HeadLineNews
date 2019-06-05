using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace HeadLineNewsAdmin
{
    /// <summary>
    /// </summary>
    public partial class AddchannelDialog : Window
    {

        Channel ChannelToEdit;
        bool[] availability = new bool[4];
        string source;
        string location;
        string name;
        byte[] icon;

        public AddchannelDialog( Window owner, Channel channelToEdit=null)
        {
            InitializeComponent();
            Owner = owner;

        ChannelToEdit = channelToEdit;
            if (ChannelToEdit != null)
            {
                tbChannelName.Text = ChannelToEdit.Ch_name;
                tbChannelSource.Text = ChannelToEdit.Source;
                tbChannelSource.IsEnabled = false;
                imgChannelPreview.Source = ChannelToEdit.IconImage;
                cbChannelLocation.Text = ChannelToEdit.Location;
                btnAddEditChannel.Content = "Update";
                Title = "Update Channel";
              //  lbCheckName.Content = Convert.ToString((char)8730);
                IconName.Visibility = Visibility.Visible;
                IconNameWrong.Visibility = Visibility.Collapsed;
                for (int i=0;i<4;i++)
                {
                    availability[i] = true;
                }
                source = ChannelToEdit.Source;
                name = ChannelToEdit.Ch_name;
                location = ChannelToEdit.Location;
                icon = ChannelToEdit.IconByte;
          
              //  lbCheckSource.Content = Convert.ToString((char)8730);
              //  lbCheckLocation.Content = Convert.ToString((char)8730);
              //  lbCheckIcon.Content = Convert.ToString((char)8730);
                IconSource.Visibility = Visibility.Visible;
                IconSourceWrong.Visibility = Visibility.Collapsed;
                IconLocation.Visibility = Visibility.Visible;
                IconLocationWrong.Visibility = Visibility.Collapsed;
                IconIcon.Visibility = Visibility.Visible;
                IconIconWrong.Visibility = Visibility.Collapsed;
            }
        }


        private void OpenFile_ButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Channel icon image|*.png|All files|*.*";
            dialog.Title = "Select Channel icon";
            dialog.ShowDialog();

            string filepath = dialog.FileName;

          
            if (dialog.FileName != "")
            {
                try
                {
                    icon = Channel.GetPhotoByte(filepath);
                    imgChannelPreview.Source = Channel.GetPhotoImage(icon);
                  //  lbCheckIcon.Content = Convert.ToString((char)8730);
                    IconIcon.Visibility = Visibility.Visible;
                    IconIconWrong.Visibility = Visibility.Collapsed;
                    availability[0] = true;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Error open icon image: " + ex.Message, "Load icon image", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                GC.Collect();
            }

       
        }

        private void TbChannelSource_LostFocus(object sender, RoutedEventArgs e)
        {
             source = tbChannelSource.Text;
            if (Channel.CheckApi(source))
            {
              //  lbCheckSource.Content = Convert.ToString((char)8730);
                IconSource.Visibility = Visibility.Visible;
                IconSourceWrong.Visibility = Visibility.Collapsed;
                availability[2] = true;

            }
            else

            {
               // lbCheckSource.Content = Convert.ToString((char)88);
                IconSourceWrong.Visibility = Visibility.Visible;
                IconSource.Visibility = Visibility.Collapsed;
                availability[2] = false; }
        }

        private void AddChannel_ButtonClick(object sender, RoutedEventArgs e)
        {
            foreach(bool a in availability)
            {
                if (a != true)
                {
                    MessageBox.Show("Please Check field,Channel info not correct ", "Add Channel- Miss information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }

            }


            if (ChannelToEdit == null)
            {
                Channel newChannel = new Channel()
                {
                    Ch_name = name,
                    Source = source,
                    Location = location,
                    IconByte = icon

                };

                Globals.Db.addChannel(newChannel);
            }
            else
            {
                ChannelToEdit.IconByte = icon;
                ChannelToEdit.Source = tbChannelSource.Text;
                ChannelToEdit.Ch_name = tbChannelName.Text;
                ChannelToEdit.Location = location;
                
                Globals.Db.UpdateChannel(ChannelToEdit);
            }
            

         

            DialogResult = true;


        }

        private void TbChannelName_LostFocus(object sender, RoutedEventArgs e)
        {

            if (tbChannelSource.IsEnabled)
            {
                if (!Globals.Db.IsChannelExist(tbChannelName.Text) && tbChannelName.Text.Length != 0)
                {
                    name = tbChannelName.Text;
                  //  lbCheckName.Content = Convert.ToString((char)8730);
                    IconName.Visibility = Visibility.Visible;
                    IconNameWrong.Visibility = Visibility.Collapsed;
                    availability[1] = true;
                }
                else
                {

                  //  lbCheckName.Content = Convert.ToString((char)88);
                    IconNameWrong.Visibility = Visibility.Visible;
                    IconName.Visibility = Visibility.Collapsed;
                    availability[1] = false;
                }
            }
            else {
                if (tbChannelName.Text != ChannelToEdit.Ch_name)
                {
                    if (!Globals.Db.IsChannelExist(tbChannelName.Text) && tbChannelName.Text.Length != 0)
                    {
                        name = tbChannelName.Text;
                      //  lbCheckName.Content = Convert.ToString((char)8730);
                        IconName.Visibility = Visibility.Visible;
                        IconNameWrong.Visibility = Visibility.Collapsed;
                        availability[1] = true;
                    }
                    else {
                      //  lbCheckName.Content = Convert.ToString((char)88);
                        IconNameWrong.Visibility = Visibility.Visible;
                        IconName.Visibility = Visibility.Collapsed;
                        availability[1] = false;
                    }
                }
                else {

                    name = tbChannelName.Text;
                   // lbCheckName.Content = Convert.ToString((char)8730);
                    IconName.Visibility = Visibility.Visible;
                    IconNameWrong.Visibility = Visibility.Collapsed;
                    availability[1] = true;
                }

            }
           
        }


        private void CbChannelLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            location = (e.AddedItems[0] as ComboBoxItem).Content as string;
            
           // lbCheckLocation.Content = Convert.ToString((char)8730);
            IconLocation.Visibility = Visibility.Visible;
            IconLocationWrong.Visibility = Visibility.Collapsed;
            availability[3] = true;
        }

       
    }
}
