using System;
using Microsoft.AspNetCore.Identity;

namespace Forester.Domain.Entities;

public class User : IdentityUser
{
    public ICollection<Log> Logs { get; set; }
}
