using System;
namespace ManBox.Common.Logging
{
    public interface ILogger
    {
        void Log(LogType logType, string message);
        void Log(Exception e);
    }
}
