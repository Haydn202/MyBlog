# UpdatePost Controller Pattern Analysis

## Overview
The UpdatePost controller method demonstrates a well-structured CQRS (Command Query Responsibility Segregation) pattern using MediatR, AutoMapper, and FluentValidation. This document outlines the complete pattern to ensure consistency across all controller methods.

## Architecture Components

### 1. Controller Layer (`PostsController.cs`)
```csharp
[Authorize(Roles = "Admin")]
[HttpPut("{id:guid}")]
public async Task<ActionResult<PostSummaryDto>> UpdatePost(PostUpdateDto dto, [FromRoute] Guid id)
{
    var command = new UpdatePost(mapper.Map<UpdatePostCommandRequest>((dto, id)));
    var response = await sender.Send(command);
    
    return Ok(response);
}
```

**Key Characteristics:**
- **Authorization**: Uses `[Authorize(Roles = "Admin")]` for role-based access control
- **HTTP Method**: `[HttpPut]` with route parameter `{id:guid}`
- **Parameters**: 
  - `PostUpdateDto dto` - Data transfer object from request body
  - `[FromRoute] Guid id` - Route parameter for entity identification
- **Return Type**: `ActionResult<PostSummaryDto>` for consistent response formatting
- **Dependency Injection**: Uses constructor injection for `IMapper`, `DataContext`, and `ISender`

### 2. DTO Layer (`PostUpdateDto.cs`)
```csharp
public class PostUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    
    [StringLength(1000, MinimumLength = 3)]
    public required string Description { get; set; } 
    
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    public required string Content { get; set; }
    public required List<Guid> TopicIds { get; init; }
}
```

**Key Characteristics:**
- **Validation Attributes**: Uses `[Required]`, `[StringLength]` for input validation
- **Required Properties**: Uses `required` keyword for non-nullable properties
- **Nullable Properties**: Optional properties marked with `?`
- **Complex Types**: Includes collections like `List<Guid> TopicIds`

### 3. Command Request (`UpdatePostCommandRequest.cs`)
```csharp
public class UpdatePostCommandRequest
{
    [Required]
    public Guid Id { get; set; }
    
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    
    // ... other properties
}
```

**Key Characteristics:**
- **Identical Structure**: Mirrors the DTO structure for internal processing
- **Validation**: Maintains validation attributes for business rule enforcement
- **Separation**: Keeps internal command structure separate from external DTO

### 4. MediatR Command (`UpdatePost.cs`)
```csharp
public class UpdatePost(UpdatePostCommandRequest request) : IRequest<PostSummaryDto>
{
    public UpdatePostCommandRequest Request { get; set; } = request;

    private sealed class UpdatePostHandler : IRequestHandler<UpdatePost, PostSummaryDto>
    {
        public async Task<PostSummaryDto> Handle(UpdatePost request, CancellationToken cancellationToken)
        {
            var post = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);
            
            post = mapper.Map(request.Request, post);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<PostSummaryDto>(post);
        }
    }
}
```

**Key Characteristics:**
- **CQRS Pattern**: Implements `IRequest<PostSummaryDto>` for command handling
- **Nested Handler**: Uses private sealed class for the actual handler implementation
- **Dependency Injection**: Injects `DataContext` and `IMapper` into handler
- **Async Operations**: Uses `async/await` with `CancellationToken` support
- **Entity Framework**: Uses `FirstOrDefaultAsync` for database queries
- **AutoMapper**: Uses mapping for both input and output transformations

### 5. Validation (`UpdatePostValidator.cs`)
```csharp
public class UpdatePostValidator : AbstractValidator<UpdatePost>
{
    public UpdatePostValidator(DataContext dbContext)
    {
        RuleFor(u => u.Request)
            .MustAsync(PostExists)
            .WithMessage("Post not found.");

        RuleFor(u => u.Request)
            .MustAsync(TitleIsUniqueUnlessSamePost)
            .WithMessage("A post already has this title.");
    }
}
```

**Key Characteristics:**
- **FluentValidation**: Extends `AbstractValidator<UpdatePost>`
- **Async Validation**: Uses `MustAsync` for database-dependent validations
- **Business Rules**: Implements domain-specific validation logic
- **Error Messages**: Provides clear, user-friendly error messages

### 6. AutoMapper Profile (`PostProfile.cs`)
```csharp
CreateMap<(PostUpdateDto Dto, Guid Id), UpdatePostCommandRequest>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Dto.Title))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Dto.Description))
    // ... other mappings
```

**Key Characteristics:**
- **Tuple Mapping**: Maps from tuple `(PostUpdateDto, Guid)` to `UpdatePostCommandRequest`
- **Explicit Mapping**: Uses `ForMember` for precise property mapping
- **Route Parameter Integration**: Combines DTO data with route parameter

## Pattern Summary

### 1. **Controller Responsibility**
- **Thin Controller**: Controllers only handle HTTP concerns
- **Command Creation**: Creates MediatR commands from DTOs
- **Response Handling**: Returns appropriate HTTP responses
- **Authorization**: Enforces access control at controller level

### 2. **Data Flow**
```
HTTP Request → Controller → DTO → AutoMapper → Command → Handler → Database → Response
```

### 3. **Separation of Concerns**
- **Controller**: HTTP protocol handling
- **DTO**: Data transfer and input validation
- **Command**: Business logic encapsulation
- **Handler**: Database operations
- **Validator**: Business rule validation
- **Mapper**: Object transformation

### 4. **Error Handling**
- **Validation**: FluentValidation for business rules
- **HTTP Status**: Appropriate status codes (200, 404, etc.)
- **Exception Handling**: Global exception handler for unhandled errors

### 5. **Consistency Requirements**
For all controller methods to follow this pattern:

1. **Use MediatR Commands/Queries** for all business logic
2. **Implement DTOs** for all input/output data
3. **Add Validators** for business rule validation
4. **Use AutoMapper** for object transformations
5. **Return Typed Results** for consistent HTTP responses
6. **Apply Authorization** at controller method level
7. **Use Async/Await** throughout the pipeline
8. **Include CancellationToken** in all async operations

## Implementation Checklist

- [ ] Create DTO with validation attributes
- [ ] Create Command Request class
- [ ] Implement MediatR Command with nested Handler
- [ ] Add FluentValidation Validator
- [ ] Configure AutoMapper Profile
- [ ] Implement Controller method with proper authorization
- [ ] Add unit tests for each component
- [ ] Ensure proper error handling and status codes

This pattern ensures maintainable, testable, and scalable code while following SOLID principles and clean architecture practices. 