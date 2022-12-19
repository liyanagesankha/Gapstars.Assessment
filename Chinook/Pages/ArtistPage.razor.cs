using Chinook.ClientModels;
using Chinook.Services;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    public partial class ArtistPage : ComponentBase, IDisposable
    {
        [Parameter] public long ArtistId { get; set; }
        [Inject] IUserPlayListService UserPlayListService { get; set; }
        [Inject] IArtistService ArtistService { get; set; }
        [Inject] ITrackService TrackService { get; set; }
        [Inject] StateContainer StateContainer { get; set; }

        private Modal PlaylistDialog { get; set; }
        private string NewPlaylistName { get; set; }
        private long SelectedPlaylistId { get; set; } = -1;

        private Artist Artist;
        private IList<PlaylistTrack> Tracks;
        private PlaylistTrack SelectedTrack;

        private string? InfoMessage;
        private string? ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            StateContainer.OnChange += StateHasChanged;
            Artist = await ArtistService.GetByIdAsync(ArtistId);
            Tracks = await TrackService.GetAllByArtistIdAsync(ArtistId);
        }

        public void Dispose()
        {
            StateContainer.OnChange -= StateHasChanged;
        }

        private void SelectedPlayListChanged(ChangeEventArgs args)
        {
            var value = args.Value;
            if (value is null)
            {
                return;
            }

            SelectedPlaylistId = long.Parse(value.ToString());
            NewPlaylistName = string.Empty;
            ErrorMessage = string.Empty;

        }

        private void NewPlaylistNameOnInput(ChangeEventArgs args)
        {
            var value = args?.Value;
            if (value is null)
            {
                return;
            }

            ErrorMessage = string.Empty;
            SelectedPlaylistId = -1;
            NewPlaylistName = value.ToString();

            if (StateContainer.UserPlaylists.Any() && StateContainer.UserPlaylists.Where(p => p.Name.Equals(NewPlaylistName, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                ErrorMessage = Constants.PlaylistNameExistMessage;
            }
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
            var track = Tracks.First(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist {favoritePlayName}.";
            Tracks = await TrackService.GetAllByArtistIdAsync(ArtistId);
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
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
            var track = Tracks.First(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist {Constants.FavoritePlayListName}.";
            Tracks = await TrackService.GetAllByArtistIdAsync(ArtistId);
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
        }

        private void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.First(t => t.TrackId == trackId);
            PlaylistDialog.Open();
        }

        private async Task AddTrackToPlaylistAsync()
        {
            CloseInfoMessage();
            var selectedPlayListId = SelectedPlaylistId;
            var playListName = string.Empty;
            if (selectedPlayListId < 1)
            {
                playListName = NewPlaylistName;
                if (string.IsNullOrWhiteSpace(playListName))
                {
                    throw new NullReferenceException(nameof(playListName));
                }

                if (UserPlayListService.GetPlayListIdByNameAsync(playListName).Result > 0)
                {
                    ErrorMessage = Constants.PlaylistNameExistMessage;
                    return;
                }

                selectedPlayListId = await UserPlayListService.AddAsync(new Playlist { Name = playListName });
            }
            else
            {
                playListName = StateContainer.UserPlaylists.First(p => p.Id == selectedPlayListId).Name;
                NewPlaylistName = string.Empty;
            }

            StateContainer.SetValueList(await UserPlayListService.GetAllAsync());
            await UserPlayListService.AddTrackAsync(selectedPlayListId, SelectedTrack.TrackId);
            InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {playListName}.";

            NewPlaylistName = string.Empty;
            SelectedPlaylistId = -1;
            Tracks = await TrackService.GetAllByArtistIdAsync(ArtistId);
            StateContainer.SetValueList(UserPlayListService.GetAllAsync().Result);
            PlaylistDialog.Close();
        }

        private void CloseInfoMessage()
        {
            InfoMessage = string.Empty;
        }
    }
}
