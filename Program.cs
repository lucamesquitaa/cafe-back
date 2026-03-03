using Google.Api;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Turify.Data;
using Turify.Facades;
using Turify.Facades.Interfaces;
using Turify.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

//HOMOLOG
// 🔐 Acessa o segredo na inicialização da aplicação
//var secretClient = SecretManagerServiceClient.Create();

//var connStringSecret = new SecretVersionName(projectId, connString, "latest");
//var resultConn = await secretClient.AccessSecretVersionAsync(connStringSecret);

//var passSecretaSecret = new SecretVersionName(projectId, passSecreta, "latest");
//var resultPass = await secretClient.AccessSecretVersionAsync(passSecretaSecret);
//HOMOLOG
//var connectionString = result.Payload.Data.ToStringUtf8();

// var connectionString = "Host=34.46.28.173;Port=5432;Database=hotelariadb;Username=postgres;Password=X(y4M&.}@Mes6TZJ";

// Prefer an explicit connection string from configuration (appsettings.*.json) or environment variable
var dbHost = "localhost";
var dbUser = "lucam";
var dbPass = "Xy4MMes6TZj";

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(serverOptions =>
{
 serverOptions.ListenAnyIP(Int32.Parse(port));
});

builder.Services.AddCors(options =>
{
 options.AddPolicy("AllowAngular",
 policy => policy
 .WithOrigins("http://localhost:4200", "https://turify.com.br")
 .AllowAnyHeader()
 .AllowAnyMethod());
});
builder.Services.AddHttpContextAccessor();

// Serviços
builder.Services.AddTransient<UserFacade>();
builder.Services.AddTransient<HotelFacade>();
builder.Services.AddTransient<QuartosFacade>();
builder.Services.AddTransient<CategoryQuartosFacade>();
builder.Services.AddTransient<UtilsFacade>();
builder.Services.AddTransient<PhotosFacade>();
builder.Services.AddTransient<MotorDeReservasFacade>();

// Registrar o Producer como singleton ou scoped
builder.Services.AddSingleton<IRabbitMqProducer>(sp =>
 new RabbitMQProducer(
 builder.Configuration["RabbitMQ:HostName"],
 builder.Configuration["RabbitMQ:QueueName"]
));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<GoogleAuthService>();
var connectionString = "Host=149.57.203.34;Port=5432;Database=hotelariadb;Username=lucam;Password=Xy4MMes6TZj";
builder.Services.AddDbContext<Turify.Data.Context>(options =>
 options.UseNpgsql(connectionString)
);

// Redis configuration: read from configuration or environment variable with sensible default.
var redisConfiguration = builder.Configuration.GetValue<string>("Redis:Configuration")
 ?? Environment.GetEnvironmentVariable("REDIS_HOST")
 ?? "localhost:6379";
var redisInstanceName = builder.Configuration.GetValue<string>("Redis:InstanceName") ?? "ReservaApi:";

builder.Services.AddStackExchangeRedisCache(options =>
{
 options.Configuration = redisConfiguration;
 options.InstanceName = redisInstanceName;
});

var jwtToken = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("jwttoken", ""));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = false,
 ValidateAudience = false,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 IssuerSigningKey = new SymmetricSecurityKey(
 jwtToken
 )
 };
 options.Events = new JwtBearerEvents
 {
 OnAuthenticationFailed = context =>
 {
 if (context.Exception is SecurityTokenExpiredException)
 {
 context.Response.Headers.Add("Token-Expired", "true");
 }
 return Task.CompletedTask;
 },
 OnChallenge = context =>
 {
 context.HandleResponse();
 context.Response.StatusCode =401;
 context.Response.ContentType = "application/json";
 var result = JsonSerializer.Serialize(new
 {
 status =401,
 error = "Token is expired or invalid"
 });
 return context.Response.WriteAsync(result);
 }
 };
 });


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotelaria API", Version = "v2" });

 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
 {
 In = ParameterLocation.Header,
 Description = "Por favor, insira o token JWT com o prefixo 'Bearer '",
 Name = "Authorization",
 Type = SecuritySchemeType.ApiKey,
 Scheme = "Bearer"
 });

 // Adiciona o requisito de segurança
 c.AddSecurityRequirement(new OpenApiSecurityRequirement
 {
 {
 new OpenApiSecurityScheme
 {
 Reference = new OpenApiReference
 {
 Type = ReferenceType.SecurityScheme,
 Id = "Bearer"
 }
 },
 new string[] {}
 }
 });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
 var context = scope.ServiceProvider.GetRequiredService<Turify.Data.Context>();
 context.Database.Migrate();
}
app.UseMiddleware<ErrorLoggingMiddleware>();
app.Use(async (context, next) =>
{
 context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
 context.Response.Headers["Cross-Origin-Embedder-Policy"] = "unsafe-none";
 await next();
});

app.UseCors("AllowAngular");

app.UseSwagger();
 app.UseSwaggerUI(c =>
 {
 c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotelaria API v2");
 });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
