using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Xaml.Schema;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class TupleItem : DynamicObject
    {
        internal static HashSet<string> AllKeys = new();

        private Dictionary<string, object> _values = new();

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _values.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _values.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _values[binder.Name] = value;
            return true;
        }

        public object this[string name]
        {
            get
            {
                if (_values.TryGetValue(name, out var value))
                    return value;

                return null;
            }
            set { _values[name] = value; }
        }
    }

    public class TupleItemXamlType : XamlType
    {
        private class TupleItemXamlMember : XamlMember
        {
            public TupleItemXamlMember(string name, TupleItemXamlType type) : base(name, type, false) { }
            protected override XamlMemberInvoker LookupInvoker() => new TupleItemXamlMemberInvoker(Name);
            protected override bool LookupIsUnknown() => false;
        }

        private class TupleItemXamlMemberInvoker(string Name) : XamlMemberInvoker
        {
            public override object GetValue(object instance) => ((TupleItem)instance)[Name];
            public override void SetValue(object instance, object value) => ((TupleItem)instance)[Name] = value;
        }

        public TupleItemXamlType(XamlSchemaContext schemaContext) : base(typeof(TupleItem), schemaContext) { }

        protected override IEnumerable<XamlMember> LookupAllMembers()
        {
            return TupleItem.AllKeys.Select(name => new TupleItemXamlMember(name, this));
        }
        protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck)
        {
            return new TupleItemXamlMember(name, this);
        }
    }
}
