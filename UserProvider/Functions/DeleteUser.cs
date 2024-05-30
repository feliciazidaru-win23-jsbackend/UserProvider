using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions
{
    public class DeleteUser(ILogger<DeleteUser> logger, DataContext context)
    {
        private readonly ILogger<DeleteUser> _logger = logger;
        private readonly DataContext _context = context;

        [Function("DeleteUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var userEmail = await req.ReadFromJsonAsync<UserEmailModel>();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail.Email);
                if (user == null)
                {
                    return new NotFoundResult();
                }
                else
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return new OkResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return new BadRequestResult();
            }
        }
    }

    public class UserEmailModel
    {
        public string Email { get; set; }
    }
}

