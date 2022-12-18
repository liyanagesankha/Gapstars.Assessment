using Chinook.ClientModels;
using Chinook.Services;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    public partial class Index : ComponentBase, IDisposable
    {
        private IList<Artist> artists;

        private string? artistNameSearchText { get; set; }

        [Inject] StateContainer StateContainer { get; set; }

        [Inject] IArtistService ArtistService { get; set; }

        [Inject] IUserPlayListService UserPlayListService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            StateContainer.OnChange += StateHasChanged;
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
            artists =  await ArtistService.GetAllAsync();
        }
                
        public void Dispose()
        {
            StateContainer.OnChange -= StateHasChanged;
        }
    }
}
