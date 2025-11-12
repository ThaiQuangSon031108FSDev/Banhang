using Microsoft.Data.SqlClient;

namespace Banhang.Areas.Admin.Data
{
    public class ProductDAO
    {
        private readonly string _conn;

        public ProductDAO(string connectionString)
        {
            _conn = connectionString;
        }

        public int CountProducts()
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Products", cn);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int CountProductsByCategory(int categoryId)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Products WHERE CategoryID=@cid", cn);
            cmd.Parameters.AddWithValue("@cid", categoryId);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        // Doanh thu (nếu bạn có cột Status trong Carts: 1=Paid)
        // SUM(Quantity * Price) trên các đơn đã thanh toán
        public decimal GetTotalRevenue()
        {
            using var cn = new SqlConnection(_conn);
            // Nếu chưa có bảng Orders, ta lấy từ CartDetails + Products + Carts(Status=1)
            var sql = @"
                SELECT SUM(cd.Quantity * p.Price)
                FROM dbo.CartDetails cd
                JOIN dbo.Carts c    ON c.CartID = cd.CartID
                JOIN dbo.Products p ON p.ProductID = cd.ProductID
                WHERE c.[Status] = 1";  // 1 = Paid
            using var cmd = new SqlCommand(sql, cn);
            cn.Open();
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value ? 0m : Convert.ToDecimal(r);
        }
    }
}