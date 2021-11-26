using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CachePerformance
{
    class Program
    {
        private const string Agreement = "Agreement";
        private const string TenantId = "perf-test-tenant";
        private static List<Agreement> agreements;
        private static IDatabase cache;

        static async Task Main(string[] args)
        {

            cache = RedisConnectorHelper.Connection.GetDatabase();

            while (true)
            {
                agreements = JsonSerializer.Deserialize<List<Agreement>>(File.ReadAllText(@"Data/Agreements.json"));
                await StringCacheGetSetRemoveNumber();
                Console.ReadLine();
            }
        }

        private static async Task StringCacheGetSetRemoveNumber()
        {
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine("-------------------------------------------");
            foreach (var a in agreements)
            {
                a.Id = Guid.NewGuid().ToString();
                var key = $"{TenantId}.{Agreement}:{a.Id}".ToLower();

                var keyslot = (long)await cache.ExecuteAsync($"CLUSTER", "KEYSLOT", key);

                stopwatch.Restart();
                await cache.StringSetAsync(key, JsonSerializer.Serialize(a));
                stopwatch.Stop();

                var shard = keyslot <= 8191 ? "shard 1" : "shard 2";
                Console.WriteLine($"keyslot - {keyslot} shard - {shard}");
                Console.WriteLine($"Key-{a.Id} 1 Record SET timing:{((stopwatch.ElapsedTicks * 1000000.0) / Stopwatch.Frequency)} microsecond");

                stopwatch.Restart();
                var data = await cache.StringGetAsync(key);
                stopwatch.Stop();

                Console.WriteLine($"Key-{a.Id} 1 Record GET timing:{((stopwatch.ElapsedTicks * 1000000.0) / Stopwatch.Frequency)} microsecond");

                stopwatch.Restart();
                await cache.KeyDeleteAsync(key);
                stopwatch.Stop();

                Console.WriteLine($"Key-{a.Id} 1 Record DELETE timing:{((stopwatch.ElapsedTicks * 1000000.0) / Stopwatch.Frequency)} microsecond\n");
            };
        }
    }
}
