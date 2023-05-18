using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class StudyContext : IDisposable
    {
        public int UniqueIDTickCount { get; } = Environment.TickCount;
        public string UniqueID { get; }
        public bool IsDryRun { get; set; }

        private readonly List<StudyStepContext> _pastSteps = new List<StudyStepContext>();
        private readonly Stack<StudyStepContext> _currentSteps = new Stack<StudyStepContext>();

        public StudyStepContext[] GetCurrentStepStack() => _currentSteps.ToArray();
        public Dictionary<string, object> Store { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> ItemContexts { get; } = new Dictionary<string, object>();

        public DateTimeOffset? Started { get; private set; }
        public StudyStepContext CurrentStep => _currentSteps?.Count > 0 ? _currentSteps.Peek() : null;
        public int StepNumber => _pastSteps.Count + _currentSteps.Count;

        public Dispatcher Dispatcher { get; }
        public Study Study { get; }
        public IStudyPresentationSink Screen { get; }
        public IStudyInputSource Input { get; }
        public object LastResult { get; set; }
        public int LastResultIndex { get; set; }

        public Random Random { get; set; }

        private bool _disposed;
        private List<string> _log = new List<string>();
        private readonly Dictionary<object, string> _logNameLookup = new Dictionary<object, string>(); // consider ReferenceEquals for comparer
        private readonly AutoResetEvent _logEvent = new AutoResetEvent(false);
        private readonly Thread _logThread;
        public string LogFile = "Log.txt";

        private static readonly char[] _ch = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        private string GenerateUniqueID()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string ticks = UniqueIDTickCount.ToString().PadLeft(3, '0');
            return string.Concat(E(now.Year), E(now.Month), E(now.Day)) + "-" + ticks.Substring(ticks.Length - 3);

            static char E(int n) => _ch[n % _ch.Length];
        }

        public StudyContext(Study study, IStudyPresentationSink screen, IStudyInputSource input)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Study = study;
            Screen = screen;
            Input = input;
            input.StudyInput += OnStudyInput;

            UniqueID = GenerateUniqueID();
            Random = new Random(study.Seed);

            _logThread = new Thread(LogThread);
            _logThread.Name = "Logging thread.";
            _logThread.IsBackground = false;
            _logThread.Start();
        }

        private void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            CurrentStep?.Step.OnStudyInput(sender, args);
        }

        public void LogRegisterName(object instance, string name)
        {
            _logNameLookup[instance] = name;
        }
        public string ToLogString(object instance) => ToLogStringWithFallback(instance, null);
        public string ToLogStringWithFallback(object instance, object fallback = null)
        {
            if (instance is string s)
                return s;

            if (instance != null)
            {
                if (_logNameLookup.TryGetValue(instance, out s))
                    return s;

                if (instance is ILogInfo info)
                    return info.ToLogString(this);
            }

            return fallback?.ToString() ?? instance?.ToString();
        }
        public string ToLogString(params object[] instances) => ToLogStringWithFallback(instances, null);
        public string ToLogString(System.Collections.IList instances) => ToLogStringWithFallback(instances, null);
        public string ToLogStringWithFallback(System.Collections.IList instances, System.Collections.IList fallbacks)
        {
            string[] strings = new string[instances.Count];
            for (int i = 0; i < instances.Count; i++)
                strings[i] = ToLogStringWithFallback(instances[i], fallbacks?[i]);
            
            return string.Join(",", strings).Replace(Environment.NewLine, "  ").Replace("\r", "  ").Replace("\n", "  ");
        }
        public void Log(IXmlLineInfo lineInfo, object source, string @event, params object[] data)
        {
            string id = null;
            string line = null;
            string col = null;
            if (lineInfo != null)
            {
                line = lineInfo.LineNumber.ToString();
                col = lineInfo.LinePosition.ToString();
            }

            string name = source as string ?? source.GetType().Name;

            if (source is StudyStep step)
                id = step.ID;
            if (source is StudyStepContext context)
                id = context.Step.ID;
            else if (source is FrameworkElement el)
                id = el.Name;

            string logLine = string.Join(",", DateTimeOffset.Now.ToString("O"), UniqueID, StepNumber, line, col, name, id, @event, ToLogString(data));
            Debug.WriteLine(logLine);

            lock (_logThread)
                _log.Add(logLine);
            
            _logEvent.Set();
        }

        private void LogThread()
        {
            List<string> log = new List<string>();

            while (true)
            {
                _logEvent.WaitOne();
                lock (_logThread)
                    log = Interlocked.Exchange(ref _log, log);

                if (!File.Exists(LogFile))
                    File.WriteAllText(LogFile, "Timestamp,SessionID,StepNumber,Line,Column,Source,ID,Event,Data\r\n");

                File.AppendAllLines(LogFile, log);
                log.Clear();

                if (_disposed)
                    break;
            }

            _logEvent.Set();
        }

        // TODO: disconnect screen and input if sequence of steps is terminated due to timeout, so that they can no longer control the screen

        public async Task Execute(StudyStep step, object itemContext, CancellationToken cancellationToken)
        {
            Started ??= DateTimeOffset.Now;

            Log(step, this, "Execute", step.GetType().Name, step.ID, itemContext);

            StudyStepContext context = new StudyStepContext(step, _pastSteps.Count);
            context.StudyContext = this;
            context.ItemContext = itemContext;

            if (step.ID != null)
                ItemContexts[step.ID] = itemContext;

            _currentSteps.Push(context);
            StepChanged?.Invoke(this, EventArgs.Empty); // takes measured time

            try
            {
                if (step is StudyIterator || step is StudyStepCollection || !IsDryRun)
                    await step.Execute(this, cancellationToken);
                else
                    step.ExecuteDry(this);
            }
            catch (Exception e)
            {
                Log(step, step, "ERROR", e.Message);

                if (e is StudyException)
                    throw;
                else
                    throw new StudyException(context, e.Message, e);
            }
            _currentSteps.Pop();
            context.Ended = DateTimeOffset.Now;

            _pastSteps.Add(context);
            StepChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler StepChanged;

        public void Dispose()
        {
            _disposed = true;
            _logEvent.Set();
            _logEvent.WaitOne();

            if (Study.Resources != null)
                foreach (System.Collections.DictionaryEntry entry in Study.Resources)
                    if (entry.Value is IStudyDisposableResource disposable)
                        disposable.Dispose(this);
        }
    }
}
