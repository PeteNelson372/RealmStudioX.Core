using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class ShapeReference
    {
        public MapLayer? ShapeLayer { get; set; }
        public ISelectable? ReferencedShape { get; set; }
    }
}
