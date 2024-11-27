using Backend_Final.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//se agrega el servicio de RFID
builder.Services.AddSingleton<RfidService>();

//agrega servicios de RosPec
builder.Services.AddSingleton<RosPecService>();
//agrega main service
builder.Services.AddSingleton<MainService>();

//agrega EpcService
builder.Services.AddSingleton(sp => new EpcService(2000)); // 5000 ms como intervalo


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
