using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ClickableImage : Image
    {
        public static string GetAreaName(Geometry obj)
        {
            return (string)obj.GetValue(AreaNameProperty);
        }

        public static void SetAreaName(Geometry obj, string value)
        {
            obj.SetValue(AreaNameProperty, value);
        }

        public static readonly DependencyProperty AreaNameProperty = DependencyProperty.RegisterAttached("AreaName", typeof(string), typeof(ClickableImage));



        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(ClickableImage), new PropertyMetadata(OnPointsChanged));
        public static readonly DependencyProperty AreasProperty = DependencyProperty.Register(nameof(Areas), typeof(GeometryCollection), typeof(ClickableImage));
        public static readonly DependencyProperty MaximumPointsProperty = DependencyProperty.Register(nameof(MaximumPoints), typeof(int), typeof(ClickableImage), new PropertyMetadata(int.MaxValue));

        public GeometryCollection Areas
        {
            get { return (GeometryCollection)GetValue(AreasProperty); }
            set { SetValue(AreasProperty, value); }
        }

        public int MaximumPoints
        {
            get { return (int)GetValue(MaximumPointsProperty); }
            set { SetValue(MaximumPointsProperty, value); }
        }

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClickableImage @this = (ClickableImage)d;
            @this.InvalidateAdorner(); // TODO: move Points property to the adorner
        }

        private PointsAdorner _adorner;
        public ClickableImage()
        {
            SetCurrentValue(PointsProperty, new PointCollection());
            SetCurrentValue(AreasProperty, new GeometryCollection());
            SetCurrentValue(PointsWithAreasProperty, new PointWithAreasCollection());
            // SetBinding(PointsProperty, new Binding("Step.Result") { Mode = BindingMode.OneWayToSource });
        }

        protected void InvalidateAdorner()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            if (layer == null)
                return;

            if (_adorner != null)
                layer.Remove(_adorner);

            if (Points != null)
                layer.Add(_adorner = new PointsAdorner(this, Points));
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            InvalidateAdorner();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled)
            {
                if (Points.Count >= MaximumPoints)
                    Points.RemoveAt(0);

                Size size = RenderSize;
                Point p = e.GetPosition(this);
                Point rp = new Point(p.X / size.Width, p.Y / size.Height);

                Points.Add(rp);

                if (Areas?.Count > 0)
                {
                    Size stretchScale = ComputeScaleFactor(RenderSize, new Size(Source.Width, Source.Height), Stretch, StretchDirection);
                    Point pixelPoint = new Point(p.X / stretchScale.Width, p.Y / stretchScale.Height);

                    PointWithAreas pwa = new PointWithAreas();
                    pwa.ScreenPoint = p;
                    pwa.SourcePoint = pixelPoint;
                    pwa.RelativePoint = rp;

                    for (int i = 0; i < Areas.Count; i++)
                        if (Areas[i].FillContains(pixelPoint))
                        {
                            string name = GetAreaName(Areas[i]);
                            if (string.IsNullOrEmpty(name))
                                SetAreaName(Areas[i], "Area" + i);
                            
                            pwa.AreasContainingPoint.Add(Areas[i]);
                        }

                    PointsWithAreas.Add(pwa);
                }

                if (Points.Count == MaximumPoints)
                    MaximumPointsReached?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler MaximumPointsReached;


        // temporary area hit testing

        public static readonly DependencyProperty PointsWithAreasProperty = DependencyProperty.Register(nameof(PointsWithAreas), typeof(PointWithAreasCollection), typeof(ClickableImage));

        public PointWithAreasCollection PointsWithAreas
        {
            get { return (PointWithAreasCollection)GetValue(PointsWithAreasProperty); }
            set { SetValue(PointsWithAreasProperty, value); }
        }

        public class PointWithAreasCollection : Collection<PointWithAreas>
        {
            public override string ToString()
            {
                return string.Join("\t", this);
            }
        }

        public class PointWithAreas
        {
            public Point ScreenPoint { get; set; }
            public Point SourcePoint { get; set; }
            public Point RelativePoint { get; set; }
            public List<Geometry> AreasContainingPoint { get; } = new();

            public override string ToString()
            {
                if (AreasContainingPoint?.Count > 0)
                    return RelativePoint + "," + string.Join(";", AreasContainingPoint.Select(GetAreaName));
                
                return RelativePoint.ToString();
            }
        }


        /// <summary>
        /// This is a helper function that computes scale factors depending on a target size and a content size
        /// </summary>
        /// <param name="availableSize">Size into which the content is being fitted.</param>
        /// <param name="contentSize">Size of the content, measured natively (unconstrained).</param>
        /// <param name="stretch">Value of the Stretch property on the element.</param>
        /// <param name="stretchDirection">Value of the StretchDirection property on the element.</param>
        private static Size ComputeScaleFactor(Size availableSize,
                                                Size contentSize,
                                                Stretch stretch,
                                                StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = !Double.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !Double.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = (IsZero(contentSize.Width)) ? 0.0 : availableSize.Width / contentSize.Width;
                scaleY = (IsZero(contentSize.Height)) ? 0.0 : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            double minscale = scaleX < scaleY ? scaleX : scaleY;
                            scaleX = scaleY = minscale;
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            double maxscale = scaleX > scaleY ? scaleX : scaleY;
                            scaleX = scaleY = maxscale;
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1.0;
                        if (scaleY < 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1.0;
                        if (scaleY > 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);

            static bool IsZero(double value)
            {
                const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
                return Math.Abs(value) < 10.0 * DBL_EPSILON;
            }
        }
    }
}
