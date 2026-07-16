using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_ModifyLandforms(MapLayer layer) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;

        private readonly Dictionary<Landform, Shape2DState> _before = [];

        private readonly HashSet<Landform> _newLandforms = [];
        private readonly HashSet<Landform> _removedLandforms = [];

        private bool disposedValue;

        public void RegisterNewLandform(Landform lf)
        {
            _newLandforms.Add(lf);
        }

        public void RegisterRemovedLandform(Landform lf)
        {
            _removedLandforms.Add(lf);

        }

        public void CaptureBefore(Landform lf)
        {
            if (!_before.ContainsKey(lf))
            {
                _before[lf] = (Shape2DState)lf.CaptureState();
            }
        }

        public void Execute()
        {
            // Remove old landforms
            foreach (var lf in _removedLandforms)
            {
                _layer.Remove(lf);
            }

            // Add new landforms
            foreach (var lf in _newLandforms)
            {
                lf.EndInteractive();
                lf.InvalidateRenderCache();

                _layer.Add(lf);
            }
        }

        public void Undo()
        {
            // Restore original geometry (but NOT removed ones that were newly added)
            foreach (var (lf, state) in _before)
            {
                lf.RestoreState(state);
            }

            // Remove newly created landforms
            foreach (var lf in _newLandforms)
            {
                _layer.Remove(lf);
            }

            // Re-add removed landforms
            foreach (var lf in _removedLandforms)
            {
                _layer.Add(lf);
            }
        }

        public bool IsRemoved(Landform lf)
        {
            return _removedLandforms.Contains(lf);
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