using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AC.SD.Core.Pages
{
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

            // Try to get data from state, if not found, use the factory method to get it
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
            // Dispose of all subscriptions
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            // Clear the subscription list to release memory
            _subscriptions.Clear();

            // Optionally trigger garbage collection
            TriggerGarbageCollection();
        }

        private void TriggerGarbageCollection()
        {
            // Force garbage collection
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
    }
}
