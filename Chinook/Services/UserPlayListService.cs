using Chinook.ClientModels;

namespace Chinook.Services
{
    public class UserPlayListService : IUserPlayListService
    {
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
                
        public Task<IList<Playlist>> GetAllAsync()
        {
            throw new NotImplementedException();
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
    }
}
