using BlazorWords.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWords.Shared
{
    public class NavMenuBase : ComponentBase
    {
        [Inject]
        public IJSRuntime _JSRuntime { get; set; }

        protected Stats? stats;

        // JSInterop
        private async Task ShowStatsModal()
        {
            await _JSRuntime.InvokeVoidAsync("showModal", Misc.STATS_MODAL);
        }

        protected async Task ShowStats()
        {
            if(stats == null) return;
            await stats.ReloadStats();
            await ShowStatsModal();
        }
    }
}
