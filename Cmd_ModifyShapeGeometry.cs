using RealmStudioShapeRenderingLib;
using SkiaSharp;


namespace RealmStudioX.Core
{
    public class Cmd_ModifyShapeGeometry(Shape2D shape, SKPath before, SKPath after) : IUndoableCommand
    {
        private readonly Shape2D _shape = shape;
        private readonly SKPath _before = new(before);
        private readonly SKPath _after = new(after);
        private bool disposedValue;

        public void Execute()
        {
            _shape.RestoreGeometry(_after);
        }

        public void Undo()
        {
            _shape.RestoreGeometry(_before);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _before.Dispose();
                    _after.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_ModifyShapeGeometry()
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
