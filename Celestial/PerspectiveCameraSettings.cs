namespace RealmStudioX.Core.Celestial
{
    public sealed class PerspectiveCameraSettings
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }

        public double LookDirectionX { get; set; }
        public double LookDirectionY { get; set; }
        public double LookDirectionZ { get; set; }

        public double UpDirectionX { get; set; }
        public double UpDirectionY { get; set; }
        public double UpDirectionZ { get; set; }

        public double FieldOfView { get; set; } = 45.0;

        public double NearPlaneDistance { get; set; } = 0.01;

        public double FarPlaneDistance { get; set; } = 10000.0;
    }
}
