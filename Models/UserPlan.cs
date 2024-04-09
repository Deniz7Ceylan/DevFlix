using System.ComponentModel.DataAnnotations.Schema;

namespace DevFlix.Models;

public class UserPlan
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public short PlanId { get; set; }
    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }
    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }
    [ForeignKey("UserId")]
    public DevFlixUser? DevFlixUser { get; set; }
    [ForeignKey("PlanId")]
    public Plan? Plan { get; set; }
}
