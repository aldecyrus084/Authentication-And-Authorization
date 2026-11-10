using MediatR;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces.Repository;

public class FetchAllUserHandler : IRequestHandler<FetchAllUserCommand, GenericResponseDTO<IEnumerable<UserResponseDTO>>>
{
    private readonly IUserRepository _userRepository;

    public FetchAllUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GenericResponseDTO<IEnumerable<UserResponseDTO>>> Handle(FetchAllUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        return new GenericResponseDTO<IEnumerable<UserResponseDTO>> 
        { 
            isSuccess = true,
            message = "Users fetched successfully",
            Data = users.Select(u => new UserResponseDTO 
            { 
                UserId = u.userId, 
                Name = u.Name, 
                Email = u.Email 
            }) 
        };
    }
}