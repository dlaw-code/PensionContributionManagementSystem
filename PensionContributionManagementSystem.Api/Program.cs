using Hangfire;
using PensionContributionManagementSystem.Api.Extensions;
using PensionContributionManagementSystem.Api.Middlewares;
using PensionContributionManagementSystem.Core.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddServices(builder.Configuration);



var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

// Schedule Background Jobs
using (var scope = app.Services.CreateScope())
{
    var jobService = scope.ServiceProvider.GetRequiredService<BackgroundJobService>();
    jobService.ScheduleJobs();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();

app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();

app.MapControllers();


app.Run();