﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Redis
{
    public interface IRedisContext
    {
        ConnectionMultiplexer ConnectionMultiplexer { get; set; }
        RedisSettings Settings { get; set; }
    }

    public class RedisSettings
    {
        public string Url { get; set; }
        public string Key { get; set; }
    }
}
