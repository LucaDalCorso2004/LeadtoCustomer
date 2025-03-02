using LeadtoCustomer;

try
{
    Console.WriteLine("Initializing database.");
    Database.Initialize();
    Console.WriteLine("Seeding data.");
  
}
catch (Exception ex)
{
    Console.WriteLine("Error in initializing database.");
    Console.WriteLine(ex.Message);
}

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


