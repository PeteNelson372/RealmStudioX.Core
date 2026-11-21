using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;

namespace RealmStudioX.Core.Celestial.Particles;

public readonly record struct ParticleState(
    Point3D Position,
    double Size,
    Color Color,
    double Opacity);