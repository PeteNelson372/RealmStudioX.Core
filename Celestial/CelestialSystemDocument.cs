namespace RealmStudioX.Core.Celestial;

/// <summary>
/// Represents a celestial system containing its identity, display information, animation setting, and
/// celestial bodies. This class is used for serialization/deserialization of the celestial system
/// to an XML file.
/// </summary>
/// <remarks>The default instance has a generated identifier, a default name, an empty description, animation
/// enabled, and an empty collection of celestial bodies.</remarks>

public sealed class CelestialSystemDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Celestial System";
    public string Description { get; set; } = string.Empty;
    public bool Animate { get; set; } = true;

    public List<CelestialBodyNodeDocument> Bodies { get; set; } = [];
}

public sealed class CelestialBodyNodeDocument
{
    public CelestialBodyDocument Body { get; set; } = new();
    public CelestialBodyPlacementDocument Placement { get; set; } = new();

    public List<CelestialBodyNodeDocument> Children { get; set; } = [];

    public bool EnableOrbitAnimation { get; set; }

    public Matrix3DDocument? LocalTransform { get; set; }
}

public sealed class CelestialBodyPlacementDocument
{
    public double CenterX { get; set; }
    public double CenterY { get; set; }
    public double CenterZ { get; set; }

    public double Size { get; set; } = 0.09;
    public double Distance { get; set; } = 0.08;
    public double Orbit { get; set; }
    public double Plane { get; set; } = 0.24;

    public string RotationDirection { get; set; } = "Prograde";
    public double RotationRate { get; set; } = 1.0;
    public double AxialTilt { get; set; }
}

public sealed class Matrix3DDocument
{
    public double M11 { get; set; } = 1.0;
    public double M12 { get; set; }
    public double M13 { get; set; }
    public double M14 { get; set; }

    public double M21 { get; set; }
    public double M22 { get; set; } = 1.0;
    public double M23 { get; set; }
    public double M24 { get; set; }

    public double M31 { get; set; }
    public double M32 { get; set; }
    public double M33 { get; set; } = 1.0;
    public double M34 { get; set; }

    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double OffsetZ { get; set; }
    public double M44 { get; set; } = 1.0;
}

public sealed class CelestialBodyDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Body";
    public string Description { get; set; } = string.Empty;

    public string BodyType { get; set; } = "TerrestrialPlanet";
    public string RendererType { get; set; } = "Planet";

    public byte BaseColorA { get; set; } = 255;
    public byte BaseColorR { get; set; } = 255;
    public byte BaseColorG { get; set; } = 255;
    public byte BaseColorB { get; set; } = 255;

    public double LightIntensity { get; set; } = 0.5;

    public CelestialTextureSourceDocument TextureSource { get; set; } = new();
    public CelestialPointLightDocument PointLightSettings { get; set; } = new();

    public List<CelestialBodyAnimationDocument> Animations { get; set; } = [];
    public List<CelestialBodyAttachmentDocument> Attachments { get; set; } = [];
}

public sealed class CelestialTextureSourceDocument
{
    public string SourceType { get; set; } = "None";

    public string? MapId { get; set; }
    public string? AssetPath { get; set; }
    public string? FilePath { get; set; }

    public string? GeneratorId { get; set; }

    public List<StringParameterDocument> GeneratorParameters { get; set; } = [];
}

public sealed class StringParameterDocument
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class CelestialPointLightDocument
{
    public bool Enabled { get; set; } = true;
    public double Intensity { get; set; } = 0.1;
    public double Range { get; set; } = 10.0;
}

public sealed class CelestialBodyAnimationDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string AnimationType { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;

    // Identifies the concrete ICelestialBodyAnimationSettings implementation.
    public string SettingsType { get; set; } = string.Empty;

    // BasicRotationSettings
    public double RotationRate { get; set; } = 1.0;
    public string RotationDirection { get; set; } = "Prograde";

    // SunAnimationSettings / CoronaAnimationSettings
    public bool EnableProminences { get; set; } = true;
    public bool EnableFlares { get; set; } = true;
    public double ActivityIntensity { get; set; } = 1.0;
    public double ActivityFrequency { get; set; } = 8.0;
    public double ActivityGeometryScale { get; set; } = 1.0;
    public double ActivityThicknessScale { get; set; } = 1.0;

    // SunAnimationSettings
    public bool EnableCorona { get; set; } = true;
    public double CoronaOpacity { get; set; } = 0.18;
    public double CoronaRotationSpeed { get; set; } = 12.0;

    // BlackHoleAnimationSettings
    public bool EnableJets { get; set; } = true;
    public double JetLength { get; set; } = 8.4;
    public double JetPulseRate { get; set; } = 0.325;

    public bool EnableJetParticles { get; set; } = true;
    public double JetParticleSpeed { get; set; } = 2.625;
    public double JetParticleEmissionRate { get; set; } = 22.0;
    public double JetParticleLifetime { get; set; } = 3.2;
    public double JetParticleDispersion { get; set; } = 1.0;

    public bool EnableDiskParticles { get; set; } = true;
    public double DiskParticleEmissionRate { get; set; } = 6.0;
    public double DiskParticleLifetime { get; set; } = 2.5;
    public double DiskParticleOutflowSpeed { get; set; } = 1.0;

    // NebulaAnimationSettings
    public int LayerCount { get; set; } = 5;
    public double Width { get; set; } = 18.0;
    public double Height { get; set; } = 12.0;
    public double Depth { get; set; } = 5.0;
    public double Opacity { get; set; } = 0.9;
    public double Brightness { get; set; } = 1.0;
    public double DetailIntensity { get; set; } = 1.0;
    public double RotationSpeed { get; set; } = 1.2;
    public double DriftSpeed { get; set; } = 0.25;

    // GasGiantAnimationSettings
    public bool EnableAtmosphericTurbulence { get; set; } = true;
    public double TurbulenceIntensity { get; set; } = 0.35;

    // RingTurbulenceSettings
    public double Intensity { get; set; } = 0.2;
    public double UpdateRate { get; set; } = 30.0;

    // BasicRotationSettings
    public bool ShouldSerializeRotationRate() =>
        SettingsType == nameof(BasicRotationSettings);

    public bool ShouldSerializeRotationDirection() =>
        SettingsType == nameof(BasicRotationSettings);


    // SunAnimationSettings / CoronaAnimationSettings
    public bool ShouldSerializeEnableProminences() =>
        IsSolarActivitySettings();

    public bool ShouldSerializeEnableFlares() =>
        IsSolarActivitySettings();

    public bool ShouldSerializeActivityIntensity() =>
        IsSolarActivitySettings();

    public bool ShouldSerializeActivityFrequency() =>
        IsSolarActivitySettings();

    public bool ShouldSerializeActivityGeometryScale() =>
        IsSolarActivitySettings();

    public bool ShouldSerializeActivityThicknessScale() =>
        IsSolarActivitySettings();


    // SunAnimationSettings only
    public bool ShouldSerializeEnableCorona() =>
        SettingsType == nameof(SunAnimationSettings);

    public bool ShouldSerializeCoronaOpacity() =>
        SettingsType == nameof(SunAnimationSettings);

    public bool ShouldSerializeCoronaRotationSpeed() =>
        SettingsType == nameof(SunAnimationSettings);


    // BlackHoleAnimationSettings
    public bool ShouldSerializeEnableJets() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetLength() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetPulseRate() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeEnableJetParticles() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetParticleSpeed() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetParticleEmissionRate() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetParticleLifetime() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeJetParticleDispersion() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeEnableDiskParticles() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeDiskParticleEmissionRate() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeDiskParticleLifetime() =>
        SettingsType == nameof(BlackHoleAnimationSettings);

    public bool ShouldSerializeDiskParticleOutflowSpeed() =>
        SettingsType == nameof(BlackHoleAnimationSettings);


    // NebulaAnimationSettings
    public bool ShouldSerializeLayerCount() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeWidth() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeHeight() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeDepth() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeOpacity() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeBrightness() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeDetailIntensity() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeRotationSpeed() =>
        SettingsType == nameof(NebulaAnimationSettings);

    public bool ShouldSerializeDriftSpeed() =>
        SettingsType == nameof(NebulaAnimationSettings);


    // GasGiantAnimationSettings
    public bool ShouldSerializeEnableAtmosphericTurbulence() =>
        SettingsType == nameof(GasGiantAnimationSettings);

    public bool ShouldSerializeTurbulenceIntensity() =>
        SettingsType == nameof(GasGiantAnimationSettings);


    // RingTurbulenceSettings
    public bool ShouldSerializeIntensity() =>
        SettingsType == nameof(RingTurbulenceSettings);

    public bool ShouldSerializeUpdateRate() =>
        SettingsType == nameof(RingTurbulenceSettings);


    // Helper
    private bool IsSolarActivitySettings() =>
        SettingsType == nameof(SunAnimationSettings) ||
        SettingsType == nameof(CoronaAnimationSettings);
}

public sealed class CelestialBodyAttachmentDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;

    public CelestialTextureSourceDocument TextureSource { get; set; } = new();

    // Identifies the concrete ICelestialBodyAttachmentSettings implementation.
    public string SettingsType { get; set; } = string.Empty;

    // PlanetaryRingSettings
    public bool RingEnabled { get; set; }
    public string RingTexturePath { get; set; } = string.Empty;
    public double InnerRadiusScale { get; set; } = 1.2;
    public double OuterRadiusScale { get; set; } = 2.0;
    public double Inclination { get; set; } = 2.0;

    public List<CelestialBodyAnimationDocument> Animations { get; set; } = [];

    public bool ShouldSerializeRingEnabled() =>
        SettingsType == nameof(PlanetaryRingSettings);

    public bool ShouldSerializeRingTexturePath() =>
        SettingsType == nameof(PlanetaryRingSettings);

    public bool ShouldSerializeInnerRadiusScale() =>
        SettingsType == nameof(PlanetaryRingSettings);

    public bool ShouldSerializeOuterRadiusScale() =>
        SettingsType == nameof(PlanetaryRingSettings);

    public bool ShouldSerializeInclination() =>
        SettingsType == nameof(PlanetaryRingSettings);
}