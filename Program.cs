using Identity_Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Identity_Repository.Concrete;
using Identity_Repository.Interface;
using Identity_Service.Concrete;
using Identity_Service.Interface;
using System.Text;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Context.SeedData;
using Identity_Infrastructure.Mapper;
using Identity_Infrastructure.Utilities.Concrete;
using Identity_Infrastructure.Utilities.Interface;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Identity_Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Identity_Infrastructure.Configurations.EmailConfiguration;


var builder = WebApplication.CreateBuilder(args);

/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7191); // Adjust the port if needed
});
*/
//For Entity Framework
var Configuration = builder.Configuration;
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});*/

//Add Controller
builder.Services.AddControllers();
//Add Context with connection-string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("name_of_connection_string")));
//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddEndpointsApiExplorer();
//Add swagger-configurations.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configure JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
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
            new string [] { }
        }
    });
});
//Add-Authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);//important

    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
//Add-authorization
builder.Services.AddAuthorization();

builder.Services.AddTransient<IEmailService,EmailService>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider,PermissionAuthorizationPolicyProvider>();
builder.Services.AddHttpClient("PermissionApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7191/api/Auth/");
});
builder.Services.AddHttpContextAccessor();  // This is required to access HttpContext in the handler


//Define Model-Lifetime
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<SeedUserData>();
builder.Services.AddScoped<IUtility, UtilityUser>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Build the Webapplication-builder.
var app = builder.Build();

//app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//seed user_roles
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();// This line ensures database creation but doesn't run migrations
        var seeder = scope.ServiceProvider.GetRequiredService<SeedUserData>();
        await seeder.SeedData(context); // Run the seeding logic
    }
   

}
catch (Exception )
{

    throw;
}


app.Run();
