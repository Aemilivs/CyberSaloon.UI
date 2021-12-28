using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace CyberSaloon.Client.Helpers
{
    public class SessionStorage
    {
        private IJSRuntime _runtime;

        public SessionStorage(IJSRuntime runtime)
        {
            _runtime = runtime;    
        }
        
        public async Task SetValue<T>(string key, T value)
        {
            await _runtime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }

        public async Task<T> GetValue<T>(string key)
        {
            return await _runtime.InvokeAsync<T>("sessionStorage.getItem", key);
        }

        public async Task DeleteValue(string key)
        {
            await _runtime.InvokeAsync<string>("sessionStorage.removeItem", key);
        }
    }
}