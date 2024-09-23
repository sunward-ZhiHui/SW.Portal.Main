using System;
using System.Threading.Tasks;

public class DataRefreshService
{
    public event Func<Task> OnRefreshRequested;
    public event Func<Task> OnRefreshEmailHeader;

    public async Task RequestRefreshAsync()
    {
        if (OnRefreshRequested != null)
        {
            await OnRefreshRequested.Invoke();
        }
    }

    public async Task RequestRefreshEmailHeaderAsync()
    {
        if (OnRefreshEmailHeader != null)
        {
            await OnRefreshEmailHeader.Invoke();
        }
    }
}
