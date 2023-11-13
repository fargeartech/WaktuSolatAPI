using WaktuSolatMY.Application.Constants;
using WaktuSolatMY.Application.Extensions;
using static WaktuSolatMY.Application.Constants.Zones;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient(Zones.ClientName.JAKIM, client =>
{
    client.BaseAddress = new Uri(BaseUrls.JAKIM);
});
builder.Services.AddApplications();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();