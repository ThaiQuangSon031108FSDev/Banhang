using Banhang.Data;

namespace Banhang
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Dự phòng về một chuỗi kết nối local (điều chỉnh nếu cần)
                connectionString = "Server=.;Database=WebBanhangDb;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine("CẢNH BÁO: Thiếu DefaultConnection; đang dùng chuỗi dự phòng.");
            }

            // DI DAO
            builder.Services.AddScoped<ProductDAO>(_ => new ProductDAO(connectionString));
            builder.Services.AddScoped<UserDAO>(_ => new UserDAO(connectionString));
            builder.Services.AddScoped<CategoryDAO>(_ => new CategoryDAO(connectionString));
            builder.Services.AddScoped<Banhang.Areas.Admin.Data.ProductDAO>(_ =>
    new Banhang.Areas.Admin.Data.ProductDAO(connectionString));
            builder.Services.AddScoped<Banhang.Areas.Admin.Data.UserDAO>(_ =>
                new Banhang.Areas.Admin.Data.UserDAO(connectionString));

            builder.Services.AddControllersWithViews();

            // Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // Route Area
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // Route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
