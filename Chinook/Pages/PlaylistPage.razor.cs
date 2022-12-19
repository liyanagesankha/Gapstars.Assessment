using Chinook.ClientModels;
using Chinook.Services;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    public partial class PlaylistPage : ComponentBase, IDisposable
    {
        [Parameter] public long PlaylistId { get; set; }
        [Inject] IUserPlayListService UserPlayListService { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] StateContainer StateContainer { get; set; }

        private Playlist Playlist;
        private string InfoMessage;
        private bool isPlayListRenaming = false;

        protected override async Task OnParametersSetAsync()
        {
            StateContainer.OnChange += StateHasChanged;
            Playlist = StateContainer.UserPlaylists.First(p => p.Id == PlaylistId);
        }

        private async Task FavoriteTrack(long trackId)
        {
            var favoritePlayName = Constants.FavoritePlayListName;
            var favoritePlayListId = await UserPlayListService.GetPlayListIdByNameAsync(favoritePlayName);
            if (favoritePlayListId < 1)
            {
                favoritePlayListId = await UserPlayListService.AddAsync(new Playlist { Name = favoritePlayName });
            }

            await UserPlayListService.AddTrackAsync(favoritePlayListId, trackId);
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist {favoritePlayName}.";
        }

        private async Task UnfavoriteTrack(long trackId)
        {
            var favoritePlayName = Constants.FavoritePlayListName;
            var favoritePlayListId = await UserPlayListService.GetPlayListIdByNameAsync(favoritePlayName);
            if (favoritePlayListId < 1)
            {
                throw new ArgumentException();
            }

            await UserPlayListService.RemoveTrackAsync(favoritePlayListId, trackId);
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
        }

        private async Task RemoveTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            await UserPlayListService.RemoveTrackAsync(PlaylistId, trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist {Playlist.Name}.";
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
        }

        private async Task RemovePlayList()
        {
            InfoMessage = $"PlayList {Playlist.Name} is removing..";
            await Task.Delay(2000);
            await UserPlayListService.DeleteAsync(PlaylistId);
            NavManager.NavigateTo("/");
        }

        private async Task RenamePlayListName()
        {
            await UserPlayListService.UpdateAsync(PlaylistId, Playlist);
            InfoMessage = $"PlayList: {Playlist.Name} name updated successfully.";
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
            isPlayListRenaming = false;
        }

        private void CloseInfoMessage()
        {
            InfoMessage = "";
        }
        public void Dispose()
        {
            StateContainer.OnChange -= StateHasChanged;
        }
    }
}
