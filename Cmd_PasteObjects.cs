using RealmStudioShapeRenderingLib;
using SkiaSharp;
using System.Diagnostics;
using System.Xml.Serialization;

namespace RealmStudioX.Core
{
    public sealed class Cmd_PasteObjects : IUndoableCommand
    {
        private readonly RealmStudioMap _map;
        private readonly Clipboard _clipboard;
        private readonly SKPoint _pastePoint;

        private List<ShapeReference> _pastedObjects = [];
        private bool _hasExecuted;
        private bool disposedValue;

        public Cmd_PasteObjects(
            RealmStudioMap map,
            Clipboard clipboard,
            SKPoint pastePoint)
        {
            _map = map;
            _clipboard = clipboard;
            _pastePoint = pastePoint;
        }

        public List<ShapeReference> PastedObjects => _pastedObjects;

        public void Execute()
        {
            if (!_hasExecuted)
            {
                _pastedObjects = PasteObjects();
                _hasExecuted = true;
            }
            else
            {
                // Redo
                AddPastedObjects();
            }
        }

        public void Undo()
        {
            RemovePastedObjects();
        }

        private List<ShapeReference> PasteObjects()
        {
            List<ShapeReference> newObjects = [];
            List<MapLayer> layersToUpdate = [];

            foreach (ClipboardItem item in _clipboard.Items)
            {
                if (string.IsNullOrEmpty(item.MapLayerId))
                    continue;

                ISelectable shape =
                    (ISelectable)DeserializeObject(
                        item.ObjectType,
                        item.SerializedObject);

                if (shape is not MapComponent2D component)
                    continue;

                component.RegenerateIds();

                if (shape is MapSymbol ms)
                {
                    ms.Location = new SKPoint(
                        _pastePoint.X - item.Offset.X,
                        _pastePoint.Y - item.Offset.Y);

                    ms.UpdateBounds();
                }
                else if (shape is MapPath mp)
                {
                    List<SKPoint> newControlPoints = [];
                    SKPoint anchor = _clipboard.Anchor;

                    float deltaX = _pastePoint.X - anchor.X;
                    float deltaY = _pastePoint.Y - anchor.Y;

                    foreach (SKPoint p in mp.ControlPoints)
                    {
                        newControlPoints.Add(new SKPoint(
                            p.X + deltaX,
                            p.Y + deltaY));
                    }

                    mp.ControlPoints = [.. newControlPoints];

                    mp.FinalizeShapeGeometry(_map);
                }
                else if (shape is IDrawnMapComponent &&
                         shape is IRectangularShape irs)
                {
                    SKPoint anchor = _clipboard.Anchor;

                    float deltaX = _pastePoint.X - anchor.X;
                    float deltaY = _pastePoint.Y - anchor.Y;

                    irs.TopLeft = new SKPoint(
                        irs.TopLeft.X + deltaX,
                        irs.TopLeft.Y + deltaY);

                    irs.BottomRight = new SKPoint(
                        irs.BottomRight.X + deltaX, irs.BottomRight.Y + deltaY);
                }
                else if (shape is IDrawnMapComponent &&
                         shape is ICenterRadiusShape icrs)
                {
                    int deltaX =
                        (int)(_pastePoint.X - item.Offset.X);

                    int deltaY =
                        (int)(_pastePoint.Y - item.Offset.Y);

                    icrs.Center = new SKPoint(deltaX, deltaY);
                }
                else if (shape is IDrawnMapComponent &&
                         shape is IPointListShape ipls)
                {
                    SKPoint anchor = _clipboard.Anchor;

                    float deltaX = _pastePoint.X - anchor.X;
                    float deltaY = _pastePoint.Y - anchor.Y;

                    ipls.Points = [.. ipls.Points
                        .Select(p => new SKPoint(
                            p.X + deltaX,
                            p.Y + deltaY))];
                }

                MapLayer? destinationLayer = MapBuilder.GetMapLayerById(_map, item.MapLayerId);

                if (destinationLayer == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to find map layer '{item.MapLayerId}' " +
                        "while pasting an object.");
                }

                destinationLayer.Add(component);

                newObjects.Add(new ShapeReference
                {
                    ReferencedShape = component,
                    ShapeLayer = destinationLayer
                });

                if (!layersToUpdate.Contains(destinationLayer))
                {
                    layersToUpdate.Add(destinationLayer);
                }
            }

            foreach (MapLayer layer in layersToUpdate)
            {
                layer.InvalidateAllTiles();
                layer.RebuildIndexes();
            }

            return newObjects;
        }

        private void RemovePastedObjects()
        {
            List<MapLayer> layersToUpdate = [];

            foreach (ShapeReference reference in _pastedObjects)
            {
                if (reference.ShapeLayer == null ||
                    reference.ReferencedShape is not MapComponent2D shape)
                {
                    continue;
                }

                reference.ShapeLayer.Remove(shape);
                shape.IsSelected = false;

                if (!layersToUpdate.Contains(reference.ShapeLayer))
                {
                    layersToUpdate.Add(reference.ShapeLayer);
                }
            }

            foreach (MapLayer layer in layersToUpdate)
            {
                layer.InvalidateAllTiles();
                layer.RebuildIndexes();
            }
        }

        private void AddPastedObjects()
        {
            List<MapLayer> layersToUpdate = [];

            foreach (ShapeReference reference in _pastedObjects)
            {
                if (reference.ShapeLayer == null ||
                    reference.ReferencedShape is not MapComponent2D shape)
                {
                    continue;
                }

                reference.ShapeLayer.Add(shape);

                if (!layersToUpdate.Contains(reference.ShapeLayer))
                {
                    layersToUpdate.Add(reference.ShapeLayer);
                }
            }

            foreach (MapLayer layer in layersToUpdate)
            {
                layer.RebuildIndexes();
            }
        }

        private static object DeserializeObject(Type objectType, string xml)
        {
            XmlSerializer serializer = new(objectType);

            serializer.UnknownNode += Serializer_UnknownNode;
            serializer.UnknownAttribute += Serializer_UnknownAttribute;

            using StringReader reader = new(xml);

            return serializer.Deserialize(reader)!;
        }

        private static void Serializer_UnknownNode(object? sender, XmlNodeEventArgs e)
        {
            Debug.WriteLine("Exception on Load. Unknown Node: " + e.Name + "\t" + e.Text);
        }

        private static void Serializer_UnknownAttribute(object? sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Debug.WriteLine("Exception on Load. Unknown Attribute: " + attr.Name + "\t" + attr.Value);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_PasteObjects()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
