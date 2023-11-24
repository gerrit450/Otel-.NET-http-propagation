using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

ConfigureOpenTelemetry(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

void ConfigureOpenTelemetry(WebApplicationBuilder builder)
{
    // These can come from a config file, constants file, etc.
    var serviceName = "Service - B";
    var serviceNamespace = "Service - B";
    var serviceEnvironment = "dev";

    // Creates only a single instance of a trace for the duration of the service lifespan
    builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

    // Add otel
    // Configure important OpenTelemetry settings, the console exporter, and instrumentation library
    _ = builder.Services
        .AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder => tracerProviderBuilder
            .AddSource(serviceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName, serviceNamespace)
                    // required by sumoLogic
                    .AddAttributes(new KeyValuePair<string, object>[]
                    {
                            new ("deployment.environment", serviceEnvironment)
                        }))
             //enables telemetry data collection of ASP.NET core
             .AddAspNetCoreInstrumentation((options) => options.Filter = httpContext =>
             {
                 if (httpContext.Request.Path.Equals("/_health", StringComparison.OrdinalIgnoreCase))
                 {
                     return false;
                 }

                 // Collect telemetry for all other endpoints
                 return true;
             })
             .AddZipkinExporter()

        //export telemetry to the console
        .AddConsoleExporter()
        );
}

