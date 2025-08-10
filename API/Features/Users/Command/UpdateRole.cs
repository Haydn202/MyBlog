using API.Data;
using API.DTOs;
using API.DTOs.Accounts;
using API.DTOs.User;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Users.Command;

public class UpdateRole(RoleUpdateDto request, string id) : IRequest<UserDto>
{
    public RoleUpdateDto Request { get; } = request;
    public string Id { get; } = id;

    private sealed class UpdateRoleHandler(
        UserManager<User> userManager,
        IMapper mapper)
        : IRequestHandler<UpdateRole, UserDto>
    {
        public async Task<UserDto> Handle(
            UpdateRole request, 
            CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            await userManager.AddToRoleAsync(user, request.Request.Role);
            
            return mapper.Map<UserDto>(user);
        }
    }
}

public class UpdateRoleValidator : AbstractValidator<UpdateRole>
{
    UserManager<User> _userManager;
    
    public UpdateRoleValidator(UserManager<User> userManager)
    {
        _userManager = userManager;
        RuleFor(u => u.Request.Role)
            .Must(r => Enum.TryParse<Role>(r, true, out var roleEnum))
            .WithMessage("The provided role is not a valid.");
        
        RuleFor(u => u.Id)
            .MustAsync(async (id, _) => await UserExists(id))
            .WithMessage("The user does not exist.");
    }

    private async Task<bool> UserExists(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user is not null;
    }
}