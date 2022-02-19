using HttpHmacAuthentication.AspnetCore.Sample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(
//    options => options.DefaultScheme = AuthSchemeConstants.MyNinjaAuthScheme)
//    .AddScheme<MyNinjaAuthSchemeOptions, MyNinjaAuthHandler>(
//        AuthSchemeConstants.MyNinjaAuthScheme, options => { });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
