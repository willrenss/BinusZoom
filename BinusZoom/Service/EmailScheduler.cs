namespace BinusZoom.Service;

public class EmailScheduler: BackgroundService
{
    private Task myTask;
    public TimeSpan Delay { get; set; }

    public EmailScheduler(Task myTask, TimeSpan delay)
    {
        this.myTask = myTask;
        this.Delay = delay;
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken stoppingToken)
    {
        // WIP
        await Task.Delay(Delay, stoppingToken);
        myTask.Start();
        return myTask.IsCompleted;
    }
}