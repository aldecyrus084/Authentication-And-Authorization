using MediatR;
using MyApp.Application.DTOs;

public class FetchAllUserCommand : IRequest<GenericResponseDTO<IEnumerable<UserResponseDTO>>>
{
    
}