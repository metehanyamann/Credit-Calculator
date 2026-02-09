using KrediHesaplama.Application;
using KrediHesaplama.Application.Services;
using KrediHesaplama.Infrastructure.Interfaces;
using KrediHesaplamaAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database Bağlantısı ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Katman Bağımlılıkları (DI) ----------
builder.Services.AddScoped<IHesaplamaService, HesaplamaService>();
builder.Services.AddScoped<ITasitKrediService, TasitKrediService>();
builder.Services.AddScoped<IKonutKrediService, KonutKrediService>();
builder.Services.AddScoped<IIhtiyacKrediService, IhtiyacKrediService>();
builder.Services.AddScoped<KonutKrediService>();
builder.Services.AddScoped<IhtiyacKrediService>();

// ---------- Controller ve Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Groq için gerekli

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------- Middleware Pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
