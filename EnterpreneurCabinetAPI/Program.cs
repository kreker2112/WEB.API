using EnterpreneurCabinetAPI.Services;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using Amazon.KeyManagementService;
using System; // Ensure System namespace is included for Console

var builder = WebApplication.CreateBuilder(args);

// Configure AWS KMS Client
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddAWSService<IAmazonKeyManagementService>(awsOptions);

// Add services to the container.
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

// MongoDB registration
var mongoDbSettings = builder.Configuration.GetSection("DatabaseSettings");

// Logging the connection string to ensure it's being retrieved correctly
var connectionString = builder.Configuration.GetConnectionString("MongoDB");
Console.WriteLine($"Connection string: {connectionString}");

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
        throw;  // Rethrow the exception to make sure the application doesn't start if there's a configuration issue
    }
});
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbSettings["DatabaseName"]);
});

// Adding CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", configurePolicy: builder =>
        builder.WithOrigins(
            "http://localhost:8080",
            "http://3.123.191.106:22891",
            "http://3.123.191.106:22892",
            "http://localhost:22892",
            "http://0.0.0.0:22892"
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
app.Run("http://0.0.0.0:22892");

