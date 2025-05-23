using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using study_worker.domain.Interfaces.Repositories;
using study_worker.infra.Data;
using study_worker.infra.Messaging;
using study_worker.infra.Repositories;
using study_worker.service.Implementations;
using study_worker.service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS (opcional, ajuste conforme necessário)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ Configuration
var rabbitMQSettings = new RabbitMQSettings();
builder.Configuration.GetSection("RabbitMQ").Bind(rabbitMQSettings);
builder.Services.AddSingleton(rabbitMQSettings);

// Configuração do repositório
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Configuracao de Service 
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Messaging registrations
builder.Services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();
builder.Services.AddHostedService<RabbitMQConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS (se configurado)
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Middleware para tratamento de erros (opcional)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred");
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsync("An unexpected error occurred.");
    }
});

app.Run();