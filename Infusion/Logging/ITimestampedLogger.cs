using System;

namespace Infusion.Logging
{
    public interface ITimestampedLogger
    {
        void Info(DateTime logTime, string message);
        void Important(DateTime logTime, string message);
        void Debug(DateTime logTime, string message);
        void Critical(DateTime logTime, string message);
        void Error(DateTime logTime, string message);
    }
}