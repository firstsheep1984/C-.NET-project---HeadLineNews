
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HeadLineNews
{
  public static  class Utiles
    {
        public static JArray GetTodayInCanada()
        {

            String url = "https://newsapi.org/v2/top-headlines?country=ca&apiKey=" + Globals.api_keys[0];
         
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Timeout = 5000;
            request.Method = "GET";
             HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string jsonstr = sr.ReadToEnd();

          

            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonstr);
            JArray items = (JArray)jo["articles"];

            return items;

        }

       public static JArray GetArticlesFromApi(String source)
          {
              string headApi = Globals.api_head;
              string api_key = Globals.api_keys[0];
              String url = headApi + source + "&apiKey=" + api_key;
              HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

              request.Timeout = 5000;
              request.Method = "GET";
              HttpWebResponse response = (HttpWebResponse)request.GetResponse();
              StreamReader sr = new StreamReader(response.GetResponseStream());
              string jsonstr = sr.ReadToEnd();

         
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonstr);
              JArray items = (JArray)jo["articles"];

         
              return items;


          }
       


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

        public static void AddOrUpdateAppSettings (string key, string value) { 
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);  
                var settings = cfa.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                cfa.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(cfa.AppSettings.SectionInformation.Name);
            
            
        }


        public static void searchArticle(string topic)
        {
            List<Article> resulte = new List<Article>();
            topic = topic.Replace(" ", "%20");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://newsapi.org/v2/everything?q="+topic+"&apiKey="+Globals.api_keys[0]);

            request.Timeout = 5000;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string jsonstr = sr.ReadToEnd();


            Globals.articlSearch = (JObject)JsonConvert.DeserializeObject(jsonstr);
            //  int totalResults = (int)jo["totalResults"];
            //JArray items = (JArray)jo["articles"];

        }

        }


    }

