using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AsDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AsDbContext")));

// AddScoped -> one object per HTTP request
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<GroupRepo>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<CouponRepo>();
builder.Services.AddScoped<PaymentRepo>();
builder.Services.AddScoped<TrainingRepo>();
builder.Services.AddScoped<OfferedCourseRepo>();
builder.Services.AddScoped<RatingRepo>();
builder.Services.AddScoped<MaterialRepo>();
builder.Services.AddScoped<LogRepo>();
builder.Services.AddScoped<DashboardRepo>();
builder.Services.AddScoped<CurrentUserHelper>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("AsAuth").AddCookie("AsAuth", opt =>
{
    opt.AccessDeniedPath = "/Auth/Denied";
    opt.LoginPath = "/Auth/Login";
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AsDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
    // Demo users, trainings, courses, payments, ratings... (runs once; set "SeedDemoData": false to skip).
    if (app.Configuration.GetValue("SeedDemoData", true))
        DemoDataSeeder.Seed(db, Path.Combine(app.Environment.WebRootPath, "uploads"));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
