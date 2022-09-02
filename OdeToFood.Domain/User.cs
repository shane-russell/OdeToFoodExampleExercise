using Microsoft.AspNetCore.Identity;

namespace OdeToFood.Domain
{
    public class User : IdentityUser<int>
    {
        // why it is empty? you need to add properties here for User and also Role class of Domain layer
    }
}