using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.Dal
{
    public class NoticeDal
    {
        private readonly IConfiguration _config;
        public NoticeDal(IConfiguration config) { _config = config; }

        private string GetConnectionString() => _config.GetConnectionString("DefaultConnection")!;

        private SqlCommand CreateCommand(SqlConnection con, int parameter)
        {
            var cmd = new SqlCommand("Sp_Notice", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", parameter);
            return cmd;
        }

        // String date ("yyyy-MM-dd" ya "dd-MM-yyyy") ko DateTime me badalta hai.
        // Khali ho to DBNull jayega.
        private static object ToDbDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DBNull.Value;

            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fff" };

            if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;

            throw new ArgumentException("Date ka format galat hai. yyyy-MM-dd ya dd-MM-yyyy use karo.");
        }

        // INSERT
        public int Insert(NoticeModel m)
        {
            using var con = new SqlConnection(GetConnectionString());
            using var cmd = CreateCommand(con, 1);
            cmd.Parameters.AddWithValue("@Notice", (object?)m.Notice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ValidFrom", ToDbDate(m.ValidFrom));
            cmd.Parameters.AddWithValue("@ValidTo", ToDbDate(m.ValidTo));
            cmd.Parameters.AddWithValue("@AddedBy", (object?)m.AddedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // UPDATE
        public bool Update(NoticeModel m)
        {
            using var con = new SqlConnection(GetConnectionString());
            using var cmd = CreateCommand(con, 2);
            cmd.Parameters.AddWithValue("@NoticeId", (object?)m.NoticeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notice", (object?)m.Notice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ValidFrom", ToDbDate(m.ValidFrom));
            cmd.Parameters.AddWithValue("@ValidTo", ToDbDate(m.ValidTo));
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)m.UpdatedBy ?? DBNull.Value);
            con.Open();
            object? r = cmd.ExecuteScalar();
            return r != null && Convert.ToInt32(r) > 0;
        }

        // DELETE (soft)
        public bool Delete(int noticeId, int? updatedBy = null)
        {
            using var con = new SqlConnection(GetConnectionString());
            using var cmd = CreateCommand(con, 3);
            cmd.Parameters.AddWithValue("@NoticeId", noticeId);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            con.Open();
            object? r = cmd.ExecuteScalar();
            return r != null && Convert.ToInt32(r) > 0;
        }

        // GET (dashboard: sab ya Id se)
        public List<NoticeModel> Get(int? noticeId = null) => Read(4, noticeId);

        // GET ACTIVE (website: sirf valid notices)
        public List<NoticeModel> GetActive() => Read(5, null);

        private List<NoticeModel> Read(int parameter, int? noticeId)
        {
            var list = new List<NoticeModel>();
            using var con = new SqlConnection(GetConnectionString());
            using var cmd = CreateCommand(con, parameter);
            if (noticeId.HasValue) cmd.Parameters.AddWithValue("@NoticeId", noticeId.Value);
            con.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new NoticeModel
                {
                    NoticeId = rd["NoticeId"] == DBNull.Value ? null : Convert.ToInt32(rd["NoticeId"]),
                    Notice = rd["Notice"] == DBNull.Value ? null : rd["Notice"].ToString(),
                    ValidFrom = rd["ValidFrom"] == DBNull.Value ? null : rd["ValidFrom"].ToString(),
                    ValidTo = rd["ValidTo"] == DBNull.Value ? null : rd["ValidTo"].ToString(),
                    //ValidFrom = rd["ValidFrom"] == DBNull.Value ? null : Convert.ToDateTime(rd["ValidFrom"]).ToString("yyyy-MM-dd"),
                    //ValidTo = rd["ValidTo"] == DBNull.Value ? null : Convert.ToDateTime(rd["ValidTo"]).ToString("yyyy-MM-dd")
                });
            }
            return list;
        }
    }
}