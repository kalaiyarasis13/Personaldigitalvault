using Microsoft.AspNetCore.Identity;
using PersonalDigitalVaultBackend.DTOs.RequestDtos.Auth;
using PersonalDigitalVaultBackend.DTOs.ResponseDtos.Auth;
using PersonalDigitalVaultBackend.Models;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend.Services.Implementations
{
    public class AuthService : IAuService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException("Username is already taken.");

            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new ApplicationUser
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByUsernameOrEmailAsync(dto.UsernameOrEmail)
                ?? throw new UnauthorizedAccessException("Invalid username/email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("This account has been disabled. Contact the administrator.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid username/email or password.");

            return BuildAuthResponse(user);
        }

        public async Task<UserProfileResponseDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");
            return ToProfileDto(user);
        }

        public async Task<UserProfileResponseDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _userRepository.EmailExistsAsync(dto.Email))
                    throw new InvalidOperationException("Email is already registered.");
                user.Email = dto.Email;
            }

            await _userRepository.UpdateAsync(user);
            return ToProfileDto(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }

        private AuthResponseDto BuildAuthResponse(ApplicationUser user)
        {
            var (token, expiresAt) = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = ToProfileDto(user)
            };
        }

        private static UserProfileResponseDto ToProfileDto(ApplicationUser user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
