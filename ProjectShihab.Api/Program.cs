using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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
builder.Services.AddScoped<LogRepo>();
builder.Services.AddScoped<CurrentUserHelper>();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
