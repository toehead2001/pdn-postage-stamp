using System;
using System.Drawing;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;

namespace PostageStampEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
        public string Copyright => ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
        public string DisplayName => ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("http://forums.getpaint.net/index.php?showtopic=108935");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Postage Stamp")]
    public class PostageStampEffectPlugin : PropertyBasedEffect
    {
        private const string StaticName = "Postage Stamp";
        private static readonly Image StaticIcon = new Bitmap(typeof(PostageStampEffectPlugin), "PostageStamp.png");

        public PostageStampEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuNames.Render, EffectFlags.Configurable)
        {
        }

        private enum PropertyNames
        {
            Amount1,
            Amount2,
            Amount3,
            Amount4,
            Amount5,
            Amount6,
            Amount7,
            Amount8,
            Amount9,
            Amount10
        }

        private enum Amount4Options
        {
            Amount4Option1,
            Amount4Option2
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new DoubleProperty(PropertyNames.Amount1, 1, 0.5, 10));
            props.Add(new Int32Property(PropertyNames.Amount2, 13, 2, 200));
            props.Add(new Int32Property(PropertyNames.Amount3, 13, 2, 200));
            props.Add(StaticListChoiceProperty.CreateForEnum<Amount4Options>(PropertyNames.Amount4, 0, false));
            props.Add(new DoubleVectorProperty(PropertyNames.Amount5, Pair.Create(0.000, 0.000), Pair.Create(-1.0, -1.0), Pair.Create(+1.0, +1.0)));
            props.Add(new BooleanProperty(PropertyNames.Amount6, false));
            props.Add(new BooleanProperty(PropertyNames.Amount7, false));
            props.Add(new Int32Property(PropertyNames.Amount8, 12, 5, 20));
            props.Add(new Int32Property(PropertyNames.Amount9, ColorBgra.ToOpaqueInt32(Color.White), 0, 0xffffff));
            props.Add(new Int32Property(PropertyNames.Amount10, 255, 0, 255));

            List<PropertyCollectionRule> propRules = new List<PropertyCollectionRule>();
            propRules.Add(new ReadOnlyBoundToBooleanRule(PropertyNames.Amount8, PropertyNames.Amount7, true));
            propRules.Add(new ReadOnlyBoundToBooleanRule(PropertyNames.Amount9, PropertyNames.Amount7, true));
            propRules.Add(new ReadOnlyBoundToBooleanRule(PropertyNames.Amount10, PropertyNames.Amount7, true));

            return new PropertyCollection(props, propRules);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.DisplayName, "Scale");
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.SliderLargeChange, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.SliderSmallChange, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.UpDownIncrement, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.DecimalPlaces, 3);
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.DisplayName, "Horizontal Perforations");
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.DisplayName, "Vertical Perforations");
            configUI.SetPropertyControlValue(PropertyNames.Amount4, ControlInfoPropertyNames.DisplayName, "Perforation Type");
            PropertyControlInfo Amount4Control = configUI.FindControlForPropertyName(PropertyNames.Amount4);
            Amount4Control.SetValueDisplayName(Amount4Options.Amount4Option1, "Curved");
            Amount4Control.SetValueDisplayName(Amount4Options.Amount4Option2, "Straight");
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.DisplayName, "Position");
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.SliderSmallChangeX, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.SliderLargeChangeX, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.UpDownIncrementX, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.SliderSmallChangeY, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.SliderLargeChangeY, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.UpDownIncrementY, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.DecimalPlaces, 3);
            Rectangle selection5 = EnvironmentParameters.GetSelection(EnvironmentParameters.SourceSurface.Bounds).GetBoundsInt();
            ImageResource imageResource5 = ImageResource.FromImage(EnvironmentParameters.SourceSurface.CreateAliasedBitmap(selection5));
            configUI.SetPropertyControlValue(PropertyNames.Amount5, ControlInfoPropertyNames.StaticImageUnderlay, imageResource5);
            configUI.SetPropertyControlValue(PropertyNames.Amount6, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Amount6, ControlInfoPropertyNames.Description, "Use Outline");
            configUI.SetPropertyControlValue(PropertyNames.Amount7, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Amount7, ControlInfoPropertyNames.Description, "Use Mat");
            configUI.SetPropertyControlValue(PropertyNames.Amount8, ControlInfoPropertyNames.DisplayName, "Mat Size");
            configUI.SetPropertyControlValue(PropertyNames.Amount9, ControlInfoPropertyNames.DisplayName, "Mat Color");
            configUI.SetPropertyControlType(PropertyNames.Amount9, PropertyControlType.ColorWheel);
            configUI.SetPropertyControlValue(PropertyNames.Amount10, ControlInfoPropertyNames.DisplayName, "");
            configUI.SetPropertyControlValue(PropertyNames.Amount10, ControlInfoPropertyNames.ControlColors, new ColorBgra[] { ColorBgra.White, ColorBgra.Black });

            return configUI;
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Amount1 = newToken.GetProperty<DoubleProperty>(PropertyNames.Amount1).Value;
            Amount2 = newToken.GetProperty<Int32Property>(PropertyNames.Amount2).Value;
            Amount3 = newToken.GetProperty<Int32Property>(PropertyNames.Amount3).Value;
            Amount4 = (byte)((int)newToken.GetProperty<StaticListChoiceProperty>(PropertyNames.Amount4).Value);
            Amount5 = newToken.GetProperty<DoubleVectorProperty>(PropertyNames.Amount5).Value;
            Amount7 = newToken.GetProperty<BooleanProperty>(PropertyNames.Amount7).Value;
            Amount8 = newToken.GetProperty<Int32Property>(PropertyNames.Amount8).Value;
            Amount9 = ColorBgra.FromOpaqueInt32(newToken.GetProperty<Int32Property>(PropertyNames.Amount9).Value);
            Amount10 = newToken.GetProperty<Int32Property>(PropertyNames.Amount10).Value;
            Amount6 = newToken.GetProperty<BooleanProperty>(PropertyNames.Amount6).Value;


            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();
            float centerX = ((selection.Right - selection.Left) / 2f) + selection.Left;
            float centerY = ((selection.Bottom - selection.Top) / 2f) + selection.Top;
            float amplitude = 4f * (float)Amount1;
            float waveLengthHalf = 5.4f * (float)Amount1;
            float cornerOffset = amplitude / 2f;
            int horPerforations = Amount2 * 2;
            int verPerforations = Amount3 * 2;
            float stampWidth = waveLengthHalf * horPerforations + amplitude + waveLengthHalf;
            float stampHeight = waveLengthHalf * verPerforations + amplitude + waveLengthHalf;
            float offsetX = centerX - stampWidth / 2f + (float)Amount5.First * (selection.Width / 2f - stampWidth / 2f - 1f);
            float offsetY = centerY - stampHeight / 2f + (float)Amount5.Second * (selection.Height / 2f - stampHeight / 2f - 1f);
            float tension = (Amount4 == 0) ? 0.75f : 0f;


            // Top
            PointF[] topPoints = new PointF[horPerforations + 2];
            topPoints[0] = new PointF(offsetX + cornerOffset, offsetY + cornerOffset);
            for (int i = 1; i < horPerforations; i += 2)
            {
                topPoints[i] = new PointF(i * waveLengthHalf + offsetX + cornerOffset, offsetY + amplitude);
                topPoints[i + 1] = new PointF((i + 1) * waveLengthHalf + offsetX + cornerOffset, offsetY);
            }
            topPoints[horPerforations + 1] = new PointF((horPerforations + 1) * waveLengthHalf + offsetX + cornerOffset, offsetY + cornerOffset);

            // Bottom
            PointF[] bottomPoints = new PointF[horPerforations + 2];
            bottomPoints[0] = new PointF(offsetX + cornerOffset, offsetY + stampHeight - cornerOffset);
            for (int i = 1; i < horPerforations; i += 2)
            {
                bottomPoints[i] = new PointF(i * waveLengthHalf + offsetX + cornerOffset, offsetY + stampHeight);
                bottomPoints[i + 1] = new PointF((i + 1) * waveLengthHalf + offsetX + cornerOffset, offsetY + stampHeight - amplitude);
            }
            bottomPoints[horPerforations + 1] = new PointF((horPerforations + 1) * waveLengthHalf + offsetX + cornerOffset, offsetY + stampHeight - cornerOffset);
            Array.Reverse(bottomPoints);

            // Left
            PointF[] leftPoints = new PointF[verPerforations + 2];
            leftPoints[0] = new PointF(offsetX + cornerOffset, offsetY + cornerOffset);
            for (int i = 1; i < verPerforations; i += 2)
            {
                leftPoints[i] = new PointF(offsetX, i * waveLengthHalf + offsetY + cornerOffset);
                leftPoints[i + 1] = new PointF(offsetX + amplitude, (i + 1) * waveLengthHalf + offsetY + cornerOffset);
            }
            leftPoints[verPerforations + 1] = new PointF(offsetX + cornerOffset, (verPerforations + 1) * waveLengthHalf + offsetY + cornerOffset);
            Array.Reverse(leftPoints);

            // Right
            PointF[] rightPoints = new PointF[verPerforations + 2];
            rightPoints[0] = new PointF(offsetX + stampWidth - cornerOffset, offsetY + cornerOffset);
            for (int i = 1; i < verPerforations; i += 2)
            {
                rightPoints[i] = new PointF(offsetX + stampWidth - amplitude, i * waveLengthHalf + offsetY + cornerOffset);
                rightPoints[i + 1] = new PointF(offsetX + stampWidth, (i + 1) * waveLengthHalf + offsetY + cornerOffset);
            }
            rightPoints[verPerforations + 1] = new PointF(offsetX + stampWidth - cornerOffset, (verPerforations + 1) * waveLengthHalf + offsetY + cornerOffset);


            #region Stamp Surface
            if (stampSurface == null)
                stampSurface = new Surface(srcArgs.Surface.Size);

            stampSurface.CopySurface(srcArgs.Surface, selection.Location);

            using (Graphics stamp = new RenderArgs(stampSurface).Graphics)
            using (GraphicsPath stampOutline = new GraphicsPath())
            {
                stampOutline.AddCurve(topPoints, tension);
                stampOutline.AddCurve(rightPoints, tension);
                stampOutline.AddCurve(bottomPoints, tension);
                stampOutline.AddCurve(leftPoints, tension);

                // Draw Mat
                if (Amount7)
                {
                    using (Pen matPen = new Pen(Color.FromArgb(Amount10, Amount9), Amount8 * (float)Amount1 * 2f))
                    {
                        stamp.SmoothingMode = SmoothingMode.None;
                        stamp.SetClip(stampOutline);
                        stamp.DrawRectangle(matPen, offsetX, offsetY, stampWidth, stampHeight);
                        stamp.ResetClip();

                        matPen.Width = 1.0f;
                        stamp.SmoothingMode = SmoothingMode.AntiAlias;
                        stamp.DrawPath(matPen, stampOutline);
                    }
                }

                // Draw Outline
                if (Amount6)
                {
                    using (Pen stampPen = new Pen(Color.LightGray, 2f))
                    {
                        stampPen.StartCap = LineCap.Round;
                        stampPen.EndCap = LineCap.Round;

                        stamp.SmoothingMode = SmoothingMode.AntiAlias;
                        stamp.DrawPath(stampPen, stampOutline);
                    }
                }
            }
            #endregion

            #region Eraser Surface
            if (eraserSurface == null)
                eraserSurface = new Surface(srcArgs.Surface.Size);
            else
                eraserSurface.Clear(Color.Transparent);

            using (Graphics eraser = new RenderArgs(eraserSurface).Graphics)
            {
                eraser.SmoothingMode = SmoothingMode.AntiAlias;

                // Clear Pixels outside of Stamp
                using (GraphicsPath clearPath = new GraphicsPath())
                {
                    clearPath.AddRectangle(eraser.ClipBounds);
                    clearPath.AddCurve(topPoints, tension);
                    clearPath.AddCurve(rightPoints, tension);
                    clearPath.AddCurve(bottomPoints, tension);
                    clearPath.AddCurve(leftPoints, tension);
                    eraser.FillPath(Brushes.Black, clearPath);
                }
            }
            #endregion


            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Render(DstArgs.Surface, SrcArgs.Surface, renderRects[i]);
            }
        }

        #region UICode
        double Amount1 = 1; // [0.5,10] Scale
        int Amount2 = 13; // [2,50] Horizontal Perforations
        int Amount3 = 13; // [2,50] Vertical Perforations
        byte Amount4 = 0; // Perforation Type|Curved|Straight
        Pair<double, double> Amount5 = Pair.Create(0.000, 0.000); // Position
        bool Amount6 = false; // [0,1] Use Outline
        bool Amount7 = false; // [0,1] Use Mat
        int Amount8 = 12; // [5,20] Mat Size
        ColorBgra Amount9 = ColorBgra.FromBgr(255, 255, 255); // [White] Mat Color
        int Amount10 = 255; // [0,255]
        #endregion

        Surface eraserSurface, stampSurface;

        private void Render(Surface dst, Surface src, Rectangle rect)
        {
            ColorBgra stampPixel;

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                if (IsCancelRequested) return;
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    stampPixel = stampSurface[x, y];
                    stampPixel.A = Int32Util.ClampToByte(byte.MaxValue - eraserSurface[x, y].A);

                    dst[x, y] = stampPixel;
                }
            }

        }

    }
}
