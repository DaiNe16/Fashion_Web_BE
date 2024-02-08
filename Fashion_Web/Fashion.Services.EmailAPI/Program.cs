using Fashion.Services.EmailAPI.Messaging;
using Fashion.Services.EmailAPI.Service;
using Fashion.Services.EmailAPI.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Config Product Client
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("Auth", u => u.BaseAddress =
new Uri(builder.Configuration["ApiSettings:ServiceUrls:AuthAPI"]));

builder.Services.AddHostedService<RabbitMQConsumer>();
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
