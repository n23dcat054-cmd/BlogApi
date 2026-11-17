using BlogApi.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Đọc chuỗi kết nối từ appsettings.json (dự phòng nếu bị rỗng)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    connectionString = "Data Source=blog.db";

// Luôn dùng file database thật, đường dẫn tuyệt đối trong thư mục project
var csb = new SqliteConnectionStringBuilder(connectionString);
if (string.IsNullOrWhiteSpace(csb.DataSource) || csb.DataSource == ":memory:")
    csb.DataSource = "blog.db";
csb.DataSource = Path.Combine(builder.Environment.ContentRootPath, csb.DataSource);
csb.Mode = SqliteOpenMode.ReadWriteCreate;

Console.WriteLine($">>> Database file: {csb.DataSource}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(csb.ToString()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Tự động tạo bảng (áp dụng migration) khi API khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("Frontend");
app.MapControllers();

app.Run();