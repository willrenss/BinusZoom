using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinusZoom.Models;

public class Meeting
{
    public string Id { get; set; }

    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [Display(Name = "Event Date")]
    public DateTime MeetingDate { get; set; }

    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [Display(Name = "Event Date")]
    public DateTime MeetingEndDate { get; set; }

    public string PosterPath { get; set; }

    public string CertificateTemplatePath { get; set; }

    [NotMapped] public string MeetingDateInput { get; set; }

    [NotMapped] public string MeetingTimeInputInWIB { get; set; }

    [Display(Name = "Foto Poster")]
    [NotMapped]
    public string PosterFile { get; set; }


    [Display(Name = "Template Sertifikat")]
    [NotMapped]
    public string TemplateFile { get; set; }
}