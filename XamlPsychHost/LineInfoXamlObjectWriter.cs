using System;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using UAM.InformatiX.Xaml;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class LineInfoXamlObjectWriter : XamlObjectWriter, IXamlLineInfoConsumer
    {
        public LineInfoXamlObjectWriter(XamlSchemaContext schemaContext) : base(schemaContext)
        {
        }

        public LineInfoXamlObjectWriter(XamlSchemaContext schemaContext, XamlObjectWriterSettings settings) : base(schemaContext, settings)
        {
        }

        private int _lineNumber;
        private int _linePosition;
        void IXamlLineInfoConsumer.SetLineInfo(int lineNumber, int linePosition)
        {
            _lineNumber = lineNumber;
            _linePosition = linePosition;

            base.SetLineInfo(lineNumber, linePosition);
        }

        bool IXamlLineInfoConsumer.ShouldProvideLineInfo => true;

        protected override void OnAfterBeginInit(object value)
        {
            if (value is IXamlLineInfoConsumer consumer)
                if (consumer.ShouldProvideLineInfo)
                    consumer.SetLineInfo(_lineNumber, _linePosition);

            base.OnAfterBeginInit(value);
        }
    }

    public class LineInfoXamlObjectWriterFactory : IXamlObjectWriterFactory
    {
        private readonly XamlSchemaContext _context;

        public LineInfoXamlObjectWriterFactory(XamlSchemaContext context)
        {
            _context = context;
        }

        public XamlObjectWriterSettings GetParentSettings()
        {
            return null;
        }

        public XamlObjectWriter GetXamlObjectWriter(XamlObjectWriterSettings settings)
        {
            return new LineInfoXamlObjectWriter(_context, settings);
        }
    }

    public static class LineInfoWpfLoader
    {
        public static T Load<T>(string path)
        {
            ParserContext parserContext = new ParserContext();

            XamlXmlReaderSettings readerSettings = new XamlXmlReaderSettings
            {
                IgnoreUidsOnPropertyElements = true,
                BaseUri = parserContext.BaseUri ?? PackUriHelper.Create(new Uri("application://")), // BaseUriHelper.PackAppBaseUri;
                ProvideLineInfo = true, 
            };

            XamlObjectWriterSettings writerSettings = new XamlObjectWriterSettings
            {
                IgnoreCanConvert = true,
                PreferUnconvertedDictionaryKeys = true,
            };

            CustomXamlSchemaContext schemaContext = new CustomXamlSchemaContext(System.Windows.Markup.XamlReader.GetWpfSchemaContext());
            schemaContext.Map(typeof(TupleItem), new TupleItemXamlType(schemaContext));

            XamlXmlReader xamlReader = new XamlXmlReader(path, schemaContext, readerSettings);

            MethodInfo _Load = typeof(System.Windows.Markup.XamlReader).Assembly.GetType("System.Windows.Markup.WpfXamlLoader").GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
            {
                    typeof(System.Xaml.XamlReader),   // xamlReader
                    typeof(IXamlObjectWriterFactory), // writerFactory
                    typeof(bool),                     // skipJournaledProperties
                    typeof(object),                   // rootObject
                    typeof(XamlObjectWriterSettings), // settings
                    typeof(Uri)                       // baseUri
            }, null);

            if (_Load != null)
                return (T)_Load.Invoke(null, new object[] { xamlReader, new LineInfoXamlObjectWriterFactory(xamlReader.SchemaContext), false, null, writerSettings, parserContext.BaseUri }); 

            return (T)System.Windows.Markup.XamlReader.Parse(File.ReadAllText(path)); // fallback to no line info
        }
    }
}
