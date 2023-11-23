using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filter;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Services;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()        
    .AddJsonOptions(options=>
        options.JsonSerializerOptions
           .ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autorização JWT usando o esquema Bearer.\r\n\r\nInforme 'Bearer'[espaço] e o seu token.\r\n\r\nExamplo: \'Bearer 12345abcdef\'",
    });

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

builder.Services.AddCors();

// Configuração do AutoMapper:
var mappingConfig = new MapperConfiguration(mc =>
    mc.AddProfile(new MappingProfile()));

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


string SqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(SqlConnection));


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
         ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
     });

// Exemplo de versionamento no Browser:
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ApiLoggingFilter>();

builder.Services.AddSwaggerGen(option=>
{
    option.SwaggerDoc(
        "apicatalogo",
        new OpenApiInfo()
        {
             Title = "API Catalogo" ,
             Version = "1.0" ,
             TermsOfService = new Uri(" http://sistemahospitalar.gear.host/"),
             Description = "Links Relevantes ao uso da APICatalogo, inclusive com envio de email.",
            License = new Microsoft.OpenApi.Models.OpenApiLicense
            {
                Name = "Canal YouTube do Dev. ",
                Url = new Uri("https://www.youtube.com/channel/UC-7rKFVKo4JNNifPBBEEoYw?view_as=subscriber")
            },
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Email = "p_bruno001@hotmail.com",
                Name = "PAULO CESAR C. BRUNO.",
                Url = new Uri("https://www.linkedin.com/in/paulo-cesar-cordovil-bruno/")
            }
        });

    var xmlCommentsFile =$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlcommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    option.IncludeXmlComments(xmlcommentsFullPath);
});

builder.Logging.AddProvider(new CustomLoggerProvider (new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddTransient<IMeuServico, MeuServico>();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger()
    .UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/apicatalogo/swagger.json", "apicatalogo");
        options.RoutePrefix = "";
    });


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors(x => x.AllowAnyHeader().AllowAnyHeader().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();
