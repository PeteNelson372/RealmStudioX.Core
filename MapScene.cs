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

        public SKRect WorldBounds => new(0, 0, Map.MapWidth, Map.MapHeight);

        /******************************************************************************************************* 
        * LANDFORM CLIP PATH CALCULATION
        *******************************************************************************************************/

        private SKPath? _landClipCache = null;
        private bool _landClipPathModified = true;

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
                lf.FinalizeShapeGeometry(Map);
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
        * WATER SYSTEM CLIP PATH CALCULATION
        *******************************************************************************************************/

        private SKPath? _waterSystemClipCache = null;
        private bool _waterSystemClipPathModified = true;

        public SKPath GetWaterSystemClipPath()
        {
            if (!_waterSystemClipPathModified && _waterSystemClipCache != null)
            {
                return _waterSystemClipCache;
            }

            _waterSystemClipCache?.Dispose();
            _waterSystemClipCache = null;

            _waterSystemClipCache = new SKPath();

            foreach (var ws in Map.WaterSystems)
            {
                foreach (var wb in ws.WaterBodies)
                {
                    _waterSystemClipCache.AddPath(wb.HitPath);
                }

                _waterSystemClipCache.AddPath(ws.MergedGeometry);
            }

            _waterSystemClipPathModified = false;

            return _waterSystemClipCache;
        }

        public void MarkWaterSystemClipPathModified()
        {
            _waterSystemClipPathModified = true;
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

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);

            // render the ocean drawing layer
            MapLayer oceanDrawinglayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANDRAWINGLAYER);

            // DrawnMapComponents (including PaintedLines) are added to the layer tiles
            // so they are rendered via layer.Draw
            oceanDrawinglayer.ProcessPlacementQueue();
            oceanDrawinglayer.Draw(canvas, Camera.Viewport);
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

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.ABOVEOCEANGRIDLAYER && mg.GridEnabled)
                {
                    mg.Render(canvas);
                }
            }

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.BELOWSYMBOLSGRIDLAYER && mg.GridEnabled)
                {
                    mg.Render(canvas);
                }
            }

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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
                if (layer.Shapes[i] is MapGrid mg && mg.GridLayerIndex == MapBuilder.DEFAULTGRIDLAYER && mg.GridEnabled)
                {
                    mg.Render(canvas);
                }
            }

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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

            // process and render DrawnMapComponents
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);

            // render the land drawing layer
            MapLayer landDrawinglayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LANDDRAWINGLAYER);

            // DrawnMapComponents (including PaintedLines) are added to the layer tiles
            // so they are rendered via layer.Draw
            landDrawinglayer.ProcessPlacementQueue();

            SKPath landClipPath = this.GetLandClipPath();
            SKPath waterClipPath = this.GetWaterSystemClipPath();

            landDrawinglayer.Draw(canvas, Camera.Viewport, landClipPath, waterClipPath);
        }

        /******************************************************************************************************* 
        * WATER SYSTEM RENDERING
        *******************************************************************************************************/

        private void RenderWaterSystems(SKCanvas canvas)
        {
            SKPath landClipPath = this.GetLandClipPath();

            MapLayer waterlayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERLAYER);
            if (!waterlayer.ShowLayer)
            {
                return;
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(landClipPath);

                foreach (var waterSystem in Map.WaterSystems)
                {
                    waterSystem.Render(canvas);
                }

                // process and render DrawnMapComponents
                waterlayer.ProcessPlacementQueue();
                waterlayer.Draw(canvas, Camera.Viewport);

                MapLayer waterdrawinglayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERDRAWINGLAYER);

                // process and render DrawnMapComponents

                SKPath waterClipPath = this.GetWaterSystemClipPath();

                waterdrawinglayer.ProcessPlacementQueue();
                waterdrawinglayer.Draw(canvas, Camera.Viewport, landClipPath, waterClipPath);
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

                    pathLowerLayer.ProcessPlacementQueue();
                    pathLowerLayer.Draw(canvas, Camera.Viewport);
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

                    pathUpperLayer.ProcessPlacementQueue();
                    pathUpperLayer.Draw(canvas, Camera.Viewport);
                }
            }
        }

        private void RenderRegions(SKCanvas canvas)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.REGIONLAYER);

                if (regionLayer.ShowLayer)
                {
                    for (int i = 0; i < regionLayer.Shapes.Count; i++)
                    {
                        if (regionLayer.Shapes[i] is MapRegion mr)
                        {
                            mr.Render(canvas, null);
                        }
                    }

                    regionLayer.ProcessPlacementQueue();
                    regionLayer.Draw(canvas, Camera.Viewport);
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

            // symbols are added to tiles and the spatial index,
            // so they are rendered via layer.Draw
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

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
        }

        private void RenderOverlays(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OVERLAYLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapScale ms)
                {
                    // there should only ever be one map scale
                    ms.Render(canvas);
                }
            }

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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
                }
            }

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
        }

        private void RenderMeasure(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.MEASURELAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            for (int i = 0; i < layer.Shapes.Count; i++)
            {
                if (layer.Shapes[i] is MapMeasure mm)
                {
                    mm.Render(canvas);
                }
            }

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
        }

        private void RenderDrawingLayer(SKCanvas canvas)
        {
            MapLayer layer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.DRAWINGLAYER);

            if (!layer.ShowLayer)
            {
                return;
            }

            // DrawnMapComponents are added to the layer tiles
            // so they are rendered via layer.Draw
            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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
                }
            }

            layer.ProcessPlacementQueue();
            layer.Draw(canvas, Camera.Viewport);
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

            // shape selection
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
                        ms.IsTransformTarget = false;

                        if (RenderContext.State.CurrentDrawingMode == MapDrawingMode.ShapeSelect)
                        {
                            selectedComponent = ms;
                            canvas.DrawRect(ms.Bounds, PaintObjects.MapSymbolSelectPaint);
                        }
                        else
                        {
                            canvas.DrawRect(ms.Bounds, PaintObjects.MapSymbolSelectPaint);
                        }
                    }
                    else if (shape is MapLabel ml)
                    {
                        ml.IsTransformTarget = false;

                        if (RenderContext.State.CurrentDrawingMode == MapDrawingMode.ShapeSelect)
                        {
                            selectedComponent = ml;
                            canvas.DrawRect(ml.Bounds, PaintObjects.LabelSelectPaint);
                        }
                        else
                        {
                            canvas.DrawRect(ml.Bounds, PaintObjects.LabelSelectPaint);
                        }
                    }
                    else if (shape is PlacedMapBox pmb)
                    {
                        selectedComponent = pmb;
                        canvas.DrawRect(pmb.Bounds, PaintObjects.BoxSelectPaint);
                    }
                    else if (shape is IDrawnMapComponent dmc)
                    {
                        selectedComponent = (MapComponent2D)dmc;
                        canvas.DrawRect(((MapComponent2D)dmc).Bounds, PaintObjects.Shape2DSelectPaint);
                    }
                }
            }

            if (selectedComponent is MapSymbol symbol)
            {
                symbol.IsTransformTarget = true;
                _transformWidget.Target = symbol;
                _transformWidget.Render(canvas, Camera.Zoom);
            }
            else if (selectedComponent is MapLabel label)
            {
                label.IsTransformTarget = true;
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

                RenderRegions(canvas);

                RenderDefaultGridLayer(canvas);

                RenderBoxes(canvas);

                RenderLabels(canvas, _fontManager!);

                RenderOverlays(canvas);

                RenderFrame(canvas);

                RenderMeasure(canvas);

                RenderDrawingLayer(canvas);

                RenderVignette(canvas);

                foreach (var layer in Layers)
                {
                    if (!layer.ShowLayer)
                    {
                        continue;
                    }

                    RenderLayerShapeSelection(layer, canvas);
                }

                RenderWaterSystemSelection(canvas);
            }
        }

        public void RenderForExport(SKCanvas canvas)
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

                RenderRegions(canvas);

                RenderDefaultGridLayer(canvas);

                RenderBoxes(canvas);

                RenderLabels(canvas, _fontManager!);

                RenderOverlays(canvas);

                RenderFrame(canvas);

                RenderMeasure(canvas);

                RenderDrawingLayer(canvas);

                RenderVignette(canvas);
            }
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
