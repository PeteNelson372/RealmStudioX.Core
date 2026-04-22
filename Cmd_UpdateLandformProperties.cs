using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    internal class Cmd_UpdateLandformProperties(Landform selectedLandform, LandformShadingSettings newShadingSettings,
        CoastlineSettings newCoastlineSettings, IAssetProvider assets) : IUndoableCommand
    {
        private readonly Landform SelectedLandform = selectedLandform;
        private readonly LandformShadingSettings NewShadingSettings = newShadingSettings;
        private readonly CoastlineSettings NewCoastlineSettings = newCoastlineSettings;

        private readonly IAssetProvider Assets = assets;

        private LandformShadingSettings? OldShadingSettings;
        private CoastlineSettings? OldCoastlineSettings;

        private bool disposedValue;

        public void Execute()
        {
            OldShadingSettings = SelectedLandform.Shading;
            OldCoastlineSettings = SelectedLandform.Coastline;

            SelectedLandform.Shading = NewShadingSettings.Clone();
            SelectedLandform.Coastline = NewCoastlineSettings.Clone();

            SelectedLandform.ResolveAssets(Assets);

            SelectedLandform.InvalidateRenderCache();
        }

        public void Undo()
        {
            if (OldShadingSettings != null && OldCoastlineSettings != null)
            {
                SelectedLandform.Shading = OldShadingSettings;
                SelectedLandform.Coastline = OldCoastlineSettings;

                SelectedLandform.ResolveAssets(Assets);

                SelectedLandform.InvalidateRenderCache();
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
