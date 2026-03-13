using NotificationService.Application.Interfaces;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Interfaces.Services;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(NotificationService.Presentation.Controllers.NotificationsController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FIAP X - Notification Service API",
        Version = "v2.0.0",
        Description = "Microsserviço de notificações com Clean Architecture"
    });
});

// ==== DEPENDENCY INJECTION - Clean Architecture ====

// Domain Layer Interfaces -> Infrastructure Implementations
builder.Services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

// Application Layer - Use Cases
builder.Services.AddScoped<ISendNotificationUseCase, SendNotificationUseCase>();
builder.Services.AddScoped<IGetUserNotificationsUseCase, GetUserNotificationsUseCase>();

// ==== END DEPENDENCY INJECTION ====

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v2.0");
    });
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/healthz", () => Results.Ok(new 
{ 
    status = "healthy", 
    service = "fiap-x-notification-service",
    architecture = "clean-architecture",
    version = "2.0.0",
    timestamp = DateTime.UtcNow 
}));

app.Run();
