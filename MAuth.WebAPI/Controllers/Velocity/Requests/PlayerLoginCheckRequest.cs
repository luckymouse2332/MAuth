using System.ComponentModel.DataAnnotations;

namespace MAuth.WebAPI.Controllers.Velocity.Requests;

public class PlayerLoginCheckRequest
{
    [Required(ErrorMessage = "{0}是必填的")]
    public string UUID { get; set; }
    
    [Required(ErrorMessage = "{0}是必填的")]
    [Display(Name = "玩家IP")]
    public string IpAddress { get; set; }
}