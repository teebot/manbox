using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ManBox.Common.Mail;

namespace ManBox.Common.Logging
{

    public class NLogLogger : ManBox.Common.Logging.ILogger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void Log(Exception e)
        { 
            var message = string.Format(@"
            Exception type: {0}
            ==============
            Message: {1}
            ==============
            Stack Trace: {2}
            ==============
            Inner Exception: {3}
            ", e.GetType().ToString(), e.Message, e.StackTrace, e.InnerException);

            Log(LogType.Error, message);
        }
        
        public void Log(LogType logType, string message)
        {
            LogLevel level;

            switch (logType) { 
                case LogType.Error:
                    level = LogLevel.Error;
                    break;
                case LogType.Fatal:
                    level = LogLevel.Fatal;
                    break;
                case LogType.Warn:
                    level = LogLevel.Warn;
                    break;
                default:
                    level = LogLevel.Info;
                    break;
            }

            logger.Log(level, message);
        }
    }
}
