using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManagementSystem.Application.Abstractions;
using ProductManagementSystem.Application.Exceptions;
using ProductManagementSystem.Application.Requests.Users;
using ProductManagementSystem.Application.Responses.Users;
using ProductManagementSystem.Domain;
using ProductManagementSystem.Infrastructure.Database;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserRepository> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Paged<UserListItemResponse>> GetUsersAsync(string? search, bool? emailConfirmed, int pageNumber, PageSize pageSize)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.UserName!.Contains(search) || u.Email!.Contains(search));

        if (emailConfirmed.HasValue)
            query = query.Where(u => u.EmailConfirmed == emailConfirmed.Value);

        var totalCount = await query.CountAsync();

        var usersWithRoles = await query
            .Skip((pageNumber - 1) * (int)pageSize)
            .Take((int)pageSize)
            .Select(u => new
            {
                User = u,
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            })
            .ToListAsync();

        var items = usersWithRoles.Select(ur => new UserListItemResponse
        (
            ur.User.Id,
            ur.User.Email!,
            ur.User.UserName!,
            ur.User.PhoneNumber,
            ur.User.EmailConfirmed,
            ur.Roles!
        )).ToList();

        return new Paged<UserListItemResponse>
        {
            Items = items,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)(int)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<GetUserDetailsResponse> GetDetailsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new UserNotFoundException(userId);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new GetUserDetailsResponse
        (
            user.Id,
            user.Email!,
            user.UserName!,
            user.PhoneNumber,
            user.EmailConfirmed,
            roles.ToList()
        );
    }

    public async Task<bool> UserExists(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return false;

        return true;
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User not found in IsUserInRoleAsync. UserId: {UserId}", userId);
            throw new UserNotFoundException(userId);
        }

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task RemoveSingleUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for deletion.", userId);
            throw new UserNotFoundException(userId);
        }

        await _userManager.DeleteAsync(user);
    }

    public async Task RemoveUsersAsync(List<string> userIds)
    {
        await _context.Users.Where(u => userIds.Contains(u.Id))
                            .ExecuteDeleteAsync();
    }

    public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.Id);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", request.Id);
            throw new UserNotFoundException(request.Id);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await UpdateUserNameAsync(user, request.UserName);
            await UpdateEmailAsync(user, request.Email);
            await UpdatePhoneNumberAsync(user, request.PhoneNumber);

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GetUserForUpdateResponse> GetUserForUpdateAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update retrieval.", userId);
            throw new UserNotFoundException(userId);
        }

        return new GetUserForUpdateResponse
        (
            user.Id,
            user.Email!,
            user.UserName!,
            user.PhoneNumber
        );
    }

    private async Task UpdateUserNameAsync(ApplicationUser user, string newUserName)
    {
        if (!string.Equals(user.UserName, newUserName, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _userManager.Users
                .Where(u => u.UserName == newUserName && u.Id != user.Id)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                _logger.LogWarning("Duplicate username detected: {UserName}", newUserName);
                throw new DuplicateUserNameException(newUserName);
            }

            var result = await _userManager.SetUserNameAsync(user, newUserName);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update username for user ID {UserId}.", user.Id);
                throw new UserUpdateException(user.Id, "Failed to update username.");
            }
        }
    }

    private async Task UpdateEmailAsync(ApplicationUser user, string newEmail)
    {
        if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _userManager.Users
                .Where(u => u.Email == newEmail && u.Id != user.Id)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                _logger.LogWarning("Duplicate email detected: {Email}", newEmail);
                throw new DuplicateEmailException(newEmail);
            }

            var result = await _userManager.SetEmailAsync(user, newEmail);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update email for user ID {UserId}.", user.Id);
                throw new UserUpdateException(user.Id, "Failed to update email.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                _logger.LogError("Failed to confirm email for user ID {UserId} after update.", user.Id);
                throw new UserUpdateException(user.Id, "Failed to confirm email after update.");
            }
        }
    }

    private async Task UpdatePhoneNumberAsync(ApplicationUser user, string? newPhoneNumber)
    {
        if (!string.Equals(user.PhoneNumber, newPhoneNumber, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(newPhoneNumber))
        {
            var existingUser = await _userManager.Users
                .Where(u => u.PhoneNumber == newPhoneNumber && u.Id != user.Id)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                _logger.LogWarning("Duplicate phone number detected: {PhoneNumber}", newPhoneNumber);
                throw new DuplicatePhoneNumberException(newPhoneNumber);
            }

            var result = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update phone number for user ID {UserId}.", user.Id);
                throw new UserUpdateException(user.Id, "Failed to update phone number.");
            }
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found when retrieving roles.", userId);
            throw new UserNotFoundException(userId);
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        return userRoles;
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync();
    }

    public async Task UpdateUserRolesAsync(string userId, List<string> selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found when updating roles.", userId);
            throw new UserNotFoundException(userId);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var current = new HashSet<string>(currentRoles, StringComparer.OrdinalIgnoreCase);
        var target = new HashSet<string>(selectedRoles, StringComparer.OrdinalIgnoreCase);

        var rolesToAdd = target.Except(current, StringComparer.OrdinalIgnoreCase).ToList();
        var rolesToRemove = current.Except(target, StringComparer.OrdinalIgnoreCase).ToList();

        if (rolesToAdd.Count == 0 && rolesToRemove.Count == 0) return;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (rolesToAdd.Count > 0)
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    _logger.LogError("Failed to add roles to user ID {UserId}.", user.Id);
                    throw new UserUpdateException(user.Id, "Failed to add roles to user.");
                }
            }

            if (rolesToRemove.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Failed to remove roles from user ID {UserId}.", user.Id);
                    throw new UserUpdateException(user.Id, "Failed to remove roles from user.");
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        var userNameExists = await _userManager.Users.AnyAsync(u => u.UserName == request.UserName);
        if (userNameExists)
        {
            _logger.LogWarning("Duplicate username detected during user creation: {UserName}", request.UserName);
            throw new DuplicateUserNameException(request.UserName);
        }

        var emailExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
        {
            _logger.LogWarning("Duplicate email detected during user creation: {Email}", request.Email);
            throw new DuplicateEmailException(request.Email);
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneNumberExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExists)
            {
                _logger.LogWarning("Duplicate phone number detected during user creation: {PhoneNumber}", request.PhoneNumber);
                throw new DuplicatePhoneNumberException(request.PhoneNumber);
            }
        }

        var newUser = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to create user with username {UserName}.", request.UserName);
            throw new UserCreationException("Failed to create user.");
        }
    }
}
