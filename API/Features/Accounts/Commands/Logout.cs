using API.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Features.Accounts.Commands;

public class Logout(string userId) : IRequest<bool>
{
    public string UserId { get; set; } = userId;
    
    private sealed class LogoutHandler(
        UserManager<User> userManager) : IRequestHandler<Logout, bool>
    {
        public async Task<bool> Handle(Logout request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            
            if (user is null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpires = null;
            await userManager.UpdateAsync(user);
            
            return true;
        }
    }
}

