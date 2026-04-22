using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class SymbolQuery
    {
        public MapSymbolType? Type { get; set; }
        public List<string> Collections { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        public string TextFilter { get; set; } = string.Empty;

        public SymbolQuery Clone()
        {
            return new SymbolQuery
            {
                Type = this.Type,
                Collections = [.. this.Collections],
                Tags = [.. this.Tags],
                TextFilter = this.TextFilter
            };
        }
    }
}
