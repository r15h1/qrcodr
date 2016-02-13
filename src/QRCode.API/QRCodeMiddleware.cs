using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QRCode.Abstraction;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace QRCode.API
{
    internal class QRCodeMiddleware
    {
        private IQRCodeGenerator generator;
        private static Dictionary<string, ImageFormat> supportedImageFormats;
        private IMemoryCache cache;
        private ILogger<QRCodeMiddleware> logger;
        private RequestDelegate next;

        static QRCodeMiddleware()
        {
            SetupSupportedImageFormats();            
        }

        private static void SetupSupportedImageFormats()
        {
            supportedImageFormats = new Dictionary<string, ImageFormat>();
            supportedImageFormats.Add("gif", ImageFormat.Gif);
            supportedImageFormats.Add("jpeg", ImageFormat.Jpeg);
            supportedImageFormats.Add("jpg", ImageFormat.Jpeg);
            supportedImageFormats.Add("png", ImageFormat.Png);
        }

        public QRCodeMiddleware(RequestDelegate next, IQRCodeGenerator generator, IMemoryCache cache, ILogger<QRCodeMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
            this.generator = generator;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            logger.LogInformation("-------------------------------------------------------------------");
            QRCodeParameters param = ParseQRParamsFromQueryString(context.Request.Query);
            logger.LogInformation(param.ToString());

            if (param.IsValid()) CreateQRCodeImage(context, param);
            else await next(context); //bad request
        }

        private void CreateQRCodeImage(HttpContext context, QRCodeParameters param)
        {
            Image img = null;
            if (!cache.TryGetValue(param.ToString(), out img))
            {
                logger.LogInformation("generating new image");
                img = generator.GenerateQRCode(param);
                cache.Set(param.ToString(), img, new MemoryCacheEntryOptions() {SlidingExpiration = DateTime.Now.AddDays(1).TimeOfDay });
            }
            else
            {
                logger.LogInformation("reading from cache");
            }

            context.Response.Headers.Add("content-type", "image/" + param.ImageFormat.ToString().ToLower());
            img.Save(context.Response.Body, param.ImageFormat);
        }


        private QRCodeParameters ParseQRParamsFromQueryString(IReadableStringCollection query)
        {
            QRCodeParameters param = new QRCodeParameters();

            var text = query["text"];
            param.Text = text;

            var size = query["size"];
            string format = query["format"];

            int intSize = 0;
            if (int.TryParse(size, out intSize)) param.Size = intSize;

            if (!string.IsNullOrWhiteSpace(format) && supportedImageFormats.ContainsKey(format.ToLower())) param.ImageFormat = supportedImageFormats[format];

            return param;
        }
    }
}