using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyMapPaths(RealmStudioMap map, MapLayer layer) : IUndoableCommand
    {
        private readonly RealmStudioMap _map = map;
        private readonly MapLayer _layer = layer;
        private bool disposedValue;

        private readonly HashSet<MapPath> _addedPaths = [];
        private readonly HashSet<MapPath> _removedPaths = [];

        private readonly Dictionary<MapPath, IShapeState> _before = [];
        private readonly Dictionary<MapPath, IShapeState> _after = [];


        public void Execute()
        {
            foreach (var b in _removedPaths)
            {
                for (var i = _layer.Shapes.Count - 1; i >= 0; i--)
                {
                    if (_layer.Shapes[i] == b)
                    {
                        _layer.Remove(b);
                    }
                }
            }

            foreach (var b in _addedPaths)
            {
                if (!_layer.Shapes.Contains(b))
                {
                    _layer.Add(b);
                }
            }

            foreach (var (path, state) in _after)
            {
                if (!_removedPaths.Contains(path))
                {
                    path.RestoreState(state);
                }
            }
        }

        public void Undo()
        {
            foreach (var (path, state) in _before)
            {
                path.RestoreState(state);
            }

            foreach (var b in _addedPaths)
            {
                for (var i = _layer.Shapes.Count - 1; i >= 0; i--)
                {
                    if (_layer.Shapes[i] == b)
                    {
                        _layer.Remove(b);
                    }
                }
            }

            foreach (var b in _removedPaths)
            {
                _layer.Add(b);
            }
        }

        // -------------------------------------------------
        // Registration
        // -------------------------------------------------

        public void RegisterAddedMapPath(MapPath path)
        {
            _addedPaths.Add(path);
        }

        public void RegisterRemovedMapPath(MapPath path)
        {
            _removedPaths.Add(path);
        }

        // -------------------------------------------------
        // State capture
        // -------------------------------------------------

        public void CaptureBefore(MapPath path)
        {
            if (!_before.ContainsKey(path))
            {
                _before[path] = path.CaptureState();
            }
        }

        public void CaptureAfter(MapPath path)
        {
            // Do not capture state for newly created bodies
            if (_addedPaths.Contains(path))
            {
                return;
            }

            if (!_after.ContainsKey(path))
            {
                _after[path] = path.CaptureState();
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
        // ~Cmd_ModifyMapPaths()
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