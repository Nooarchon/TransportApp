using Dapper;
using Npgsql;
using System.Globalization;
using System.Text.RegularExpressions;
using TransportApp.Models;

namespace TransportApp;

public static class DataHelper
{
    public static async Task InitializeDatabase(string connectionString, string path)
    {
        using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Создание схемы таблиц
        var sql = @"
            DROP TABLE IF EXISTS stop_times;
            DROP TABLE IF EXISTS trips;
            DROP TABLE IF EXISTS stops;
            DROP TABLE IF EXISTS routes;

            CREATE TABLE stops (
                stop_id TEXT PRIMARY KEY,
                stop_name TEXT,
                stop_lat DOUBLE PRECISION,
                stop_lon DOUBLE PRECISION
            );

            CREATE TABLE trips (
                trip_id TEXT PRIMARY KEY,
                route_id TEXT,
                trip_headsign TEXT
            );

            CREATE TABLE stop_times (
                trip_id TEXT,
                arrival_time TEXT,
                departure_time TEXT,
                stop_id TEXT,
                stop_sequence INT
            );

            CREATE TABLE routes (
                route_id TEXT PRIMARY KEY,
                route_short_name TEXT
            );";

        await using (var cmd = new NpgsqlCommand(sql, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        Console.WriteLine("--- Tables created. Importing GTFS data... ---");

        // 2. Последовательный импорт всех файлов
        await ImportStops(conn, Path.Combine(path, "stops.txt"));
        await ImportRoutes(conn, Path.Combine(path, "routes.txt"));
        await ImportTrips(conn, Path.Combine(path, "trips.txt"));
        await ImportStopTimes(conn, Path.Combine(path, "stop_times.txt"));

        Console.WriteLine("--- Import complete! ---");
    }

    static async Task ImportStops(NpgsqlConnection conn, string filePath)
    {
        Console.WriteLine("Importing stops...");
        using var reader = new StreamReader(filePath);

        // 1. Read Header and map columns
        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null) return;
        var headers = headerLine.Split(',').Select(h => h.Trim('"')).ToList();

        int idIdx = headers.IndexOf("stop_id");
        int nameIdx = headers.IndexOf("stop_name");
        int latIdx = headers.IndexOf("stop_lat");
        int lonIdx = headers.IndexOf("stop_lon");

        using var writer = conn.BeginBinaryImport("COPY stops (stop_id, stop_name, stop_lat, stop_lon) FROM STDIN (FORMAT BINARY)");

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Use Regex split to handle commas inside stop names like "Praha, Masarykovo nádraží"
            var p = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                         .Select(x => x.Trim('"'))
                         .ToArray();

            try
            {
                writer.StartRow();
                writer.Write(p[idIdx]);
                writer.Write(p[nameIdx]);

                if (double.TryParse(p[latIdx], CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(p[lonIdx], CultureInfo.InvariantCulture, out double lon))
                {
                    writer.Write(lat);
                    writer.Write(lon);
                }
                else
                {
                    // If we reach here, the data in those columns is invalid
                    Console.WriteLine($"Skipping row due to invalid coordinates: {p[latIdx]}, {p[lonIdx]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line: {line}");
                throw;
            }
        }
        await writer.CompleteAsync();
    }

    static async Task ImportStopTimes(NpgsqlConnection conn, string filePath)
    {
        Console.WriteLine("Importing stop_times...");
        using var reader = new StreamReader(filePath);
        var headerLine = await reader.ReadLineAsync();
        var headers = headerLine?.Split(',').Select(h => h.Trim('"')).ToList();

        // Map column indices for stop_times
        int tripIdIdx = headers.IndexOf("trip_id");
        int arrIdx = headers.IndexOf("arrival_time");
        int depIdx = headers.IndexOf("departure_time");
        int stopIdIdx = headers.IndexOf("stop_id");
        int seqIdx = headers.IndexOf("stop_sequence");

        using var writer = conn.BeginBinaryImport("COPY stop_times (trip_id, arrival_time, departure_time, stop_id, stop_sequence) FROM STDIN (FORMAT BINARY)");
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = line.Split(',').Select(x => x.Trim('"')).ToArray();

            writer.StartRow();
            writer.Write(p[tripIdIdx]);
            writer.Write(p[arrIdx]);
            writer.Write(p[depIdx]);
            writer.Write(p[stopIdIdx]);
            writer.Write(int.Parse(p[seqIdx]));
        }
        await writer.CompleteAsync();
    }

    static async Task ImportRoutes(NpgsqlConnection conn, string filePath)
    {
        Console.WriteLine("Importing routes...");
        using var reader = new StreamReader(filePath);
        await reader.ReadLineAsync();

        using var writer = conn.BeginBinaryImport("COPY routes (route_id, route_short_name) FROM STDIN (FORMAT BINARY)");
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = line.Split(',');
            writer.StartRow();
            writer.Write(p[0].Trim('"'));
            writer.Write(p[2].Trim('"'));
        }
        await writer.CompleteAsync();
    }

    static async Task ImportTrips(NpgsqlConnection conn, string filePath)
    {
        Console.WriteLine("Importing trips...");
        using var reader = new StreamReader(filePath);
        await reader.ReadLineAsync();

        using var writer = conn.BeginBinaryImport("COPY trips (route_id, trip_id, trip_headsign) FROM STDIN (FORMAT BINARY)");
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = line.Split(',');
            writer.StartRow();
            writer.Write(p[0].Trim('"'));
            writer.Write(p[2].Trim('"'));
            writer.Write(p[3].Trim('"'));
        }
        await writer.CompleteAsync();
    }

    public static async Task LoadExtraData()
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            var response = await client.GetStringAsync("https://data.pid.cz/pointsOfSale/json/pointsOfSale.json");
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = System.Text.Json.JsonSerializer.Deserialize<PointsOfSaleRoot>(response, options);
            Console.WriteLine($"Успех! Найдено точек продажи: {root?.points?.Count ?? 0}");
        }
        catch (Exception ex) { Console.WriteLine($"JSON Error: {ex.Message}"); }
    }
}