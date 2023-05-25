using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumers(Assembly.GetExecutingAssembly());
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("localhost");
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("serviceB", false));
        configurator.UseMessageRetry((retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)));
    });
});

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
