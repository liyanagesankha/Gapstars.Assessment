using Chinook.ClientModels;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        public Task<IList<PlaylistTrack>> GetAllByArtistIdAsync(long artistId)
        {
            throw new NotImplementedException();
        }
    }
}
