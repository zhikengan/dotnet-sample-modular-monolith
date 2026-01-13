using App.Web.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using App.Modules.User;
using App.Modules.Ordering;
using App.Modules.Notification;
using App.Modules.Catalog;
using MediatR;
using App.Shared.Infrastructure;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Blazor & MudBlazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// MediatR & Pipeline Behaviors
// Note: Module-specific handlers are now registered in their own modules.
// We register Program/Shared assemblies here and attach global behaviors.
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblies(
        typeof(App.Shared.Infrastructure.ICommand).Assembly,
        typeof(Program).Assembly
    );
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // In a real app, use a secure key from configuration
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345678901234567890")),
        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EncryptionKey12345678901234567890")) // JWE Decryption
    };

    // Read token from Cookie if Header is missing
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthCookie"))
            {
                context.Token = context.Request.Cookies["AuthCookie"];
            }
            return Task.CompletedTask;
        }
    };
});

// Default Policy: Require Authenticated User
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// MassTransit with SQL Transport (PostgreSQL)
builder.Services.AddOptions<SqlTransportOptions>().Configure(options =>
{
    options.ConnectionString = "Host=localhost;Port=5432;Database=marketplace_db;Username=postgres;Password=postgres";
});

builder.Services.AddPostgresMigrationHostedService();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    // Consumers from Modules
    x.AddConsumers(typeof(UserModule).Assembly);
    x.AddConsumers(typeof(OrderingModule).Assembly);
    x.AddConsumers(typeof(NotificationModule).Assembly);

    x.UsingPostgres((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

// Add HttpContextAccessor for LoginUser handler cookie access
builder.Services.AddHttpContextAccessor();

// Modules
builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddOrderingModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(global::App.Portal.Admin._Imports).Assembly);

// Map Endpoints
app.MapUserModuleEndpoints();
app.MapOrderingModuleEndpoints();
app.MapCatalogModuleEndpoints();

app.Run();
