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

using SkyWalking.Context.Trace;

namespace SkyWalking.Context
{
    public interface ITracerContext
    {
        void Inject(IContextCarrier carrier);

        void Extract(IContextCarrier carrier);
        
        IContextSnapshot Capture { get; }
        
        ISpan ActiveSpan { get; }

        void Continued(IContextSnapshot snapshot);

        string GetReadableGlobalTraceId();

        ISpan CreateEntrySpan(string operationName);

        ISpan CreateLocalSpan(string operationName);

        ISpan CreateExitSpan(string operationName, string remotePeer);     

        void StopSpan(ISpan span);
    }
}
