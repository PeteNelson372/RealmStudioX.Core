using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_UpdateMapPathProperties(MapPath selectedMapPath, PathRenderStyle renderStyle, IAssetProvider assets) : IUndoableCommand
    {
        private readonly MapPath SelectedMapPath = selectedMapPath;
        private readonly PathRenderStyle NewPathRenderStyle = renderStyle;

        private readonly IAssetProvider Assets = assets;

        private PathRenderStyle? OldPathRenderStyle;

        private bool disposedValue;

        public void Execute()
        {
            OldPathRenderStyle = SelectedMapPath.RenderStyle;

            SelectedMapPath.RenderStyle = NewPathRenderStyle.Clone();

            SelectedMapPath.ResolveAssets(Assets);
        }

        public void Undo()
        {
            if (OldPathRenderStyle != null)
            {
                SelectedMapPath.RenderStyle = OldPathRenderStyle;

                SelectedMapPath.ResolveAssets(Assets);
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
        // ~Cmd_UpdateLandformProperties()
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
