using RealmStudioShapeRenderingLib;
using SkiaSharp;

namespace RealmStudioX.Core
{
    public interface IToolEditor
    {
        void Activate();
        void Deactivate();

        void OnMouseDown(PointerState state);
        void OnMouseMove(PointerState state);
        void OnMouseUp(PointerState state);
        void OnMouseDoubleClick(PointerState state);
        void OnMouseWheel(PointerState state);
        void Cancel();

        void RenderOverlay(SKCanvas canvas, SKPoint world);
    }

    //==========================================
    // Pointer State Struct
    //==========================================
    public struct PointerState
    {
        public SKPoint WorldPoint;
        public SKPoint ScreenPoint;
        public bool IsDoubleClick;
        public bool IsMouseWheelScrolled;
        public EditorMouseButton Button;
        public int WheelDelta;
        public InputModifiers Modifiers;
    }
}
