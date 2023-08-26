global using Microsoft.EntityFrameworkCore;
global using Persistence;
global using Domain;
global using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => {
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// Referrer Policy Header - Controls included information on navigation.
app.UseReferrerPolicy(options => options.NoReferrer());
// X-Content-Type-Options Header - Prevents MIME-sniffing of the content type.
app.UseXContentTypeOptions();
// X-Frame-Options Header - Defends against attacks like clickjacking by banning a browser from framing the site.
app.UseXfo(options => options.Deny());
// Content-Security-Policy Header - Protection from XSS attacks.
app.UseXXssProtection(options => options.EnabledWithBlockMode());
// CSP: Whitelists certain content and prevents all other malicious assets.
app.UseCsp(options => options
    .BlockAllMixedContent()
    .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com"))
    .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
    .FormActions(s => s.Self())
    .FrameAncestors(s => s.Self())
    .ImageSources(s => s.Self().CustomSources("blob:", "https://res.cloudinary.com"))
    .ScriptSources(s => s.Self())
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Strict-Transport-Security Header:
    // Strengthens implementation of TLS by getting the User Agent to enforce the use of HTTPS
    app.Use(async (context, next) => 
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        await next.Invoke();
    });
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try 
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager);
}
catch(Exception e) 
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occured during migration");
}

app.Run();