using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xaml;
using System.Xml;
using UAM.Optics.ColorScience;
using UAM.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum RandomizationMode
    {
        None,
        Circumference,
        Area,
        Surface,
        Volume
    }

    public class InteractiveDiagonalTransform : ShowStimuli
    {
        public static readonly DependencyProperty GamutBounceStepsProperty = DependencyProperty.Register(nameof(GamutBounceSteps), typeof(StudyStepCollection), typeof(InteractiveDiagonalTransform));

        public StudyStepCollection GamutBounceSteps
        {
            get { return (StudyStepCollection)GetValue(GamutBounceStepsProperty); }
            set { SetValue(GamutBounceStepsProperty, value); }
        }

        public InteractiveDiagonalTransform()
        {
            GamutBounceSteps = new StudyStepCollection();
        }

        #region Hints

        private static readonly DependencyPropertyKey LeftHintPropertyKey = DependencyProperty.RegisterReadOnly(nameof(LeftHint), typeof(SolidColorBrush[]), typeof(InteractiveDiagonalTransform), new PropertyMetadata(Array.Empty<SolidColorBrush>()));
        private static readonly DependencyPropertyKey RightHintPropertyKey = DependencyProperty.RegisterReadOnly(nameof(RightHint), typeof(SolidColorBrush[]), typeof(InteractiveDiagonalTransform), new PropertyMetadata(Array.Empty<SolidColorBrush>()));
        private static readonly DependencyPropertyKey UpHintPropertyKey = DependencyProperty.RegisterReadOnly(nameof(UpHint), typeof(SolidColorBrush[]), typeof(InteractiveDiagonalTransform), new PropertyMetadata(Array.Empty<SolidColorBrush>()));
        private static readonly DependencyPropertyKey DownHintPropertyKey = DependencyProperty.RegisterReadOnly(nameof(DownHint), typeof(SolidColorBrush[]), typeof(InteractiveDiagonalTransform), new PropertyMetadata(Array.Empty<SolidColorBrush>()));

        public static readonly DependencyProperty LeftHintProperty = LeftHintPropertyKey.DependencyProperty;
        public static readonly DependencyProperty RightHintProperty = RightHintPropertyKey.DependencyProperty;
        public static readonly DependencyProperty UpHintProperty = UpHintPropertyKey.DependencyProperty;
        public static readonly DependencyProperty DownHintProperty = DownHintPropertyKey.DependencyProperty;

        public static readonly DependencyProperty HintCountProperty = DependencyProperty.Register(nameof(HintCount), typeof(int), typeof(InteractiveDiagonalTransform), new PropertyMetadata(0, OnHintCountChanged));
        public static readonly DependencyProperty HintSizeProperty = DependencyProperty.Register(nameof(HintSize), typeof(double), typeof(InteractiveDiagonalTransform), new PropertyMetadata(100.0));

        private static void OnHintCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InteractiveDiagonalTransform @this = (InteractiveDiagonalTransform)d;
            int count = (int)e.NewValue;

            @this.GenerateHints(count, LeftHintPropertyKey, RightHintPropertyKey, UpHintPropertyKey, DownHintPropertyKey);
        }

        private void GenerateHints(int count, params DependencyPropertyKey[] properties)
        {
            foreach (DependencyPropertyKey property in properties)
            {
                SolidColorBrush[] brushes = new SolidColorBrush[count];
                for (int i = 0; i < count; i++)
                    brushes[i] = new SolidColorBrush();

                SetValue(property, brushes);
            }

            InvalidateHints();
        }

        private void InvalidateHints()
        {
            InvalidateHints(LeftHint, Primaries.Green.u, UV.Y);
            InvalidateHints(RightHint, Primaries.Red.u, UV.Y);
            InvalidateHints(UpHint, UV.X, Primaries.Green.v);
            InvalidateHints(DownHint, UV.X, Primaries.Blue.v);
        }
        private void InvalidateHints(SolidColorBrush[] hint, double u, double v)
        {
            if (hint == null)
                return;

            Edge full = new Edge(UV, new Point(u, v));

            for (int i = hint.Length - 1; i >= 0; i--)
            {
                Edge step = full.Scale((i + 1) / (double)hint.Length);

                ColorRGB color = ToRGB(step.B);
                hint[i].Color = Color.Multiply((Color)color, (float)MultiplierRGBA);
            }
        }

        public SolidColorBrush[] DownHint
        {
            get { return (SolidColorBrush[])GetValue(DownHintProperty); }
            private set { SetValue(DownHintPropertyKey, value); }
        }
        public SolidColorBrush[] UpHint
        {
            get { return (SolidColorBrush[])GetValue(UpHintProperty); }
            private set { SetValue(UpHintPropertyKey, value); }
        }
        public SolidColorBrush[] RightHint
        {
            get { return (SolidColorBrush[])GetValue(RightHintProperty); }
            private set { SetValue(RightHintPropertyKey, value); }
        }
        public SolidColorBrush[] LeftHint
        {
            get { return (SolidColorBrush[])GetValue(LeftHintProperty); }
            private set { SetValue(LeftHintPropertyKey, value); }
        }

        public int HintCount
        {
            get { return (int)GetValue(HintCountProperty); }
            set { SetValue(HintCountProperty, value); }
        }
        public double HintSize
        {
            get { return (double)GetValue(HintSizeProperty); }
            set { SetValue(HintSizeProperty, value); }
        }

        #endregion

        #region RGB

        private static readonly ColorRGB GamutRed = new ColorRGB(1, 0, 0);
        private static readonly ColorRGB GamutGreen = new ColorRGB(0, 1, 0);
        private static readonly ColorRGB GamutBlue = new ColorRGB(0, 0, 1);

        public static readonly DependencyProperty RGBAProperty = DependencyProperty.Register(nameof(RGBA), typeof(Color), typeof(InteractiveDiagonalTransform), new PropertyMetadata(Colors.White, OnTransformChanged));
        public static readonly DependencyProperty StepSizeRGBProperty = DependencyProperty.Register(nameof(StepSizeRGB), typeof(float), typeof(InteractiveDiagonalTransform), new PropertyMetadata(0.05f));
        public static readonly DependencyProperty MultiplierRGBAProperty = DependencyProperty.Register(nameof(MultiplierRGBA), typeof(double), typeof(InteractiveDiagonalTransform), new PropertyMetadata(1.0, OnUVChanged));
        public static readonly DependencyProperty MultiplierStepProperty = DependencyProperty.Register(nameof(MultiplierStep), typeof(double), typeof(InteractiveDiagonalTransform), new PropertyMetadata(1.1));

        public double MultiplierStep
        {
            get { return (double)GetValue(MultiplierStepProperty); }
            set { SetValue(MultiplierStepProperty, value); }
        }
        public double MultiplierRGBA
        {
            get { return (double)GetValue(MultiplierRGBAProperty); }
            set { SetValue(MultiplierRGBAProperty, value); }
        }

        public float StepSizeRGB
        {
            get { return (float)GetValue(StepSizeRGBProperty); }
            set { SetValue(StepSizeRGBProperty, value); }
        }

        public Color RGBA
        {
            get { return (Color)GetValue(RGBAProperty); }
            set { SetValue(RGBAProperty, value); }
        }

        private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InteractiveDiagonalTransform @this = (InteractiveDiagonalTransform)d;
            Color color = @this.RGBA;
            @this.StudyContext.Log(@this, @this, "TransformChanged", color.ScR, color.ScG, color.ScB);
        }

        private static ColorRGB ToRGB(Point uv)
        {
            Point p = sRGBgamut.Clip(uv);
            BarycentricPoint b = BarycentricPoint.FromOrthogonal(p, sRGBgamut);

            float max = (float)Math.Max(Math.Max(b.A, b.B), b.C);

            ColorRGB rgb = GamutRed * (float)b.A / max + GamutGreen * (float)b.B / max + GamutBlue * (float)b.C / max;
            return rgb;
        }

        #endregion

        #region UV

        private static readonly RGBPrimaries<Chromaticity1976uv> Primaries = new RGBPrimaries<Chromaticity1976uv>(new Chromaticity1931xy(0.6491f, 0.3392f).Touv(), new Chromaticity1931xy(0.3383f, 0.6094f).Touv(), new Chromaticity1931xy(0.1409f, 0.0770f).Touv());
        private static readonly Triangle sRGBgamut = new Triangle(Primaries.Red, Primaries.Green, Primaries.Blue);

        public static readonly DependencyProperty UVProperty = DependencyProperty.Register(nameof(UV), typeof(Point), typeof(InteractiveDiagonalTransform), new PropertyMetadata(default(Point), OnUVChanged, CoerceUV));
        public static readonly DependencyProperty StepSizeUVProperty = DependencyProperty.Register(nameof(StepSizeUV), typeof(double), typeof(InteractiveDiagonalTransform), new PropertyMetadata(0.001));
        public static readonly DependencyProperty StartingUVProperty = DependencyProperty.Register(nameof(StartingUV), typeof(Point?), typeof(InteractiveDiagonalTransform));

        private static object CoerceUV(DependencyObject d, object baseValue)
        {
            InteractiveDiagonalTransform @this = (InteractiveDiagonalTransform)d;

            Point uv = (Point)baseValue;
            Point clipped = sRGBgamut.Clip(uv);

            if (@this.GamutBounceSteps?.Count > 0 && uv != clipped)
                _ = @this.StudyContext.Execute(@this.GamutBounceSteps, @this.ItemContext, CancellationToken.None);

            @this.StudyContext.Log(d as IXmlLineInfo, d, "ChangingUV", uv.X, uv.Y, clipped.X, clipped.Y);
            return clipped;
        }

        private static void OnUVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((InteractiveDiagonalTransform)d).OnUVChanged();
        private void OnUVChanged()
        {
            BarycentricPoint b = BarycentricPoint.FromOrthogonal(UV, sRGBgamut);

            float max = (float)Math.Max(Math.Max(b.A, b.B), b.C);

            ColorRGB rgb = GamutRed * (float)b.A / max + GamutGreen * (float)b.B / max + GamutBlue * (float)b.C / max;

            RGBA = Color.Multiply((Color)rgb, (float)MultiplierRGBA);
            Result = string.Join(",", UV, MultiplierRGBA, rgb);
            InvalidateHints();
        }

        public Point UV
        {
            get { return (Point)GetValue(UVProperty); }
            set { SetValue(UVProperty, value); }
        }
        public Point? StartingUV
        {
            get { return (Point?)GetValue(StartingUVProperty); }
            set { SetValue(StartingUVProperty, value); }
        }

        public double StepSizeUV
        {
            get { return (double)GetValue(StepSizeUVProperty); }
            set { SetValue(StepSizeUVProperty, value); }
        }

        #endregion

        #region Randomization

        public static readonly DependencyProperty RandomizeProperty = DependencyProperty.Register(nameof(Randomize), typeof(RandomizationMode), typeof(InteractiveDiagonalTransform));
        public static readonly DependencyProperty MaximumRandomDistanceProperty = DependencyProperty.Register(nameof(MaximumRandomDistance), typeof(double), typeof(InteractiveDiagonalTransform));

        public double MaximumRandomDistance
        {
            get { return (double)GetValue(MaximumRandomDistanceProperty); }
            set { SetValue(MaximumRandomDistanceProperty, value); }
        }
        public RandomizationMode Randomize
        {
            get { return (RandomizationMode)GetValue(RandomizeProperty); }
            set { SetValue(RandomizeProperty, value); }
        }

        protected Point3D RandomizePoint(Point3D p)
        {
            if (Randomize == RandomizationMode.None || MaximumRandomDistance == 0)
                return p;

            while (true) // skip rather than clip out of gamut values to keep the distribution
            {
                double elevation = StudyContext.Random.NextDouble() * Math.PI;    // always calculate all three to preserve random sequence regardless of mode
                double azimuth = StudyContext.Random.NextDouble() * Math.PI * 2;
                double r = StudyContext.Random.NextDouble() * MaximumRandomDistance;

                if (Randomize == RandomizationMode.Circumference || Randomize == RandomizationMode.Surface)
                    r = MaximumRandomDistance;

                if (Randomize == RandomizationMode.Area || Randomize == RandomizationMode.Circumference)
                    elevation = Math.Acos(0);

                double x = r * Math.Sin(elevation) * Math.Cos(azimuth);
                double y = r * Math.Sin(elevation) * Math.Sin(azimuth);
                double z = r * Math.Cos(elevation);

                Point3D randomized = new Point3D(p.X + x, p.Y + y, p.Z + z);

                if (sRGBgamut.Contains(new Point(randomized.X, randomized.Y))) // TODO: support p.Z gamut check
                    return randomized;
            }
        }

        #endregion

        protected override Task Execute(CancellationToken cancellationToken)
        {
            Point uv = StartingUV ?? sRGBgamut.Centroid;

            if (Randomize != RandomizationMode.None)
            {
                Point3D randomizedStart = RandomizePoint(new Point3D(uv.X, uv.Y, 0));
                StudyContext.Log(this, this, "RandomOffset", randomizedStart.X - uv.X, randomizedStart.Y - uv.Y, randomizedStart.Z);
                uv = new Point(randomizedStart.X, randomizedStart.Y);
            }

            UV = uv;

            return base.Execute(cancellationToken);
        }

        public override void ExecuteDry(StudyContext context)
        {
            if (Randomize != RandomizationMode.None)
            {
                context.Random.NextDouble();
                context.Random.NextDouble();
                context.Random.NextDouble();
            }
        }

        public override void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            base.OnStudyInput(sender, args);

            switch (args.Input)
            {
                case "+R": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR + StepSizeRGB, RGBA.ScG, RGBA.ScB); break;
                case "-R": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR - StepSizeRGB, RGBA.ScG, RGBA.ScB); break;
                case "+G": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR, RGBA.ScG + StepSizeRGB, RGBA.ScB); break;
                case "-G": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR, RGBA.ScG - StepSizeRGB, RGBA.ScB); break;
                case "+B": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR, RGBA.ScG, RGBA.ScB + StepSizeRGB); break;
                case "-B": RGBA = Color.FromScRgb(RGBA.ScA, RGBA.ScR, RGBA.ScG, RGBA.ScB - StepSizeRGB); break;

                case "Add":
                case "+RGB": MultiplierRGBA *= MultiplierStep; break;

                case "Subtract":
                case "-RGB": MultiplierRGBA /= MultiplierStep; break;

                case "Right":
                case "+u": UV = new Point(UV.X + StepSizeUV, UV.Y); break;

                case "Left":
                case "-u": UV = new Point(UV.X - StepSizeUV, UV.Y); break;

                case "Up":
                case "+v": UV = new Point(UV.X, UV.Y + StepSizeUV); break;

                case "Down":
                case "-v": UV = new Point(UV.X, UV.Y - StepSizeUV); break;

                case "W": UV = sRGBgamut.Centroid; break;
                case "M": MultiplierRGBA = 1.0; break;
            }
        }
    }
}
