using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataOperation
{
 public static   class SqlHelper
    {
        public static string StrConn = "server=120.55.169.75;database=Music;uid=sa;pwd=wqawd520";
        public static  DataTable ExcuterQuery(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(StrConn))
                {
                

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
                    {

                        adapter.Fill(dt);
                    }
                }
                return dt;
            }
            catch
            {
                return dt;
            }   
           
        }
    }
}
