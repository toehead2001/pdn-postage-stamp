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
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://forums.getpaint.net/index.php?showtopic=108935");
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
            Scale,
            HorPerfs,
            VerPerfs,
            PerfType,
            Position,
            Outline,
            Mat,
            MatSize,
            MatColor
        }

        private enum PerfType
        {
            Curved,
            Straight
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>
            {
                new DoubleProperty(PropertyNames.Scale, 1, 0.5, 10),
                new Int32Property(PropertyNames.HorPerfs, 13, 2, 200),
                new Int32Property(PropertyNames.VerPerfs, 13, 2, 200),
                StaticListChoiceProperty.CreateForEnum<PerfType>(PropertyNames.PerfType, 0, false),
                new DoubleVectorProperty(PropertyNames.Position, Pair.Create(0.0, 0.0), Pair.Create(-1.0, -1.0), Pair.Create(+1.0, +1.0)),
                new BooleanProperty(PropertyNames.Outline, false),
                new BooleanProperty(PropertyNames.Mat, false),
                new Int32Property(PropertyNames.MatSize, 12, 5, 20),
                new Int32Property(PropertyNames.MatColor, unchecked((int)ColorBgra.White.Bgra), int.MinValue, int.MaxValue)
            };

            List<PropertyCollectionRule> propRules = new List<PropertyCollectionRule>
            {
                new ReadOnlyBoundToBooleanRule(PropertyNames.MatSize, PropertyNames.Mat, true),
                new ReadOnlyBoundToBooleanRule(PropertyNames.MatColor, PropertyNames.Mat, true)
            };

            return new PropertyCollection(props, propRules);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(PropertyNames.Scale, ControlInfoPropertyNames.DisplayName, "Scale");
            configUI.SetPropertyControlValue(PropertyNames.Scale, ControlInfoPropertyNames.SliderLargeChange, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Scale, ControlInfoPropertyNames.SliderSmallChange, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Scale, ControlInfoPropertyNames.UpDownIncrement, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Scale, ControlInfoPropertyNames.DecimalPlaces, 3);
            configUI.SetPropertyControlValue(PropertyNames.HorPerfs, ControlInfoPropertyNames.DisplayName, "Horizontal Perforations");
            configUI.SetPropertyControlValue(PropertyNames.VerPerfs, ControlInfoPropertyNames.DisplayName, "Vertical Perforations");
            configUI.SetPropertyControlValue(PropertyNames.PerfType, ControlInfoPropertyNames.DisplayName, "Perforation Type");
            PropertyControlInfo perfTypeControl = configUI.FindControlForPropertyName(PropertyNames.PerfType);
            perfTypeControl.SetValueDisplayName(PerfType.Curved, "Curved");
            perfTypeControl.SetValueDisplayName(PerfType.Straight, "Straight");
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.DisplayName, "Position");
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.SliderSmallChangeX, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.SliderLargeChangeX, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.UpDownIncrementX, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.SliderSmallChangeY, 0.05);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.SliderLargeChangeY, 0.25);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.UpDownIncrementY, 0.01);
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.DecimalPlaces, 3);
            Rectangle selBounds = EnvironmentParameters.GetSelection(EnvironmentParameters.SourceSurface.Bounds).GetBoundsInt();
            ImageResource selImage = ImageResource.FromImage(EnvironmentParameters.SourceSurface.CreateAliasedBitmap(selBounds));
            configUI.SetPropertyControlValue(PropertyNames.Position, ControlInfoPropertyNames.StaticImageUnderlay, selImage);
            configUI.SetPropertyControlValue(PropertyNames.Outline, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Outline, ControlInfoPropertyNames.Description, "Use Outline");
            configUI.SetPropertyControlValue(PropertyNames.Mat, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Mat, ControlInfoPropertyNames.Description, "Use Mat");
            configUI.SetPropertyControlValue(PropertyNames.MatSize, ControlInfoPropertyNames.DisplayName, "Mat Size");
            configUI.SetPropertyControlValue(PropertyNames.MatColor, ControlInfoPropertyNames.DisplayName, "Mat Color");
            configUI.SetPropertyControlType(PropertyNames.MatColor, PropertyControlType.ColorWheel);

            return configUI;
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            double scale = newToken.GetProperty<DoubleProperty>(PropertyNames.Scale).Value;
            int horPerfs = newToken.GetProperty<Int32Property>(PropertyNames.HorPerfs).Value;
            int verPerfs = newToken.GetProperty<Int32Property>(PropertyNames.VerPerfs).Value;
            int perfType = (int)newToken.GetProperty<StaticListChoiceProperty>(PropertyNames.PerfType).Value;
            Pair<double, double> position = newToken.GetProperty<DoubleVectorProperty>(PropertyNames.Position).Value;
            bool mat = newToken.GetProperty<BooleanProperty>(PropertyNames.Mat).Value;
            int matSize = newToken.GetProperty<Int32Property>(PropertyNames.MatSize).Value;
            ColorBgra matColor = ColorBgra.FromUInt32(unchecked((uint)newToken.GetProperty<Int32Property>(PropertyNames.MatColor).Value));
            bool outline = newToken.GetProperty<BooleanProperty>(PropertyNames.Outline).Value;

            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();
            float centerX = ((selection.Right - selection.Left) / 2f) + selection.Left;
            float centerY = ((selection.Bottom - selection.Top) / 2f) + selection.Top;
            float amplitude = 4f * (float)scale;
            float waveLengthHalf = 5.4f * (float)scale;
            float cornerOffset = amplitude / 2f;
            int horPerforations = horPerfs * 2;
            int verPerforations = verPerfs * 2;
            float stampWidth = waveLengthHalf * horPerforations + amplitude + waveLengthHalf;
            float stampHeight = waveLengthHalf * verPerforations + amplitude + waveLengthHalf;
            float offsetX = centerX - stampWidth / 2f + (float)position.First * (selection.Width / 2f - stampWidth / 2f - 1f);
            float offsetY = centerY - stampHeight / 2f + (float)position.Second * (selection.Height / 2f - stampHeight / 2f - 1f);
            float tension = (perfType == 0) ? 0.75f : 0f;

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
            {
                stampSurface = new Surface(srcArgs.Surface.Size);
            }

            stampSurface.CopySurface(srcArgs.Surface, selection.Location);

            using (Graphics stamp = new RenderArgs(stampSurface).Graphics)
            using (GraphicsPath stampOutline = new GraphicsPath())
            {
                stampOutline.AddCurve(topPoints, tension);
                stampOutline.AddCurve(rightPoints, tension);
                stampOutline.AddCurve(bottomPoints, tension);
                stampOutline.AddCurve(leftPoints, tension);

                // Draw Mat
                if (mat)
                {
                    using (Pen matPen = new Pen(matColor, matSize * (float)scale * 2f))
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
                if (outline)
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

        private Surface eraserSurface, stampSurface;

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

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                eraserSurface?.Dispose();
                stampSurface?.Dispose();
            }

            base.OnDispose(disposing);
        }
    }
}
