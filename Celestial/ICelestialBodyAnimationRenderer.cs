using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core.Celestial;

public interface ICelestialBodyAnimationRenderer : ICelestialAnimatedObject
{
    CelestialBodyAnimationType AnimationType { get; }

    void Update(CelestialBodyAnimation animation, ICelestialBodyAnimationTarget target);
}