using System;
using System.Collections.Generic;
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
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(ClickableImage), new PropertyMetadata(OnPointsChanged));

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClickableImage @this = (ClickableImage)d;
            @this.InvalidateAdorner(); // TODO: move Points property to the adorner
        }

        public static readonly DependencyProperty MaximumPointsProperty = DependencyProperty.Register(nameof(MaximumPoints), typeof(int), typeof(ClickableImage), new PropertyMetadata(int.MaxValue));

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

        private PointsAdorner _adorner;
        public ClickableImage()
        {
            SetCurrentValue(PointsProperty, new PointCollection());
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

                Points.Add(new Point(p.X / size.Width, p.Y / size.Height));

                if (Points.Count == MaximumPoints)
                    MaximumPointsReached?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler MaximumPointsReached;
    }
}
