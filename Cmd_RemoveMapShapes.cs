using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_RemoveMapShapes(MapScene scene, List<ShapeReference> shapesToRemove) : IUndoableCommand
    {
        private readonly MapScene? currentScene = scene;
        private readonly RealmStudioMap currentMap = scene.Map;
        private readonly List<ShapeReference> _shapesToRemove = [..shapesToRemove];

        private readonly HashSet<ShapeReference> _removedShapes = [];

        private bool disposedValue;

        public void Execute()
        {
            if (currentScene != null)
            {
                _removedShapes.Clear();

                foreach (var sr in _shapesToRemove)
                {
                    foreach (MapLayer layer in currentMap.MapLayers)
                    {
                        for (int i = layer.Shapes.Count - 1; i >= 0; i--)
                        {
                            ISelectable layerShape = layer.Shapes[i];

                            if (layerShape is MapComponent2D && layerShape.Id == sr.ReferencedShape?.Id)
                            {
                                var shapeReference = new ShapeReference()
                                {
                                    ShapeLayer = layer,
                                    ReferencedShape = layerShape
                                };

                                _removedShapes.Add(shapeReference);

                                layer.Remove((MapComponent2D)layerShape);
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
                    foreach (ShapeReference shapeReference in _removedShapes)
                    {
                        if (shapeReference.ShapeLayer != null
                            && layer.MapLayerId == shapeReference.ShapeLayer.MapLayerId
                            && shapeReference.ReferencedShape != null)
                        {
                            shapeReference.ShapeLayer.Add((MapComponent2D)shapeReference.ReferencedShape);
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
}
