
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BinusZoom.Models;

public class UserLogin
{
    [Key]
    [PersonalData]
    public string Username { get; set; }
    [PersonalData]
    public string Password { get; set; }
}