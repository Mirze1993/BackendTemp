using System.Collections.Concurrent;
using AuthApi.Hubs.Models;

namespace AuthApi.Hubs;

public class InMemory
{
    public static ConcurrentDictionary<string, InMemoryModel> ActiveUser { get; private set; } = new();
}