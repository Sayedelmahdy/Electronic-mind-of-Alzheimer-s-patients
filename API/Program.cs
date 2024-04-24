using API.CustomTokenProvider;
using BLL.Helper;
using BLL.Interfaces;
using BLL.Services;
using DAL.Context;
using DAL.Interfaces;
using DAL.Model;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Threading.RateLimiting;
using BLL.Hubs;
using System.Reflection;
using API.Middlewares;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("./Log/LoggingFile-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSignalRSwaggerGen(_ =>
    {
        _.UseHubXmlCommentsSummaryAsTagDescription = true;
        _.UseHubXmlCommentsSummaryAsTag = true;
        _.UseXmlComments(xmlPath);
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<Mail>(builder.Configuration.GetSection("Mail"));

builder.Services.AddDbContext<DBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddIdentity<User, IdentityRole>(option=>
{
    option.SignIn.RequireConfirmedEmail= true;
    option.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
}).AddEntityFrameworkStores<DBContext>().AddDefaultTokenProviders().AddTokenProvider<EmailTokenProvider<User>>("emailconfirmation");
builder.Services.AddIdentityCore<Patient>(option =>
{
   
}).AddEntityFrameworkStores<DBContext>();
builder.Services.AddIdentityCore<Family>(option =>
{
   
}).AddEntityFrameworkStores<DBContext>();
builder.Services.AddIdentityCore<Caregiver>(option =>
{
   
}).AddEntityFrameworkStores<DBContext>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(option =>
{
    option.TokenLifespan=TimeSpan.FromHours(2);
});
builder.Services.Configure<EmailConfirmationTokenProviderOptions>(option =>
{
    option.TokenLifespan = TimeSpan.FromDays(5);
});
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFamilyService, FamilyService>();
builder.Services.AddScoped<ICaregiverService,CaregiverService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IDecodeJwt, DecodeJwt>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddRateLimiter(option =>
{
    option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    option.AddPolicy("Fixed", Context => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: Context.User.Identity?.ToString(),
        factory: op => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(10),
        }));
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; 
});
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; 
    x.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.Services.AddRazorPages();
builder.Services.AddSignalR().AddAzureSignalR("Endpoint=https://notificationalzheimer.service.signalr.net;AccessKey=YujUCHeAPIHNhjWcm3HcFpgR9gD+ijcZIe9q1iB/t2U=;Version=1.0;");
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder
        .WithOrigins("https://localhost:7174")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
        
    }));
var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{

}*/

app.MapHub<MedicineReminderHub>("hubs/medicineReminder");
app.MapHub<AppointmentHub>("hubs/Appointment");
app.MapHub<GPSHub>("hubs/GPS");


app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();

}

if (app.Environment.IsProduction())
{
    app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electronic mind of Alzheimer's patients v1");
    c.RoutePrefix = string.Empty;
});
}
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthorization();
app.MapRazorPages();
app.UseLoggingMiddleware();
app.MapControllers();

app.Run();
