namespace RealmStudioX.Core.Celestial;

public sealed class SceneEffectsSettings
{
    // Blur
    public bool EnableBlur { get; set; }
    public double BlurAmount { get; set; } = 5.0;

    // Bleach bypass
    public bool EnableBleachBypass { get; set; }
    public double BleachBypassOpacity { get; set; } = 50.0;

    // Bloom
    public bool EnableBloom { get; set; }
    public double BloomBlurWidth { get; set; } = 1.0;
    public double BloomIntensity { get; set; } = 0.25;
    public double BloomHighlightThreshold { get; set; } = 0.90;
    public double BloomHighlightIntensity { get; set; } = 0.15;

    // Sepia
    public bool EnableSepia { get; set; }
    public double SepiaAmount { get; set; } = 50.0;

    // Grayscale
    public bool EnableGrayscale { get; set; }
    public double DesaturationFactor { get; set; } = 50.0;

    // Lens flare
    public bool EnableLensFlare { get; set; }
    public double LensFlareIntensity { get; set; } = 0.8;
    public double LensFlareGlowSize { get; set; } = 0.035;
    public double LensFlareStreakLength { get; set; } = 0.65;
    public double LensFlareGhostIntensity { get; set; } = 0.35;
    public double LensFlareStarburstIntensity { get; set; } = 0.2;

    // Background star scintillation
    public bool EnableStarScintillation { get; set; }
    public double StarScintillationIntensity { get; set; } = 1.0;
    public double StarScintillationSpeed { get; set; } = 2.0;
    public double StarScintillationThreshold { get; set; } = 0.72;
    public double StarScintillationContrastThreshold { get; set; } = 0.12;
    public double StarScintillationMaximumStarSize { get; set; } = 5.0;
}