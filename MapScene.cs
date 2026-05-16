using RealmStudioShapeRenderingLib;
using SkiaSharp;

namespace RealmStudioX.Core
{
    public sealed class MapScene : IDisposable
    {
        public event Action? SceneChanged;

        private SKPicture? _backgroundCache;
        private bool _backgroundModified = true;
        private SKImage? _backgroundTexture;

        private SKPicture? _oceanTextureCache;
        private bool _oceanTextureModified = true;
        private SKImage? _oceanTexture;

        private bool disposedValue;

        private readonly OceanShorelineSettings _oceanShorelineSettings = new();

        public RealmStudioMap Map { get; }
        public Camera2D Camera { get; }

        public IReadOnlyList<MapLayer> Layers => Map.MapLayers;

        private RenderContext _renderContext = null!;

        private FontManager? _fontManager;

        private readonly TransformWidget _transformWidget = new();
        public TransformWidget TransformWidget => _transformWidget;

        public RenderContext RenderContext
        {
            get => _renderContext;
            set
            {
                if (_renderContext != null)
                {
                    throw new InvalidOperationException("RenderContext can only be set once.");
                }
                _renderContext = value ?? throw new ArgumentNullException(nameof(value));
            }
        }   

        public MapScene(RealmStudioMap map, FontManager fontManager)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Map.Validate();
            Camera = new Camera2D();

            Camera.ViewChanged += () => SceneChanged?.Invoke();

            _fontManager = fontManager ?? throw new ArgumentNullException(nameof(fontManager));
        }

        public SKRect WorldBounds =>
            new(
                0, 0,
                Map.MapWidth,
                Map.MapHeight
            );

        /******************************************************************************************************* 
        * LANDFORM CLIP PATH CALCULATION
        *******************************************************************************************************/

        private SKPath? _landClipCache;
        private bool _landClipPathModified;

        public SKPath GetLandClipPath()
        {
            if (!_landClipPathModified && _landClipCache != null)
            {
                return _landClipCache;
            }

            _landClipCache?.Dispose();
            _landClipCache = null;

            _landClipCache = new SKPath();

            var landLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LANDFORMLAYER);

            foreach (var lf in landLayer.Shapes.OfType<Landform>())
            {
                _landClipCache.AddPath(lf.HitPath);
            }

            _landClipPathModified = false;

            return _landClipCache;
        }

        public void MarkLandClipPathModified()
        {
            _landClipPathModified = true; 
        }

        /******************************************************************************************************* 
        * BACKGROUND RENDERING
        *******************************************************************************************************/
        private void RenderBackground(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            if (_backgroundModified)
            {
                RebuildBackgroundCache();
            }

            if (_backgroundCache != null)
            {
                canvas.DrawPicture(_backgroundCache);
            }
        }

        public void MarkBackgroundModified()
        {
            _backgroundModified = true;
        }

        public void SetBackgroundTexture(SKImage? backgroundTexture)
        {
            _backgroundTexture = backgroundTexture;
            _backgroundModified = true;
        }

        private void RebuildBackgroundCache()
        {
            _backgroundCache?.Dispose();
            _backgroundCache = null;

            if (_backgroundTexture == null)
                return;

            var recorder = new SKPictureRecorder();
            var canvas = recorder.BeginRecording(
                new SKRect(0, 0, Map.MapWidth, Map.MapHeight));

            DrawBackground(canvas);

            _backgroundCache = recorder.EndRecording();
            _backgroundModified = false;
        }

        private void DrawBackground(SKCanvas canvas)
        {
            if (_backgroundTexture == null)
                return;

            var settings = Map.Background;

            float scale = settings.Scale;

            var tileModeX = settings.Mirror
                ? SKShaderTileMode.Mirror
                : SKShaderTileMode.Repeat;

            var tileModeY = tileModeX;

            var matrix = SKMatrix.CreateScale(scale, scale);

            using var shader = SKShader.CreateImage(
                _backgroundTexture,
                tileModeX,
                tileModeY,
                matrix);

            using var paint = new SKPaint
            {
                Shader = shader
            };

            canvas.DrawRect(
                new SKRect(0, 0, Map.MapWidth, Map.MapHeight),
                paint);
        }

        /******************************************************************************************************* 
        * OCEAN RENDERING
        *******************************************************************************************************/
        private void RenderOcean(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            if (_oceanTextureModified)
            {
                RebuildOceanTextureCache();
            }

            if (_oceanTextureCache != null)
            {
                canvas.DrawPicture(_oceanTextureCache);
            }
        }

        public void MarkOceanTextureModified()
        {
            _oceanTextureModified = true;
        }

        private void DrawOceanOverlay(SKCanvas canvas)
        {
            var settings = Map.Ocean;

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = settings.OverlayColor   // opacity to determined by the overlay color alpha channel
            };

            canvas.DrawRect(
                new SKRect(0, 0, Map.MapWidth, Map.MapHeight),
                paint);
        }

        public void SetOceanTexture(SKImage? oceanTexture)
        {
            _oceanTexture = oceanTexture;
            _oceanTextureModified = true;
        }

        private void RebuildOceanTextureCache()
        {
            var _oceanSettings = Map.Ocean;

            _oceanTextureCache?.Dispose();
            _oceanTextureCache = null;

            var recorder = new SKPictureRecorder();
            var canvas = recorder.BeginRecording(
                new SKRect(0, 0, Map.MapWidth, Map.MapHeight));

            DrawOceanTexture(canvas);

            if (_oceanSettings.ColorOverlayEnabled)
            {
                DrawOceanOverlay(canvas);
            }

            _oceanTextureCache = recorder.EndRecording();
            _oceanTextureModified = false;
        }

        private void DrawOceanTexture(SKCanvas canvas)
        {
            if (_oceanTexture == null)
                return;

            var _oceanSettings = Map.Ocean;

            if (string.IsNullOrEmpty(_oceanSettings.TextureId))
                return;

            float scale = _oceanSettings.Scale;

            var tileModeX = _oceanSettings.Mirror
                ? SKShaderTileMode.Mirror
                : SKShaderTileMode.Repeat;

            var tileModeY = tileModeX;

            var matrix = SKMatrix.CreateScale(scale, scale);

            using var shader = SKShader.CreateImage(
                _oceanTexture,
                tileModeX,
                tileModeY,
                matrix);

            using var paint = new SKPaint
            {
                Shader = shader,
                Color = SKColors.White.WithAlpha(
                    (byte)(255 * _oceanSettings.TextureOpacity)),
                IsAntialias = false
            };

            canvas.DrawRect(
                new SKRect(0, 0, Map.MapWidth, Map.MapHeight),
                paint);
        }


        /******************************************************************************************************* 
        * OCEAN SHORELINE RENDERING
        *******************************************************************************************************/
        private void RenderOceanShorelineBlend(SKCanvas canvas)
        {
            MapLayer oceanlayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (!Map.Ocean.EnableCoastlineBlur || !oceanlayer.ShowLayer)
            {
                return;
            }

            float blurRadius = _oceanShorelineSettings.ShoreDepth;

            MapLayer landformlayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LANDFORMLAYER);

            foreach (var shape in landformlayer.Shapes)
            {
                if (shape is not Landform lf)
                    continue;

                // render the ocean shoreline blend using the landform coastline color
                // this blends it seamlessly with the landform coastline effect
                RenderShoreForLandform(canvas, lf, blurRadius, lf.Coastline.CoastlineColor);
            }
        }

        private static void RenderShoreForLandform(
            SKCanvas canvas,
            Landform lf,
            float blurRadius,
            SKColor shallowColor)
        {
            if (lf.HitPath.IsEmpty)
                return;

            canvas.Save();

            // Only affect ocean
            canvas.ClipPath(lf.HitPath, SKClipOperation.Difference, true);

            using var crisp = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                Color = shallowColor.WithAlpha(150),
                BlendMode = SKBlendMode.SoftLight,
                IsAntialias = true
            };

            canvas.DrawPath(lf.PerimeterPath, crisp);

            // Create blurred layer
            using var blurFilter = SKImageFilter.CreateBlur(
                blurRadius,
                blurRadius,
                SKShaderTileMode.Clamp);

            using var layerPaint = new SKPaint
            {
                ImageFilter = blurFilter
            };

            canvas.SaveLayer(layerPaint);

            using var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = blurRadius * 0.8f,
                Color = shallowColor,
                BlendMode = SKBlendMode.Overlay,
                IsAntialias = true
            };

            canvas.DrawPath(lf.PerimeterPath, strokePaint);

            canvas.Restore(); // apply blur filter
            canvas.Restore();
        }

        /******************************************************************************************************* 
        * WINDROSE RENDERING
        *******************************************************************************************************/

        private void RenderWindroses(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WINDROSELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapWindrose mw)
                {
                    mw.Render(canvas);
                }
            }

        }

        /******************************************************************************************************* 
        * GRID RENDERING
        *******************************************************************************************************/

        private void RenderAboveOceanGridLayer(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.ABOVEOCEANGRIDLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.ABOVEOCEANGRIDLAYER)
                {
                    mg.Render(canvas);
                    break; // there should only be one grid
                }
            }
        }

        private void RenderBelowSymbolsGridLayer(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.BELOWSYMBOLSGRIDLAYER)
                {
                    mg.Render(canvas);
                    break;
                }
            }
        }

        private void RenderDefaultGridLayer(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.DEFAULTGRIDLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.DEFAULTGRIDLAYER)
                {
                    mg.Render(canvas);
                    break;
                }
            }
        }

        /******************************************************************************************************* 
        * LANDFORM RENDERING
        *******************************************************************************************************/

        private void RenderLandformCoastlines(SKCanvas canvas)
        {
            // TODO: render coastlines on landform coastline layer
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LANDFORMLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is Landform lf)
                {
                    if (lf.IsInteractive)
                    {
                        lf.RenderCoastlineInteractive(canvas);
                    }
                    else
                    {
                        lf.RenderCoastlineFinal(canvas);
                    }
                }
            }
        }

        private void RenderLandforms(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LANDFORMLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is Landform lf)
                {
                    if (lf.IsInteractive)
                    {
                        lf.RenderInteriorFast(canvas);
                    }
                    else
                    {
                        lf.RenderInteriorFinal(canvas);
                    }
                }
            }

        }

        /******************************************************************************************************* 
        * WATER SYSTEM RENDERING
        *******************************************************************************************************/

        private void RenderWaterSystems(SKCanvas canvas)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(GetLandClipPath());

                foreach (var waterSystem in Map.WaterSystems)
                {
                    waterSystem.Render(canvas);
                }
            }
        }

        /******************************************************************************************************* 
        * MAP PATH RENDERING
        *******************************************************************************************************/

        private void RenderLowerMapPaths(SKCanvas canvas)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(GetLandClipPath());

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER);
                
                if (pathLowerLayer.ShowLayer)
                {
                    for (int i = 0; i < pathLowerLayer.Shapes.Count; i++)
                    {
                        if (pathLowerLayer.Shapes[i] is MapPath mp)
                        {
                            mp.Render(canvas, null);
                        }
                    }
                }
            }
        }

        private void RenderUpperMapPaths(SKCanvas canvas)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(GetLandClipPath());

                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER);

                if (pathUpperLayer.ShowLayer)
                {
                    for (int i = 0; i < pathUpperLayer.Shapes.Count; i++)
                    {
                        if (pathUpperLayer.Shapes[i] is MapPath mp)
                        {
                            mp.Render(canvas, null);
                        }
                    }
                }
            }
        }

        private void RenderSymbols(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            layer.ProcessPlacementQueue();

            layer.Draw(canvas, Camera.Viewport);
        }

        private void RenderBoxes(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BOXLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is PlacedMapBox pmb)
                {
                    pmb.Render(canvas);
                }
            }
        }

        private void RenderLabels(SKCanvas canvas, FontManager fontManager)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LABELLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapLabel ml)
                {
                    ml.Render(canvas, fontManager);
                }
            }
        }

        private void RenderFrame(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.FRAMELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is PlacedMapFrame pmf)
                {
                    // there should only ever be one frame
                    pmf.Render(canvas);
                    break;
                }
            }
        }

        private void RenderVignette(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.VIGNETTELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapVignette vignette)
                {
                    // there should only ever be one vignette
                    vignette.Bounds = WorldBounds;
                    vignette.Render(canvas);
                    break;
                }
            }
        }

        /******************************************************************************************************* 
        * OTHER SHAPE RENDERING (will be divided as features as added)
        *******************************************************************************************************/
        private static void RenderShapes(MapLayer layer, SKCanvas canvas)
        {
            // PASS 3 — all other shapes
            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is not Landform && layer.Shapes[i] is not WaterBody && layer.Shapes[i] is not MapPath)
                {
                    MapComponent2D shape = layer.Shapes[i];
                    shape.Render(canvas);
                }
            }
        }

        /******************************************************************************************************* 
        * RENDER SHAPE SELECTION
        *******************************************************************************************************/

        private void RenderLayerShapeSelection(MapLayer layer, SKCanvas canvas)
        {
            MapComponent2D? selectedComponent = null;

            // PASS 4 — shape selection
            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i].IsSelected)
                {
                    var shape = layer.Shapes[i];

                    if (!shape.IsSelected)
                    {
                        continue;
                    }

                    if (shape is Landform lf)
                    {
                        selectedComponent = lf;
                        canvas.DrawRect(lf.Bounds, PaintObjects.LandformSelectPaint);
                    }
                    else if (shape is MapPath mp)
                    {
                        selectedComponent = mp;
                        canvas.DrawRect(mp.Bounds, PaintObjects.MapPathSelectPaint);
                        mp.Editor.RenderEditableHandles(canvas, Camera.Zoom);
                    }
                    else if (shape is MapSymbol ms)
                    {
                        selectedComponent = ms;
                    }
                    else if (shape is MapLabel ml)
                    {
                        selectedComponent = ml;
                    }
                    else if (shape is PlacedMapBox pmb)
                    {
                        selectedComponent = pmb;
                    }
                    else
                    {
                        selectedComponent = layer.Shapes[i];
                        canvas.DrawRect(layer.Shapes[i].Bounds, PaintObjects.Shape2DSelectPaint);
                    }
                }
            }

            if (selectedComponent is MapSymbol symbol)
            {
                _transformWidget.Target = symbol;
                _transformWidget.Render(canvas, Camera.Zoom);
            }
            else if (selectedComponent is MapLabel label)
            {
                _transformWidget.Target = label;
                _transformWidget.Render(canvas, Camera.Zoom);
            }
            else if (selectedComponent is PlacedMapBox box)
            {
                _transformWidget.Target = box;
                _transformWidget.Render(canvas, Camera.Zoom);
            }
            else
            {
                _transformWidget.Target = null;
            }
        }

        private void RenderWaterSystemSelection(SKCanvas canvas)
        {
            // PASS 5 — water system and water body selection
            for (int i = 0; i < Map.WaterSystems.Count; i++)
            {
                if (Map.WaterSystems[i].IsSelected)
                {
                    canvas.DrawRect(Map.WaterSystems[i].Bounds, PaintObjects.WaterSystemSelectPaint);
                }

                for (int j = 0; j <  Map.WaterSystems[i].WaterBodies.Count; j++)
                {
                    if (Map.WaterSystems[i].WaterBodies.ElementAt(j).IsSelected)
                    {
                        if (Map.WaterSystems[i].WaterBodies.ElementAt(j) is River r)
                        {
                            canvas.DrawRect(Map.WaterSystems[i].WaterBodies.ElementAt(j).Bounds, PaintObjects.RiverSelectPaint);

                            r.Editor.RenderEditableHandles(canvas, Camera.Zoom);
                        }
                        else
                        {
                            canvas.DrawRect(Map.WaterSystems[i].WaterBodies.ElementAt(j).Bounds, PaintObjects.WaterFeatureSelectPaint);
                        }
                    }
                }
            }
        }

        /******************************************************************************************************* 
        * MAP SCENE RENDER
        *******************************************************************************************************/
        public void Render(SKCanvas canvas)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            ArgumentNullException.ThrowIfNull(RenderContext);

            canvas.Clear(SKColors.White);

            using (RenderContextScope.Begin(RenderContext))
            {
                RenderBackground(canvas);

                RenderOcean(canvas);

                RenderOceanShorelineBlend(canvas);

                RenderWindroses(canvas);

                RenderAboveOceanGridLayer(canvas);

                RenderLandformCoastlines(canvas);

                RenderLandforms(canvas);

                RenderWaterSystems(canvas);

                RenderBelowSymbolsGridLayer(canvas);

                RenderLowerMapPaths(canvas);

                RenderSymbols(canvas);

                RenderUpperMapPaths(canvas);

                RenderDefaultGridLayer(canvas);

                RenderBoxes(canvas);

                RenderLabels(canvas, _fontManager!);

                RenderFrame(canvas);

                RenderVignette(canvas);

                foreach (var layer in Layers)
                {
                    if (!layer.ShowLayer)
                    {
                        continue;
                    }

                    // this is commented out, because it can lead to
                    // shapes being rendered by the dedicated method for the shape
                    // and also by the generic Shape2D rendering, which can lead to confusion
                    //RenderShapes(layer, canvas);

                    RenderLayerShapeSelection(layer, canvas);
                }

                RenderWaterSystemSelection(canvas);
            }
        }

        /******************************************************************************************************* 
        * HIT TESTING
        *******************************************************************************************************/

        public List<ISelectable> HitTestAll(SKPoint worldPoint)
        {
            var hits = new List<ISelectable>();

            using (RenderContextScope.Begin(RenderContext))
            {
                foreach (var layer in Layers)
                {
                    if (!layer.ShowLayer)
                    {
                        continue;
                    }

                    for (int i = layer.Shapes.Count - 1; i >= 0; i--)
                    {
                        var shape = layer.Shapes[i];

                        if (shape is MapSymbol ms)
                        {
                            if (ms.HitTest(worldPoint))
                            {
                                hits.Add(ms);
                            }
                        }
                        else if (shape is MapLabel ml)
                        {
                            if (ml.HitTest(worldPoint))
                            {
                                hits.Add(ml);
                            }
                        }
                        else if (shape.HitTest(worldPoint))
                        {
                            hits.Add(shape);
                        }
                    }
                }

                foreach (var waterSystem in Map.WaterSystems)
                {
                    if (waterSystem.HitTest(worldPoint))
                    {
                        hits.Add(waterSystem);
                    }

                    foreach (var waterBody in waterSystem.WaterBodies)
                    {
                        if (waterBody.HitTest(worldPoint))
                        {
                            hits.Add(waterBody);
                        }
                    }
                }
            }

            return hits;
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
        // ~MapScene()
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
