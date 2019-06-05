using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HeadLineNews
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

        public User LoginVerification(string userEmail)
        {
            User user=null;
            SqlCommand cmdSelectUser = new SqlCommand(@"select * from Client where email=@email", conn);
            cmdSelectUser.Parameters.AddWithValue("email", userEmail);
            using (SqlDataReader reader = cmdSelectUser.ExecuteReader())
            {
                while (reader.Read())
                {
                    int clientId = (int)reader[0];
                    string userName = (string)reader[1];
                    string password = (string)reader[2];
                    string email = (string)reader[3];
                     user = new User() { User_id = clientId, Username = userName, Password = password, Email = email };
                }
            }
            return user;
        }

        public List<Channel> GetSubscriptChannels(User user)
        {
           List<Channel> ChannelList = new List<Channel>();
            BitmapImage icon = null;

            SqlCommand cmdSelectSubscriptChannels = new SqlCommand(@"select c.ch_id,c.ch_name,c.source,c.location ,c.icon from Subscript as s inner join Channel as c on c.ch_id=s.ch_id  where s.ClientId=@ClientId ", conn);
            cmdSelectSubscriptChannels.Parameters.AddWithValue("ClientId", user.User_id);
           using (SqlDataReader reader = cmdSelectSubscriptChannels.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ch_id = (int)reader[0];
                    string ch_name = (string)reader[1];
                    string source = (string)reader[2];
                    string location = (string)reader[3];
                    byte[] imagebyte = (byte[])reader[4];

                    icon = Channel.GetPhotoImage(imagebyte);

                    ChannelList.Add(new Channel() { Ch_id = ch_id, Ch_name = ch_name, Source = source, Location = location, IconByte = imagebyte, IconImage = icon });
                }

            }
            return ChannelList;
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
                    ChannelList.Add(new Channel() { Ch_id = ch_id, Ch_name = ch_name, Source = source, Location = location, IconByte = imagebyte, IconImage = icon });
                }
            }
            return ChannelList;

        }

        public Channel GetOneChannels(int id)
        {
            Channel Channel = new Channel();
            BitmapImage icon = null;
            SqlCommand cmdSelectOneChannel = new SqlCommand(@"select * from Channel where ch_id=@ch_id ", conn);
            cmdSelectOneChannel.Parameters.AddWithValue("ch_id", id);

            using (SqlDataReader reader = cmdSelectOneChannel.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ch_id = (int)reader[0];
                    string ch_name = (string)reader[1];
                    string source = (string)reader[2];
                    string location = (string)reader[3];
                    byte[] imagebyte = (byte[])reader[4];

                    icon = Channel.GetPhotoImage(imagebyte);
                    Channel  = new Channel() { Ch_id = ch_id, Ch_name = ch_name, Source = source, Location = location, IconByte = imagebyte, IconImage = icon };
                }
            }
            return Channel;

        }
        public int NewUserSignUp(User user)
        {

            SqlCommand cmdAddNewSub = new SqlCommand(@"insert into Client (username,password,email,type)  OUTPUT INSERTED.Clientid values (@username,@password,@email,@type)", conn);
            cmdAddNewSub.Parameters.AddWithValue("username", user.Username);
            cmdAddNewSub.Parameters.AddWithValue("email", user.Email);

            cmdAddNewSub.Parameters.AddWithValue("password", user.Password);
            cmdAddNewSub.Parameters.AddWithValue("type", "free");
            int insertId = (int)cmdAddNewSub.ExecuteScalar();
            user.User_id = insertId; // we may choose to do it
            return insertId;
              }



        public void UpdateSubscribed(List<Channel> newSub)
        {
            SqlCommand cmdDeletOldSub = new SqlCommand(@"delete from Subscript where ClientId=@ClientId ", conn);
            cmdDeletOldSub.Parameters.AddWithValue("ClientId", Globals.currUser.User_id);
            cmdDeletOldSub.ExecuteNonQuery();
            foreach (Channel c in newSub)
            {
                SqlCommand cmdAddNewSub = new SqlCommand(@"insert into Subscript (ClientId,ch_id) values (@ClientId,@ch_id)", conn);
                cmdAddNewSub.Parameters.AddWithValue("ClientId", Globals.currUser.User_id);

                cmdAddNewSub.Parameters.AddWithValue("ch_id",c.Ch_id);
                cmdAddNewSub.ExecuteNonQuery();

            }
        }


        public void UpdateUserAccount(User user,string newPwd)
        {
            SqlCommand cmdUpdate = new SqlCommand("UPDATE Client SET password=@password WHERE email=@email", conn);
            cmdUpdate.Parameters.AddWithValue("password", newPwd);
            cmdUpdate.Parameters.AddWithValue("email", user.Email);
            cmdUpdate.ExecuteNonQuery();

        }
    }
}
