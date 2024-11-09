using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data;
using static SmartLockSystem.Controllers.LockEventsController;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("db1")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllHeaders",
//    c =>

//    {
//        c.AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://smartlock.z6.web.core.windows.net") // Allow specific origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow credentials
        });
});


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        builder => builder.AllowAnyOrigin()
//                          .AllowAnyMethod()
//                          .AllowAnyHeader());
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
//app.UseCors("AllowSpecificOrigin");
//app.UseCors("AllowAllHeaders");
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Define the URL of your SignalR Hub here
    endpoints.MapHub<LockHub>("/lockHub");
    endpoints.MapControllers();
});
app.Run();