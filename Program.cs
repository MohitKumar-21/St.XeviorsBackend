using st_xaviers_website_api.Dal;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Filters;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditFilter>();
});

// Swagger (Authorize button ke saath)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Sirf token paste karo (Bearer likhne ki zaroorat nahi)"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
// DAL services
builder.Services.AddScoped<ContactUsDal>();
builder.Services.AddScoped<StudentRegistrationDal>();
builder.Services.AddScoped<EnquiryFormDal>();
builder.Services.AddScoped<TestimonialDal>();
builder.Services.AddScoped<UserDal>();
builder.Services.AddScoped<NoticeDal>();
builder.Services.AddScoped<TopperDal>();
builder.Services.AddScoped<CoverImageDal>();
builder.Services.AddScoped<GalleryDal>();
builder.Services.AddScoped<st_xaviers_website_api.Services.CloudinaryService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebsite", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("AllowWebsite");
app.UseHttpsRedirection();

app.UseAuthentication();   // <-- naya, UseAuthorization se pehle
app.UseAuthorization();

app.MapControllers();

app.Run();