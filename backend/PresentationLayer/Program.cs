using DataAccessLayer;
using BusinessLogicLayer;
using PresentationLayer.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Register services to the container.
builder.Services.AddBLLServices(builder.Configuration);
builder.Services.AddDALServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware Chaining
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

app.UseHttpsRedirection();

// Jwt Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();