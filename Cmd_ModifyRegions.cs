namespace RealmStudioX.Core
{
    using RealmStudioShapeRenderingLib;

    public class Cmd_ModifyRegions(MapLayer layer) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly List<MapRegion> _added = [];
        private readonly List<MapRegion> _removed = [];
        private readonly List<(MapRegion region, MapRegionState before, MapRegionState after)> _modified = new();

        private bool disposedValue;

        public void RegisterNewRegion(MapRegion region)
        {
            _added.Add(region);
        }

        public void RegisterRemovedRegion(MapRegion region)
        {
            _removed.Add(region);
        }

        public void RegisterModifiedRegion(MapRegion region, MapRegionState before, MapRegionState after)
        {
            _modified.Add((region, before, after));
        }

        public void Execute()
        {
            // Redo: reapply everything

            // 1. Add regions
            foreach (var r in _added)
            {
                _layer.Add(r);
            }

            // 2. Remove regions
            foreach (var r in _removed)
            {
                _layer.Remove(r);
            }

            // 3. Apply modifications
            foreach (var (region, _, after) in _modified)
            {
                region.RestoreState(after);
            }
        }

        public void Undo()
        {
            // 1. Undo modifications
            foreach (var (region, before, _) in _modified)
            {
                region.RestoreState(before);
            }

            // 2. Restore removed regions
            foreach (var r in _removed)
            {
                _layer.Add(r);
            }

            // 3. Remove added regions
            foreach (var r in _added)
            {
                _layer.Remove(r);
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

