using StackExchange.Redis;
using System;

namespace CachePerformance
{
    public class RedisConnectorHelper
    {
        static RedisConnectorHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("platformcache.vpzwfu.clustercfg.usw1.cache.amazonaws.com:6379,allowAdmin=True");
                //return ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=True");
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
