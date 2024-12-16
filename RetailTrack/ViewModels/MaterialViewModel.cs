using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System.Collections.Generic;

public class MaterialViewModel
{
    public Material Material { get; set; } = new Material();

    public IEnumerable<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
}
