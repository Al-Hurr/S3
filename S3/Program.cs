using Microsoft.EntityFrameworkCore;
using S3.ApplicationDbContext;
using S3.Models;
using S3.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    x => x.UseNpgsql(builder.Configuration.GetConnectionString("S3DbConnection")));

MinioClientSettings minioClientSettings = new();
ConfigurationBinder.Bind(builder.Configuration, nameof(MinioClientSettings), minioClientSettings);
builder.Services.AddSingleton<MinioClientSettings>(minioClientSettings);
builder.Services.AddTransient<MinioClientService>();
builder.Services.AddTransient<S3ObjectService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();