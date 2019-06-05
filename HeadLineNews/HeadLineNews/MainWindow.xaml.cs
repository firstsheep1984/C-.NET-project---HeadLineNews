
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

namespace HeadLineNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Article> articleList = new List<Article>();
        //  int loadCount = 0;
        JArray items;


        public MainWindow()
        {

            Globals.currUserSub = new List<Channel>();
            InitializeComponent();
            lbNewsLists.ItemsSource = articleList;
            
            string isRemember = ConfigurationManager.AppSettings["isRemember"];


// if not login or first sign up user, will open today's canada news list

            if (Globals.currUserSub.Count==0&&isRemember=="false")
            {
               

                Label messageLable = new Label();
                messageLable.Content = "Today's news in Canada. Login to see your channels";
                wpChannels.Children.Add(messageLable);
                try
                {
                    items = Utiles.GetTodayInCanada();
                    AddArticleList(items);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fatal error: enable to connect API \n" + ex.Message,
                       "HeadLine News API error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
             

            }
            else
            {
                //read from app.config, to see if user checked remember me
                try
                {
                    string userEmail = ConfigurationManager.AppSettings["userEmail"];
                    string password = ConfigurationManager.AppSettings["password"];

                    Globals.Db = new Database();
                    //get current user info
                    Globals.currUser = Globals.Db.LoginVerification(userEmail);
                    //get current user subscribed channel info
                    Globals.currUserSub = Globals.Db.GetSubscriptChannels(Globals.currUser);
                    loadCurrUserSub();
                    loadArticle();
                    lbNewsLists.Items.Refresh();

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Fatal error: unable to connext to database\n" + ex.Message,
                        "HeadLine News Database", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }



              
              
            }
         
          

            HideScriptErrors(wbIE, true);
        }
        // silence javascript error
        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
        //helper method to add article into the list
        public void AddArticleList(JArray items)
        {
            foreach (var article in items)
            {
                Article a = new Article()
                {
                    Title = article["title"].ToString(),
                    Url = article["url"].ToString()
                };

                articleList.Add(a);

            }
        }



        //helper method to load current user sub channel image into wrap panel
        public void loadCurrUserSub()
        {


            int totalChannel = Globals.currUserSub.Count;
            wpChannels.Children.Clear();

            if (Globals.currUser != null && Globals.currUserSub.Count == 0)
            {

                Label messageLable = new Label();
                messageLable.Content = "Welcome! Add your presonnal channels";
                wpChannels.Children.Add(messageLable);
            }

            for (int i = 0; i < totalChannel; i++)
            {
                BitmapImage icon = Globals.currUserSub[i].IconImage;
                Image img = new Image();
                img.Source = icon;
                img.Width = 60;
                img.Tag = Globals.currUserSub[i].Source;
                //  img.Margin = new Thickness(5);
                img.MouseLeftButtonDown += Img_MouseLeftButtonDown1; ;
                wpChannels.Children.Add(img);

            }
        }

        public void loadArticle()
        {
            if (Globals.currUserSub.Count != 0)
            {
                try
                {
                    items = Utiles.GetArticlesFromApi(Globals.currUserSub[0].Source);
                    AddArticleList(items);
                }
                catch (Exception ex){

                    MessageBox.Show("Fatal error: unable to connext to API\n" + ex.Message,
                       "HeadLine News API error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            lbNewsLists.Items.Refresh();
        }


        private void Img_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            Image imgClick = (Image)sender;

            string source = imgClick.Tag.ToString();

             items = Utiles.GetArticlesFromApi(source);
            articleList.Clear();

            AddArticleList(items);

            lbNewsLists.Items.Refresh();

            wbIE.Navigate(new Uri(items[0]["url"].ToString()));
        }



        private void ArticleTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Article a = (Article)lbNewsLists.SelectedItem;
            wbIE.Navigate(new Uri(a.Url));



        }

        private void LbNewsLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Article a = (Article)lbNewsLists.SelectedItem;
            if (a != null)
            {
                wbIE.Navigate(new Uri(a.Url));
            }

        }

        private void ChannelManage_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Globals.currUser == null)
            {
                MessageBox.Show("Please login to manager your channels",
                                              "User not found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ChannelMagagement channelMagagement = new ChannelMagagement();
                channelMagagement.ShowDialog();
                if (channelMagagement.DialogResult == true)
                {

                    loadCurrUserSub();
                }
            }
        }

        private void OpenBrowser_ButtonClick(object sender, RoutedEventArgs e)
        {
            Article a = (Article)lbNewsLists.SelectedItem;
            if (a != null)
            {
                System.Diagnostics.Process.Start(a.Url);
            }
            else
            {
                MessageBox.Show("Please select an article",
                              "Open in IE error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                dlg.PrintVisual(wbIE, "Print Receipt");
            }

        }

        private void BtnSearch_ButtonClick(object sender, RoutedEventArgs e)
        {
            string topic = tbSearch.Text;
            Utiles.searchArticle(topic);
            int totalResults = (int)Globals.articlSearch["totalResults"];
            //JArray items = (JArray)jo["articles"];
            articleList.Clear();
            LoadArticle();
            lbNewsLists.Items.Refresh();
            wbIE.Navigate(new Uri(articleList[0].Url));
        }

        public void LoadArticle()
        {

            JArray items = (JArray)Globals.articlSearch["articles"];

            for (int i = 0; i < 20; i++)

            {
                Article a = new Article()
                {
                    Title = items[i]["title"].ToString(),
                    Url = items[i]["url"].ToString()
                };

                articleList.Add(a);

            }


        }

        private void RegisteLogout_ButtonClick(object sender, RoutedEventArgs e)
        {
            RegisteLogout registeLogout = new RegisteLogout();
            registeLogout.ShowDialog();
            if (Globals.currUser != null)
            {
                loadCurrUserSub();

                loadArticle();

            }
            else {
                wpChannels.Children.Clear();
                Label messageLable = new Label();
                messageLable.Content = "Today's news in Canada. Login to see your channels";
                wpChannels.Children.Add(messageLable);
                items = Utiles.GetTodayInCanada();

                AddArticleList(items);
                lbNewsLists.Items.Refresh();
            }
           
        }
    }
}
