using EnterpreneurCabinetAPI.Services;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //var awsOptions = builder.Configuration.GetAWSOptions();

        builder.Services.AddControllers();
        builder.Services.AddSingleton<MongoDBService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        });
        builder.Services.AddMemoryCache();

        var mongoDbSettings = builder.Configuration.GetSection("DatabaseSettings");

        var connectionString = builder.Configuration.GetConnectionString("MongoDB");
        var configValue = builder.Configuration["ConnectionStrings:MongoDB"];
        Console.WriteLine($"MongoDB Connection String from appsettings: '{configValue}'");

        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                return new MongoClient(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating MongoClient: {ex.Message}");
                throw;
            }
        });

        builder.Services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDbSettings["DatabaseName"]);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyCorsPolicy", configurePolicy: builder =>
                builder.WithOrigins(
                    "http://localhost:8080",
                    "http://3.123.191.106:22891",
                    "http://3.123.191.106:22892",
                    "http://localhost:22892",
                    "http://0.0.0.0:22892",
                    "http://127.0.0.1:27017"
                )
                       .AllowAnyHeader()
                       .AllowAnyMethod());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("MyCorsPolicy");
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
