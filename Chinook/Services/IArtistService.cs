using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<IList<Artist>> GetAllAsync();
        Task<Artist> GetByIdAsync(long artistId);
        Task<IList<Artist>> GetAllFilterByNameAsync(string searchText);
    }
}
