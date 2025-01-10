using TicketingSystem.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Interfaces;

public interface IAuthService
{
    Task<Response<IdentityResult>> RegisterUser(UserRegisterDTO userDto);
    Task<Response<LoginResponseDTO>> LoginUser(UserLoginDTO loginDto);
}