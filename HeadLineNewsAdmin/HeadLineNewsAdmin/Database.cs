using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HeadLineNewsAdmin
{
    class Database
    {
        const String DbConnectionString = @"Server=tcp:ipd16-ym.database.windows.net,1433;Initial Catalog=DotNetProject;Persist Security Info=False;User ID=sqladmin;Password=IPD16yym;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private SqlConnection conn;

        public Database()
        {
            conn = new SqlConnection(DbConnectionString);
            conn.Open();
        }


        public void addChannel(Channel channel)
        {

            SqlCommand cmdInsert = new SqlCommand(@"insert into Channel(ch_name,source,location,icon)values (@ch_name,@source,@location,@icon)", conn);

            cmdInsert.Parameters.AddWithValue("ch_name", channel.Ch_name);
            cmdInsert.Parameters.AddWithValue("source", channel.Source);
            cmdInsert.Parameters.AddWithValue("location", channel.Location);
            cmdInsert.Parameters.AddWithValue("icon", channel.IconByte);
            cmdInsert.ExecuteNonQuery();

        }

        public bool UpdateChannel(Channel channel)
        {
            int rowsAffected = -1;

            if (IsChannelExist(channel.Ch_name))
            {
                SqlCommand cmdUpdate = new SqlCommand("UPDATE Channel SET ch_name=@ch_name,source=@source,location=@location,icon=@icon WHERE ch_id=@ch_id", conn);
                cmdUpdate.Parameters.AddWithValue("ch_name", channel.Ch_name);
                cmdUpdate.Parameters.AddWithValue("source", channel.Source);
                cmdUpdate.Parameters.AddWithValue("location", channel.Location);
                cmdUpdate.Parameters.AddWithValue("icon", channel.IconByte);
                cmdUpdate.Parameters.AddWithValue("ch_id", channel.Ch_id);

                rowsAffected = cmdUpdate.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool deleteChannel(int ch_id)
        {

            SqlCommand cmdDelete = new SqlCommand(@"delete from Channel where ch_id=@ch_id", conn);
            cmdDelete.Parameters.AddWithValue("ch_id", ch_id);
            int rowsAffected = cmdDelete.ExecuteNonQuery();
            return rowsAffected > 0;
        }



        public bool IsChannelExist(String ch_name)
        {

            SqlCommand cmdSelectOne = new SqlCommand(@"select count(*)  from Channel where ch_name=@ch_name", conn);
            cmdSelectOne.Parameters.AddWithValue("ch_name", ch_name);

            int ch_id = (int)cmdSelectOne.ExecuteScalar();
            return ch_id > 0;

        }

        public List<Channel> GetAllChannels()
        {
            List<Channel> ChannelList = new List<Channel>();
            BitmapImage icon = null;
            SqlCommand cmdSelectAll = new SqlCommand(@"select * from Channel", conn);
            using (SqlDataReader reader = cmdSelectAll.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ch_id = (int)reader[0];
                    string ch_name = (string)reader[1];
                    string source = (string)reader[2];
                    string location = (string)reader[3];
                    byte[] imagebyte = (byte[])reader[4];

                    icon = Channel.GetPhotoImage(imagebyte);
                    ChannelList.Add(new Channel() { Ch_id = ch_id, Ch_name = ch_name, Source = source, Location = location, IconByte = imagebyte, IconImage = icon,IsReachable=Channel.CheckApi(source) });
                }
            }
            return ChannelList;

        }

        public string FindAdminPassword(string adminEmail)
        {
            SqlCommand cmdSelectAdminPsw = new SqlCommand(@"select admin_password from Admin where admin_email=@admin_email", conn);
            cmdSelectAdminPsw.Parameters.AddWithValue("admin_email", adminEmail);
            String pswAdminMd5= (String)cmdSelectAdminPsw.ExecuteScalar();
            return pswAdminMd5;
        }
    }
}
