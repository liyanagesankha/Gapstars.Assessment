using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class UserPlayListService : IUserPlayListService
    {
        #region Private Variables

        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IAuthService _authService;

        #endregion Private Variables

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contextFactory">DB Context Factory</param>
        /// <param name="authService">Authenticated User Service</param>
        /// <exception cref="ArgumentNullException">Nullable parameters</exception>
        public UserPlayListService(IDbContextFactory<ChinookContext> contextFactory, IAuthService authService)
        {
            _dbFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public Task<long> AddAsync(Playlist playList)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(long id, Playlist playList)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
                
        public async Task<IList<Playlist>> GetAllAsync()
        {
            var currentUserId = await _authService.GetUserIdAsync();
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.UserPlaylists
                .Where(p => p.UserId == currentUserId)
                .Include(p => p.Playlist.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Select(p => new ClientModels.Playlist()
                {
                    Id = p.PlaylistId,
                    Name = p.Playlist.Name,
                    Tracks = p.Playlist.Tracks.Select(t => new ClientModels.PlaylistTrack()
                    {
                        AlbumTitle = t.Album.Title,
                        ArtistName = t.Album.Artist.Name,
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.FavoritePlayListName)).Any()
                    }).ToList()
                })
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public Task<Playlist> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetPlayListIdByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task AddTrackAsync(long id, long trackId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTrackAsync(long id, long trackId)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}
