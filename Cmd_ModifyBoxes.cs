using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyBoxes(MapLayer layer) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly List<PlacedMapBox> _before = [];

        private readonly HashSet<PlacedMapBox> _newBoxes = [];
        private readonly HashSet<PlacedMapBox> _removedBoxes = [];

        private bool disposedValue;

        public void RegisterNewBox(PlacedMapBox box)
        {
            _newBoxes.Add(box);
        }

        public void RegisterRemovedBox(PlacedMapBox box)
        {
            _removedBoxes.Add(box);
        }

        public void CaptureBefore(PlacedMapBox box)
        {
            if (!_before.Contains(box))
            {
                _before.Add(box);
            }
        }

        public void Execute()
        {
            // Remove old boxes
            foreach (var b in _removedBoxes)
            {
                _layer.Remove(b);
            }

            // Add new boxes
            foreach (var b in _newBoxes)
            {              
                _layer.Add(b);
            }
        }

        public void Undo()
        {
            // Remove newly created boxes
            foreach (var b in _newBoxes)
            {
                _layer.Remove(b);
            }

            // Re-add removed boxes
            foreach (var b in _removedBoxes)
            {
                _layer.Add(b);
            }
        }

        public bool IsRemoved(PlacedMapBox b)
        {
            return _removedBoxes.Contains(b);
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