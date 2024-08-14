using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AC.SD.Core.Pages;
public class BasePage : ComponentBase, IDisposable
{
    private readonly IList<PersistingComponentStateSubscription> _subscriptions = new List<PersistingComponentStateSubscription>();

    [Inject]
    public PersistentComponentState ApplicationState { get; set; }

    protected async Task<TResult?> GetOrAddState<TResult>(string key, Func<Task<TResult?>> addStateFactory)
    {
        TResult? data = default;

        _subscriptions.Add(ApplicationState.RegisterOnPersisting(() =>
        {
            ApplicationState.PersistAsJson(key, data);
            return Task.CompletedTask;
        }));

        if (ApplicationState.TryTakeFromJson(key, out TResult? storedData))
        {
            data = storedData;
        }
        else
        {
            data = await addStateFactory.Invoke();
        }

        return data;
    }

    public void Dispose()
    {
        if (_subscriptions.Count > 0)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}

