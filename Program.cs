global using SuperHeroAPI.Data;
global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.ResolveConflictingActions(apiDescriptions =>
        {
            var descriptions = apiDescriptions as ApiDescription[] ?? apiDescriptions.ToArray();
            var first = descriptions.First(); // build relative to the 1st method
            var parameters = descriptions.SelectMany(d => d.ParameterDescriptions).ToList();

            first.ParameterDescriptions.Clear();
            // add parameters and make them optional
            foreach (var parameter in parameters)
                if (first.ParameterDescriptions.All(x => x.Name != parameter.Name))
                {
                    first.ParameterDescriptions.Add(new ApiParameterDescription
                    {
                        ModelMetadata = parameter.ModelMetadata,
                        Name = parameter.Name,
                        ParameterDescriptor = parameter.ParameterDescriptor,
                        Source = parameter.Source,
                        IsRequired = false,
                        DefaultValue = null
                    });
                }
            return first;
        });
    }
);

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
