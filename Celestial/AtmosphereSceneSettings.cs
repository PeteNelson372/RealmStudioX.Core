using System.Windows.Media;

namespace RealmStudioX.Core.Celestial;

public sealed class AtmosphereSceneSettings
{
    public bool EnableAtmosphere { get; set; }

    public bool EnableClouds { get; set; } = true;

    public Color AtmosphereColor { get; set; } = Colors.Transparent;

    public double AtmosphereDensity { get; set; } = 1.0;

    public double AtmosphereAltitude { get; set; } = 1.0;

    public bool UseDefaultCloudTexture { get; set; }

    public bool UseCustomCloudTexture { get; set; }

    public string? CustomCloudTexturePath { get; set; }

    public Color CloudColor { get; set; } = Colors.Transparent;

    public double CloudOpacity { get; set; } = 0.4;

    public double CloudRotationRate { get; set; } = 1.0;
}