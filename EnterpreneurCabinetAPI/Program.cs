using EnterpreneurCabinetAPI.Services;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options=>options.SerializerSettings.ContractResolver
    = new DefaultContractResolver());
;
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
            "http://35.156.19.206:8080",
            "http://35.156.19.206",
            "http://35.157.100.194:22892",
            "http://35.156.19.206:22892",
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

app.Run();
