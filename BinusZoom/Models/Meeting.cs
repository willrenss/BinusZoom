using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [ValidateNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public String Id { get; set; }

    [Required(ErrorMessage = "Harap mengisi meeting date")]
    [DataType(DataType.Date)]
    [Display(Name = "Event Date")]
    public DateTime MeetingDate { get; set; }
    
    public string PosterPath { get; set; }

}