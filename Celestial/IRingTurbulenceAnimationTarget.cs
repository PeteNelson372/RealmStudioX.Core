using System.Windows.Media.Imaging;

namespace RealmStudioX.Core.Celestial;

public interface IRingTurbulenceAnimationTarget
{
    BitmapSource? SourceTexture { get; }

    void SetAnimatedTexture(BitmapSource? texture);
}