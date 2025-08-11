using email_sender.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our services
builder.Services.AddSingleton<RabbitMqProducer>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<EmailVerificationService>();

// Add CORS with proper policy name
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "https://127.0.0.1:5500"
           
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Also keep default policy for development
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // This should show detailed error info
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Add static files support
app.UseDefaultFiles(); // This will serve index.html as default
app.UseStaticFiles();

// Add request logging for debugging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    try
    {
        await next();
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        throw;
    }
});

// IMPORTANT: Order matters! HTTPS redirection should come first
app.UseHttpsRedirection();

// Add CORS middleware - USE THE SPECIFIC POLICY
app.UseCors("AllowLocalhost");

// Add routing
app.UseRouting();

// Add authorization
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapRazorPages();

app.Run();