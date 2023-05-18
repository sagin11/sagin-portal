using System.ComponentModel.DataAnnotations.Schema;
using Org.BouncyCastle.Crypto.Generators;

namespace SaginPortal.Models.ExamModels; 

[Table("Categories")]
public class ExamCategoryModel {
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public int UserId { get; set; }
}