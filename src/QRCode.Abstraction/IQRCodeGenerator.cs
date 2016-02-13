using System.Drawing;
using System.Drawing.Imaging;

namespace QRCode.Abstraction
{
    public interface IQRCodeGenerator
    {
        Image GenerateQRCode(QRCodeParameters param);
    }

    public class QRCodeParameters
    {
        public string Text { get; set; }

        private int? size;
        public int Size
        {
            get
            {
                if (!size.HasValue) { return 100; }
                return size.Value;
            }
            set
            {
                size = value;
            }
        }

        private ImageFormat format;
        public ImageFormat ImageFormat
        {
            get
            {
                if (format == null) return ImageFormat.Jpeg;
                return format;
            }
            set
            {
                format = value;
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Text) && Size > 0 && Size <= 100;
        }

        public override string ToString()
        {
            string s = "text: " + (string.IsNullOrWhiteSpace(Text) ? "" : Text);
            s += ", size: " + Size.ToString();
            s += ", format: " + ImageFormat.ToString();
            return s;
        }
    }
}
