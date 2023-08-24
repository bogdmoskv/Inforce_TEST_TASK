using Inforce_.NET_Task_Moskvichev_Bogdan;
using Inforce_.NET_Task_Moskvichev_Bogdan.Helpers;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


string ?connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddHttpContextAccessor();



builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ���������, ����� �� �������������� �������� ��� ��������� ������
            ValidateIssuer = true,
            // ������, �������������� ��������
            ValidIssuer = AuthOptions.ISSUER,
            // ����� �� �������������� ����������� ������
            ValidateAudience = true,
            // ��������� ����������� ������
            ValidAudience = AuthOptions.AUDIENCE,
            // ����� �� �������������� ����� �������������
            ValidateLifetime = true,
            // ��������� ����� ������������
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // ��������� ����� ������������
            ValidateIssuerSigningKey = true,
        };
    });



// ���������� ��������������� ���� � ���������� ���������
//TODO: ��������� ����� � FromMinutes()
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "AuthToken"; // ��� ����, ������������� ��� �������� ������
    options.IdleTimeout = TimeSpan.FromMinutes(2); // ����� ������������ ������, ����� �������� �� �������� (� ������ ������ - 30 �����)
});





var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    InitializeRoles(dbContext);
}



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

app.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Trim('/');
    var urlMatch = db.Urls.FirstOrDefault(x =>
        x.ShortUrl.ToLower().Trim() == path.Trim());

    if (urlMatch == null)
        return Results.BadRequest("Invalid request!");

    return Results.Redirect(urlMatch.Url);
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseCors(builder => builder
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod());


app.Run();



void InitializeRoles(ApplicationDbContext dbContext)
{
    if (!dbContext.Roles.Any())
    {
        var adminRole = new Role { Name = "Admin" };
        var userRole = new Role { Name = "User" };

        dbContext.Roles.AddRange(adminRole, userRole);
        dbContext.SaveChanges();
    }
}