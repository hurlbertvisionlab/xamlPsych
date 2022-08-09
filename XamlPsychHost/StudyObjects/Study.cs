using System;
using System.ComponentModel;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Study : FrameworkContentElement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Theme { get; set; }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize { get; set; } = 32;

        public InputProviderCollection InputProviders { get; } = new InputProviderCollection();
        public StudyStepCollection Protocol { get; } = new StudyStepCollection();
        public int Seed { get; set; } = Environment.TickCount;

        public Study()
        {
            InputMappings = new MapInputCollection(this);
        }

        // This is just a XAML syntax sugar that will be used to reconstruct InputProviders (by MapInputCollection).
        // It allows the user to use
        //
        // <Study.InputMappings>
        //     <MapInput FromKeyboard.Key="Enter" FromGamepad.Key="GamepadLeftShoulder" ToInput="Confirm" />
        // </Study.InputMappings>
        //
        // instead of
        //
        // <Study.InputProviders>
        //     <KeyboardInputProvider>
        //         <MapKeyboard Key="Enter" ToInput="Confirm" />
        //     </KeyboardInputProvider>
        //     <GamepadInputProvider>
        //         <MapGamepad Key="GamepadLeftShoulder" ToInput="Confirm" />
        //     </GamepadInputProvider>
        // </Study.InputProviders>

        public MapInputCollection InputMappings { get; }

        public static readonly RoutedEvent StudyInputEvent = EventManager.RegisterRoutedEvent(nameof(StudyInput), RoutingStrategy.Bubble, typeof(StudyInputRoutedEventHandler), typeof(Study));

        public event StudyInputRoutedEventHandler StudyInput
        {
            add { AddHandler(StudyInputEvent, value); }
            remove { RemoveHandler(StudyInputEvent, value); }
        }
    }
}
