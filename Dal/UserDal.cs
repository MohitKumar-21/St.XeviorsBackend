using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;
using System.Data;

namespace st_xaviers_website_api.Dal
{
    public class UserDal
    {
        private readonly IConfiguration _config;
        public UserDal(IConfiguration config) { _config = config; }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection")!;

        public UserModel? Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("Sp_Users", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Parameter", 1);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new UserModel
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Username = reader["Username"].ToString()!,
                        FullName = reader["FullName"] == DBNull.Value ? null : reader["FullName"].ToString(),
                        RoleId = Convert.ToInt32(reader["RoleId"])
                    };
                }
            }
        }

        public void UpdateLastLogin(int userId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("Sp_Users", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Parameter", 2);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}