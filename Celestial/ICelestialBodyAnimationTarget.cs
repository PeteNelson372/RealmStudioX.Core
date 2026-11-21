using System.Windows.Media.Media3D;

namespace RealmStudioX.Core.Celestial;

public interface ICelestialBodyAnimationTarget
{
    public Transform3DGroup AnimationTransform { get; }
}