using CaptureWebcam.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureWebcam.Database
{
    public class DLDatabase
    {
        private static string ConnectionString = ConfigurationSettings.AppSettings["DatabaseConnection"];
        private SqlConnection conn = new SqlConnection(ConnectionString);
        private SqlCommand cmd = new SqlCommand();
        private SqlDataReader dr;
        private SqlDataAdapter da;

        /// <summary>
        /// Lấy lịch sử chấm công để xuất excel
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DataTable GetHistoryForExport(string sqlCommand)
        {
            DataTable dt = new DataTable();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                da = new SqlDataAdapter(sqlCommand, conn);
                da.Fill(dt);
                conn.Close();
            }
            catch (Exception ee)
            {

            }
            return dt;
        }

        /// <summary>
        /// Lấy thông tin hàng theo QR Code
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DataTable GetMaterialInforByCode(string sqlCommand)
        {
            DataTable dt = new DataTable();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                da = new SqlDataAdapter(sqlCommand, conn);
                da.Fill(dt);
                conn.Close();
            }
            catch (Exception ee)
            {

            }
            return dt;
        }

        /// <summary>
        /// insert lịch sử phân loại vào database
        /// </summary>
        /// <param name="Stored"></param>
        /// <param name="material"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void InsertHistory(string Stored, clsMaterial material, String CommandID)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Stored;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CommandID", CommandID);
                cmd.Parameters.AddWithValue("@QRCode", material.QRCode);
                cmd.Parameters.AddWithValue("@ProductCode", material.ProductCode);
                cmd.Parameters.AddWithValue("@ProductName", material.ProductName);
                cmd.Parameters.AddWithValue("@ProductHeight", material.ProductHeight);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ee)
            {

            }
        }
    }
}
