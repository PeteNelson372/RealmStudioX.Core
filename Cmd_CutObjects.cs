using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_CutObjects : IUndoableCommand
    {
        private readonly RealmStudioMap _map;
        private readonly List<ShapeReference> _objects;
        private bool disposedValue;


        public Cmd_CutObjects(IEnumerable<ShapeReference> objects, RealmStudioMap map)
        {
            // Make our own copy of the list so subsequent changes to
            // the SelectionService selection do not affect this command.
            _objects = [.. objects];
            _map = map;
        }

        public void Execute()
        {
            List<MapLayer> layersToUpdate = [];

            foreach (ShapeReference reference in _objects)
            {
                if (reference.ShapeLayer == null)
                    continue;

                if (reference.ReferencedShape is not MapComponent2D shape)
                    continue;

                shape.IsSelected = false;
                reference.ShapeLayer.Remove(shape);

                if (!layersToUpdate.Contains(reference.ShapeLayer))
                {
                    layersToUpdate.Add(reference.ShapeLayer);
                }
            }

            foreach (MapLayer layer in layersToUpdate)
            {
                layer.InvalidateAllTiles();
                layer.RebuildIndexes();
            }

        }

        public void Undo()
        {
            List<MapLayer> layersToUpdate = [];

            foreach (ShapeReference reference in _objects)
            {
                if (reference.ShapeLayer == null)
                    continue;

                if (reference.ReferencedShape is not MapComponent2D shape)
                    continue;

                reference.ShapeLayer.Add(shape);

                if (!layersToUpdate.Contains(reference.ShapeLayer))
                {
                    layersToUpdate.Add(reference.ShapeLayer);
                }
            }

            foreach (MapLayer layer in layersToUpdate)
            {
                layer.InvalidateAllTiles();
                layer.RebuildIndexes();
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
        // ~Cmd_RemoveMapShapes()
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
