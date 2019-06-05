using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HeadLineNewsAdmin
{
    public class Channel
    {
       

        public int Ch_id { get; set; }
        public string Ch_name { get; set; }
        public string Source { get; set; }
        public string Location { get; set; }
        // not stored - computed only
        public bool IsReachable { get;
            set; 
        }

        public byte[] IconByte
        {
            get;
            set;
          
        }


        public BitmapImage IconImage
        {
          get;

            set;
            
        }

        public static byte[] GetPhotoByte(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            byte[] photo = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();

            return photo;
        }
        public static BitmapImage GetPhotoImage(Byte[] iconByte)
        {

            MemoryStream ms = new MemoryStream(iconByte, true);
            ms.Seek(0, SeekOrigin.Begin);
            BitmapImage iconImage = new BitmapImage();
            iconImage.BeginInit();
            iconImage.StreamSource = ms;
            iconImage.EndInit();

            return iconImage;
        }

        public static bool CheckApi(string source)
        {
            string headApi = Globals.api_head;
            string api_key = Globals.api_keys[0];
            String url = headApi + source + "&apiKey=" + api_key;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Timeout = 5000;
            request.Method = "GET";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;

            }
            catch (WebException ex)
            {
                return false;
            }

        }

    }
}
