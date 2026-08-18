using RealmStudioShapeRenderingLib;
using SkiaSharp;

namespace RealmStudioX.Core
{
    public class SymbolThumbnailCache(SymbolImageCache imageCache)
    {
        private readonly SymbolImageCache _imageCache = imageCache;

        private readonly Dictionary<string, SKBitmap> _cache = [];

        public SKBitmap? GetOrCreate(MapSymbolDefinition def, int size)
        {
            var key = $"{def.Id}_{size}";

            if (_cache.TryGetValue(key, out var bmp))
                return bmp;

            bmp = GenerateThumbnail(def, size);

            if (bmp != null)
            {
                _cache[key] = bmp;
            }

            return bmp;
        }

        public void Clear()
        {
            foreach (var bmp in _cache.Values)
            {
                bmp.Dispose();
            }

            _imageCache.Clear();
            _cache.Clear();
        }

        private SKBitmap GenerateThumbnail(MapSymbolDefinition def, int size)
        {
            var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);

            canvas.Clear(SKColors.Transparent);

            var resource = _imageCache.Get(def.SymbolFilePath);

            if (resource == null)
                return bitmap;

            switch (resource)
            {
                case BitmapResource bmp:
                    DrawBitmapThumbnail(canvas, bmp.Image, size);
                    break;

                case SvgResource svg:
                    DrawSvgThumbnail(canvas, svg, size);
                    break;
            }

            return bitmap;
        }

        private static void DrawBitmapThumbnail(SKCanvas canvas, SKImage bmp, int size)
        {
            SKRect bounds = new(0, 0, bmp.Width, bmp.Height);
            var scale = ComputeScale(bounds, size);

            canvas.Translate(size / 2f, size / 2f);
            canvas.Scale(scale);
            canvas.Translate(-bmp.Width / 2f, -bmp.Height / 2f);

            canvas.DrawImage(bmp, 0, 0, SKSamplingOptions.Default);
        }

        private static void DrawSvgThumbnail(SKCanvas canvas, SvgResource svg, int size)
        {
            var bounds = svg.Bounds;

            var scale = ComputeScale(bounds, size);

            canvas.Translate(size / 2f, size / 2f);
            canvas.Scale(scale);
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawImage(svg.Image, 0, 0, SKSamplingOptions.Default);
        }

        private static float ComputeScale(SKRect bounds, int size)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
                return 1f;

            var maxDim = Math.Max(bounds.Width, bounds.Height);

            return (size * 0.8f) / maxDim;
        }
    }


}
