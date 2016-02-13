using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QRCode.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCaching();
            services.AddLogging();
            services.AddScoped<QRCode.Abstraction.IQRCodeGenerator, QRCode.Implementation.QRCodeGenerator>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            app.MapWhen(ctx => ctx.Request.Path.Equals("/api/qrcode") && ctx.Request.Method.Equals("GET"), HandleQRCode);
            app.Run(async (context) =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("usage:  /api/qrcode?text=yourtext");                
            });
        }

        private void HandleQRCode(IApplicationBuilder app)
        {
            app.UseMiddleware<QRCodeMiddleware>();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
