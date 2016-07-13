using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploading.Data
{
    public class FileUploadingManager
    {
        private readonly string _connectionString;

        public FileUploadingManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddImage(string image, DateTime date)
        {
            var command = CreateCommand("INSERT INTO Images (Image,Date,Count) VALUES (@image,@date,@count)");
            command.Parameters.AddWithValue("@image", image);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@count", 0);
            command.ExecuteNonQuery();
            command.Connection.Dispose();
        }

        public void UpdateCountById(int id)
        {
            var command = CreateCommand("Update Images SET Count += 1 WHERE Id = @id");
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            command.Connection.Dispose();
        }

        public int GetCountById(int id)
        {
            var command = CreateCommand("SELECT Count FROM Images WHERE Id = @id");
            command.Parameters.AddWithValue("@id", id);
            var count = (int)command.ExecuteScalar();
            command.Connection.Dispose();
            return count;
        }

        public IEnumerable<Image> GetTop5Viewed()
        {
            var command = CreateCommand("SELECT TOP 5 * FROM Images ORDER BY Count DESC");
            var reader = command.ExecuteReader();
            var images = new List<Image>();
            while (reader.Read())
            {
                images.Add(new Image
                {
                    Id = (int)reader["Id"],
                    ImageId = (string)reader["Image"],
                    Count = (int)reader["Count"],
                    Date = (DateTime)reader["Date"]
                });
            }

            command.Connection.Dispose();
            return images;
        }

        public IEnumerable<Image> GetTop5RecentlyAdded()
        {
            var command = CreateCommand("SELECT TOP 5 * FROM Images ORDER BY Date DESC");
            var reader = command.ExecuteReader();
            var images = new List<Image>();
            while (reader.Read())
            {
                images.Add(new Image
                {
                    Id = (int)reader["Id"],
                    ImageId = (string)reader["Image"],
                    Count = (int)reader["Count"],
                    Date = (DateTime)reader["Date"]
                });
            }

            command.Connection.Dispose();
            return images;
        }

        private SqlCommand CreateCommand(string query)
        {
            var connection = new SqlConnection(_connectionString);
            var command = connection.CreateCommand();
            command.CommandText = query;
            connection.Open();
            return command;
        }

        public Image GetImageById(int? imageid)
        {
            var command = CreateCommand("SELECT * FROM Images WHERE Id = @id");
            command.Parameters.AddWithValue("@id", imageid);
            var reader = command.ExecuteReader();
            Image g = new Image();
            while (reader.Read())
            {
                g.Id = (int) reader["Id"];
                g.ImageId = (string) reader["Image"];
                g.Count = (int) reader["Count"];
                g.Date = (DateTime) reader["Date"];
            }
            command.Connection.Dispose();
            return g;
        }

        public void GenerateLink(Link l)
        {
            var command = CreateCommand("INSERT INTO Links (Link,ImageId,Expiration) VALUES (@link,@imageId,@expiration)");
            command.Parameters.AddWithValue("@Link", l.ShareLink);
            command.Parameters.AddWithValue("@imageId", l.ImageId);
            command.Parameters.AddWithValue("@expiration", l.Expiration);
            command.ExecuteNonQuery();
            command.Connection.Dispose();
        }

        public int? GetIdByLink(string link)
        {
            var command = CreateCommand("SELECT ImageId FROM Links WHERE Link = @link");
            command.Parameters.AddWithValue("@link", link);
            var id = (int?)command.ExecuteScalar();
            command.Connection.Dispose();
            return id;
        }

        public DateTime CheckExpiration(int id,string imageLink)
        {
            var command = CreateCommand("SELECT Expiration FROM Links WHERE ImageId = @id AND Link = @link");
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@link", imageLink);
            var date = (DateTime)command.ExecuteScalar();
            command.Connection.Dispose();
            return date;
        }

        public void DeleteLink(int id, string imageLink)
        {
            var command = CreateCommand("DELETE FROM Links WHERE ImageId = @id AND Link = @link");
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@link", imageLink);
            command.ExecuteNonQuery();
            command.Connection.Dispose();
        }

        public void Like(int id, string name)
        {
            var command = CreateCommand("INSERT INTO Likes (ImageId,UserId) VALUES (@id,@name)");
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);
            command.ExecuteNonQuery();
            command.Connection.Dispose();
        }
    }
}
