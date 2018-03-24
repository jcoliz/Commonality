using System;
using System.Collections.Generic;
using System.Linq;

namespace Commonality.Test.Helpers
{
    public class TestSettingsManager : ISettings
    {
        Dictionary<string, IEnumerable<string>> Values = new Dictionary<string, IEnumerable<string>>();

        public IEnumerable<string> GetCompositeKey(string name)
        {
            if (Values.ContainsKey(name))
                return Values[name];
            else
                return null;
        }

        public string GetKey(string name)
        {
            if (Values.ContainsKey(name))
                return Values[name].First();
            else
                return null;
        }

        public void SetCompositeKey(string name, IEnumerable<string> values)
        {
            Values[name] = values;
        }

        public void SetKey(string name, string value)
        {
            Values[name] = new[] { value };
        }
    }
}
