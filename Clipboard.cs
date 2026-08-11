using SkiaSharp;

namespace RealmStudioX.Core
{
    public sealed class ClipboardItem
    {
        public required string MapLayerId { get; init; }

        public required SKPoint Offset { get; init; }

        public required Type ObjectType { get; init; }

        public required string SerializedObject { get; init; }
    }

    public sealed class Clipboard
    {
        // Center (or top-left) of the selection bounds
        public SKPoint Anchor { get; init; }

        public List<ClipboardItem> Items { get; } = new List<ClipboardItem>();
    }
}
