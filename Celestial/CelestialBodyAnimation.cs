using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core.Celestial;

public sealed class CelestialBodyAnimation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public CelestialBodyAnimationType AnimationType { get; set; }

    public bool Enabled { get; set; } = true;

    public ICelestialBodyAnimationSettings? Settings { get; set; }
}