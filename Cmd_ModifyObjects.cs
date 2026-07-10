using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public sealed class Cmd_ModifyObjects : IUndoableCommand, IDisposable
    {
        private readonly RealmStudioMap _map;
        private readonly List<(MapComponent2D Object, IShapeState State)> _before = [];
        private readonly List<(MapComponent2D Object, IShapeState State)> _after = [];
        private bool disposedValue;

        public Cmd_ModifyObjects(RealmStudioMap map)
        {
            _map = map;
        }

        public void CaptureBefore(IEnumerable<MapComponent2D> objects)
        {
            _before.Clear();

            foreach (MapComponent2D obj in objects)
            {
                _before.Add((obj, obj.CaptureState()));
            }
        }

        public void CaptureAfter(IEnumerable<MapComponent2D> objects)
        {
            _after.Clear();

            foreach (MapComponent2D obj in objects)
            {
                _after.Add((obj, obj.CaptureState()));
            }
        }

        public void Execute()
        {
            Restore(_after);
        }

        public void Undo()
        {
            Restore(_before);
        }

        private void Restore(List<(MapComponent2D Object, IShapeState State)> states)
        {
            foreach (var entry in states)
            {
                if (entry.Object is MapComponent2D undoable)
                {
                    undoable.RestoreState(entry.State);
                }
            }

            foreach (MapLayer layer in _map.MapLayers)
            {
                layer.InvalidateAllTiles();
            }
        }

        private void Dispose(bool disposing)
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
        // ~Cmd_ModifyObjects()
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