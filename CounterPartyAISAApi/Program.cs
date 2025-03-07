using AADEPortalApi;

//using AADEPortalApi.Utils;
using Asp.Versioning;

using CosmoONE.Common.Utilities;
using CounterPartyAISAApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using static CosmoONE.Common.Utilities.Enums.Enums;

//var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AADE Api - V1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "AADE Api - V2", Version = "v2.0" });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    // If the client hasn't specified the API version in the request, use the default API version number as the default value
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    //options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();




var x = AppDomain.CurrentDomain.GetAssemblies();
//Add the Auto Mapper 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



//dbContext 



builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddHttpClient();


//Services 


//Serilog Logging
Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();



//todo: Symplhrwsh twn swston JwtBearerSchemes  sto appsettings.json

var jwtSettings = builder.Configuration.GetSection("Authentication:JwtBearerSchemes").Get<List<JwtSettings>>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Authentication:JwtBearer"));



// Configure Authentication
foreach (var schemeConfig in jwtSettings)
{
    builder.Services.AddAuthentication()
        .AddJwtBearer(schemeConfig.Name, options =>
        {
            options.Authority = schemeConfig.Authority;
            options.RequireHttpsMetadata = schemeConfig.RequireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = schemeConfig.ValidIssuer,
                ValidateAudience = false,

                ValidAudience = schemeConfig.ValidAudience,
                ValidateLifetime = true,
                //ClockSkew = TimeSpan.FromSeconds(schemeConfig.ClockSkewSeconds)
            };
            options.IncludeErrorDetails = true;
        });
}




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Common_Policy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("Common_Scheme"));

    options.AddPolicy("QnR_Policy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("QnR_Scheme"));

    options.AddPolicy("Both_Policy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("Common_Scheme", "QnR_Scheme"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}




app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
