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
            artists = await GetArtistsAsync();
        }

        private async Task OnArtistNameSearchTextChange(ChangeEventArgs args)
        {
            var value = args?.Value;
            if (value is null)
            {
                return;
            }

            artistNameSearchText = value.ToString();
            artists = await GetArtistsAsync(artistNameSearchText);
        }
        public void Dispose()
        {
            StateContainer.OnChange -= StateHasChanged;
        }

        private async Task<IList<Artist>> GetArtistsAsync(string? searchText = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return await ArtistService.GetAllAsync() ?? new List<Artist>();
            }

            return await ArtistService.GetAllFilterByNameAsync(searchText);
        }
    }
}
