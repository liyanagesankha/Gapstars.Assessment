using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public ArtistService(IDbContextFactory<ChinookContext> contextFactory)
        {
            _dbFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<IList<Artist>> GetAllAsync()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Artists
                .Include(a => a.Albums)
                .Select(a => new Artist()
                {
                    Id = a.ArtistId,
                    Name = a.Name,
                    AlbumCount = a.Albums.Count
                }).ToListAsync();
        }

        public async Task<IList<Artist>> GetAllFilterByNameAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentNullException(nameof(searchText));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Artists
                .Include(a => a.Albums)
                .Where(a => !string.IsNullOrWhiteSpace(a.Name) && a.Name.Contains(searchText))
                .Select(a => new Artist()
                {
                    Id = a.ArtistId,
                    Name = a.Name,
                    AlbumCount = a.Albums.Count
                }).ToListAsync();
        }

        public async Task<Artist> GetByIdAsync(long artistId)
        {
            if (artistId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(artistId));
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Artists
                .Include(a => a.Albums)
                .Where(a => a.ArtistId == artistId)
                .Select(a => new Artist()
                {
                    Id = a.ArtistId,
                    Name = a.Name,
                    AlbumCount = a.Albums.Count
                }).SingleAsync();
        }
    }
}
