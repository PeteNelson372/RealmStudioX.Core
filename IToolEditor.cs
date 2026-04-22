using RealmStudioShapeRenderingLib;
using SkiaSharp;

namespace RealmStudioX.Core
{
    public interface IToolEditor
    {
        void Activate();
        void Deactivate();

        void OnMouseDown(SKPoint worldPos, EditorMouseButton button);
        void OnMouseMove(SKPoint worldPos, EditorMouseButton button);
        void OnMouseUp(SKPoint worldPos, EditorMouseButton button);
        void OnMouseDoubleClick(SKPoint worldPos, EditorMouseButton button);

        void Cancel();

        void RenderOverlay(SKCanvas canvas, SKPoint world);
    }
}
