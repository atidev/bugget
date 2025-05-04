using SixLabors.ImageSharp.Formats.Webp;

namespace Bugget.Entities.BO.AttachmentBo
{
    public class OptimizatorSettings
    {
        public int MaxOriginalWidth { get; set; } = 1440;
        public int MaxPreviewSize { get; set; } = 50;
        public int WebpQuality { get; set; } = 85;
        public WebpFileFormatType FileFormat { get; set; } = WebpFileFormatType.Lossy;
        // качество и скорость посередине
        public WebpEncodingMethod Method { get; set; } = WebpEncodingMethod.Level4;
        public WebpTransparentColorMode TransparentColorMode { get; set; } = WebpTransparentColorMode.Preserve;
    }
}