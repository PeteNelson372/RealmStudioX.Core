using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class SymbolIndex
    {
        private readonly List<MapSymbolDefinition> _all = [];

        private readonly Dictionary<MapSymbolType, List<MapSymbolDefinition>> _byType = [];
        private readonly Dictionary<string, List<MapSymbolDefinition>> _byCollection = [];
        private readonly Dictionary<string, List<MapSymbolDefinition>> _byTag = [];

        // Optional: normalized name cache
        private readonly Dictionary<MapSymbolDefinition, string> _normalizedNames = [];
        private readonly Dictionary<MapSymbolDefinition, List<string>> _normalizedTags = [];

        // -------------------------------------------------
        // Add symbol
        // -------------------------------------------------

        public void Add(MapSymbolDefinition symbol)
        {
            _all.Add(symbol);

            // --- Type ---
            if (!_byType.TryGetValue(symbol.SymbolType, out var typeList))
            {
                typeList = [];
                _byType[symbol.SymbolType] = typeList;
            }
            typeList.Add(symbol);

            // --- Collection ---
            if (!_byCollection.TryGetValue(symbol.CollectionName, out var colList))
            {
                colList = [];
                _byCollection[symbol.CollectionName] = colList;
            }
            colList.Add(symbol);

            // --- Tags ---
            foreach (var tag in symbol.SymbolTags)
            {
                if (!_byTag.TryGetValue(tag, out var tagList))
                {
                    tagList = [];
                    _byTag[tag] = tagList;
                }

                tagList.Add(symbol);
            }

            // --- Name cache ---
            _normalizedNames[symbol] = symbol.SymbolName.ToLowerInvariant();

            // --- tag cache ---
            _normalizedTags[symbol] = [.. symbol.SymbolTags.Select(t => t.ToLowerInvariant())];
        }

        // -------------------------------------------------
        // Query
        // -------------------------------------------------

        public List<MapSymbolDefinition> Query(
            MapSymbolType? type,
            IEnumerable<string>? collections,
            IEnumerable<string>? tags,
            string? textFilter)
        {
            List<MapSymbolDefinition> result = _all;

            // --- Type ---
            if (type != null && _byType.TryGetValue(type.Value, out var typeList))
            {
                result = typeList;
            }

            // --- Collection filter ---
            if (collections != null && collections.Any())
            {
                var set = new HashSet<MapSymbolDefinition>();

                foreach (var c in collections)
                {
                    if (_byCollection.TryGetValue(c, out var list))
                    {
                        foreach (var s in list)
                            set.Add(s);
                    }
                }

                result = [.. result.Intersect(set)];
            }

            // --- Tag filter ---
            if (tags != null && tags.Any())
            {
                var set = new HashSet<MapSymbolDefinition>();

                foreach (var tag in tags)
                {
                    if (_byTag.TryGetValue(tag, out var list))
                    {
                        foreach (var s in list)
                            set.Add(s);
                    }
                }

                result = [.. result.Intersect(set)];
            }

            // --- text filter ---
            if (!string.IsNullOrWhiteSpace(textFilter))
            {
                var nf = textFilter.ToLowerInvariant();

                var tokens = nf.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                result = [.. result
                    .Where(s =>
                        tokens.Any(token =>
                            _normalizedNames[s].Contains(token) ||
                            _normalizedTags[s].Any(t => t.Contains(token))))];
            }

            return result;
        }
    }
}
