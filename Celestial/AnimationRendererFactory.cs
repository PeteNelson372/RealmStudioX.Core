using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core.Celestial;

public static class AnimationRendererFactory
{
    public static ICelestialBodyAnimationRenderer Create(
        CelestialBodyAnimation animation)
    {
        ArgumentNullException.ThrowIfNull(animation);

        return animation.AnimationType switch
        {
            CelestialBodyAnimationType.BasicRotation =>
                new BasicRotationAnimationRenderer(),

            CelestialBodyAnimationType.RingTurbulenceActivity =>
                new RingTurbulenceAnimationRenderer(),

            CelestialBodyAnimationType.SolarActivity =>
                new SolarActivityAnimationRenderer(),

            CelestialBodyAnimationType.BlackHoleActivity =>
                new BlackHoleActivityAnimationRenderer(),

            CelestialBodyAnimationType.AtmosphereTurbulenceActivity =>
                new AtmosphereTurbulenceRenderer(),

            _ => throw new NotSupportedException(
                $"Animation type '{animation.AnimationType}' is not supported.")
        };
    }
}