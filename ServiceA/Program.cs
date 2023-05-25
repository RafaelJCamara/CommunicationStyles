using Polly;
using Polly.Timeout;
using ServiceA.Clients;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        Random jitterer = new Random();

        builder.Services.AddHttpClient<ServiceBHttpClient>(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5023");
                    })
                    .AddTransientHttpErrorPolicy(configurePolicy: policyBuilder => policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                        5,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                        + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                        onRetry: (outcome, timespan, retryAttempt) =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<ServiceBHttpClient>>()?
                                .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                        }
                    ))
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                        3,
                        TimeSpan.FromSeconds(15),
                        onBreak: (outcome, timespan) =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<ServiceBHttpClient>>()?
                                .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
                        },
                        onReset: () =>
                        {
                            var serviceProvider = builder.Services.BuildServiceProvider();
                            serviceProvider.GetService<ILogger<ServiceBHttpClient>>()?
                                .LogWarning($"Closing the circuit...");
                        }
                    ))
                    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));


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
    }
}