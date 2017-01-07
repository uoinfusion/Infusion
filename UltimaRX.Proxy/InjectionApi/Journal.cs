using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace UltimaRX.Proxy.InjectionApi
{
    public class Journal : IEnumerable<string>
    {
        private static readonly AutoResetEvent ReceivedAwaitedWordsEvent = new AutoResetEvent(false);
        private static string[] awaitingWords = {};
        private ImmutableList<string> journal = ImmutableList<string>.Empty;

        public IEnumerator<string> GetEnumerator() => journal.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => journal.GetEnumerator();

        public bool InJournal(params string[] words) => journal.Any(line => words.Any(w => line.Contains(w)));

        public void DeleteJournal()
        {
            journal = ImmutableList<string>.Empty;
        }

        public void WaitForJournal(params string[] words)
        {
            awaitingWords = words;

            ReceivedAwaitedWordsEvent.Reset();
            ReceivedAwaitedWordsEvent.WaitOne();

            awaitingWords = new string[] {};
        }

        public void AddMessage(string message)
        {
            journal = journal.Add(message);

            if (awaitingWords.Any(w => message.Contains(w)))
            {
                ReceivedAwaitedWordsEvent.Set();
            }
        }
    }
}