using System.Data;
using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.Dal
{
    public class TopperDal
    {
        private readonly string _connectionString;

        public TopperDal(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int Insert(TopperModel model)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Topper", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Parameter", 2);
            cmd.Parameters.AddWithValue("@Name", (object?)model.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Class", (object?)model.Class ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultYear", (object?)model.ResultYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Percentage", (object?)model.Percentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoPath", (object?)model.PhotoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AddedBy", (object?)model.AddedBy ?? DBNull.Value);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(TopperModel model)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Topper", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Parameter", 3);
            cmd.Parameters.AddWithValue("@TopperId", (object?)model.TopperId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", (object?)model.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Class", (object?)model.Class ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ResultYear", (object?)model.ResultYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Percentage", (object?)model.Percentage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoPath", (object?)model.PhotoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public bool Delete(int topperId, int? updatedBy = null)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Topper", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Parameter", 4);
            cmd.Parameters.AddWithValue("@TopperId", topperId);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        // Admin: sab toppers ya Id se
        public List<TopperModel> Get(int? topperId = null) => Read(1, topperId, null);

        // Website: current year ke toppers, optional class ("10th" / "12th")
        public List<TopperModel> GetCurrentYear(string? studentClass = null) => Read(5, null, studentClass);

        private List<TopperModel> Read(int parameter, int? topperId, string? studentClass)
        {
            var list = new List<TopperModel>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Topper", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Parameter", parameter);
            cmd.Parameters.AddWithValue("@TopperId", (object?)topperId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Class",
                string.IsNullOrWhiteSpace(studentClass) ? DBNull.Value : studentClass.Trim());

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TopperModel
                {
                    TopperId = Convert.ToInt32(reader["TopperId"]),
                    Name = reader["Name"].ToString(),
                    Class = reader["Class"].ToString(),
                    ResultYear = reader["ResultYear"].ToString(),
                    Percentage = reader["Percentage"] == DBNull.Value ? null : Convert.ToDecimal(reader["Percentage"]),
                    PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString()
                });
            }
            return list;
        }
    }
}