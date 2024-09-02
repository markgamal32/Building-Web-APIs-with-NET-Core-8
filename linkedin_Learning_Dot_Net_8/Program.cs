using linkedin_Learning_Dot_Net_8.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*    Configures the ShopContext to use an in-memory database named "shop"
	  for dependency injection. This setup is typically used for testing purposes
	  or for running the application without a persistent database. 
*/
builder.Services.AddDbContext<ShopContext>(options => { options.UseInMemoryDatabase("shop"); });

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



using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
	await db.Database.EnsureCreatedAsync();
}
// getting the available products using Minimal API
app.MapGet("/products/available", async (ShopContext _context) =>
	Results.Ok(await _context.Products.Where(p => p.IsAvailable).ToArrayAsync())
);

app.Run();
