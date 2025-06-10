    using System.Text;
    using Microsoft.IdentityModel.Tokens;
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using OcelotGateway.Middlewares;

    var builder = WebApplication.CreateBuilder(args);

    var ocelotConfig = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .Build();

    builder.Configuration.AddConfiguration(ocelotConfig);

    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins",
            policy => policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    builder.Services.AddOcelot(ocelotConfig);
    builder.Services.AddSwaggerForOcelot(ocelotConfig);

    var app = builder.Build();

    app.UseCors("AllowSpecificOrigins");
    app.UseHttpsRedirection();


    app.UseMiddleware<TokenCheckerMiddleware>();
    app.UseMiddleware<InterceptionMiddleware>();

    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";  
    });
    
    await app.UseOcelot();

    app.Run();