using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_AddDrawnShape(MapLayer layer, IDrawnMapComponent drawnShape) : IUndoableCommand
    {
        private readonly MapLayer _layer = layer;
        private readonly IDrawnMapComponent _drawnShape = drawnShape;
        private bool disposedValue;

        public static string Description => "Add Drawn Shape";

        public void Execute()
        {
            _layer.Add((MapComponent2D)_drawnShape);
        }

        public void Undo()
        {
            _layer.Remove((MapComponent2D)_drawnShape);
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
        // ~Cmd_AddShape()
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
