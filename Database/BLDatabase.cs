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
        /// Lấy lịch sử chấm công để xuất excel
        /// </summary>
        /// <returns></returns>
        public DataTable GetHistory()
        {
            DateTime now = DateTime.Now;
            DateTime ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            DateTime FromDate = new DateTime(now.Year, now.Month, 1, 00, 00, 00);

            string sqlCommand = "EXEC dbo.Proc_GetHistoryForExport @FromDate = '" + FromDate + "', @ToDate = '" + ToDate + "', @EmployeeCode = 'all'";
            return db.GetHistoryForExport(sqlCommand);
        }


        public DataSet GetHistoryForReport()
        {
            throw new NotImplementedException();
        }
    }
}
