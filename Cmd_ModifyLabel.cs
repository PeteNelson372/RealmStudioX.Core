using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyLabel : IUndoableCommand
    {
        private readonly MapLayer _layer;
        private readonly MapLabel _label;

        private readonly MapLabelState _before;
        private MapLabelState? _after;

        private bool _hasAfter;

        private bool disposedValue;

        public Cmd_ModifyLabel(MapLayer layer, MapLabel label)
        {
            _layer = layer;
            _label = label;

            _before = (MapLabelState)_label.CaptureState();
        }

        public void CaptureAfter()
        {
            if (!_hasAfter)
            {
                _after = (MapLabelState)_label.CaptureState();
                _hasAfter = true;
            }
        }

        public void Execute()
        {
            if (_after != null && _hasAfter)
            {
                _label.RestoreState(_after);
            }
        }

        public void Undo()
        {
            _label.RestoreState(_before);
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
