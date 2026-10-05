using System.Data;
using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.DAL
{
    public class StudentRegistrationDal
    {
        private readonly string _connectionString;

        public StudentRegistrationDal(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public int Insert(StudentRegistrationModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_StudentRegistration", con))
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

        public bool Update(StudentRegistrationModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_StudentRegistration", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 3);
                cmd.Parameters.AddWithValue("@StudentRegistrationId", model.StudentRegistrationId);
                AddModelParameters(cmd, model);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);   // naya

                con.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public bool Delete(int StudentRegistrationId, int? updatedBy = null)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_StudentRegistration", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 4);
                cmd.Parameters.AddWithValue("@StudentRegistrationId", StudentRegistrationId);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);   // naya

                con.Open();
                object? result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public List<StudentRegistrationModel> Get(int? StudentRegistrationId = null)
        {
            var list = new List<StudentRegistrationModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_StudentRegistration", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 1);
                cmd.Parameters.AddWithValue("@StudentRegistrationId", (object?)StudentRegistrationId ?? DBNull.Value);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StudentRegistrationModel
                        {
                            //Id = Convert.ToInt32(reader["Id"]),
                            StudentRegistrationId = reader["StudentRegistrationId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentRegistrationId"]),
                            StudentName = reader["StudentName"].ToString() ?? string.Empty,
                            ClassApplyingFor = reader["ClassApplyingFor"].ToString() ?? string.Empty,
                            //DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                            DateOfBirth = reader["DateOfBirth"].ToString() ?? string.Empty,
                            Gender = reader["Gender"] == DBNull.Value ? null : reader["Gender"].ToString(),
                            FathersName = reader["FathersName"].ToString() ?? string.Empty,
                            MothersName = reader["MothersName"].ToString() ?? string.Empty,
                            LastSchoolAttended = reader["LastSchoolAttended"] == DBNull.Value ? null : reader["LastSchoolAttended"].ToString(),
                            Percentage = reader["Percentage"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Percentage"]),
                            ContactNumber = reader["ContactNumber"].ToString() ?? string.Empty,
                            WhatsAppNumber = reader["WhatsAppNumber"] == DBNull.Value ? null : reader["WhatsAppNumber"].ToString(),
                            Email = reader["Email"].ToString() ?? string.Empty,
                            Nationality = reader["Nationality"] == DBNull.Value ? null : reader["Nationality"].ToString(),
                            PermanentAddress = reader["PermanentAddress"].ToString() ?? string.Empty,
                        });
                    }
                }
            }
            return list;
        }

        // Insert aur Update ke common parameters (duplicate code hataya)
        private static void AddModelParameters(SqlCommand cmd, StudentRegistrationModel model)
        {
            cmd.Parameters.AddWithValue("@StudentName", model.StudentName);
            cmd.Parameters.AddWithValue("@ClassApplyingFor", model.ClassApplyingFor);
            cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", (object?)model.Gender ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FathersName", model.FathersName);
            cmd.Parameters.AddWithValue("@MothersName", model.MothersName);
            cmd.Parameters.AddWithValue("@LastSchoolAttended", (object?)model.LastSchoolAttended ?? DBNull.Value);

            // decimal ke liye precision/scale explicitly set kiya (AddWithValue se round-off ho sakta hai)
            var pct = new SqlParameter("@Percentage", SqlDbType.Decimal)
            {
                Precision = 5,
                Scale = 2,
                Value = model.Percentage.HasValue ? model.Percentage.Value : DBNull.Value
            };
            cmd.Parameters.Add(pct);

            cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber);
            cmd.Parameters.AddWithValue("@WhatsAppNumber", (object?)model.WhatsAppNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Nationality", (object?)model.Nationality ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PermanentAddress", model.PermanentAddress);
        }
    }
}