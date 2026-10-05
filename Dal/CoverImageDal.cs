// DAL/CoverImageDal.cs
using System.Data;
using Microsoft.Data.SqlClient;   // agar aap System.Data.SqlClient use karte ho to wahi likho
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.DAL
{
    public class CoverImageDal
    {
        private readonly string _conn;

        public CoverImageDal(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection")!;
        }

        public int Insert(CoverImageModel m)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CoverImage", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 2);
            cmd.Parameters.AddWithValue("@CoverTitle", (object?)m.CoverTitle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImagePath", (object?)m.ImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AddedBy", (object?)m.AddedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(CoverImageModel m)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CoverImage", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 3);
            cmd.Parameters.AddWithValue("@CoverImgId", m.CoverImgId);
            cmd.Parameters.AddWithValue("@CoverTitle", (object?)m.CoverTitle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImagePath", (object?)m.ImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)m.UpdatedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public bool Delete(int coverImgId, int? updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CoverImage", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 4);
            cmd.Parameters.AddWithValue("@CoverImgId", coverImgId);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public List<CoverImageModel> Get(int? coverImgId = null)
        {
            var list = new List<CoverImageModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CoverImage", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 1);
            cmd.Parameters.AddWithValue("@CoverImgId", (object?)coverImgId ?? DBNull.Value);
            con.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new CoverImageModel
                {
                    CoverImgId = Convert.ToInt32(r["CoverImgId"]),
                    CoverTitle = r["CoverTitle"] as string,
                    ImagePath = r["ImagePath"] as string,
                });
            }
            return list;
        }
    }
}