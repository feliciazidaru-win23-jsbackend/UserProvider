
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class ApplicationUser : IdentityUser
{

    public string? UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    public string? UserAddressId { get; set; }
    public UserAddress? UserAddress { get; set; }
   
}
public class UserProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    public string? ProfileImageUrl { get; set; } = "https://silicon.blob.core.windows.net/profileimages/avatar.jpg";
    public string? Biography { get; set; }
}
public class UserAddress

{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;


}