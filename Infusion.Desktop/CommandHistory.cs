using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop
{
    public class CommandHistory
    {
        private readonly int maxHistoryLength;
        private readonly Queue<string> history;
        private int currentHistoryIndex;

        public CommandHistory(int maxHistoryLength = 128)
        {
            this.maxHistoryLength = maxHistoryLength;
            history = new Queue<string>(maxHistoryLength);
        }

        public void EnterCommand(string command)
        {
            if (history.Count == maxHistoryLength)
                history.Dequeue();

            history.Enqueue(command);

            currentHistoryIndex = history.Count;
        }

        public string GetOlder()
        {
            if (currentHistoryIndex == 0)
                return null;

            currentHistoryIndex--;
            return history.ElementAt(currentHistoryIndex);
        }

        public string GetNewer()
        {
            if (currentHistoryIndex >= history.Count - 1)
                return null;

            currentHistoryIndex++;
            return history.ElementAt(currentHistoryIndex);
        }
    }
}
