using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class Menus
    {
        private readonly Queue<Tuple<string, string>> waiting = new Queue<Tuple<string, string>>();
        private readonly DialogBoxObservers dialogBoxObservers;
        private readonly object menuLock = new object();

        public Menus(DialogBoxObservers dialogBoxObservers)
        {
            this.dialogBoxObservers = dialogBoxObservers;

            dialogBoxObservers.DialogBoxOpened += HandleDialogBoxOpened;
        }

        private void HandleDialogBoxOpened(DialogBox dialogBox)
        {
            lock (menuLock)
            {
                if (!waiting.Any())
                    return;
                var current = waiting.Peek();
                if (dialogBox.Question.IndexOf(current.Item1, StringComparison.OrdinalIgnoreCase) < 0)
                    return;

                waiting.Dequeue();
                if (waiting.Any())
                    dialogBoxObservers.BlockQuestion(waiting.Peek().Item1);

                var response = dialogBox.Responses.FirstOrDefault(x => x.Text.Equals(current.Item2, StringComparison.Ordinal));
                if (response == null)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(25);
                        dialogBoxObservers.UnblockQuestion();
                        waiting.Clear();
                        dialogBoxObservers.CloseDialogBox();
                    });
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(25);
                        dialogBoxObservers.TriggerDialogBox(response.Index);
                    });
                }
            }
        }

        internal void WaitMenu(string[] parameters)
        {
            if (!parameters.Any())
                return;

            if ((parameters.Length % 2) != 0)
                throw new ArgumentException($"Invalid number of parameters {parameters.Length}, parameters must come in pairs.", nameof(parameters));

            lock (menuLock)
            {
                waiting.Clear();

                dialogBoxObservers.BlockQuestion(parameters[0]);
                for (int i = 0; i < parameters.Length; i += 2)
                {
                    waiting.Enqueue(new Tuple<string, string>(parameters[i], parameters[i + 1]));
                }
            }
        }
    }
}
