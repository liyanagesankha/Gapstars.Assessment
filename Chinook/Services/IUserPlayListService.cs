using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IUserPlayListService
    {
        Task<long> AddAsync(Playlist playList);
        Task UpdateAsync(long id, Playlist playList);
        Task DeleteAsync(long id);
        Task<IList<Playlist>> GetAllAsync();
        Task<Playlist> GetByIdAsync(long id);
        Task<long> GetPlayListIdByNameAsync(string name);
        Task AddTrackAsync(long id, long trackId);
        Task RemoveTrackAsync(long id, long trackId);
    }
}
