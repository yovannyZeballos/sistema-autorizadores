
using Microsoft.OpenApi.Models;
using SGP.Api.Services.BctService;
using SGP.Api.Services.SgpService;
using SGP.Api.Services;
using SGP.Api.Services.CenService;

namespace SGP.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Register your custom class
            builder.Services.AddSingleton<SgpService>();
            builder.Services.AddSingleton<SPT03Service>();
            builder.Services.AddSingleton<HPCT02Service>();
            builder.Services.AddSingleton<TPCT02Service>();
            builder.Services.AddSingleton<BctSpsaService>();
            builder.Services.AddSingleton<BctTpsaService>();
            builder.Services.AddSingleton<BctHpsaService>();
            builder.Services.AddSingleton<CenService>();

			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "APIs SPSA",
                    Description = "Servicios de uso interno SPSA",
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
