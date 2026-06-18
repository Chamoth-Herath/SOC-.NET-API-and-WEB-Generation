using SOC_API.Data;
using SOC_API.Repos;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SOC_API.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration
		.GetConnectionString("conn")));


builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler =
			System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(Program));

// ← Register all Repos!
builder.Services.AddScoped<EventRepo>();
builder.Services.AddScoped<OrganizerRepo>();
builder.Services.AddScoped<PublicCitizenRepo>();
builder.Services.AddScoped<EventRegisterRepo>();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
// Add this right after builder.Build() — before app.Run()
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseCors("AllowAll");
app.UseStaticFiles(); // ← Add this so uploaded images are served!
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();