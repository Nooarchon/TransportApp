using Dapper;
using Npgsql;
using TransportApp;
using TransportApp.Models;
using TransportApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES CONFIGURATION (Must be before builder.Build()) ---
string connString = "Host=localhost;Username=postgres;Password=1;Database=pid_transport";
string gtfsPath = @"C:\Users\julia\source\repos\TransportApp";

// Register your Database helper
builder.Services.AddSingleton(new Database(connString));

// Register NpgsqlConnection for Dapper (using the connection string above)
builder.Services.AddScoped<NpgsqlConnection>(sp => new NpgsqlConnection(connString));

// Register Business Services
builder.Services.AddSingleton<RoutingService>();
builder.Services.AddScoped<StopsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 2. BUILD THE APP ---
var app = builder.Build(); // The service collection is now read-only.

// --- 3. INITIALIZATION (Using the built services) ---
using (var scope = app.Services.CreateScope())
{
    var routing = scope.ServiceProvider.GetRequiredService<RoutingService>();
    var db = scope.ServiceProvider.GetRequiredService<Database>();

    try
    {
        using var conn = db.GetConnection();
        await conn.OpenAsync();

        // Check if there is already data in the stops table
        var count = await conn.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM stops");

        if (count == 0)
        {
            Console.WriteLine("Database is empty. Starting GTFS import...");
            await DataHelper.InitializeDatabase(connString, gtfsPath);
        }
        else
        {
            Console.WriteLine("Data already loaded. Skipping import.");
        }

        // Build the Dijkstra graph for the RoutingService
        await routing.BuildGraph(conn);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during initialization: {ex.Message}");
    }
}

// --- 4. MIDDLEWARE & ROUTES ---
if (app.Environment.IsDevelopment())
{
    // If you use Swagger, it would be configured here
}

app.MapControllers();
app.MapGet("/", () => "PID Transport API is running!");

// --- 5. RUN ---
app.Run();