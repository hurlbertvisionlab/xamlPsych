using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

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
            Content = e.Exception.GetBaseException();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
        }

        public async Task Load(string path)
        {
            try
            {
                _currentStudy = LineInfoWpfLoader.Load<Study>(path);
                _currentStudy.InputProviders.BindTo(this);

                Title = _currentStudy.Title + " - Lab Study";
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

                string md5 = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
                _context.Log(null, "Host", "ProtocolHash", "MD5", md5);

                _sessionCode.Text = "Session " + _context.UniqueID;

                await Preload();
            }
            catch (Exception ex)
            {
                Content = ex.GetBaseException();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_running)
                Run();
        }

        private async Task Preload()
        {
            foreach (DictionaryEntry resource in _context.Study.Resources)
                if (resource.Value is IStudyPreloadable preloadable)
                    if (preloadable.Preload)
                    {
                        _status.Text = $"Preloading {resource.Key}...";
                        _statusProgress.Text = null;

                        await preloadable.DoPreload(_context, new Progress<string>(ReportPreloadProgress), CancellationToken.None);
                    }

            _status.Text = "Press any key to start";
            _statusProgress.Text = null;
        }
        private void ReportPreloadProgress(string s)
        {
            _statusProgress.Text = s;
        }

        private bool _running;
        private CancellationTokenSource _runCancellation = new CancellationTokenSource();
        public async void Run()
        {
            try
            {
                _running = true;
                await _context.Execute(_currentStudy.Protocol, null, _runCancellation.Token);

                Close();
            }
            //catch (StudyException sx)
            //{
            //    while (sx.InnerException is StudyException inner)
            //        sx = inner;

            //    Content = sx;
            //}
            catch (Exception ex)
            {
                Content = ex.GetBaseException();
            }
        }

        private void OnStudyStepChanged(object sender, EventArgs e)
        {
            if (sender is StudyContext context && context.CurrentStep != null)
                Title = $"{_currentStudy.Title} [{context.StepNumber}:{context.CurrentStep.Name}] - Lab Study";
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
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () => MoveFocus(new TraversalRequest(FocusNavigationDirection.First)));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_context != null)
            {
                _runCancellation.Cancel();
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
