using Application.Extensions;
using Infrastructure.Database.Extensions;
using Infrastructure.Security.Extensions;
using Microsoft.AspNetCore.Http.Features;
using SIAES.API.Extensions;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddSwaggerExtensions();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioningExtensions();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddSecurityServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabaseAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
