using Microsoft.AspNetCore.Components;

namespace Chinook.Shared
{
    public partial class NavMenu : ComponentBase, IDisposable
    {
        [Inject] StateContainer StateContainer { get; set; }

        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            StateContainer.OnChange += StateHasChanged;
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public void Dispose()
        {
            StateContainer.OnChange -= StateHasChanged;
        }
    }
}
