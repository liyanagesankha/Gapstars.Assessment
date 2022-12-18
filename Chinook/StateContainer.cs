using Chinook.ClientModels;

namespace Chinook
{
    /// <summary>
    /// This class responsible for state management
    /// </summary>
    public class StateContainer
    {
        public IList<Playlist> UserPlaylists { get; set; } = new List<Playlist>();

        public void SetValueList(IList<Playlist> value)
        {
            if (value != null)
            {
                UserPlaylists = value;
                NotifyStateChange();
            }
        }

        private void NotifyStateChange() => OnChange?.Invoke();

        public event Action? OnChange;
    }
}
