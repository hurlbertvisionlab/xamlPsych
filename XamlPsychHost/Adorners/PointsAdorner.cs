using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class PointsAdorner : Adorner
    {
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(PointsAdorner), new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(PointsAdorner), new PropertyMetadata(5.0));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof(Radius), typeof(double), typeof(PointsAdorner), new PropertyMetadata(25.0));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        private PointCollection _points;
        public PointsAdorner(UIElement adornedElement, PointCollection points) : base(adornedElement)
        {
            _points = points;
            _points.Changed += OnPointsChanged;
        }

        private void OnPointsChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Stroke, StrokeThickness);

            foreach (Point p in _points)
                drawingContext.DrawEllipse(null, pen, p, Radius, Radius);
        }
    }
}
