using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyBox : IUndoableCommand
    {
        private readonly MapLayer _layer;
        private readonly PlacedMapBox _box;

        private readonly PlacedBoxState _before;
        private PlacedBoxState? _after;

        private bool _hasAfter;

        private bool disposedValue;

        public Cmd_ModifyBox(MapLayer layer, PlacedMapBox box)
        {
            _layer = layer;
            _box = box;

            _before = (PlacedBoxState)_box.CaptureState();
        }

        public void CaptureAfter()
        {
            if (!_hasAfter)
            {
                _after = (PlacedBoxState)_box.CaptureState();
                _hasAfter = true;
            }
        }

        public void Execute()
        {
            if (_after != null && _hasAfter)
            {
                _box.RestoreState(_after);
            }
        }

        public void Undo()
        {
            _box.RestoreState(_before);
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
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_ModifyLabel()
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
