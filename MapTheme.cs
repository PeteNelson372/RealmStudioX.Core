/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using RealmStudioShapeRenderingLib;
using RealmStudioX.WPF.EditorUtilities;
using SkiaSharp;
using System.Xml.Serialization;

namespace RealmStudioX.Core
{
    [XmlRoot("RealmStudioXMapTheme")]
    public class MapTheme
    {
        [XmlElement("ThemeName", IsNullable = false)]
        public string ThemeName { get; set; } = string.Empty;

        [XmlElement("IsDefaultTheme")]
        public bool IsDefaultTheme { get; set; } = false;

        [XmlElement("IsSystemTheme")]
        public bool IsSystemTheme { get; set; } = false;

        [XmlElement("BackgroundTextureId", IsNullable = true)]
        public string? BackgroundTextureId { get; set; }

        [XmlElement("BackgroundTextureScale")]
        public float BackgroundTextureScale { get; set; } = 1f;

        [XmlElement("MirrorBackgroundTexture")]
        public bool MirrorBackgroundTexture { get; set; }

        [XmlElement("OceanTextureId", IsNullable = true)]
        public string? OceanTextureId { get; set; }

        [XmlElement("OceanTextureScale")]
        public float OceanTextureScale { get; set; } = 1f;

        [XmlElement("MirrorOceanTexture")]
        public bool MirrorOceanTexture { get; set; } = false;

        [XmlElement("EnableCoastlineBlur")]
        public bool EnableCoastlineBlur { get; set; } = true;

        [XmlElement("OceanTextureOpacity")]
        public float OceanTextureOpacity { get; set; } = 1f;

        [XmlElement("OceanColorOverlayEnabled")]
        public bool OceanColorOverlayEnabled { get; set; }

        [XmlIgnore]
        public SKColor OceanOverlayColor { get; set; } = SKColors.Transparent;

        [XmlElement("OceanOverlayColor")]
        public string OceanOverlayColorXml
        {
            get => XmlColorConverter.Serialize(OceanOverlayColor);
            set => OceanOverlayColor = XmlColorConverter.Deserialize(value);
        }

        [XmlElement("UseLandformTextureBackground")]
        public bool UseLandformTextureBackground { get; set; } = true;

        [XmlIgnore]
        public SKColor LandformBackgroundColor { get; set; } = new(140, 180, 120);

        [XmlElement("LandformBackgroundColor")]
        public string LandformBackgroundColorXml
        {
            get => XmlColorConverter.Serialize(LandformBackgroundColor);
            set => LandformBackgroundColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor LandformOutlineColor { get; set; } = new(62, 55, 40);

        [XmlElement("LandformOutlineColor")]
        public string LandformOutlineColorXml
        {
            get => XmlColorConverter.Serialize(LandformOutlineColor);
            set => LandformOutlineColor = XmlColorConverter.Deserialize(value);
        }

        [XmlElement("LandformTextureId", IsNullable = true)]
        public string? LandformTextureId { get; set; }

        [XmlElement("LandformOutlineWidth")]
        public int LandformOutlineWidth { get; set; } = 2;

        [XmlElement("LandformShadingDepth")]
        public int LandformShadingDepth { get; set; } = 16;


        [XmlElement("CoastlineStyle")]
        public LandformCoastlineStyle CoastlineStyle { get; set; } = LandformCoastlineStyle.UniformBlend;

        [XmlElement("CoastlineEffectDistance")]
        public int CoastlineEffectDistance { get; set; } = 120;

        // Base color
        [XmlIgnore]
        public SKColor CoastlineColor { get; set; } = SKColor.Parse("#BB9CC3B7");

        [XmlElement("CoastlineColor")]
        public string CoastlineColorXml
        {
            get => XmlColorConverter.Serialize(CoastlineColor);
            set => CoastlineColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor ShorelineColor { get; set; } = SKColor.Parse("#A19076");

        [XmlElement("ShorelineColor")]
        public string ShorelineColorXml
        {
            get => XmlColorConverter.Serialize(ShorelineColor);
            set => ShorelineColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor DeepWaterColor { get; set; } = new SKColor(120, 180, 220, 255);

        [XmlElement("DeepWaterColor")]
        public string DeepWaterColorXml
        {
            get => XmlColorConverter.Serialize(DeepWaterColor);
            set => DeepWaterColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor ShallowWaterColor { get; set; } = new SKColor(30, 80, 140, 255);

        [XmlElement("ShallowWaterColor")]
        public string ShallowWaterColorXml
        {
            get => XmlColorConverter.Serialize(ShallowWaterColor);
            set => ShallowWaterColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor PathColor { get; set; } = SKColor.Parse("#4B311A");

        [XmlElement("PathColor")]
        public string PathColorXml
        {
            get => XmlColorConverter.Serialize(PathColor);
            set => PathColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public FontStyleModel LabelFont { get; set; } = new FontStyleModel();

        [XmlElement("LabelFontFamily")]
        public string LabelFontFamily { get; set; } = "Arial";

        [XmlElement("LabelFontSize")]
        public float LabelFontSize { get; set; } = 24f;

        [XmlElement("LabelFontBold")]
        public bool LabelFontBold { get; set; }

        [XmlElement("LabelFontItalic")]
        public bool LabelFontItalic { get; set; }

        [XmlIgnore]
        public SKColor LabelColor { get; set; } = SKColor.Parse("#3D351E");

        [XmlElement("LabelColor")]
        public string LabelColorXml
        {
            get => XmlColorConverter.Serialize(LabelColor);
            set => LabelColor = XmlColorConverter.Deserialize(value);
        }

        [XmlIgnore]
        public SKColor LabelOutlineColor { get; set; } = SKColor.Parse("#A1D6CAAB");

        [XmlElement("LabelOutlineColor")]
        public string LabelOutlineColorXml
        {
            get => XmlColorConverter.Serialize(LabelOutlineColor);
            set => LabelOutlineColor = XmlColorConverter.Deserialize(value);
        }

        [XmlElement("LabelOutlineWidth")]
        public float? LabelOutlineWidth { get; set; }
        
        [XmlIgnore]
        public SKColor LabelGlowColor { get; set; } = SKColors.White;

        [XmlElement("LabelGlowColor")]
        public string LabelGlowColorXml
        {
            get => XmlColorConverter.Serialize(LabelGlowColor);
            set => LabelGlowColor = XmlColorConverter.Deserialize(value);
        }

        [XmlElement("LabelGlowStrength")]
        public int LabelGlowStrength { get; set; }

        [XmlIgnore]
        public SKColor VignetteColor { get; set; } = SKColor.Parse("#C9977B");

        [XmlElement("VignetteColor")]
        public string VignetteColorXml
        {
            get => XmlColorConverter.Serialize(VignetteColor);
            set => VignetteColor = XmlColorConverter.Deserialize(value);
        }

        [XmlElement("VignetteStrength")]
        public int VignetteStrength { get; set; } = 148;
        
        [XmlElement("VignetteShape")]
        public VignetteShapeType VignetteShape { get; set; } = VignetteShapeType.Oval;

        [XmlIgnore]
        public SKColor[] CustomSymbolColors { get; set; } = new SKColor[3];

        [XmlArray("CustomSymbolColors")]
        [XmlArrayItem("SymbolColor")]
        public string[] CustomSymbolColorsXml
        {
            get => [.. CustomSymbolColors.Select(XmlColorConverter.Serialize)];

            set
            {
                if (value == null)
                {
                    CustomSymbolColors = [];
                    return;
                }

                CustomSymbolColors = [.. value.Select(XmlColorConverter.Deserialize)];
            }
        }

        [XmlArray("LabelPresets")]
        [XmlArrayItem("LabelPreset")]
        private List<LabelPreset> _labelPresets = [];
        public List<LabelPreset> LabelPresets
        {
            get { return _labelPresets; }
            set { _labelPresets.AddRange(value); }
        }
    }
}
