using FluentValidation;
using FluentValidation.AspNetCore;
using InnoGotchi.API.Extensions;
using InnoGotchi.API.MIddlewares;
using InnoGotchi.Application.Validators.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureAuthenticationService();
builder.Services.ConfigureFarmService();
builder.Services.ConfigureGenerateFarmStatisticsService();
builder.Services.ConfigureGenerateTokenService();
builder.Services.ConfigurePetConditionService();
builder.Services.ConfigurePetService();
builder.Services.ConfigureUserService();
builder.Services.ConfigureAutoMapper();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserForRegistrationDtoValidator>();
builder.Services.ConfigurActionFilters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthorization();

app.MapControllers();

app.Run();
