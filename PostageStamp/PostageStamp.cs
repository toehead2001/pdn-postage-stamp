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
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Postage Stamp")]
    public class PostageStampEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "Postage Stamp";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return new Bitmap(typeof(PostageStampEffectPlugin), "PostageStamp.png");
            }
        }

        public static string SubmenuName
        {
            get
            {
                return SubmenuNames.Render;
            }
        }

        public PostageStampEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.Configurable)
        {
        }

        public enum PropertyNames
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

        public enum Amount4Options
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

            // Left
            PointF[] leftPoints = new PointF[verPerforations + 2];
            leftPoints[0] = new PointF(offsetX + cornerOffset, offsetY + cornerOffset);
            for (int i = 1; i < verPerforations; i += 2)
            {
                leftPoints[i] = new PointF(offsetX, i * waveLengthHalf + offsetY + cornerOffset);
                leftPoints[i + 1] = new PointF(offsetX + amplitude, (i + 1) * waveLengthHalf + offsetY + cornerOffset);
            }
            leftPoints[verPerforations + 1] = new PointF(offsetX + cornerOffset, (verPerforations + 1) * waveLengthHalf + offsetY + cornerOffset);

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
            else
                stampSurface.Clear(Color.Transparent);

            stampSurface.CopySurface(srcArgs.Surface, selection.Location);

            using (RenderArgs ra = new RenderArgs(stampSurface))
            {
                Graphics stamp = ra.Graphics;

                // Draw Mat
                if (Amount7)
                {
                    using (Pen matPen = new Pen(Color.FromArgb(Amount10, Amount9), Amount8 * (float)Amount1))
                    {
                        matPen.Alignment = PenAlignment.Inset;
                        stamp.DrawRectangle(matPen, offsetX, offsetY, stampWidth, stampHeight);
                    }
                }

                // Draw Outline
                if (Amount6)
                {
                    stamp.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Pen stampPen = new Pen(Color.FromArgb(255, Color.LightGray), 2f))
                    {
                        stampPen.StartCap = LineCap.Round;
                        stampPen.EndCap = LineCap.Round;

                        stamp.DrawCurve(stampPen, topPoints, tension);
                        stamp.DrawCurve(stampPen, bottomPoints, tension);
                        stamp.DrawCurve(stampPen, leftPoints, tension);
                        stamp.DrawCurve(stampPen, rightPoints, tension);
                    }
                }
            }
            #endregion

            #region Eraser Surface
            if (eraserSurface == null)
                eraserSurface = new Surface(srcArgs.Surface.Size);
            else
                eraserSurface.Clear(Color.Transparent);

            PointF[] edgeTopPoints = new PointF[4];
            edgeTopPoints[0] = new PointF(offsetX + stampWidth, offsetY + cornerOffset);
            edgeTopPoints[1] = new PointF(offsetX, offsetY - stampWidth);
            edgeTopPoints[2] = new PointF(offsetX + stampWidth, offsetY - stampWidth);
            edgeTopPoints[3] = new PointF(offsetX, offsetY);
            PointF[] eraserTopPoints = new PointF[topPoints.Length + edgeTopPoints.Length];
            topPoints.CopyTo(eraserTopPoints, 0);
            edgeTopPoints.CopyTo(eraserTopPoints, topPoints.Length);

            PointF[] edgeRightPoints = new PointF[4];
            edgeRightPoints[0] = new PointF(offsetX + stampWidth - cornerOffset, offsetY + stampHeight);
            edgeRightPoints[1] = new PointF(offsetX + stampWidth + stampHeight, offsetY);
            edgeRightPoints[2] = new PointF(offsetX + stampWidth + stampHeight, offsetY + stampHeight);
            edgeRightPoints[3] = new PointF(offsetX + stampWidth, offsetY);
            PointF[] eraserRightPoints = new PointF[rightPoints.Length + edgeRightPoints.Length];
            rightPoints.CopyTo(eraserRightPoints, 0);
            edgeRightPoints.CopyTo(eraserRightPoints, rightPoints.Length);

            PointF[] edgeBottomPoints = new PointF[4];
            edgeBottomPoints[0] = new PointF(offsetX + stampWidth, offsetY + stampHeight);
            edgeBottomPoints[1] = new PointF(offsetX, offsetY + stampHeight + stampWidth);
            edgeBottomPoints[2] = new PointF(offsetX + stampWidth, offsetY + stampHeight + stampWidth);
            edgeBottomPoints[3] = new PointF(offsetX + cornerOffset, offsetY + stampHeight);
            PointF[] eraserBottomPoints = new PointF[bottomPoints.Length + edgeBottomPoints.Length];
            bottomPoints.CopyTo(eraserBottomPoints, 0);
            edgeBottomPoints.CopyTo(eraserBottomPoints, bottomPoints.Length);

            PointF[] edgeLeftPoints = new PointF[4];
            edgeLeftPoints[0] = new PointF(offsetX, offsetY + stampHeight);
            edgeLeftPoints[1] = new PointF(offsetX - stampHeight, offsetY);
            edgeLeftPoints[2] = new PointF(offsetX - stampHeight, offsetY + stampHeight);
            edgeLeftPoints[3] = new PointF(offsetX + cornerOffset, offsetY);
            PointF[] eraserLeftPoints = new PointF[leftPoints.Length + edgeLeftPoints.Length];
            leftPoints.CopyTo(eraserLeftPoints, 0);
            edgeLeftPoints.CopyTo(eraserLeftPoints, leftPoints.Length);

            using (RenderArgs ra = new RenderArgs(eraserSurface))
            {
                Graphics eraser = ra.Graphics;

                using (Pen eraserPen = new Pen(Color.Black))
                {
                    eraserPen.Width = offsetY - selection.Top;
                    eraser.DrawLine(eraserPen, selection.Left, selection.Top + eraserPen.Width / 2f, selection.Right, selection.Top + eraserPen.Width / 2f);

                    eraserPen.Width = selection.Right - offsetX - stampWidth;
                    eraser.DrawLine(eraserPen, selection.Right - eraserPen.Width / 2f, selection.Top, selection.Right - eraserPen.Width / 2f, selection.Bottom);

                    eraserPen.Width = selection.Bottom - offsetY - stampHeight;
                    eraser.DrawLine(eraserPen, selection.Left, selection.Bottom - eraserPen.Width / 2f, selection.Right, selection.Bottom - eraserPen.Width / 2f);

                    eraserPen.Width = offsetX - selection.Left;
                    eraser.DrawLine(eraserPen, selection.Left + eraserPen.Width / 2f, selection.Top, selection.Left + eraserPen.Width / 2f, selection.Bottom);

                    eraser.SmoothingMode = SmoothingMode.AntiAlias;

                    eraserPen.Width = 2.5f * (float)Amount1;
                    eraser.DrawLine(eraserPen, offsetX - cornerOffset, offsetY + amplitude, offsetX + amplitude, offsetY - cornerOffset);
                    eraser.DrawLine(eraserPen, offsetX + stampWidth + cornerOffset, offsetY + amplitude, offsetX + stampWidth - amplitude, offsetY - cornerOffset);
                    eraser.DrawLine(eraserPen, offsetX - cornerOffset, offsetY + stampHeight - amplitude, offsetX + amplitude, offsetY + stampHeight + cornerOffset);
                    eraser.DrawLine(eraserPen, offsetX + stampWidth + cornerOffset, offsetY + stampHeight - amplitude, offsetX + stampWidth - amplitude, offsetY + stampHeight + cornerOffset);
                }

                using (Brush eraserBrush = new SolidBrush(Color.Black))
                {
                    eraser.SmoothingMode = SmoothingMode.None;

                    float block = 2f * (float)Amount1;
                    eraser.FillRectangle(eraserBrush, offsetX, offsetY, block, block);
                    eraser.FillRectangle(eraserBrush, offsetX + stampWidth - block, offsetY, block, block);
                    eraser.FillRectangle(eraserBrush, offsetX + stampWidth - block, offsetY + stampHeight - block, block, block);
                    eraser.FillRectangle(eraserBrush, offsetX, offsetY + stampHeight - block, block, block);

                    eraser.SmoothingMode = SmoothingMode.AntiAlias;

                    eraser.FillClosedCurve(eraserBrush, eraserTopPoints, FillMode.Alternate, tension);
                    eraser.FillClosedCurve(eraserBrush, eraserRightPoints, FillMode.Alternate, tension);
                    eraser.FillClosedCurve(eraserBrush, eraserBottomPoints, FillMode.Alternate, tension);
                    eraser.FillClosedCurve(eraserBrush, eraserLeftPoints, FillMode.Alternate, tension);
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

        void Render(Surface dst, Surface src, Rectangle rect)
        {
            ColorBgra stampPixel, eraserPixel;

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                if (IsCancelRequested) return;
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    eraserPixel = eraserSurface[x, y];
                    stampPixel = stampSurface[x, y];
                    stampPixel.A = Int32Util.ClampToByte(255 - eraserPixel.A);

                    dst[x, y] = stampPixel;
                }
            }

        }

    }
}
