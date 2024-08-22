using RetwisCS;
using RetwisCS.Features.Authentication;
using RetwisCS.Features.UserRegistration;
using RetwisCS.General;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IConnectionMultiplexer>(_ =>
{
    var config = new ConfigurationOptions()
    {
        EndPoints = { {"localhost", 6379 } },
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
    };
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddScoped<IDatabase>(services =>
    services.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddScoped<IAuthMap, AuthMap>();
builder.Services.AddScoped<IUserIdMap, UserIdMap>();

builder.AddEndpoints();

builder.Services.AddScoped<INewUserRegister, NewUserRegister>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterEndpoints();

app.Run();
