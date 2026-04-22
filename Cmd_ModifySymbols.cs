namespace RealmStudioX.Core
{
    using RealmStudioShapeRenderingLib;

    public class Cmd_ModifySymbols(MapLayer layer, bool forceExecute = false) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly List<MapSymbol> _added = [];
        private readonly List<MapSymbol> _removed = [];
        private readonly List<(MapSymbol symbol, MapSymbolState before, MapSymbolState after)> _modified = new();

        private readonly bool _forceExecute = forceExecute;
        private bool _hasExecuted;
        private bool disposedValue;

        public void RegisterNewSymbol(MapSymbol symbol)
        {
            _added.Add(symbol);
        }

        public void RegisterRemovedSymbol(MapSymbol symbol)
        {
            _removed.Add(symbol);
        }

        public void RegisterModifiedSymbol(MapSymbol symbol, MapSymbolState before, MapSymbolState after)
        {
            _modified.Add((symbol, before, after));
        }

        public void Execute()
        {
            if (!_forceExecute)
            {
                // First execution = already applied interactively
                if (!_hasExecuted)
                {
                    _hasExecuted = true;
                    return;
                }
            }

            // Redo: reapply everything

            // 1. Add symbols
            foreach (var s in _added)
            {
                _layer.Add(s);
            }

            // 2. Remove symbols
            foreach (var s in _removed)
            {
                _layer.Remove(s);
            }

            // 3. Apply modifications
            foreach (var (symbol, _, after) in _modified)
            {
                symbol.RestoreState(after);
            }

            _layer.InvalidateAllTiles();
        }

        public void Undo()
        {
            // 1. Undo modifications
            foreach (var (symbol, before, _) in _modified)
            {
                symbol.RestoreState(before);
            }

            // 2. Restore removed symbols
            foreach (var s in _removed)
            {
                _layer.Add(s);
            }

            // 3. Remove added symbols
            foreach (var s in _added)
            {
                _layer.Remove(s);
            }

            _layer.InvalidateAllTiles();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_ModifySymbols()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

