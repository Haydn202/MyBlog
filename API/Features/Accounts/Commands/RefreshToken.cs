using API.DTOs.Accounts;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Accounts.Commands;

public class RefreshToken(RefreshTokenCommandRequest request) : IRequest<UserDto?>
{
    public RefreshTokenCommandRequest Request { get; set; } = request;
    
    private sealed class RefreshTokenHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IMapper mapper) : IRequestHandler<RefreshToken, UserDto?>
    {
        public async Task<UserDto?> Handle(RefreshToken request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == request.Request.RefreshToken 
                    && x.RefreshTokenExpires > DateTime.UtcNow, cancellationToken);

            if (user is null)
            {
                return null;
            }

            // Generate new tokens
            var newAccessToken = await tokenService.CreateToken(user);
            var newRefreshToken = await tokenService.CreateRefreshToken();
            
            // Update user's refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var userDto = mapper.Map<UserDto>(user);
            userDto.Token = newAccessToken;
            userDto.RefreshToken = newRefreshToken;
            
            return userDto;
        }
    }
}

