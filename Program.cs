using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Concrete;
using Repository.Interface;
using Services.Concrete;
using Services.Interface;
using System.Text;
using Infrastructure.Entity;
using Infrastructure.Context.SeedData;
using Infrastructure.Mapper;
using Infrastructure.Utilities.Concrete;
using Infrastructure.Utilities.Interface;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);



//For Entity Framework
var Configuration = builder.Configuration;
//Add Controller
builder.Services.AddControllers();
//Add Context with connection-string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("name_of_connection_string")));
//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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


//Define Model-Lifetime
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<SeedUserRole>();
builder.Services.AddScoped<IUtility, UtilityUser>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Build the Webapplication-builder.
var app = builder.Build();

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
        var seeder = scope.ServiceProvider.GetRequiredService<SeedUserRole>();
        await seeder.SeedRoles(context); // Run the seeding logic
    }
   

}
catch (Exception e)
{

    throw;
}


app.Run();
