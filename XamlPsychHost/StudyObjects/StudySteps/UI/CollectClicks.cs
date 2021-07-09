//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows;

//namespace HurlbertVisionLab.XamlPsychHost
//{
//    public class CollectClicks : ShowStimuli
//    {
//        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(CollectClicks), new PropertyMetadata(OnPointsChanged));

//        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            ClickableImage @this = (ClickableImage)d;
//            @this.InvalidateAdorner(); // TODO: move Points property to the adorner
//        }

//        static CollectClicks()
//        {
//            ContentProperty.OverrideMetadata(typeof(CollectClicks)
//        }

//        public static readonly DependencyProperty MaximumPointsProperty = DependencyProperty.Register(nameof(MaximumPoints), typeof(int), typeof(CollectClicks), new PropertyMetadata(int.MaxValue));

//        public int MaximumPoints
//        {
//            get { return (int)GetValue(MaximumPointsProperty); }
//            set { SetValue(MaximumPointsProperty, value); }
//        }

//        public PointCollection Points
//        {
//            get { return (PointCollection)GetValue(PointsProperty); }
//            set { SetValue(PointsProperty, value); }
//        }

//        private PointsAdorner _adorner;
//        public CollectClicks()
//        {
//            SetCurrentValue(PointsProperty, new PointCollection());
//            // SetBinding(PointsProperty, new Binding("Step.Result") { Mode = BindingMode.OneWayToSource });
//        }

//        protected void InvalidateAdorner()
//        {
//            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
//            if (layer == null)
//                return;

//            if (_adorner != null)
//                layer.Remove(_adorner);

//            if (Points != null)
//                layer.Add(_adorner = new PointsAdorner(this, Points));
//        }

//        protected override void OnVisualParentChanged(DependencyObject oldParent)
//        {
//            base.OnVisualParentChanged(oldParent);

//            InvalidateAdorner();
//        }

//        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
//        {
//            base.OnPreviewMouseLeftButtonDown(e);
//        }

//        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
//        {
//            base.OnMouseLeftButtonDown(e);

//            if (!e.Handled)
//            {
//                if (Points.Count >= MaximumPoints)
//                    Points.RemoveAt(0);

//                Points.Add(e.GetPosition(this));
//            }
//        }

//    }
//}
