using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class SymbolSelectionService
    {
        public MapSymbolDefinition? PrimarySelectedSymbol { get; private set; }

        private readonly List<MapSymbolDefinition> _secondary = [];
        public IReadOnlyList<MapSymbolDefinition> SecondarySelectedSymbols => _secondary;

        public event Action? SelectionChanged;

        // -------------------------------------------------
        // Set Primary
        // -------------------------------------------------

        public void SetPrimarySelectedSymbol(MapSymbolDefinition? def)
        {
            PrimarySelectedSymbol = def;
            SelectionChanged?.Invoke();
        }

        // -------------------------------------------------
        // Secondary selections
        // -------------------------------------------------

        public bool AddSecondary(MapSymbolDefinition def)
        {
            if (def == null)
                return false;

            // Avoid duplicate
            if (_secondary.Contains(def))
                return false;

            _secondary.Add(def);

            SelectionChanged?.Invoke();
            return true;
        }

        public bool RemoveSecondary(MapSymbolDefinition def)
        {
            if (def == null)
                return false;

            if (!_secondary.Remove(def))
                return false;

            SelectionChanged?.Invoke();
            return true;
        }

        public bool ContainsSecondary(MapSymbolDefinition def)
        {
            if (def == null)
                return false;

            return _secondary.Contains(def);
        }

        public void ToggleSecondarySelectedSymbol(MapSymbolDefinition def)
        {
            if (!_secondary.Remove(def))
            {
                _secondary.Add(def);
            }

            SelectionChanged?.Invoke();
        }

        public void ClearSecondary()
        {
            _secondary.Clear();
            SelectionChanged?.Invoke();
        }
    }
}
