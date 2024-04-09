using System.ComponentModel.DataAnnotations.Schema;

namespace DevFlix.Models;

public class UserWatched
{
    public long UserId { get; set; }
    public long EpisodeId { get; set; }
    [ForeignKey("UserId")]
    public DevFlixUser? DevFlixUser { get; set; }
    [ForeignKey("EpisodeId")]
    public Episode? Episode { get; set; }
}
