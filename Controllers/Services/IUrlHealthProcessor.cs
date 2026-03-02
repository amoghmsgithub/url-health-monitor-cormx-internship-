namespace UrlHealthMonitor.Services
{
    public interface IUrlHealthProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}