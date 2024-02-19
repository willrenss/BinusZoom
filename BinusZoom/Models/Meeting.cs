using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BinusZoom.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

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

    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [DataType(DataType.Date)]
    [Display(Name = "Event Date")]
    public DateTime MeetingDate { get; set; }
    
    [Url(ErrorMessage = "Harap menaruh URL yang tepat")]
    [Required(ErrorMessage = "Mohon mengisi link meeting")]
    [Display(Name = "URL untuk link meeting")]
    public String LinkUrl { get; set; }
    
    [ValidateNever] // exclude from modelstate.isvalid when create
    public string PosterPath { get; set; }

    [ValidateNever] // exclude from modelstate.isvalid when create
    public ICollection<Registration> Registrations { get; set; } = default!;
}