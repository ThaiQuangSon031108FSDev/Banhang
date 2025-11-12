using Microsoft.Data.SqlClient;

namespace Banhang.Areas.Admin.Data
{
    public class UserDAO
    {
        private readonly string _conn;

        public UserDAO(string connectionString)
        {
            _conn = connectionString;
        }

        public int CountUsers()
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users", cn);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int CountEmployees()
        {
            using var cn = new SqlConnection(_conn);
            // giả sử RoleID: 1=Admin, 2=Employee, 3=Customer
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE RoleID IN (1,2)", cn);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int CountCustomers()
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE RoleID = 3", cn);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}