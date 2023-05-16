using System.ComponentModel.DataAnnotations;

namespace SaginPortal.Models; 

public class LoginModel {
    
    [Required(ErrorMessage = "Pole email jest puste.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Pole z hasłem jest puste.")]
    public string Password { get; set; }
}