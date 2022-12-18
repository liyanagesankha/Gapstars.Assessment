using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface ITrackService
    {
        Task<IList<PlaylistTrack>> GetAllByArtistIdAsync(long artistId);
    }
}
