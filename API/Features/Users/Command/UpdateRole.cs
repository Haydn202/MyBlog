using API.Data;
using API.DTOs.Accounts;
using API.DTOs.User;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Users.Command;

public class UpdateRole(RoleUpdateDto request, Guid id) : IRequest<UserDto>
{
    private RoleUpdateDto Request { get; } = request;
    private Guid id { get; } = id;

    private sealed class UpdateRoleHandler(DataContext dbContext, IMapper mapper)
        : IRequestHandler<UpdateRole, UserDto?>
    {
        public async Task<UserDto?> Handle(UpdateRole request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => 
                x.Id == request.id, cancellationToken: cancellationToken);

            if (user is null)
            {
                return null;
            }
            
            // add validation for this instead of defaulting role
            Enum.TryParse<Entities.Role>(request.Request.Role, true, out var roleEnum);

            user.Role = roleEnum;
            await dbContext.SaveChangesAsync(cancellationToken);

            return mapper.Map<UserDto>(user);
        }
    }
}