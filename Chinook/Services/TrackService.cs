using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IAuthService _authService;

        public TrackService(IDbContextFactory<ChinookContext> contextFactory, IAuthService authService)
        {
            _dbFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<IList<PlaylistTrack>> GetAllByArtistIdAsync(long artistId)
        {
            if (artistId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(artistId));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            var currentUserId = await _authService.GetUserIdAsync();

            return await dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                 .Include(a => a.Album)
                 .Select(t => new PlaylistTrack()
                 {
                     AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                     TrackId = t.TrackId,
                     TrackName = t.Name,
                     IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.FavoritePlayListName)).Any()
                 }).ToListAsync();
        }
    }
}
