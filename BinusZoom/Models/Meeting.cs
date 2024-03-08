using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinusZoom.Models;

public class Meeting
{
    // this is primary key, add annotation
    // exclude from modelstate.isvalid when create
    // auto generate guid
    [Key]
    [ValidateNever] // exclude from modelstate.isvalid when create
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public String Id { get; set; }

    [Required(ErrorMessage = "Harap mengisi judul meeting")]
    [Display(Name = "Judul Meeting")]
    public String Title { get; set; }
    
    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [DataType(DataType.Date)]
    [Display(Name = "Event Date")]
    public DateTime MeetingDate { get; set; }
    
    [Url(ErrorMessage = "Harap mengisi URL yang tepat")]
    [Required(ErrorMessage = "Mohon mengisi link meeting")]
    [Display(Name = "URL untuk link meeting")]
    public String LinkUrl { get; set; }
    
    [ValidateNever] // exclude from modelstate.isvalid when create
    public string PosterPath { get; set; }

    [ValidateNever]
    public string CertificatePath { get; set; }
    
    [ValidateNever] // exclude from modelstate.isvalid when create
    public ICollection<Registration> Registrations { get; set; } = default!;
    
    [ValidateNever]
    public Boolean hasSendCertificateToAll { get; set; } = false;
}