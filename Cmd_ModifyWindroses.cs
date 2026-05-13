using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyWindroses(MapLayer layer) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly List<MapWindrose> _before = [];

        private readonly HashSet<MapWindrose> _newWindroses = [];
        private readonly HashSet<MapWindrose> _removedWindroses = [];

        private bool disposedValue;

        public void RegisterNewWindrose(MapWindrose box)
        {
            _newWindroses.Add(box);
        }

        public void RegisterRemovedWindrose(MapWindrose box)
        {
            _removedWindroses.Add(box);
        }

        public void CaptureBefore(MapWindrose box)
        {
            if (!_before.Contains(box))
            {
                _before.Add(box);
            }
        }

        public void Execute()
        {
            // Remove old windroses
            foreach (var w in _removedWindroses)
            {
                _layer.Remove(w);
            }

            // Add new windroses
            foreach (var w in _newWindroses)
            {              
                _layer.Add(w);
            }
        }

        public void Undo()
        {
            // Remove newly created windroses
            foreach (var w in _newWindroses)
            {
                _layer.Remove(w);
            }

            // Re-add removed windroses
            foreach (var w in _removedWindroses)
            {
                _layer.Add(w);
            }
        }

        public bool IsRemoved(MapWindrose b)
        {
            return _removedWindroses.Contains(b);
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}