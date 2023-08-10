using System;
using System.Threading.Tasks;

namespace WorkerApp
{
    public interface IJobCache
    {
        Task AddCancellationRequest(Guid jobId);
        Task<bool> CheckForCancellation(Guid jobId);
    }
}
