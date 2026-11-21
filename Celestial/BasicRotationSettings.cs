using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core.Celestial;

public sealed class BasicRotationSettings : ICelestialBodyAnimationSettings
{
    public double RotationRate { get; set; } = 1.0;

    public WorldRotationDirection RotationDirection { get; set; }
        = WorldRotationDirection.Prograde;

    public ICelestialBodyAnimationSettings Clone()
    {
        return new BasicRotationSettings
        {
            RotationRate = RotationRate,
            RotationDirection = RotationDirection
        };
    }
}