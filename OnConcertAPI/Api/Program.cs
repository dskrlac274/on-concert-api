using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnConcert.Api.Controllers;
using OnConcert.Api.Swagger.Filters;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Services.AuthService;
using OnConcert.BL.Services.BandService;
using OnConcert.BL.Services.EventApplicationService;
using OnConcert.BL.Services.EventRatingService;
using OnConcert.BL.Services.EventService;
using OnConcert.BL.Services.EventMessageService;
using OnConcert.BL.Services.OrganizerService;
using OnConcert.BL.Services.PlaceService;
using OnConcert.BL.Services.UserService;
using OnConcert.Core.Helpers;
using OnConcert.DAL;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using OnConcert.BL.Services.BandRatingService;
using OnConcert.BL.Services.PlaceRatingService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

#region Serialization

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

#endregion

#region Swagger

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "OnConcert API",
        Description = "An ASP.NET Core Web API for managing events."
    });

    config.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme, e.g. \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    config.ExampleFilters();

    config.ParameterFilter<RoleParameterFilter>();

    config.OperationFilter<SecurityRequirementsOperationFilter>();
    config.OperationFilter<AuthorizationMarkupFilter>();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<AuthController>();

#endregion

#region Database

builder.Services.AddDbContext<OnConcertContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region AutoMapper

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#endregion

#region Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExternalAuthService, GoogleAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBandService, BandService>();
builder.Services.AddScoped<IOrganizerService, OrganizerService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventApplicationService, EventApplicationService>();
builder.Services.AddScoped<IEventRatingService, EventRatingService>();
builder.Services.AddScoped<IEventMessageService, EventMessageService>();
builder.Services.AddScoped<IPlaceRatingService, PlaceRatingService>();
builder.Services.AddScoped<IBandRatingService, BandRatingService>();

#endregion

#region Repositories

builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Organizer>, Repository<Organizer>>();
builder.Services.AddScoped<IRepository<Band>, Repository<Band>>();
builder.Services.AddScoped<IRepository<Place>, Repository<Place>>();
builder.Services.AddScoped<IRepository<Visitor>, Repository<Visitor>>();
builder.Services.AddScoped<IRepository<Event>, Repository<Event>>();
builder.Services.AddScoped<IRepository<EventApplication>, Repository<EventApplication>>();
builder.Services.AddScoped<IRepository<Rating>, Repository<Rating>>();
builder.Services.AddScoped<IRepository<EventRating>, Repository<EventRating>>();
builder.Services.AddScoped<IRepository<PlaceRating>, Repository<PlaceRating>>();
builder.Services.AddScoped<IRepository<BandRating>, Repository<BandRating>>();
builder.Services.AddScoped<IRepository<Message>, Repository<Message>>();

builder.Services.AddScoped<IReadOnlyRepository<User>, ReadOnlyRepository<User>>();
builder.Services.AddScoped<IReadOnlyRepository<Organizer>, ReadOnlyRepository<Organizer>>();
builder.Services.AddScoped<IReadOnlyRepository<Band>, ReadOnlyRepository<Band>>();
builder.Services.AddScoped<IReadOnlyRepository<Place>, ReadOnlyRepository<Place>>();
builder.Services.AddScoped<IReadOnlyRepository<Visitor>, ReadOnlyRepository<Visitor>>();
builder.Services.AddScoped<IReadOnlyRepository<Rating>, ReadOnlyRepository<Rating>>();
builder.Services.AddScoped<IReadOnlyRepository<EventRating>, ReadOnlyRepository<EventRating>>();
builder.Services.AddScoped<IReadOnlyRepository<PlaceRating>, ReadOnlyRepository<PlaceRating>>();
builder.Services.AddScoped<IReadOnlyRepository<BandRating>, ReadOnlyRepository<BandRating>>();
builder.Services.AddScoped<IReadOnlyRepository<Message>, ReadOnlyRepository<Message>>();

#endregion

#region Utils

builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

#endregion

#region Auth

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    builder.Configuration.GetSection("AppSettings:Jwt:Secret").Value!
                )
            ),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("AppSettings:Jwt:Issuer").Value!,
            ValidateAudience = false
        }
    );

#endregion

#region Policies

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Organizer", policy =>
    {
        policy.RequireRole(nameof(UserRole.Organizer));
    });
    
    options.AddPolicy("Organizer && Visitor", policy =>
    {
        policy.RequireRole(nameof(UserRole.Organizer), nameof(UserRole.Visitor));
    });

    options.AddPolicy("Organizer && Band", policy =>
    {
        policy.RequireRole(nameof(UserRole.Organizer), nameof(UserRole.Band));
    });
    
    options.AddPolicy("Organizer && Place", policy =>
    {
        policy.RequireRole(nameof(UserRole.Organizer), nameof(UserRole.Place));
    });

    options.AddPolicy("Organizer && Place && Band", policy =>
    {
        policy.RequireRole(nameof(UserRole.Organizer), nameof(UserRole.Place), nameof(UserRole.Band));
    });
    
    options.AddPolicy("Band", policy =>
    {
        policy.RequireRole(nameof(UserRole.Band));
    });

    options.AddPolicy("Band && Visitor", policy =>
    {
        policy.RequireRole(nameof(UserRole.Band), nameof(UserRole.Visitor));
    });
});

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();