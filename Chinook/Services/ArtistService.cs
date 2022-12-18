using Chinook.ClientModels;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        public Task<IList<Artist>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Artist>> GetAllFilterByNameAsync(string searchText)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> GetByIdAsync(long artistId)
        {
            throw new NotImplementedException();
        }
    }
}
