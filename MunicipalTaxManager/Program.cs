using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.DataLayer;
using MunicipalTaxManager.Middleware;
using MunicipalTaxManager.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaxDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryTaxDb"));

builder.Services.AddScoped<ITaxRecordRepository, TaxRecordRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
