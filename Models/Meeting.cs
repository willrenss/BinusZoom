using BinusZoom.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BinusZoom.Models
{
    public class Meeting
    {

        public string Id { get; set; }

        [Required(ErrorMessage = "Harap mengisi meeting date")]
        [Display(Name = "Event Date")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Harap mengisi meeting date")]
        [Display(Name = "Event Date")]
        public DateTime MeetingEndDate { get; set; }

        public string PosterPath{ get; set; }

        public string CertificateTemplatePath { get; set; }

        [NotMapped]
        public String MeetingDateInput { get; set; }

        [NotMapped]
        public String MeetingTimeInputInWIB{ get; set; }

        [Display(Name = "Foto Poster")]
        [NotMapped]
        public String PosterFile { get; set; }


        [Display(Name = "Template Sertifikat")]
        [NotMapped]
        public String TemplateFile { get; set; }
    }

    public class MeetingDBContext : ApplicationDbContext
    {
        public DbSet<Meeting> Meetings { get; set; }
    }
}