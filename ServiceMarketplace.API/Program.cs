using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceMarketplace.Application.DependencyInjection;
using ServiceMarketplace.Infrastructure.Features.Requests.Queries;
using ServiceMarketplace.Infrastructure.Identity;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddMediatR(typeof(GetRequestsHandler).Assembly);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityServices();

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        )
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Bearer token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("request.create", policy =>
        policy.RequireClaim("permission", "request.create"));

    options.AddPolicy("request.accept", policy =>
        policy.RequireClaim("permission", "request.accept"));

    options.AddPolicy("request.complete", policy =>
        policy.RequireClaim("permission", "request.complete"));
});

// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC",
        policy =>
        {
            policy.WithOrigins("https://localhost:7158") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();


    await DbInitializer.SeedRoles(roleManager);
    await DbInitializer.SeedPermissions(roleManager, context);
    await DbInitializer.SeedAdminUser(userManager, roleManager);

}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMVC");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();