// <copyright file="OpenTelemetryLoggerProvider.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OpenTelemetry.Logs
{
    [ProviderAlias("OpenTelemetry")]
    public class OpenTelemetryLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IOptionsMonitor<OpenTelemetryLoggerOptions> options;
        private readonly IDictionary<string, ILogger> loggers;
        private IExternalScopeProvider scopeProvider;

        public OpenTelemetryLoggerProvider(IOptionsMonitor<OpenTelemetryLoggerOptions> options)
        {
            this.options = options;
            this.loggers = new Dictionary<string, ILogger>(StringComparer.Ordinal);
        }

        internal IExternalScopeProvider ScopeProvider
        {
            get
            {
                if (this.scopeProvider == null)
                {
                    this.scopeProvider = new LoggerExternalScopeProvider();
                }

                return this.scopeProvider;
            }
        }

        void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            // TODO: set existing loggers
            this.scopeProvider = scopeProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            lock (this.loggers)
            {
                ILogger logger;

                if (this.loggers.TryGetValue(categoryName, out logger))
                {
                    return logger;
                }

                logger = new OpenTelemetryLogger(categoryName, this.options.CurrentValue);
                this.loggers.Add(categoryName, logger);
                return logger;
            }
        }

        public void Dispose()
        {
        }
    }
}
#endif
