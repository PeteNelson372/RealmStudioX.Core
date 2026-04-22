using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifySymbol : IUndoableCommand
    {
        private readonly MapLayer _layer;
        private readonly MapSymbol _symbol;

        private readonly MapSymbolState _before;
        private MapSymbolState? _after;

        private bool _hasAfter;

        private bool disposedValue;

        public Cmd_ModifySymbol(MapLayer layer, MapSymbol symbol)
        {
            _layer = layer;
            _symbol = symbol;

            _before = (MapSymbolState)_symbol.CaptureState();
        }

        public void CaptureAfter()
        {
            if (!_hasAfter)
            {
                _after = (MapSymbolState)_symbol.CaptureState();
                _hasAfter = true;
            }
        }

        public void Execute()
        {
            // Redo: apply AFTER
            if (_after != null && _hasAfter)
            {
                _symbol.RestoreState(_after);
                _symbol.UpdateBounds();

                _layer.InvalidateAllTiles();
            }
        }

        public void Undo()
        {
            _symbol.RestoreState(_before);
            _symbol.UpdateBounds();

            _layer.InvalidateAllTiles();
        }

        public bool HasChange
        {
            get
            {
                return _hasAfter &&
                       _after != null &&
                       !_before.Equals(_after);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}