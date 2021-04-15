using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;
using System.Xml;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyObject : DependencyObject
    {
        public static readonly DependencyProperty StudyContextProperty = DependencyProperty.RegisterAttached("StudyContext", typeof(StudyContext), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static StudyContext GetStudyContext(DependencyObject obj)
        {
            return (StudyContext)obj.GetValue(StudyContextProperty);
        }
        public static void SetStudyContext(DependencyObject obj, StudyContext value)
        {
            obj.SetValue(StudyContextProperty, value);
        }

        public static readonly DependencyProperty StepContextProperty = DependencyProperty.RegisterAttached("StepContext", typeof(StudyStepContext), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static StudyStepContext GetStepContext(DependencyObject obj)
        {
            return (StudyStepContext)obj.GetValue(StepContextProperty);
        }
        public static void SetStepContext(DependencyObject obj, StudyStepContext value)
        {
            obj.SetValue(StepContextProperty, value);
        }

        public static readonly DependencyProperty ItemContextProperty = DependencyProperty.RegisterAttached("ItemContext", typeof(object), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static object GetItemContext(DependencyObject obj)
        {
            return (object)obj.GetValue(ItemContextProperty);
        }
        public static void SetItemContext(DependencyObject obj, object value)
        {
            obj.SetValue(ItemContextProperty, value);
        }


        //public static readonly DependencyProperty StudyContextProperty = DependencyProperty.Register(nameof(StudyContext), typeof(StudyContext), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        //public static readonly DependencyProperty StepContextProperty = DependencyProperty.Register(nameof(StepContext), typeof(StudyStepContext), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        //public static readonly DependencyProperty ItemContextProperty = DependencyProperty.Register(nameof(ItemContext), typeof(object), typeof(StudyObject), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public StudyContext StudyContext
        {
            get { return (StudyContext)GetValue(StudyContextProperty); }
            set { SetValue(StudyContextProperty, value); }
        }

        public StudyStepContext StepContext
        {
            get { return (StudyStepContext)GetValue(StepContextProperty); }
            set { SetValue(StepContextProperty, value); }
        }

        public object ItemContext
        {
            get { return GetValue(ItemContextProperty); }
            set { SetValue(ItemContextProperty, value); }
        }
    }

    public abstract class StudyStep : StudyObject, IXamlLineInfoConsumer, IXmlLineInfo
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(nameof(ID), typeof(string), typeof(StudyStep));
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(nameof(Result), typeof(object), typeof(StudyStep), new PropertyMetadata(OnResultChanged));
        public static readonly DependencyProperty StoreResultAsProperty = DependencyProperty.Register(nameof(StoreResultAs), typeof(string), typeof(StudyStep));

        public static readonly DependencyProperty AdvanceOnResultProperty = DependencyProperty.Register(nameof(AdvanceOnResult), typeof(bool), typeof(StudyStep));
        public static readonly DependencyProperty AdvanceAfterProperty = DependencyProperty.Register(nameof(AdvanceAfter), typeof(TimeSpan), typeof(StudyStep));
        public static readonly DependencyProperty AdvanceOnInputProperty = DependencyProperty.Register(nameof(AdvanceOnInput), typeof(TokenStringSet), typeof(StudyStep));
        public static readonly DependencyProperty AdvanceWhenDoneProperty = DependencyProperty.Register(nameof(AdvanceWhenDone), typeof(bool), typeof(StudyStep), new PropertyMetadata(true));

        private static readonly DependencyPropertyKey RemainingToAdvancePropertyKey = DependencyProperty.RegisterReadOnly(nameof(RemainingToAdvance), typeof(TimeSpan), typeof(StudyStep), new PropertyMetadata(TimeSpan.Zero));
        public static readonly DependencyProperty RemainingToAdvanceProperty = RemainingToAdvancePropertyKey.DependencyProperty;

        public bool AdvanceWhenDone
        {
            get { return (bool)GetValue(AdvanceWhenDoneProperty); }
            set { SetValue(AdvanceWhenDoneProperty, value); }
        }

        public TokenStringSet AdvanceOnInput
        {
            get { return (TokenStringSet)GetValue(AdvanceOnInputProperty); }
            set { SetValue(AdvanceOnInputProperty, value); }
        }

        public bool AdvanceOnResult
        {
            get { return (bool)GetValue(AdvanceOnResultProperty); }
            set { SetValue(AdvanceOnResultProperty, value); }
        }

        public TimeSpan RemainingToAdvance
        {
            get { return (TimeSpan)GetValue(RemainingToAdvanceProperty); }
            private set { SetValue(RemainingToAdvancePropertyKey, value); }
        }

        public TimeSpan AdvanceAfter
        {
            get { return (TimeSpan)GetValue(AdvanceAfterProperty); }
            set { SetValue(AdvanceAfterProperty, value); }
        }

        public string StoreResultAs
        {
            get { return (string)GetValue(StoreResultAsProperty); }
            set { SetValue(StoreResultAsProperty, value); }
        }

        public object Result
        {
            get { return GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StudyStep @this = (StudyStep)d;
            if (@this.AdvanceOnResult)
                @this._resultTaskSource.SetResult(e.NewValue);
        }

        private DispatcherTimer _advanceTimer;
        private DateTimeOffset _advanceTimerStarted;
        private TaskCompletionSource<TimeSpan> _timeoutTaskSource;
        private CancellationTokenSource _timeoutCancellation;

        private TaskCompletionSource<string> _inputTaskSource;
        private TaskCompletionSource<object> _resultTaskSource;
        private TaskCompletionSource<string> _explicitTaskSource;

        private void OnAdvanceTimerTick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTimeOffset.Now - _advanceTimerStarted;
            RemainingToAdvance = AdvanceAfter - elapsed;
            if (elapsed > AdvanceAfter)
            {
                _advanceTimer.Stop();
                _timeoutTaskSource.SetResult(elapsed);
            }
        }

        public StudyStep()
        {
            AdvanceOnInput = new TokenStringSet();
        }

        public IDictionary<string, object> ItemContexts => StudyContext?.ItemContexts;

        protected virtual void PrepareForNewExecution()
        {
            _advanceTimer?.Stop();

            _timeoutCancellation = new CancellationTokenSource();
            _timeoutTaskSource = new TaskCompletionSource<TimeSpan>();
            _inputTaskSource = new TaskCompletionSource<string>();
            _resultTaskSource = new TaskCompletionSource<object>();
            _explicitTaskSource = new TaskCompletionSource<string>();

            if (AdvanceAfter != TimeSpan.Zero)
            {
                RemainingToAdvance = AdvanceAfter;
                _advanceTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Input, OnAdvanceTimerTick, Dispatcher.CurrentDispatcher);
                _timeoutCancellation.CancelAfter(AdvanceAfter);
            }
        }

        public virtual async Task Execute(StudyContext context, CancellationToken cancellationToken)
        {
            StudyContext = context;
            StepContext = context.CurrentStep;
            ItemContext = StepContext.ItemContext;

            using (cancellationToken.Register(OnExecutionCancelled, false)) // we do not want the cancellation to propagate to the parent, i.e. if <ForEach> timeouts,
            {                                                               // we want all the children to be cancelled but still run the next step normally
                PrepareForNewExecution();

                Task done = Execute(_timeoutCancellation.Token);
                List<Task> advances = new() { _timeoutTaskSource.Task, _inputTaskSource.Task, _resultTaskSource.Task, _explicitTaskSource.Task };

                if (AdvanceWhenDone)
                    advances.Add(done);

                Task<Task> advance = Task.WhenAny(advances);

                if (!advance.IsCompleted)
                    if (AdvanceAfter != TimeSpan.Zero)
                    {
                        _advanceTimerStarted = DateTimeOffset.Now;
                        _advanceTimer.Start();
                    }

                Task winner = await advance;
                if (winner.IsFaulted)
                    throw winner.Exception;

                FinalizeExecution(winner);
            }
        }

        public virtual void ExecuteDry(StudyContext context)
        {
        }

        protected virtual void FinalizeExecution(Task advancingTask)
        {
            if (advancingTask == _timeoutTaskSource.Task)
                StudyContext.Log(this, this, "Advancing", "TimeOut", _timeoutTaskSource.Task.Result);

            else if (advancingTask == _inputTaskSource.Task)
                StudyContext.Log(this, this, "Advancing", "StudyInput", _inputTaskSource.Task.Result);

            else if (advancingTask == _resultTaskSource.Task)
                StudyContext.Log(this, this, "Advancing", "Result", _resultTaskSource.Task.Result);

            else if (advancingTask == _explicitTaskSource.Task)
                StudyContext.Log(this, this, "Advancing", "Explicit", _explicitTaskSource.Task.Result);

            else
                StudyContext.Log(this, this, "Advancing", "Done");

            if (Result != null)
            {
                StudyContext.Log(this, this, "Result", Result);
                StudyContext.LastResult = Result;

                if (ItemContext is System.Collections.IList list)
                {
                    int index = list.IndexOf(Result);
                    StudyContext.Log(this, this, "ResultIndex", index, Result);
                    StudyContext.LastResultIndex = index;
                }
            }

            if (StoreResultAs != null)
            {
                StudyContext.Log(this, this, "Store", StoreResultAs, Result);
                StudyContext.Store[StoreResultAs] = Result;
            }
        }

        public virtual void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            StudyContext.Log(this, this, "StudyInput", sender?.GetType().Name, args.Input, args.Handled);

            if (!args.Handled)
                if (AdvanceOnInput.Contains(args.Input))
                    _inputTaskSource.SetResult(args.Input);
        }

        private void OnExecutionCancelled()
        {
            _timeoutCancellation.Cancel();
        }

        protected void Advance(string reason)
        {
            _explicitTaskSource.SetResult(reason);
        }

        protected abstract Task Execute(CancellationToken cancellationToken);

        private int _lineNumber;
        private int _linePosition;
        int IXmlLineInfo.LineNumber => _lineNumber;
        int IXmlLineInfo.LinePosition => _linePosition;
        bool IXmlLineInfo.HasLineInfo() => _lineNumber != 0;
        bool IXamlLineInfoConsumer.ShouldProvideLineInfo => true;

        void IXamlLineInfoConsumer.SetLineInfo(int lineNumber, int linePosition)
        {
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }
    }
}
