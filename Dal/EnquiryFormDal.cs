using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;
using System.Data;

namespace st_xaviers_website_api.Dal
{
    public class EnquiryFormDal
    {
        private readonly string _connectionString;

        public EnquiryFormDal(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public int Insert(EnquiryFormModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_EnquiryForm", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 2);
                AddModelParameters(cmd, model);
                cmd.Parameters.AddWithValue("@AddedBy", (object?)model.AddedBy ?? DBNull.Value);   // naya

                con.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public bool Update(EnquiryFormModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_EnquiryForm", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 3);
                cmd.Parameters.AddWithValue("@EnquiryFormId", model.EnquiryFormId);
                AddModelParameters(cmd, model);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);   // naya

                con.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public bool Delete(int EnquiryFormId, int? updatedBy = null)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_EnquiryForm", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 4);   
                cmd.Parameters.AddWithValue("@EnquiryFormId", EnquiryFormId);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);   // naya

                con.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public List<EnquiryFormModel> Get(int? EnquiryFormId = null)
        {
            var list = new List<EnquiryFormModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_EnquiryForm", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 1);
                cmd.Parameters.AddWithValue("@EnquiryFormId", (object?)EnquiryFormId ?? DBNull.Value);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EnquiryFormModel
                        {
                            EnquiryFormId = Convert.ToInt32(reader["EnquiryFormId"]),
                            FirstName = reader["FirstName"].ToString() ?? string.Empty,
                            LastName = reader["LastName"].ToString() ?? string.Empty,
                            Age = Convert.ToInt32(reader["Age"]),
                            FathersName = reader["FathersName"].ToString() ?? string.Empty,
                            MobileNumber = reader["MobileNumber"].ToString() ?? string.Empty,
                            Email = reader["Email"].ToString() ?? string.Empty,
                            ClassApplyingFor = reader["ClassApplyingFor"] == DBNull.Value ? null : reader["ClassApplyingFor"].ToString(),
                            CurrentSchoolName = reader["CurrentSchoolName"].ToString() ?? string.Empty,
                            City = reader["City"].ToString() ?? string.Empty,
                            Address = reader["Address"].ToString() ?? string.Empty,
                        });
                    }
                }
            }
            return list;
        }

        private static void AddModelParameters(SqlCommand cmd, EnquiryFormModel model)
        {
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName);
            cmd.Parameters.AddWithValue("@Age", model.Age);
            cmd.Parameters.AddWithValue("@FathersName", model.FathersName);
            cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@ClassApplyingFor", (object?)model.ClassApplyingFor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CurrentSchoolName", model.CurrentSchoolName);
            cmd.Parameters.AddWithValue("@City", model.City);
            cmd.Parameters.AddWithValue("@Address", model.Address);
        }
    }
}
