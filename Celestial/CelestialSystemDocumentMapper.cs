using RealmStudioShapeRenderingLib;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RealmStudioX.Core.Celestial;

public static class CelestialSystemDocumentMapper
{
    #region Celestial system

    public static CelestialSystemDocument ToDocument(CelestialSystem source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialSystemDocument
        {
            Id = source.Id.ToString(),
            Name = source.Name,
            Description = source.Description,
            Animate = source.Animate,
            Bodies = [.. source.Bodies.Select(ToDocument)]
        };
    }

    public static CelestialSystem FromDocument(CelestialSystemDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (!Guid.TryParse(document.Id, out Guid id))
        {
            throw new InvalidOperationException(
                $"Invalid celestial system ID: '{document.Id}'.");
        }

        CelestialSystem result = new()
        {
            Id = id,
            Name = document.Name,
            Description = document.Description,
            Animate = document.Animate
        };

        foreach (CelestialBodyNodeDocument bodyNode in document.Bodies)
        {
            result.Bodies.Add(FromDocument(bodyNode));
        }

        return result;
    }

    #endregion

    #region Body nodes

    private static CelestialBodyNodeDocument ToDocument(CelestialBodyNode source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialBodyNodeDocument
        {
            Body = ToDocument(source.Body),
            Placement = ToDocument(source.Placement),
            EnableOrbitAnimation = source.EnableOrbitAnimation,
            LocalTransform = source.LocalTransform is null
                ? null
                : ToDocument(source.LocalTransform.Value),
            Children = source.Children
                .Select(ToDocument)
                .ToList()
        };
    }

    private static CelestialBodyNode FromDocument(
        CelestialBodyNodeDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        CelestialBodyNode result = new()
        {
            Body = FromDocument(document.Body),
            Placement = FromDocument(document.Placement),
            EnableOrbitAnimation = document.EnableOrbitAnimation,
            LocalTransform = document.LocalTransform is null
                ? null
                : new MatrixTransform3D(
                    FromDocument(document.LocalTransform))
        };

        foreach (CelestialBodyNodeDocument child in document.Children)
        {
            result.Children.Add(FromDocument(child));
        }

        return result;
    }

    #endregion

    #region Body placement

    private static CelestialBodyPlacementDocument ToDocument(
        CelestialBodyPlacementSettings source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialBodyPlacementDocument
        {
            CenterX = source.Center.X,
            CenterY = source.Center.Y,
            CenterZ = source.Center.Z,
            Size = source.Size,
            Distance = source.Distance,
            Orbit = source.Orbit,
            Plane = source.Plane,
            RotationDirection = source.RotationDirection.ToString(),
            RotationRate = source.RotationRate,
            AxialTilt = source.AxialTilt
        };
    }

    private static CelestialBodyPlacementSettings FromDocument(
        CelestialBodyPlacementDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new CelestialBodyPlacementSettings
        {
            Center = new Point3D(
                document.CenterX,
                document.CenterY,
                document.CenterZ),

            Size = document.Size,
            Distance = document.Distance,
            Orbit = document.Orbit,
            Plane = document.Plane,

            RotationDirection = ParseEnum<WorldRotationDirection>(
                document.RotationDirection,
                nameof(document.RotationDirection)),

            RotationRate = document.RotationRate,
            AxialTilt = document.AxialTilt
        };
    }

    #endregion

    #region Matrix transforms

    private static Matrix3DDocument ToDocument(Matrix3D matrix)
    {
        return new Matrix3DDocument
        {
            M11 = matrix.M11,
            M12 = matrix.M12,
            M13 = matrix.M13,
            M14 = matrix.M14,

            M21 = matrix.M21,
            M22 = matrix.M22,
            M23 = matrix.M23,
            M24 = matrix.M24,

            M31 = matrix.M31,
            M32 = matrix.M32,
            M33 = matrix.M33,
            M34 = matrix.M34,

            OffsetX = matrix.OffsetX,
            OffsetY = matrix.OffsetY,
            OffsetZ = matrix.OffsetZ,
            M44 = matrix.M44
        };
    }

    private static Matrix3D FromDocument(Matrix3DDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new Matrix3D(
            document.M11,
            document.M12,
            document.M13,
            document.M14,

            document.M21,
            document.M22,
            document.M23,
            document.M24,

            document.M31,
            document.M32,
            document.M33,
            document.M34,

            document.OffsetX,
            document.OffsetY,
            document.OffsetZ,
            document.M44);
    }

    #endregion

    #region Celestial body

    private static CelestialBodyDocument ToDocument(CelestialBody source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialBodyDocument
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,

            BodyType = source.BodyType.ToString(),
            RendererType = source.RendererType.ToString(),

            BaseColorA = source.BaseColor.A,
            BaseColorR = source.BaseColor.R,
            BaseColorG = source.BaseColor.G,
            BaseColorB = source.BaseColor.B,

            LightIntensity = source.LightIntensity,

            TextureSource = ToDocument(source.TextureSource),
            PointLightSettings = ToDocument(source.PointLightSettings),

            Animations = source.Animations
                .Select(ToDocument)
                .ToList(),

            Attachments = source.Attachments
                .Select(ToDocument)
                .ToList()
        };
    }

    private static CelestialBody FromDocument(
        CelestialBodyDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        CelestialBody result = new()
        {
            Id = document.Id,
            Name = document.Name,
            Description = document.Description,

            BodyType = ParseEnum<CelestialBodyType>(
                document.BodyType,
                nameof(document.BodyType)),

            RendererType = ParseEnum<CelestialBodyRendererType>(
                document.RendererType,
                nameof(document.RendererType)),

            BaseColor = Color.FromArgb(
                document.BaseColorA,
                document.BaseColorR,
                document.BaseColorG,
                document.BaseColorB),

            LightIntensity = document.LightIntensity,

            TextureSource = FromDocument(document.TextureSource),
            PointLightSettings = FromDocument(document.PointLightSettings)
        };

        foreach (CelestialBodyAnimationDocument animation in document.Animations)
        {
            result.Animations.Add(FromDocument(animation));
        }

        foreach (CelestialBodyAttachmentDocument attachment in document.Attachments)
        {
            result.Attachments.Add(FromDocument(attachment));
        }

        return result;
    }

    #endregion

    #region Texture source

    private static CelestialTextureSourceDocument ToDocument(
        CelestialTextureSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialTextureSourceDocument
        {
            SourceType = source.SourceType.ToString(),
            MapId = source.MapId,
            AssetPath = source.AssetPath,
            FilePath = source.FilePath,
            GeneratorId = source.GeneratorId,

            GeneratorParameters = source.GeneratorParameters
                .Select(parameter => new StringParameterDocument
                {
                    Key = parameter.Key,
                    Value = parameter.Value
                })
                .ToList()
        };
    }

    private static CelestialTextureSource FromDocument(
        CelestialTextureSourceDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        CelestialTextureSource result = new()
        {
            SourceType = ParseEnum<CelestialTextureSourceType>(
                document.SourceType,
                nameof(document.SourceType)),

            MapId = document.MapId,
            AssetPath = document.AssetPath,
            FilePath = document.FilePath,
            GeneratorId = document.GeneratorId
        };

        foreach (StringParameterDocument parameter in document.GeneratorParameters)
        {
            result.GeneratorParameters.Add(
                parameter.Key,
                parameter.Value);
        }

        return result;
    }

    #endregion

    #region Point light

    private static CelestialPointLightDocument ToDocument(
        CelestialPointLightSettings source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new CelestialPointLightDocument
        {
            Enabled = source.Enabled,
            Intensity = source.Intensity,
            Range = source.Range
        };
    }

    private static CelestialPointLightSettings FromDocument(
        CelestialPointLightDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new CelestialPointLightSettings
        {
            Enabled = document.Enabled,
            Intensity = document.Intensity,
            Range = document.Range
        };
    }

    #endregion

    #region Animations

    private static CelestialBodyAnimationDocument ToDocument(
        CelestialBodyAnimation source)
    {
        ArgumentNullException.ThrowIfNull(source);

        CelestialBodyAnimationDocument document = new()
        {
            Id = source.Id,
            AnimationType = source.AnimationType.ToString(),
            Enabled = source.Enabled
        };

        if (source.Settings is null)
        {
            document.SettingsType = string.Empty;
            return document;
        }

        switch (source.Settings)
        {
            case BasicRotationSettings settings:
                document.SettingsType = nameof(BasicRotationSettings);
                document.RotationRate = settings.RotationRate;
                document.RotationDirection =
                    settings.RotationDirection.ToString();
                break;

            case SunAnimationSettings settings:
                document.SettingsType = nameof(SunAnimationSettings);
                CopySolarActivity(settings, document);

                document.EnableCorona = settings.EnableCorona;
                document.CoronaOpacity = settings.CoronaOpacity;
                document.CoronaRotationSpeed = settings.CoronaRotationSpeed;
                break;

            case CoronaAnimationSettings settings:
                document.SettingsType = nameof(CoronaAnimationSettings);
                CopySolarActivity(settings, document);
                break;

            case BlackHoleAnimationSettings settings:
                document.SettingsType = nameof(BlackHoleAnimationSettings);

                document.EnableJets = settings.EnableJets;
                document.JetLength = settings.JetLength;
                document.JetPulseRate = settings.JetPulseRate;

                document.EnableJetParticles = settings.EnableJetParticles;
                document.JetParticleSpeed = settings.JetParticleSpeed;
                document.JetParticleEmissionRate = settings.JetParticleEmissionRate;
                document.JetParticleLifetime = settings.JetParticleLifetime;
                document.JetParticleDispersion = settings.JetParticleDispersion;

                document.EnableDiskParticles = settings.EnableDiskParticles;
                document.DiskParticleEmissionRate = settings.DiskParticleEmissionRate;
                document.DiskParticleLifetime = settings.DiskParticleLifetime;
                document.DiskParticleOutflowSpeed = settings.DiskParticleOutflowSpeed;
                break;

            case NebulaAnimationSettings settings:
                document.SettingsType = nameof(NebulaAnimationSettings);

                document.LayerCount = settings.LayerCount;
                document.Width = settings.Width;
                document.Height = settings.Height;
                document.Depth = settings.Depth;
                document.Opacity = settings.Opacity;
                document.Brightness = settings.Brightness;
                document.DetailIntensity = settings.DetailIntensity;
                document.RotationSpeed = settings.RotationSpeed;
                document.DriftSpeed = settings.DriftSpeed;
                break;

            case GasGiantAnimationSettings settings:
                document.SettingsType = nameof(GasGiantAnimationSettings);

                document.EnableAtmosphericTurbulence =
                    settings.EnableAtmosphericTurbulence;

                document.TurbulenceIntensity = settings.TurbulenceIntensity;
                break;

            case RingTurbulenceSettings settings:
                document.SettingsType = nameof(RingTurbulenceSettings);
                document.Intensity = settings.Intensity;
                document.UpdateRate = settings.UpdateRate;
                break;

            default:
                throw new NotSupportedException(
                    $"Animation settings type '{source.Settings.GetType().FullName}' " +
                    "is not supported by the celestial document mapper.");
        }

        return document;
    }

    private static CelestialBodyAnimation FromDocument(
        CelestialBodyAnimationDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        CelestialBodyAnimation result = new()
        {
            Id = document.Id,

            AnimationType = ParseEnum<CelestialBodyAnimationType>(
                document.AnimationType,
                nameof(document.AnimationType)),

            Enabled = document.Enabled,
            Settings = FromDocumentSettings(document)
        };

        return result;
    }

    private static ICelestialBodyAnimationSettings? FromDocumentSettings(
        CelestialBodyAnimationDocument document)
    {
        if (string.IsNullOrWhiteSpace(document.SettingsType))
            return null;

        return document.SettingsType switch
        {
            nameof(BasicRotationSettings) => new BasicRotationSettings
            {
                RotationRate = document.RotationRate,

                RotationDirection = ParseEnum<WorldRotationDirection>(
                    document.RotationDirection,
                    nameof(document.RotationDirection))
            },

            nameof(SunAnimationSettings) => new SunAnimationSettings
            {
                EnableProminences = document.EnableProminences,
                EnableFlares = document.EnableFlares,
                ActivityIntensity = document.ActivityIntensity,
                ActivityFrequency = document.ActivityFrequency,
                ActivityGeometryScale = document.ActivityGeometryScale,
                ActivityThicknessScale = document.ActivityThicknessScale,
                EnableCorona = document.EnableCorona,
                CoronaOpacity = document.CoronaOpacity,
                CoronaRotationSpeed = document.CoronaRotationSpeed
            },

            nameof(CoronaAnimationSettings) => new CoronaAnimationSettings
            {
                EnableProminences = document.EnableProminences,
                EnableFlares = document.EnableFlares,
                ActivityIntensity = document.ActivityIntensity,
                ActivityFrequency = document.ActivityFrequency,
                ActivityGeometryScale = document.ActivityGeometryScale,
                ActivityThicknessScale = document.ActivityThicknessScale
            },

            nameof(BlackHoleAnimationSettings) => new BlackHoleAnimationSettings
            {
                EnableJets = document.EnableJets,
                JetLength = document.JetLength,
                JetPulseRate = document.JetPulseRate,

                EnableJetParticles = document.EnableJetParticles,
                JetParticleSpeed = document.JetParticleSpeed,
                JetParticleEmissionRate = document.JetParticleEmissionRate,
                JetParticleLifetime = document.JetParticleLifetime,
                JetParticleDispersion = document.JetParticleDispersion,

                EnableDiskParticles = document.EnableDiskParticles,
                DiskParticleEmissionRate = document.DiskParticleEmissionRate,
                DiskParticleLifetime = document.DiskParticleLifetime,
                DiskParticleOutflowSpeed = document.DiskParticleOutflowSpeed
            },

            nameof(NebulaAnimationSettings) => new NebulaAnimationSettings
            {
                LayerCount = document.LayerCount,
                Width = document.Width,
                Height = document.Height,
                Depth = document.Depth,
                Opacity = document.Opacity,
                Brightness = document.Brightness,
                DetailIntensity = document.DetailIntensity,
                RotationSpeed = document.RotationSpeed,
                DriftSpeed = document.DriftSpeed
            },

            nameof(GasGiantAnimationSettings) => new GasGiantAnimationSettings
            {
                EnableAtmosphericTurbulence =
                    document.EnableAtmosphericTurbulence,

                TurbulenceIntensity = document.TurbulenceIntensity
            },

            nameof(RingTurbulenceSettings) => new RingTurbulenceSettings
            {
                Intensity = document.Intensity,
                UpdateRate = document.UpdateRate
            },

            _ => throw new NotSupportedException(
                $"Animation settings type '{document.SettingsType}' " +
                "is not supported by the celestial document mapper.")
        };
    }

    private static void CopySolarActivity(
        SunAnimationSettings source,
        CelestialBodyAnimationDocument destination)
    {
        destination.EnableProminences = source.EnableProminences;
        destination.EnableFlares = source.EnableFlares;
        destination.ActivityIntensity = source.ActivityIntensity;
        destination.ActivityFrequency = source.ActivityFrequency;
        destination.ActivityGeometryScale = source.ActivityGeometryScale;
        destination.ActivityThicknessScale = source.ActivityThicknessScale;
    }

    private static void CopySolarActivity(
        CoronaAnimationSettings source,
        CelestialBodyAnimationDocument destination)
    {
        destination.EnableProminences = source.EnableProminences;
        destination.EnableFlares = source.EnableFlares;
        destination.ActivityIntensity = source.ActivityIntensity;
        destination.ActivityFrequency = source.ActivityFrequency;
        destination.ActivityGeometryScale = source.ActivityGeometryScale;
        destination.ActivityThicknessScale = source.ActivityThicknessScale;
    }

    #endregion

    #region Attachments

    private static CelestialBodyAttachmentDocument ToDocument(
        CelestialBodyAttachment source)
    {
        ArgumentNullException.ThrowIfNull(source);

        CelestialBodyAttachmentDocument document = new()
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Type = source.Type.ToString(),
            Enabled = source.Enabled,
            TextureSource = ToDocument(source.TextureSource)
        };

        switch (source.AttachmentSettings)
        {
            case null:
                document.SettingsType = string.Empty;
                break;

            case PlanetaryRingSettings settings:
                document.SettingsType = nameof(PlanetaryRingSettings);
                document.RingEnabled = settings.Enabled;
                document.RingTexturePath = settings.TexturePath;
                document.InnerRadiusScale = settings.InnerRadiusScale;
                document.OuterRadiusScale = settings.OuterRadiusScale;
                document.Inclination = settings.Inclination;
                break;

            default:
                throw new NotSupportedException(
                    $"Attachment settings type " +
                    $"'{source.AttachmentSettings.GetType().FullName}' " +
                    "is not supported by the celestial document mapper.");
        }

        document.Animations = source.Animations
            .Select(ToDocument)
            .ToList();

        return document;
    }

    private static CelestialBodyAttachment FromDocument(
        CelestialBodyAttachmentDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        CelestialBodyAttachment result = new()
        {
            Id = document.Id,
            Name = document.Name,
            Description = document.Description,

            Type = ParseEnum<CelestialBodyAttachmentType>(
                document.Type,
                nameof(document.Type)),

            Enabled = document.Enabled,
            TextureSource = FromDocument(document.TextureSource),
            AttachmentSettings = FromDocumentAttachmentSettings(document)
        };

        foreach (CelestialBodyAnimationDocument animation in document.Animations)
        {
            result.Animations.Add(FromDocument(animation));
        }

        return result;
    }

    private static ICelestialBodyAttachmentSettings? FromDocumentAttachmentSettings(
        CelestialBodyAttachmentDocument document)
    {
        if (string.IsNullOrWhiteSpace(document.SettingsType))
            return null;

        return document.SettingsType switch
        {
            nameof(PlanetaryRingSettings) => new PlanetaryRingSettings
            {
                Enabled = document.RingEnabled,
                TexturePath = document.RingTexturePath,
                InnerRadiusScale = document.InnerRadiusScale,
                OuterRadiusScale = document.OuterRadiusScale,
                Inclination = document.Inclination
            },

            _ => throw new NotSupportedException(
                $"Attachment settings type '{document.SettingsType}' " +
                "is not supported by the celestial document mapper.")
        };
    }

    #endregion

    #region Enum parsing

    private static TEnum ParseEnum<TEnum>(
        string value,
        string propertyName)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, ignoreCase: true, out TEnum result) &&
            Enum.IsDefined(result))
        {
            return result;
        }

        throw new InvalidOperationException(
            $"Invalid {typeof(TEnum).Name} value '{value}' " +
            $"in {propertyName}.");
    }

    #endregion
}