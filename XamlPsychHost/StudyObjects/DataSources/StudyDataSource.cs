using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyDataSource : DependencyObject, IXamlLineInfoConsumer, IXmlLineInfo
    {
        public abstract IEnumerable<object> GenerateItems(StudyContext context);
        public abstract int? GetItemsCount(StudyContext context);

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
