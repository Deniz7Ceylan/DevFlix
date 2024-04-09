using System.ComponentModel.DataAnnotations.Schema;

namespace DevFlix.Models;

public class UserFavorite
{
    public long UserId { get; set; }
    public int MediaId { get; set; }
    [ForeignKey("UserId")]
    public DevFlixUser? DevFlixUser { get; set; }
    [ForeignKey("MediaId")]
    public Media? Media { get; set; }
}
