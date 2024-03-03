using System;
using BinusZoom.Models;
using Microsoft.AspNetCore.Mvc;


namespace BinusZoom.Controllers;

public class _LayoutController : Controller
{

	public static string IsActiveAction(string currentAction, string expectedAction)
	{
		return string.Equals(currentAction, expectedAction, StringComparison.OrdinalIgnoreCase) ? "Aktif" : "" ;
	}
}


