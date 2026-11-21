using RealmStudioShapeRenderingLib;
using System.Windows.Media.Media3D;

namespace RealmStudioX.Core.Celestial
{
    public interface ICelestialAttachmentRenderer
    {
        CelestialBodyAttachmentType Type { get; }

        Model3DGroup Root { get; }

        void Update(CelestialBodyAttachment attachment);
    }
}