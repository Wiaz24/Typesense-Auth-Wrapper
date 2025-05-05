using System.Text.Json.Serialization;
using TypesenseAuthWrapper.Bootstrapper.Auth;
using TypesenseAuthWrapper.Bootstrapper.Cors;
using TypesenseAuthWrapper.Bootstrapper.Exceptions;
using TypesenseAuthWrapper.Bootstrapper.Logging;
using TypesenseAuthWrapper.Bootstrapper.OpenApi;
using TypesenseAuthWrapper.Bootstrapper.SystemsManager;
using TypesenseAuthWrapper.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSystemsManager();
builder.AddCustomLogging();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddShared();
builder.Services.AddAuth();
builder.Services.AddHttpClient();
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddOpenApi(options => options.UseOpenIdConnectAuthentication(builder.Configuration));
builder.Services.AddProblemDetails();
builder.Services.AddTransient<CustomExceptionHandler>();

var app = builder.Build();
app.UseCustomCors();
app.UseAuth();
app.UseMiddleware<CustomExceptionHandler>();
app.UseShared();
app.UseStaticFiles("/api/typesenseauthwrapper");
app.MapCustomOpenApi();
app.MapControllers();
app.Run();