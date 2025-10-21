using FluentValidation;
using LogsManagment.Core.Features.Users.Commands.Models;
using LogsManagment.Data.Entities; // AppUser, Log entities
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogsManagment.Core.Features.Users.Commands.Validators
{
    public class AssignUserToLogValidator : AbstractValidator<AssignUserToLogCommandModel>
    {
        private readonly Data.AppContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AssignUserToLogValidator(Data.AppContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            // استخدم MustAsync لأننا هنعمل استعلام DB غير متزامن
            RuleFor(x => x.UserId)
                .MustAsync(UserExistsAndNotAdminAsync)
                .WithMessage("User does not exist or is an Admin.");

            RuleFor(x => x.LogId)
                .MustAsync(LogExistsAsync)
                .WithMessage("Log does not exist.");
        }

        private async Task<bool> UserExistsAndNotAdminAsync(int userId, CancellationToken ct)
        {
            // تأكد إن المستخدم موجود
            var user = await _context.AppUsers
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
                return false;

            // استخدم UserManager للحصول على أدوار المستخدم بشكل موثوق
            var roles = await _userManager.GetRolesAsync(user);

            // تأكد إن مفيش دور Admin
            return !roles.Contains("Admin");
        }

        private async Task<bool> LogExistsAsync(int logId, CancellationToken ct)
        {
            return await _context.Logs
                                 .AsNoTracking()
                                 .AnyAsync(l => l.Id == logId, ct);
        }
    }
}
