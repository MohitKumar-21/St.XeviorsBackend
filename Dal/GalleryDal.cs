// DAL/GalleryDal.cs
using System.Data;
using Microsoft.Data.SqlClient;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.DAL
{
    public class GalleryDal
    {
        private readonly string _conn;

        public GalleryDal(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Multiple images ek transaction me
        public int InsertMany(int coverImgId, List<string> imagePaths, int? addedBy)
        {
            using var con = new SqlConnection(_conn);
            con.Open();
            using var tran = con.BeginTransaction();
            try
            {
                int count = 0;
                foreach (var path in imagePaths)
                {
                    using var cmd = new SqlCommand("SP_Gallery", con, tran) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.AddWithValue("@Parameter", 2);
                    cmd.Parameters.AddWithValue("@CoverImgId", coverImgId);
                    cmd.Parameters.AddWithValue("@ImagePath", path);
                    cmd.Parameters.AddWithValue("@AddedBy", (object?)addedBy ?? DBNull.Value);
                    cmd.ExecuteScalar();
                    count++;
                }
                tran.Commit();
                return count;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool Delete(int GalleryId, int? updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_Gallery", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 4);
            cmd.Parameters.AddWithValue("@GalleryId", GalleryId);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public List<GalleryModel> Get(int? coverImgId = null)
        {
            var list = new List<GalleryModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_Gallery", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 1);
            cmd.Parameters.AddWithValue("@CoverImgId", (object?)coverImgId ?? DBNull.Value);
            con.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new GalleryModel
                {
                    GalleryId = Convert.ToInt32(r["GalleryId"]),
                    CoverImgId = Convert.ToInt32(r["CoverImgId"]),
                    CoverTitle = r["CoverTitle"] as string,
                    ImagePath = r["ImagePath"] as string,                
                });
            }
            return list;
        }

        public bool Update(int galleryId, int? coverImgId, string? imagePath, int? updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_Gallery", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Parameter", 3);
            cmd.Parameters.AddWithValue("@GalleryId", galleryId);
            cmd.Parameters.AddWithValue("@CoverImgId", (object?)coverImgId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImagePath", (object?)imagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}