using Microsoft.AspNetCore.Identity;

namespace DevFlix.Models;

public class DevFlixRole : IdentityRole<long>
{
    public DevFlixRole(string name) : base(name) { }
}
