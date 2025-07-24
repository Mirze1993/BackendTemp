using System.Collections.Concurrent;
using AiAgentApi.Hubs.Models;

namespace AiAgentApi.Hubs;

public class InMemory
{
    public static ConcurrentDictionary<string, InMemoryModel> ActiveUser { get; private set; } = new();
}