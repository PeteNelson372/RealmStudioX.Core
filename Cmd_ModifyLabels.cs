using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyLabels(MapLayer layer) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly List<MapLabel> _before = [];

        private readonly HashSet<MapLabel> _newLabels = [];
        private readonly HashSet<MapLabel> _removedLabels = [];

        private bool disposedValue;

        public void RegisterNewLabel(MapLabel label)
        {
            _newLabels.Add(label);
        }

        public void RegisterRemovedLabel(MapLabel label)
        {
            _removedLabels.Add(label);
        }

        public void CaptureBefore(MapLabel label)
        {
            if (!_before.Contains(label))
            {
                _before.Add(label);
            }
        }

        public void Execute()
        {
            // Remove old labels
            foreach (var l in _removedLabels)
            {
                _layer.Remove(l);
            }

            // Add new labels
            foreach (var l in _newLabels)
            {              
                _layer.Add(l);
            }
        }

        public void Undo()
        {
            // Remove newly created labels
            foreach (var l in _newLabels)
            {
                _layer.Remove(l);
            }

            // Re-add removed labels
            foreach (var l in _removedLabels)
            {
                _layer.Add(l);
            }
        }

        public bool IsRemoved(MapLabel l)
        {
            return _removedLabels.Contains(l);
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