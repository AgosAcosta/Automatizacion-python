using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Processors;
using AutomatizacionReportes.Services;
using AutomatizacionReportes.Writers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Init(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ExecutionLockService>();
builder.Services.AddScoped<WhatsappProcessService>();
builder.Services.AddScoped<SmsProcessService>();

builder.Services.AddScoped<FileScanner>();
builder.Services.AddScoped<WhatsappProcessor>();
builder.Services.AddScoped<ExcelWriterWhatsapp>();

builder.Services.AddScoped<SmsProcessor>();
builder.Services.AddScoped<ExcelWriterSms>();

builder.Services.AddScoped<MailProcessService>();
builder.Services.AddScoped<MailProcessor>();
builder.Services.AddScoped<ExcelWriterMail>();

builder.Services.AddScoped<MailAsignacionProcessService>();
builder.Services.AddScoped<MailAsignacionProcessor>();
builder.Services.AddScoped<ExcelWriterMailAsignacion>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Mapear la raíz a index.html
app.MapGet("/", context =>
{
    context.Response.ContentType = "text/html";
    return context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
});

app.Run();
