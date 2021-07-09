using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace HurlbertVisionLab.XamlPsychHost
{
    public partial class MainWindow : Window, IStudyPresentationSink, IStudyInputSource
    {
        private Study _currentStudy;
        private StudyContext _context;

        public MainWindow()
        {
            InitializeComponent();

            Dispatcher.UnhandledException += OnDispatcherException;
        }

        private void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Content = e.Exception;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }

            try
            {
                _currentStudy = LineInfoWpfLoader.Load<Study>("HuaweiMultiLight.xaml");
                _currentStudy.InputProviders.BindTo(this);

                Title = _currentStudy.Title + " - Hurlbert Vision Lab Study";
                if ("Dark".Equals(_currentStudy.Theme, StringComparison.OrdinalIgnoreCase))
                {
                    Resources["ThemeBackground"] = new SolidColorBrush(Colors.Black);
                    Resources["ThemeForeground"] = new SolidColorBrush(Colors.White);
                }

                _context = new StudyContext(_currentStudy, this, this);
                _context.StepChanged += OnStudyStepChanged;

                PowerManagement.Request(PowerRequestType.DisplayRequired | PowerRequestType.ExecutionRequired | PowerRequestType.SystemRequired, Title);

                _context.Log(null, "Host", "======START======", _currentStudy.Title, _currentStudy.Date, _currentStudy.Author);
                _context.Log(null, "Host", "Environment", Environment.MachineName, Environment.UserName, Environment.Version, Environment.TickCount);
                _context.Log(null, "Host", "RandomSeed", _currentStudy.Seed);

                try
                {
                    await _context.Execute(_currentStudy.Protocol, null, CancellationToken.None);

                    Close();
                }
                catch (StudyException sx) { Content = sx; }
            }
            catch (Exception ex)
            {
                Content = ex;
            }
        }

        private void OnStudyStepChanged(object sender, EventArgs e)
        {
            if (sender is StudyContext context && context.CurrentStep != null)
                Title = $"{_currentStudy.Title} [{context.StepNumber}:{context.CurrentStep.Name}] + Hurlbert Vision Lab Study";
        }

        public void Show(object element)
        {
            if (Content == element)
            {
                Content = null; // do not reuse templated steps
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action<object>(Show), element);
            }
            else
            {
                Content = element;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_context != null)
            {
                _context.Log(null, "Host", "~~~~~~END~~~~~~", _currentStudy.Title);
                _context.Dispose();
            }
        }

        public event StudyInputRoutedEventHandler StudyInput;
        public void ReportStudyInput(IInputProvider source, string toInput)
        {
            StudyInputEventArgs args = new StudyInputEventArgs(source, toInput);
            StudyInput?.Invoke(source, args);
            RaiseEvent(args);
        }
    }
}
