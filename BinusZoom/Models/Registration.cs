using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinusZoom.Models;

public class Registration
{
    [Key]
    [ValidateNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public String Id { get; set; }
    
    [Required(ErrorMessage = "Harap mengisi NIM")]
    [Display(Name = "NIM")]
    public String NIM { get; set; }
    
    [Required(ErrorMessage = "Harap mengisi Email")]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public String Email { get; set; }
    
    [Required(ErrorMessage = "Harap mengisi Nama")]
    [Display(Name = "Nama")]
    public String NamaLengkap { get; set; }
}