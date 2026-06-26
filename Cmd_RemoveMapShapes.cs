using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_RemoveMapShapes(MapScene scene, List<ISelectable> shapesToRemove) : IUndoableCommand
    {
        private readonly MapScene? currentScene = scene;
        private readonly RealmStudioMap currentMap = scene.Map;
        private readonly List<ISelectable> _shapesToRemove = [..shapesToRemove];

        private readonly HashSet<RemovedShapeReference> _removedShapes = [];

        private bool disposedValue;

        public void Execute()
        {
            if (currentScene != null)
            {
                _removedShapes.Clear();

                foreach (var shape in _shapesToRemove)
                {
                    foreach (MapLayer layer in currentMap.MapLayers)
                    {
                        for (int i = layer.Shapes.Count - 1; i >= 0; i--)
                        {
                            MapComponent2D layerShape = layer.Shapes[i];

                            if (layerShape.Id == shape.Id)
                            {
                                var shapeReference = new RemovedShapeReference()
                                {
                                    shapeLayer = layer,
                                    removedShape = layerShape
                                };

                                _removedShapes.Add(shapeReference);

                                layer.Remove(layerShape);
                            }
                        }

                        layer.RebuildIndexes();
                    }
                }
            }
        }

        public void Undo()
        {
            if (currentScene != null)
            {
                foreach (MapLayer layer in currentMap.MapLayers)
                {
                    foreach (RemovedShapeReference shapeReference in _removedShapes)
                    {
                        if (shapeReference.shapeLayer != null
                            && layer.MapLayerId == shapeReference.shapeLayer.MapLayerId
                            && shapeReference.removedShape != null)
                        {
                            shapeReference.shapeLayer.Add(shapeReference.removedShape);
                        }
                    }

                    layer.RebuildIndexes();
                }
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

    public class RemovedShapeReference
    {
        public MapLayer? shapeLayer { get; set; }
        public MapComponent2D? removedShape { get; set; }
    }
}
