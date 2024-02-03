using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinusZoom.Models;

public class Meeting
{
    // this is primary key, add annotation
    [Key]
    public string Id { get; set; }

    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [DataType(DataType.Date)]
    [Display(Name = "Event Date")]
    public DateTime MeetingDate { get; set; }
    
    public string PosterPath { get; set; }

    public string CertificateTemplatePath { get; set; }
}