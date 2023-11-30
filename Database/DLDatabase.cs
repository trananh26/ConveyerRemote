using System;
using System.Collections.Generic;
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
    }
}
