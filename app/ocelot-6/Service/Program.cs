// Learn more about configuring Ocelot/OpenAPI at https://ocelot.readthedocs.io/
// ref : "API Gateway Ocelot .Net Core 6.1 Setup" at https://stackoverflow.com/questions/71264496

// Import library
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

// Creaate application builder
var builder = WebApplication.CreateBuilder(args);

// Generate ocelot configuration file with multiple json file
// Setting ocelot service with hostingContext and config object.
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) => {
    // Setting host server setting and default ocelot configuration
    // When this function execute, default appsettings will not working, you must addition by yourself.
    var env = hostingContext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", true, true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
          .AddJsonFile("ocelot.json")
          .AddEnvironmentVariables();
    // Merge ocelot configuration come from path "/app/ocelot", and replace local ocelot.json.
    config.AddOcelot("/app/ocelot", env);
});

// Add ocelot configuration with single json file
// Declare ocelot configuration by setting json file.
/*
IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("ocelot.json")
                            .Build();
*/
// Set ocelot in services.
builder.Services.AddOcelot();

// Build and setting application
var app = builder.Build();

// Sets up all the Ocelot middleware
await app.UseOcelot();

// Sets up application routing
app.MapGet("/", () => "Hello World!");

// Startup application
app.Run();
