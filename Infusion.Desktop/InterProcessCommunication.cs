using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using Infusion.LegacyApi;

namespace Infusion.Desktop
{
    internal static class InterProcessCommunication
    {
        private static readonly EventWaitHandle MessageSentEvent = new EventWaitHandle(false, EventResetMode.ManualReset,
            "Infusion.Desktop.CommandMessageSent");

        public static void StartReceiving()
        {
            var receivingThread = new Thread(ReceivingLoop);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        private static void ReceivingLoop(object data)
        {
            MemoryMappedFile messageFile =
                MemoryMappedFile.CreateOrOpen("Infusion.Desktop.CommandMessages", 2048);

            while (true)
            {
                MessageSentEvent.WaitOne();

                string command;

                using (var stream = messageFile.CreateViewStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        command = reader.ReadLine();
                    }
                }

                if (!string.IsNullOrEmpty(command))
                {
                    if (command.StartsWith(","))
                        Legacy.CommandHandler.Invoke(command);
                    else
                        Legacy.CommandHandler.Invoke("," + command);
                }
            }
        }

        public static void SendCommand(string command)
        {
            MemoryMappedFile messageFile =
                MemoryMappedFile.CreateOrOpen("Infusion.Desktop.CommandMessages", 2048);

            using (var stream = messageFile.CreateViewStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(command);
                    writer.Flush();
                }
            }

            MessageSentEvent.Set();
            MessageSentEvent.Reset();
        }
    }
}