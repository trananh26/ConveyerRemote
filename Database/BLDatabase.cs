using CaptureWebcam.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureWebcam.Database
{
    public class BLDatabase
    {
        DLDatabase db = new DLDatabase();

        /// <summary>
        /// Lấy lịch sử phân loại để xuất excel
        /// </summary>
        /// <returns></returns>
        public DataTable GetHistory()
        {
            DateTime now = DateTime.Now;
            DateTime ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            DateTime FromDate = new DateTime(now.Year, now.Month, 1, 00, 00, 00);

            string sqlCommand = "EXEC dbo.Proc_GetHistoryForExport @FromDate = '" + FromDate + "', @ToDate = '" + ToDate + "'";
            return db.GetHistoryForExport(sqlCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet GetHistoryForReport()
        {
            DataSet ds = new DataSet();
            return ds;
        }

        /// <summary>
        /// Lấy thông tin hàng dựa theo qrcode
        /// </summary>
        /// <param name="qrcode"></param>
        /// <returns></returns>
        public DataTable GetMaterialInforByCode(string qrcode)
        {
            string sqlCommand = "EXEC dbo.Proc_GetMaterialInforByCode @FromDate = '" + qrcode + "'";
            return db.GetMaterialInforByCode(sqlCommand);
        }

        /// <summary>
        /// cập nhật lịch sử phân loại sản phẩm
        /// </summary>
        /// <param name="material"></param>
        public void InsertHistory(clsMaterial material)
        {
            string cmd = "Proc_InsertHistory";
            db.InsertHistory(cmd, material);
        }

        /// <summary>
        /// Lấy hiệu quả làm việc trong ngày
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTable GetPerformentToday()
        {
            throw new NotImplementedException();
        }
    }
}
