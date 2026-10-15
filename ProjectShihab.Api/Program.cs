using EventManagement.Shared;
using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AsDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AsDbContext")));

builder.Services.AddScoped<OfferedCourseRepo>(); // AddScoped -> Everytime calling in a same method returns same object(Per Http request)
                                             // AddTransient -> Everytime returns new object(Even in same method)
                                             // AddSingleton -> Creates only one object in a whole session(Even in different method)
                                             // all represent lifetime

builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<GroupRepo>();
builder.Services.AddScoped<CourseRepo>();
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
