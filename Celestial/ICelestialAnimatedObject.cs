namespace RealmStudioX.Core.Celestial
{
    public interface ICelestialAnimatedObject
    {
        void AdvanceAnimation(CelestialAnimationFrame frame);

        void ResetAnimation();
    }
}