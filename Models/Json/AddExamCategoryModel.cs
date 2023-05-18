using System.ComponentModel.DataAnnotations.Schema;
using Org.BouncyCastle.Crypto.Generators;

namespace SaginPortal.Models.ExamModels; 


public class AddExamCategoryModel {
    public string? CategoryName { get; set; }
}
