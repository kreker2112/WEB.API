using EnterpreneurCabinetAPI.Services;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using Amazon.KeyManagementService;


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
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDB"));
    return new MongoClient(settings);
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
            "http://localhost:22892"
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

