using TicketingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Interfaces;


public interface IAuthRepository
{
    Task<User> GetUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<SignInResult> ValidateUserCredentialsAsync(User user, string password);
}
