using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using RetailTrack.ViewModels;
using RetailTrack.Services;
using System.Linq;
using System.Threading.Tasks;

public class MaterialController : Controller
{
    private readonly MaterialService _materialService;
    private readonly SizeService _sizeService;
    public MaterialController(MaterialService materialService, SizeService sizeService)
    {
        _materialService = materialService;
        _sizeService     = sizeService;
    }

    // GET: Crear Material - Vista
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var materialTypes = await _materialService.GetAllMaterialTypesAsync();
        var viewModel = new MaterialViewModel
        {
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            })
        };
        return View(viewModel);
    }

    // POST: Crear Material - Acción
    [HttpPost]
    public async Task<IActionResult> Create(MaterialViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });
            return View(viewModel);
        }

        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = viewModel.Name,
            MaterialTypeId = viewModel.MaterialTypeId,
            SizeId = viewModel.SizeId
        };

        await _materialService.AddMaterialAsync(material);
        TempData["Message"] = "Material creado con éxito.";
        return RedirectToAction("Index");
    }

    // GET: Editar Material - Vista
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var material = await _materialService.GetMaterialByIdAsync(id);
        if (material == null)
        {
            return NotFound();
        }

        var materialTypes = await _materialService.GetAllMaterialTypesAsync();
        var viewModel = new MaterialViewModel
        {
            Name = material.Name,
            MaterialTypeId = material.MaterialTypeId,
            SizeId = material.SizeId,
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            })
        };

        return View(viewModel);
    }

    // POST: Editar Material - Acción
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, MaterialViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });
            return View(viewModel);
        }

        var material = await _materialService.GetMaterialByIdAsync(id);
        if (material == null)
        {
            return NotFound();
        }

        // Actualizar las propiedades
        material.Name = viewModel.Name;
        material.MaterialTypeId = viewModel.MaterialTypeId;
        material.SizeId = viewModel.SizeId;

        await _materialService.UpdateMaterialAsync(material);
        TempData["Message"] = "Material actualizado con éxito.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CreatePartial()
    {
        var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
        var sizes           = await _sizeService.GetAllSizesAsync();
        
        var viewModel = new MaterialViewModel(); 
        viewModel.Sizes = sizes.Select(sz => new SelectListItem
        {
            Value = sz.Size_Id.ToString(),
            Text = sz.Size_Name
        });

        viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
        {
            Value = mt.Id.ToString(),
            Text = mt.Name
        });
      
      
      
        return PartialView("_CreateMaterialPartial", viewModel);
    }

}
