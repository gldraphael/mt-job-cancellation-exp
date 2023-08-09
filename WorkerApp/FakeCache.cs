using System;
using System.Collections.Concurrent;

namespace WorkerApp;

public static class FakeCache
{
    private static ConcurrentDictionary<Guid, bool> CancellationRequests { get; set; } = new ();

    public static void AddCancellationRequest(Guid id) => CancellationRequests.TryAdd(id, true);
    public static bool CheckForCancellation(Guid id) => CancellationRequests.ContainsKey(id);
}
