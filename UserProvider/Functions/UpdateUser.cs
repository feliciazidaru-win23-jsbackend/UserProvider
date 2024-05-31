using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UpdateUser(ILogger<UpdateUser> logger, DataContext context)
{
    private readonly ILogger<UpdateUser> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var userForm = await req.ReadFromJsonAsync<UserForm>();

            if (userForm != null)
            {
                var user = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.UserAddress)
                    .FirstOrDefaultAsync(u => u.Email == userForm.Email);  // Find the user by email

                if (user != null)
                {
                    user.PhoneNumber = userForm.PhoneNumber;

                    if (user.UserAddressId != null)
                    {
                        var address = await _context.UserAddresses.FindAsync(user.UserAddressId);
                        if (address != null)
                        {
                            address.AddressLine_1 = userForm.AddressLine_1;
                            address.AddressLine_2 = userForm.AddressLine_2;
                            address.PostalCode = userForm.PostalCode;
                            address.City = userForm.City;
                        }
                    }
                    else
                    {
                        user.UserAddress = new UserAddress
                        {
                            AddressLine_1 = userForm.AddressLine_1,
                            AddressLine_2 = userForm.AddressLine_2,
                            PostalCode = userForm.PostalCode,
                            City = userForm.City
                        };
                    }

                    if (user.UserProfileId != null)
                    {
                        var profile = await _context.UserProfiles.FindAsync(user.UserProfileId);
                        if (profile != null)
                        {
                            profile.FirstName = userForm.FirstName;
                            profile.LastName = userForm.LastName;
                            profile.Biography = userForm.Biography;
                        }
                    }
                    else
                    {
                        user.UserProfile = new UserProfile
                        {
                            FirstName = userForm.FirstName,
                            LastName = userForm.LastName,
                            Biography = userForm.Biography
                        };
                    }

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return new OkObjectResult(user);
                }
                else
                {
                    return new NotFoundObjectResult("User not found");
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid user form");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
    }
}

    public class UserForm
{
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Biography { get; set; }
}