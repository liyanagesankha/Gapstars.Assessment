using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Services
{
    public class AuthService : IAuthService
    {
        private AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new NullReferenceException(nameof(userId));
            }

            return userId;
        }
    }
}
