﻿using kafka_stream_core.Processors;
using System;
using System.Collections.Generic;
using System.Text;

namespace kafka_stream_core.State
{
    public interface StoreBuilder
    {
        IDictionary<string, string> LogConfig { get; }
        bool LoggingEnabled { get; }
        string Name { get; }
        object build();
    }

    public interface StoreBuilder<T>  : StoreBuilder
        where T : IStateStore
    {
        StoreBuilder<T> WithCachingEnabled();
        StoreBuilder<T> WithCachingDisabled();
        StoreBuilder<T> WithLoggingEnabled(IDictionary<String, String> config);
        StoreBuilder<T> WithLoggingDisabled();
        T Build();
    }
}