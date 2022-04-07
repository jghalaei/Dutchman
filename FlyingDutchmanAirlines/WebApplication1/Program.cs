using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<FlightService>();
builder.Services.AddTransient<BookingService>();
builder.Services.AddTransient<FlightRepository>();
builder.Services.AddTransient<AirportRepository>();
builder.Services.AddTransient<FlyingDutchmanAirlinesContext>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swagger => swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Flying Dutchman Airlines"));
}

app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();

app.Run();
