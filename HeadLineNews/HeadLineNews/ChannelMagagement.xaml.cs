using System;
using System.Collections.Generic;
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

namespace HeadLineNews
{
    /// <summary>
    /// Interaction logic for ChannelMagagement.xaml
    /// </summary>
    public partial class ChannelMagagement : Window
    {
         int countChannelSub=Globals.currUserSub.Count;
       static List<Channel> channelsList = Globals.Db.GetAllChannels();
        int totalChannel = channelsList.Count;
        public ChannelMagagement()
        {
            InitializeComponent();
         
           
            wpChannels.Children.Clear();

         
            for (int i = 0; i < totalChannel; i++)
            {
                BitmapImage icon = channelsList[i].IconImage;
                Image img = new Image();
                img.Source = icon;
                img.Width = 60;
                img.Tag = channelsList[i].Source;
                img.Margin = new Thickness(5);
                img.ToolTip = channelsList[i].Ch_name;
                CheckBox checkImg = new CheckBox();
                checkImg.Name = "cb"+channelsList[i].Ch_id.ToString();
               
                foreach (Channel s in Globals.currUserSub)
                {
                    if (s.Source == channelsList[i].Source)
                    {
                   
                        checkImg.IsChecked = true;
                    }
                   
                }

                checkImg.Checked += CheckImg_Checked;
                checkImg.Unchecked += CheckImg_Unchecked;


                wpChannels.Children.Add(img);
                wpChannels.Children.Add(checkImg);

            }



        }

        private void CheckImg_Unchecked(object sender, RoutedEventArgs e)
        {

            CheckBox checkBox = (CheckBox)sender;
            string ch_idStr = checkBox.Name.Substring(2);
           int ch_id= int.Parse(ch_idStr);
            countChannelSub = countChannelSub - 1;
            Channel channelRemove = Globals.Db.GetOneChannels(ch_id);
            for (int i = Globals.currUserSub.Count-1; i >=0; i--)
            {
                if (Globals.currUserSub[i].Ch_id == ch_id)
                    Globals.currUserSub.Remove(Globals.currUserSub[i]);   
                 }
         
        }

        private void CheckImg_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            countChannelSub = countChannelSub + 1;

            if (countChannelSub == 6)
            {
                checkBox.IsChecked = false;

            }
            else
            {
                string ch_idStr = checkBox.Name.Substring(2);
                int ch_id = int.Parse(ch_idStr);
                Channel channelAdd = Globals.Db.GetOneChannels(ch_id);
                Globals.currUserSub.Add(channelAdd);

            }

       
        }

        private void UpdateChannel_ButtonClick(object sender, RoutedEventArgs e)
        {
           Globals.Db.UpdateSubscribed(Globals.currUserSub);
            DialogResult = true;
        }
    }
}
