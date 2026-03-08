using Libwebp.Net.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// ── Register the WebP middleware conversion service ──────────────────────
builder.Services.AddWebpConversion(options =>
{
    options.QualityFactor = 80;       // good default quality
    options.MultiThreading = true;    // use all cores
    // Skip the /Home/Convert endpoint — that action does its own encoding
    // with full user-specified options from the form.
    options.SkipConversion = (context, file) =>
        context.Request.Path.StartsWithSegments("/Home/Convert");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ── Activate middleware: every image upload is auto-converted to WebP ────
app.UseWebpUploadConversion();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


