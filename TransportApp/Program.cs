using Dapper;
using Npgsql;
using TransportApp;
using TransportApp.Models;
using TransportApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES
string connString = "Host=localhost;Username=postgres;Password=1;Database=pid_transport";
string gtfsPath = @"C:\Users\julia\source\repos\TransportApp";

// 1. SERVICES
builder.Services.AddSingleton(new Database(connString));
builder.Services.AddSingleton<RoutingService>();
builder.Services.AddScoped<StopsService>(); // Добавьте это!
builder.Services.AddControllers();
var app = builder.Build();

// 2. INITIALIZATION
using (var scope = app.Services.CreateScope())
{
    var routing = scope.ServiceProvider.GetRequiredService<RoutingService>();
    var db = scope.ServiceProvider.GetRequiredService<Database>();

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

    await routing.BuildGraph(conn);
}

// 3. MIDDLEWARE & ROUTES
app.MapControllers();
app.MapGet("/", () => "PID Transport API is running!");

// 4. RUN
app.Run();