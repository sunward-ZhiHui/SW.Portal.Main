using System;
using System.Threading.Tasks;

public class DataRefreshService
{
    public event Func<Task> OnRefreshRequested;    

    public async Task RequestRefreshAsync()
    {
        if (OnRefreshRequested != null)
        {
            await OnRefreshRequested.Invoke();
        }
    }
   
}
