﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------
// <copyright file="Loggly.cs">
// Copyright 2012 Joe Fitzgerald
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

using NLog.Config;
using LogglyLogger = Loggly.Logger;

namespace NLog.Targets
{
    using System.Linq;

    /// <summary>
    /// A Loggly NLog Target
    /// </summary>
    [Target("Loggly")]
    public class Loggly : NLog.Targets.TargetWithLayout
    {
        private LogglyLogger logger;

        private string inputKey;

        public Loggly()
        {

        }

        [RequiredParameter]
        public string InputKey
        {
            get
            {
                return this.inputKey;
            }
            set
            {
                this.inputKey = value;
                this.logger = new LogglyLogger(this.inputKey);
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = this.Layout.Render(logEvent);
            if (logEvent.Properties != null && logEvent.Properties.Count > 0)
            {
                //this.logger.Log(logMessage, logEvent.Level.Name, logEvent.Properties.ToDictionary(k => k.Key != null ? k.Key.ToString() : string.Empty, v => v.Value));
                this.logger.LogJson<LogEventInfo>(logEvent);
            }
            else
            {
                this.logger.Log(logMessage, logEvent.Level.Name);
            }
        }
    }
}
