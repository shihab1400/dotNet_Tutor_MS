using EventManagement.Shared;
using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Repos;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AsDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AsDbContext")));
builder.Services.AddScoped<OfferedCourseRepo>();
builder.Services.AddScoped<RecommendationRepo>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<GroupRepo>();
builder.Services.AddScoped<RatingRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<CurrentUserHelper>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication("AsAuth").AddCookie("AsAuth", opt =>
{
    opt.AccessDeniedPath = "/Auth/Denied";
    opt.LoginPath = "/Auth/Login";
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=OfferedCourse}/{action=Index}/{id?}");

app.Run();
