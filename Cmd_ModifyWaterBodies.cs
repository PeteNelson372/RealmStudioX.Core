namespace RealmStudioX.Core
{
    using RealmStudioShapeRenderingLib;

    public class Cmd_ModifyWaterBodies(RealmStudioMap map) : IUndoableCommand
    {
        private readonly RealmStudioMap _map = map;

        private readonly HashSet<WaterBody> _addedBodies = [];
        private readonly HashSet<WaterBody> _removedBodies = [];

        private readonly HashSet<WaterSystem> _addedSystems = [];
        private readonly HashSet<WaterSystem> _removedSystems = [];

        private readonly Dictionary<WaterBody, IShapeState> _before = [];
        private readonly Dictionary<WaterBody, IShapeState> _after = [];
        private readonly Dictionary<WaterBody, WaterSystem> _removedBodySystems = [];
        private readonly Dictionary<WaterBody, WaterSystem> _addedBodySystems = [];

        private bool disposedValue;

        // -------------------------------------------------
        // Registration
        // -------------------------------------------------

        public void RegisterAddedWaterBody(WaterBody body)
        {
            if (body.WaterSystem != null)
            {
                _addedBodySystems[body] = body.WaterSystem;
            }

            _addedBodies.Add(body);
        }

        public void RegisterRemovedWaterBody(WaterBody body)
        {
            if (body.WaterSystem != null)
            {
                _removedBodySystems[body] = body.WaterSystem;
            }

            _removedBodies.Add(body);
        }

        public void RegisterAddedWaterSystem(WaterSystem system)
        {
            _addedSystems.Add(system);
        }

        public void RegisterRemovedWaterSystem(WaterSystem system)
        {
            _removedSystems.Add(system);
        }

        // -------------------------------------------------
        // State capture
        // -------------------------------------------------

        public void CaptureBefore(WaterBody body)
        {
            if (!_before.ContainsKey(body))
            {
                _before[body] = body.CaptureState();
            }
        }

        public void CaptureAfter(WaterBody body)
        {
            // Do not capture state for newly created bodies
            if (_addedBodies.Contains(body))
            {
                return;
            }

            if (!_after.ContainsKey(body))
            {
                _after[body] = body.CaptureState();
            }
        }

        // -------------------------------------------------
        // Execute (redo)
        // -------------------------------------------------

        public void Execute()
        {
            foreach (var sys in _removedSystems)
            {
                _map.WaterSystems.Remove(sys);
            }

            foreach (var sys in _addedSystems)
            {
                _map.WaterSystems.Add(sys);
            }

            foreach (var b in _removedBodies)
            {
                if (_removedBodySystems.TryGetValue(b, out var sys))
                {
                    sys.Remove(b);
                }
            }

            foreach (var b in _addedBodies)
            {
                if (_addedBodySystems.TryGetValue(b, out var sys))
                {
                    sys.Add(b);
                }
            }

            foreach (var (body, state) in _after)
            {
                if (!_removedBodies.Contains(body))
                {
                    body.RestoreState(state);
                }
            }

            RebuildSystems();
        }

        // -------------------------------------------------
        // Undo
        // -------------------------------------------------

        public void Undo()
        {
            foreach (var (body, state) in _before)
            {
                body.RestoreState(state);
            }

            foreach (var b in _addedBodies)
            {
                if (_addedBodySystems.TryGetValue(b, out var sys))
                {
                    sys.Remove(b);
                }
            }

            foreach (var b in _removedBodies)
            {
                if (_removedBodySystems.TryGetValue(b, out var sys))
                {
                    sys.Add(b);
                }
            }

            foreach (var sys in _addedSystems)
            {
                _map.WaterSystems.Remove(sys);
            }

            foreach (var sys in _removedSystems)
            {
                _map.WaterSystems.Add(sys);
            }

            RebuildSystems();
        }

        // -------------------------------------------------
        // Helpers
        // -------------------------------------------------

        private void RebuildSystems()
        {
            var systems = new HashSet<WaterSystem>();

            foreach (var body in _before.Keys)
            {
                if (body.WaterSystem != null)
                {
                    systems.Add(body.WaterSystem);
                }
            }

            foreach (var body in _after.Keys)
            {
                if (body.WaterSystem != null)
                {
                    systems.Add(body.WaterSystem);
                }
            }

            foreach (var sys in systems)
            {
                //sys.RebuildMergedGeometry();
                sys.InvalidateRenderCache();
            }
        }

        // -------------------------------------------------
        // Disposal
        // -------------------------------------------------

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