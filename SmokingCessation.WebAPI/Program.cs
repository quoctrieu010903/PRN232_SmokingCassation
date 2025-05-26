using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Extentions;
using SmokingCessation.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();



builder.Services
    .AddSwagger()

    // in the infrastructure (Add Configguration about JWT 
    .AddJwtAuthentication(builder.Configuration)
    //in the infrastructure
    .AddInfrastructure(builder.Configuration);

   
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder =scope.ServiceProvider.GetRequiredService<ISmokingSessationSeeder>();
await seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.WithSwagger();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// adding Cors ..
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.Run();
