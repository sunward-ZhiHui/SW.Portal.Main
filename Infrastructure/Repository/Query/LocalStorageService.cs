using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Application.Queries;
using Azure.Core;
using System.Data.Common;
using Application.Common;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Repositories.Query.Base;

namespace Infrastructure.Repository.Query
{
    public class LocalStorageService<T> : ILocalStorageService<T> where T : class
    {
        private IJSRuntime _jsRuntime;
        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T> GetItem<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");

            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        public async Task<string> GetItemOne(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "PushToken");
            //if (json == null)
            //    return default;
            //var result = JsonSerializer.Deserialize<ApplicationUser>(json).UserName;
            //return result;
            return json;
        }


        public async Task SetItem<T>(string key, T value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        public async Task RemoveItem(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

    }
}
