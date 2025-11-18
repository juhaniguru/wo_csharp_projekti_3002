using System.Text;
using API.Data;
using API.Interfaces;
using API.Middlewares;
using API.Policies;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAutoMapper(typeof(Program));

// yläpuolella: var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    // varmistetaan, että TokenKey löytyy
    var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("token key not found");
    // konfataan tässä, mitä tarkistetaan
    o.TokenValidationParameters = new TokenValidationParameters
    {

        // varmistaa allekirjoituksen
        ValidateIssuerSigningKey = true,
        // jotta allekirjoituksen voi tarkistaa,
        // pitää kertoa, mitä avainta allekirjoituksessa käytetään
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        // issuerin tarkistus on pois päältä

        ValidateIssuer = false,
        // myös audiencen tarkistus on pois päältä
        ValidateAudience = false

    };

    o.MapInboundClaims = false;

});

builder.Services.AddScoped<IBlogService, BlogService>();


// yläpuolella automapper
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    option.AddPolicy("Require1000Xp", policy => policy.Requirements.Add(new XpRequirement(1000)));
    option.AddPolicy("Require100Xp", policy => policy.Requirements.Add(new XpRequirement(100)));
});

builder.Services.AddScoped<IAuthorizationHandler, XpAuthorizationHandler>();




// Add services to the container.

builder.Services.AddScoped<ITokenTool, SymmetricTokenTool>();

// nämä kolme liitännäistä tarvitaan siihen, että swagger 
// OpenAPI-dokumentaatio toimii automaattisesti
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Nyt softamme tietää, mitä tauluja tietokantamme sisältää (ja myös mitä kenttiä taulut sisältävät)
// sekä pstymme yhdistämään siihen.
builder.Services.AddDbContext<DataContext>(opt =>
{
    // AddDbContextille pitää kertoa, mistä tietokantayhteyden speksit löytyvät

    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<RequireLoggedInUserMiddleware>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // tämän pitäisi näyttää /swagger-osoitteessa
        // WithName-metodin poarametrin routen vieressä

        // materiaalia kirjoittaessa tämä ei toiminut
        options.DisplayOperationId();        
    });
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<RequireLoggedInUserMiddleware>();



app.Run();
