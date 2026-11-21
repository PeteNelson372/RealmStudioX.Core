using System.Windows.Media.Imaging;

namespace RealmStudioX.Core.Celestial
{
    public interface IAttachmentContainer
    {
        void RefreshAttachments(Func<CelestialBodyAttachment, BitmapSource?> textureResolver);
    }
}
