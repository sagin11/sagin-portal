using System.ComponentModel.DataAnnotations;

namespace SaginPortal.Models.ExamModels; 

public class AddExamModel {
    [Required(ErrorMessage = "Pole nazwa jest puste.")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Pole kategoria jest puste.")]
    public int Category { get; set; }
    public string? CategoryName { get; set; }
}