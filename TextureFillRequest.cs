using SkiaSharp;

namespace RealmStudioX.Core
{
    public class TextureFillRequest
    {
        public string? TextureId { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; } = 0;
        public float Opacity { get; set; } = 1;
        public bool Mirror { get; set; }
        public SKColor Color { get; set; } = SKColor.Empty;
    }
}
