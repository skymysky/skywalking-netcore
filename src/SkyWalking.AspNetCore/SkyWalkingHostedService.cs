﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SkyWalking.AspNetCore.Diagnostics;
using SkyWalking.Boot;
using SkyWalking.Config;
using SkyWalking.Remote;

namespace SkyWalking.AspNetCore
{
    public class SkyWalkingHostedService : IHostedService
    {
        private readonly IEnumerable<ITracingDiagnosticListener> _tracingDiagnosticListeners;
        private readonly DiagnosticListener _diagnosticListener;
        
        public SkyWalkingHostedService(IOptions<SkyWalkingOptions> options, IHostingEnvironment hostingEnvironment,
            IEnumerable<ITracingDiagnosticListener> tracingDiagnosticListeners, DiagnosticListener diagnosticListener)
        {
            if (string.IsNullOrEmpty(options.Value.DirectServers))
            {
                throw new ArgumentException("DirectServers cannot be empty or null.");
            }

            if (string.IsNullOrEmpty(options.Value.ApplicationCode))
            {
                options.Value.ApplicationCode = hostingEnvironment.ApplicationName;
            }

            AgentConfig.ApplicationCode = options.Value.ApplicationCode;
            CollectorConfig.DirectServers = options.Value.DirectServers;

            _tracingDiagnosticListeners = tracingDiagnosticListeners;
            _diagnosticListener = diagnosticListener;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await GrpcChannelManager.Instance.ConnectAsync();
            await ServiceManager.Instance.Initialize();
            foreach (var tracingDiagnosticListener in _tracingDiagnosticListeners)
                _diagnosticListener.SubscribeWithAdapter(tracingDiagnosticListener);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await GrpcChannelManager.Instance.ShutdownAsync();
            ServiceManager.Instance.Dispose();
        }
    }
}