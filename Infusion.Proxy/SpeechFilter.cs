using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.TextFilters;

namespace Infusion.Proxy
{
    public class SpeechFilter
    {
        private readonly Configuration configuration;
        private ITextFilter[] speechFilters;

        public SpeechFilter(Configuration configuration)
        {
            this.configuration = configuration;
            configuration.PropertyChanged += Configuration_PropertyChanged;

            ParseFilters(configuration.IgnoredWords);
        }

        private void ParseFilters(IEnumerable<string> ignoredWords)
        {
            List<ITextFilter> filters = new List<ITextFilter>();

            foreach (var words in ignoredWords)
            {
                filters.Add(TextFilterSpecificationParser.Parse(words));
            }

            speechFilters = filters.ToArray();
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IgnoredWords")
            {
                ParseFilters(configuration.IgnoredWords);
            }
        }

        public bool IsPassing(string text) => speechFilters.All(f => f.IsPassing(text));
    }
}
