using Microsoft.Data.SqlClient;
using Banhang.Models;

namespace Banhang.Data
{
    public class UserDAO
    {
        private readonly string _connStr;
        public UserDAO(string connStr) => _connStr = connStr;

        public User? CheckLogin(string username, string password)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"
                SELECT u.UserID, u.Username, u.FullName, u.Email, u.Phone, u.RoleID,
                       r.RoleName, u.IsActive, u.CreatedAt
                FROM Users u
                INNER JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.Username=@u AND u.[Password]=@p AND u.IsActive=1", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password); // Lưu ý: demo; nên hash trong thực tế.

            conn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return MapUser(rd);
        }

        public int RegisterUser(User u, string plainPassword)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"
                INSERT INTO Users(Username, [Password], FullName, Email, Phone, RoleID, IsActive)
                VALUES (@un, @pw, @fn, @em, @ph, @role, 1);
                SELECT SCOPE_IDENTITY();", conn);

            cmd.Parameters.AddWithValue("@un", u.Username);
            cmd.Parameters.AddWithValue("@pw", plainPassword); // TODO: hash
            cmd.Parameters.AddWithValue("@fn", u.FullName);
            cmd.Parameters.AddWithValue("@em", u.Email);
            cmd.Parameters.AddWithValue("@ph", (object?)u.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@role", u.RoleID);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public User? GetUserByID(int id)
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"
                SELECT u.UserID, u.Username, u.FullName, u.Email, u.Phone, u.RoleID,
                       r.RoleName, u.IsActive, u.CreatedAt
                FROM Users u
                INNER JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.UserID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            return rd.Read() ? MapUser(rd) : null;
        }

        public List<User> GetAllEmployees()
        {
            var list = new List<User>();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"
                SELECT u.UserID, u.Username, u.FullName, u.Email, u.Phone, u.RoleID,
                       r.RoleName, u.IsActive, u.CreatedAt
                FROM Users u
                INNER JOIN Roles r ON u.RoleID = r.RoleID
                WHERE r.RoleName IN (N'Admin', N'Employee')", conn);

            conn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(MapUser(rd));
            return list;
        }

        public int InsertEmployee(User u, string plainPassword, bool isAdmin = false)
        {
            u.RoleID = isAdmin ? 1 : 2; // 1-Admin, 2-Employee
            return RegisterUser(u, plainPassword);
        }

        private static User MapUser(SqlDataReader rd) => new User
        {
            UserID = rd.GetInt32(rd.GetOrdinal("UserID")),
            Username = rd.GetString(rd.GetOrdinal("Username")),
            FullName = rd.GetString(rd.GetOrdinal("FullName")),
            Email = rd.GetString(rd.GetOrdinal("Email")),
            Phone = rd["Phone"] as string,
            RoleID = rd.GetInt32(rd.GetOrdinal("RoleID")),
            RoleName = rd.GetString(rd.GetOrdinal("RoleName")),
            IsActive = rd.GetBoolean(rd.GetOrdinal("IsActive")),
            CreatedAt = rd.GetDateTime(rd.GetOrdinal("CreatedAt"))
        };
        public int CountUsers()
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users", conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int CountEmployees()
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE RoleID IN (1,2)", conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int CountCustomers()
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE RoleID = 3", conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}
