using System.Data;
using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.DAL
{
    public class TestimonialDal
    {
        private readonly string _connectionString;

        public TestimonialDal(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int Insert(TestimonialModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Testimonial", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 2);
                cmd.Parameters.AddWithValue("@Name", (object)model.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhotoPath", (object)model.PhotoPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddedBy", (object)model.AddedBy ?? DBNull.Value);   // naya

                con.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public bool Update(TestimonialModel model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Testimonial", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 3);
                cmd.Parameters.AddWithValue("@TestimonialId", (object)model.TestimonialId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", (object)model.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhotoPath", (object)model.PhotoPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object)model.UpdatedBy ?? DBNull.Value);   // naya

                con.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public bool Delete(int testimonialId, int? updatedBy = null)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Testimonial", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 4);
                cmd.Parameters.AddWithValue("@TestimonialId", testimonialId);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object)updatedBy ?? DBNull.Value);   // naya

                con.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public List<TestimonialModel> Get(int? testimonialId = null)
        {
            var list = new List<TestimonialModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Testimonial", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parameter", 1);
                cmd.Parameters.AddWithValue("@TestimonialId", (object)testimonialId ?? DBNull.Value);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TestimonialModel
                        {
                            TestimonialId = reader["TestimonialId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TestimonialId"]),
                            Name = reader["Name"] == DBNull.Value ? null : reader["Name"].ToString(),
                            PhotoPath = reader["PhotoPath"] == DBNull.Value ? null : reader["PhotoPath"].ToString(),
                            Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}