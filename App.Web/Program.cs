using App.Web.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using App.Modules.User;
using App.Modules.Ordering;
using App.Modules.Notification;
using App.Web.Endpoints; // New Endpoints
using MediatR;
using App.Shared.Infrastructure;
using MudBlazor.Services;
using App.Shared.Protos; // Proto Namespace

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

// gRPC Client for Catalog Service
builder.Services.AddGrpcClient<CatalogService.CatalogServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Services:Catalog"] ?? "https://localhost:7001"); // Default dev port, will override in docker-compose
});



builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    // Consumers from Modules
    x.AddConsumers(typeof(UserModule).Assembly);
    x.AddConsumers(typeof(OrderingModule).Assembly);
    x.AddConsumers(typeof(NotificationModule).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@localhost:5672");
        cfg.ConfigureEndpoints(context);
    });
});

// Add HttpContextAccessor for LoginUser handler cookie access
builder.Services.AddHttpContextAccessor();

// Modules
builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddOrderingModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);
// Catalog Module Removed (Remote Service)

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
app.MapCatalogEndpoints(); // New Remote Endpoints

app.Run();
