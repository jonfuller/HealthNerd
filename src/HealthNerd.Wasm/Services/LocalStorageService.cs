using Microsoft.JSInterop;
using Newtonsoft.Json;
using HealthNerd.Wasm.Models;

namespace HealthNerd.Wasm.Services;

public class LocalStorageService(IJSRuntime js)
{
    private const string Key = "healthnerd_settings";

    public async Task<WebSettings> LoadAsync()
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", Key);
        if (json is null) return new WebSettings();
        return JsonConvert.DeserializeObject<WebSettings>(json) ?? new WebSettings();
    }

    public async Task SaveAsync(WebSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings);
        await js.InvokeVoidAsync("localStorage.setItem", Key, json);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", Key);
    }
}
