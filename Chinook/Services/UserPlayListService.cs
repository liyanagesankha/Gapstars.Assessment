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

        public async Task<long> AddAsync(ClientModels.Playlist playList)
        {
            if (playList == null)
            {
                throw new ArgumentNullException(nameof(playList));
            }

            var newPlayListName = playList.Name;
            if (string.IsNullOrWhiteSpace(newPlayListName))
            {
                throw new ArgumentNullException(nameof(newPlayListName));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            var currentUserId = await _authService.GetUserIdAsync();
            var newPlayListId = await dbContext.Playlists.Select(p => p.PlaylistId).MaxAsync() + 1;
            var newPlayList = new Models.Playlist
            {
                PlaylistId = newPlayListId,
                Name = newPlayListName
            };

            var userPlaylist = new Models.UserPlaylist
            {
                Playlist = newPlayList,
                UserId = currentUserId
            };

            await dbContext.Playlists.AddAsync(newPlayList);
            await dbContext.UserPlaylists.AddAsync(userPlaylist);
            await dbContext.SaveChangesAsync();
            return newPlayListId;
        }

        public async Task UpdateAsync(long id, ClientModels.Playlist playList)
        {
            throw new NotImplementedException(); 
        }

        public async Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ClientModels.Playlist>> GetAllAsync()
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

        public async Task<long> GetPlayListIdByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            var currentUserId = await _authService.GetUserIdAsync();
            return await dbContext.UserPlaylists
                .Where(p => p.UserId == currentUserId && p.Playlist.Name == name)
                .Select(p => p.PlaylistId)
                .FirstOrDefaultAsync();
        }

        public async Task AddTrackAsync(long id, long trackId)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (trackId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(trackId));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            var playList = await dbContext.Playlists.FindAsync(id);
            if (playList == null)
            {
                throw new NullReferenceException(nameof(playList));
            }

            var track = await dbContext.Tracks.FindAsync(trackId);
            if (track == null)
            {
                throw new NullReferenceException(nameof(track));
            }
            playList.Tracks.Add(track);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTrackAsync(long id, long trackId)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (trackId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(trackId));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            var playList = await dbContext.Playlists.Where(p => p.PlaylistId == id).Include(p => p.Tracks).SingleAsync();

            var track = await dbContext.Tracks.FindAsync(trackId);
            if (track == null)
            {
                throw new NullReferenceException(nameof(track));
            }

            playList.Tracks.Remove(track);
            await dbContext.SaveChangesAsync();
        }

        #endregion Public Methods
    }
}
