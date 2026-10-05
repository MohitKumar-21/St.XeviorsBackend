using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;
using System.Data;

namespace st_xaviers_website_api.DAL
{
    public class ContactUsDal
    {
        private readonly IConfiguration _configuration;

        public ContactUsDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // Get Connection String
        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException(
                       "DefaultConnection not found."
                   );
        }


        // =========================
        // INSERT
        // =========================
        public int Insert(ContactUsModel model)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("Sp_ContactUs", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Parameter", 1);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Subject",
                    string.IsNullOrWhiteSpace(model.Subject) ? DBNull.Value : model.Subject);
                command.Parameters.AddWithValue("@Message", model.Message);
                command.Parameters.AddWithValue("@AddedBy", (object?)model.AddedBy ?? DBNull.Value);   // naya

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // =========================
        // UPDATE
        // =========================
        public bool Update(ContactUsModel model)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("Sp_ContactUs", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Parameter", 2);
                command.Parameters.AddWithValue("@ContactUsId", model.ContactUsId);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Subject",
                    string.IsNullOrWhiteSpace(model.Subject) ? DBNull.Value : model.Subject);
                command.Parameters.AddWithValue("@Message", model.Message);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);   // naya

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }

        // =========================
        // DELETE
        // =========================
        public bool Delete(int ContactUsId, int? updatedBy = null)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            using (SqlCommand command = new SqlCommand("Sp_ContactUs", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Parameter", 3);
                command.Parameters.AddWithValue("@ContactUsId", ContactUsId);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);   // naya

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }
        // =========================
        // GET
        // =========================
        public List<ContactUsModel> Get(int? ContactUsId = null)
        {
            List<ContactUsModel> list = new List<ContactUsModel>();

            using (SqlConnection connection =
                   new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command =
                       new SqlCommand("Sp_ContactUs", connection))
                {
                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.AddWithValue(
                        "@Parameter",
                        4
                    );

                    command.Parameters.AddWithValue(
                        "@ContactUsId",
                        ContactUsId.HasValue
                            ? ContactUsId.Value
                            : DBNull.Value
                    );

                    connection.Open();

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ContactUsModel model =
                                new ContactUsModel();

                            model.ContactUsId =
                                Convert.ToInt32(reader["ContactUsId"]);

                            model.Name =
                                reader["Name"] == DBNull.Value
                                    ? string.Empty
                                    : reader["Name"].ToString();

                            model.Email =
                                reader["Email"] == DBNull.Value
                                    ? string.Empty
                                    : reader["Email"].ToString();

                            model.Subject =
                                reader["Subject"] == DBNull.Value
                                    ? string.Empty
                                    : reader["Subject"].ToString();

                            model.Message =
                                reader["Message"] == DBNull.Value
                                    ? string.Empty
                                    : reader["Message"].ToString();

                            list.Add(model);
                        }
                    }
                }
            }

            return list;
        }
    }
}