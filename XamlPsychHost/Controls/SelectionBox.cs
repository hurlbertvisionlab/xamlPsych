using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class SelectionBox : ListBox
    {
        public static readonly DependencyProperty ForceSelectionProperty = DependencyProperty.Register(nameof(ForceSelection), typeof(bool), typeof(SelectionBox));

        public bool ForceSelection
        {
            get { return (bool)GetValue(ForceSelectionProperty); }
            set { SetValue(ForceSelectionProperty, value); }
        }


        private static readonly Dictionary<string, Key> _inputToKey = new Dictionary<string, Key>(StringComparer.OrdinalIgnoreCase)
        {
            { "Left", Key.Left },
            { "Right", Key.Right },
            { "Up", Key.Up },
            { "Down", Key.Down },
        };

        public SelectionBox()
        {
            AddHandler(Study.StudyInputEvent, new StudyInputRoutedEventHandler(OnStudyInput));

            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (SelectedIndex >= 0)
                if (ItemContainerGenerator.ContainerFromIndex(SelectedIndex) is UIElement el)
                    el.Focus();
        }


        private void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            if (sender is not KeyboardInputProvider && _inputToKey.TryGetValue(args.Input, out Key key))
                OnKeyDown(key);
        }

        protected bool OnKeyDown(Key key)
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), Environment.TickCount, key);
            args.RoutedEvent = KeyDownEvent;
            base.OnKeyDown(args);
            return args.Handled;
        }

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (ForceSelection)
                    SelectedIndex = 0;

                int selectedIndex = Math.Max(0, SelectedIndex);
                if (ItemContainerGenerator.ContainerFromIndex(selectedIndex) is UIElement)
                    Focus();
            }
        }
    }
}
