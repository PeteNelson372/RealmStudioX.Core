namespace RealmStudioX.Core.Celestial;

public sealed class WorldGlobeSceneDocument
{
    public int FormatVersion { get; set; } = 1;

    public string Name { get; set; } = "World Globe Scene";

    public string Description { get; set; } = string.Empty;

    public SceneBackgroundSettings Background { get; set; } = new();

    public CelestialSystemDocument CelestialSystem { get; set; } = new();

    public WorldGlobeSceneSettings WorldGlobe { get; set; } = new();

    public AtmosphereSceneSettings Atmosphere { get; set; } = new();

    public CelestialLightingSettings Lighting { get; set; } = new();

    public SceneEffectsSettings Effects { get; set; } = new();

    public PerspectiveCameraSettings Camera { get; set; } = new();
}