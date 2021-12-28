using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace CyberSaloon.Client.Helpers
{
    public class LocalStorage
    {
        private IJSRuntime _runtime;

        public LocalStorage(IJSRuntime runtime)
        {
            _runtime = runtime;    
        }
        
        public async Task SetValue<T>(string key, T value)
        {
            await _runtime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task<T> GetValue<T>(string key)
        {
            return await _runtime.InvokeAsync<T>("localStorage.getItem", key);
        }

        public async Task DeleteValue(string key)
        {
            await _runtime.InvokeAsync<string>("localStorage.removeItem", key);
        }
    }
}