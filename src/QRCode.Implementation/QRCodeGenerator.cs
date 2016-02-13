using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using QRCode.Abstraction;
using System;
using System.Drawing;
using System.IO;

namespace QRCode.Implementation
{
    /// <summary>
    /// use QRCode.Net to generat
    /// </summary>
    public class QRCodeGenerator : IQRCodeGenerator
    {
        public Image GenerateQRCode(QRCodeParameters param)
        {
            if (param == null) throw new ArgumentNullException();

            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.L);
            QrCode qrCode = encoder.Encode(param.Text);

            GraphicsRenderer gRenderer = new GraphicsRenderer(new FixedModuleSize(param.Size, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            MemoryStream ms = new MemoryStream();            
            gRenderer.WriteToStream(qrCode.Matrix, param.ImageFormat, ms);
            return Image.FromStream(ms);
        }
    }
}
