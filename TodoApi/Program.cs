using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

//Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
});


//Configure services
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql")), ServiceLifetime.Scoped);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
    {
         policy.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
     });
});

var app = builder.Build();


// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
app.UseSwaggerUI(c=>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

//Enable CORS
app.UseCors("AllowAll");

app.MapGet("/", () => "hello");
app.MapGet("/items", GetAllTasks);

async Task GetAllTasks(ToDoDbContext dbContext, HttpContext context)
{
    var tasks = await dbContext.Items.ToListAsync();
    await context.Response.WriteAsJsonAsync(tasks);
}

app.MapPost("/items", async (ToDoDbContext dbContext, Item item) =>
{
    dbContext.Items.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/GET/{item.Id}", item);
});

app.MapPut("/items/{id}", async (ToDoDbContext dbContext, int id, Item updatedItem) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    
    if (existingItem == null)
        return Results.NotFound("Item not found");

    existingItem.Name = updatedItem.Name ?? existingItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete ?? existingItem.IsComplete;
    await dbContext.SaveChangesAsync();
    return Results.Ok(existingItem);
});

app.MapDelete("/items/{id}", async (ToDoDbContext dbContext, int id) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    
    if (existingItem == null)
        return Results.NotFound("Item not found");
    dbContext.Items.Remove(existingItem);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();