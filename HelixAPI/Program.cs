using Helix.API;
using Helix.Infrastructure.Context.DbInitializer;
using Helix.Service;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add Services 
builder.Services.AddServiceDependancies(builder.Configuration, builder.Environment)
                .AddApiDenpendancies(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()||true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Helix API V1");
        c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
        c.DisplayRequestDuration();
    });
    app.UseCors("AllowAll"); // Allow all origins in development
}
else
{
    // In production, use specific origins if configured, otherwise fall back to AllowAll
    // Check if AllowSpecificOrigins policy exists by checking configuration
    var corsSettings = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
    if (corsSettings != null && corsSettings.Length > 0)
    {
        app.UseCors("AllowSpecificOrigins"); // Use specific origins in production
    }
    else
    {
        app.UseCors("AllowAll"); // Fallback to AllowAll if not configured  
    }
}
 
// Only use HTTPS redirection if not behind a reverse proxy (IIS handles this)
// Comment out if causing issues on monsterasp.net
// app.UseHttpsRedirection();
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
if (!File.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

// Enable static file serving for uploaded files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/Uploads"
});

// Authentication & Authorization middleware (order matters!)
app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

#region Update and Initialize Database
// Applies pending migrations also if there are no roles, create the default
// admin user with email = admin@Helix.com and password = Admin#123 you can also use username = admin
// This helper function creates a scope to resolve services and run the database initializer.
static async Task SeedDatabaseAsync(IHost app)
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Starting database initialization and seeding...");

            var dbInitializer = serviceProvider.GetRequiredService<IDbInitializer>();
            await dbInitializer.Initialize();

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization.");
            // Re-throwing the exception is important here. It will stop the application from starting
            // in a potentially broken state, making it clear that a critical startup step failed.
            throw;
        }
    }
}

// Run the database seeder synchronously at startup.
// This ensures that the application doesn't start if the database migration or seeding fails.
await SeedDatabaseAsync(app);
#endregion

app.Run();
