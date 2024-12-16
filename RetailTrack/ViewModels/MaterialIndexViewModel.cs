using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System.Collections.Generic;

public class MaterialIndexViewModel
{
    public IEnumerable<Material> Materials { get; set; } = new List<Material>();
    public IEnumerable<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
    public string? SelectedMaterialTypeId { get; set; }
    public string? NameFilter { get; set; }
}
