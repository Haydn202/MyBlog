using API.Data;
using API.DTOs;
using API.DTOs.Accounts;
using API.DTOs.User;
using API.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Users.Command;

public class UpdateRole(RoleUpdateDto request, Guid id) : IRequest<UserDto>
{
    public RoleUpdateDto Request { get; } = request;
    private Guid id { get; } = id;

    private sealed class UpdateRoleHandler(
        DataContext dbContext, 
        IMapper mapper)
        : IRequestHandler<UpdateRole, UserDto>
    {
        public async Task<UserDto> Handle(
            UpdateRole request, 
            CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => 
                x.Id == request.id, cancellationToken: cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }
            
            Enum.TryParse<Role>(request.Request.Role, true, out var roleEnum);

            user.Role = roleEnum;
            await dbContext.SaveChangesAsync(cancellationToken);

            var userDto = mapper.Map<UserDto>(user);

            return userDto;
        }
    }
}

public class UpdateRoleValidator : AbstractValidator<UpdateRole>
{
    public UpdateRoleValidator()
    {
        RuleFor(u => u.Request.Role)
            .Must(r => Enum.TryParse<Role>(r, true, out var roleEnum))
            .WithMessage("The provided role is not a valid.");
    }
}