using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManBox.Common.Logging;

namespace ManBox.Common.UnitTesting
{
    public class MockLogger : ILogger
    {
        public void Log(LogType logType, string message)
        {
            Console.WriteLine("{0} - {1}", logType, message);
        }

        public void Log(Exception e)
        {
            Console.WriteLine("{0} - {1}", e.GetType().Name, e.Message);
        }
    }
}
