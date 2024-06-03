using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

string jsonSecrets = await HelperSecretManager.GetSecretsAsync();

KeysModel keysModel = JsonConvert.DeserializeObject<KeysModel>(jsonSecrets);
builder.Services.AddSingleton<KeysModel>(x => keysModel);

builder.Services.AddAWSService<IAmazonS3>();


//Habilitamos session dentro de nuestro servidor
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

//Habilitamos la seguridad en servicios
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();


// Add services to the container.
builder.Services.AddHttpContextAccessor();

//Personalizamos nuestras rutas
builder.Services.AddControllersWithViews
    (options => options.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession();

// Agrega servicios al contenedor de dependencias.
builder.Services.AddScoped<UploadFilesController>();

builder.Services.AddTransient<HelperMails>();
builder.Services.AddTransient<HelperTools>();
builder.Services.AddTransient<HelperPathProvider>();

builder.Services.AddTransient<ServiceEventos>();
builder.Services.AddTransient<ServiceStorageAWS>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Cambiamos el mapeo de Rutas para poder usar la Seguridad
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Eventos}/{action=Index}/{id?}");
});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Eventos}/{action=Index}/{id?}");

app.Run();
