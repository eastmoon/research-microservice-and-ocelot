// Learn more about configuring Ocelot/OpenAPI at https://ocelot.readthedocs.io/
// ref : "API Gateway Ocelot .Net Core 6.1 Setup" at https://stackoverflow.com/questions/71264496

// Import library
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

// Creaate application builder
var builder = WebApplication.CreateBuilder(args);

// Declare ocelot configuration by setting json file.
IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("ocelot.json")
                            .Build();
// Setting ocelot service with configuration variable.
builder.Services.AddOcelot(configuration);

// Build and setting application
var app = builder.Build();

// Sets up all the Ocelot middleware
await app.UseOcelot();

// Sets up application routing
app.MapGet("/", () => "Hello World!");

// Startup application
app.Run();
