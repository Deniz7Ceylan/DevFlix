using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DevFlix.Models;

public class DevFlixUser : IdentityUser<long>
{
    [Column(TypeName = "date")]
    public DateTime BirthDate { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";
    public bool Passive { get; set; }
    [NotMapped]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = "";

    [NotMapped]
    public byte? Restriction
    {
        get
        {
            int age = DateTime.Today.Year - BirthDate.Year;

            if (age < 7)
            {
                return 7;
            }
            else
            {
                if (age < 13)
                {
                    return 13;
                }
                else
                {
                    if (age < 18)
                    {
                        return 18;
                    }
                }
            }
            return null;
        }
    }
}
