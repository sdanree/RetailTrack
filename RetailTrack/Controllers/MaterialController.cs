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
        _sizeService = sizeService;
    }

    // GET: Crear Material - Vista
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var materialTypes = await _materialService.GetAllMaterialTypesAsync();
        var sizes = await _sizeService.GetAllSizesAsync();

        var viewModel = new MaterialViewModel
        {
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            }),
            Sizes = sizes.Select(s => new SelectListItem
            {
                Value = s.Size_Id.ToString(),
                Text = s.Size_Name
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
//            var sizes = await _sizeService.GetAllSizesAsync();

            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });

/*            viewModel.Sizes = sizes.Select(s => new SelectListItem
            {
                Value = s.Size_Id.ToString(),
                Text = s.Size_Name
            });
*/
            return View(viewModel);
        }

        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = viewModel.Name,
            MaterialTypeId = viewModel.MaterialTypeId,
//            SizeId = viewModel.SizeId,
//            Stock = 0,
//            Cost = 0
        };

        await _materialService.AddMaterialAsync(material);

        TempData["Message"] = "Material creado con éxito.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CreatePartial()
    {
        var materialTypes = await _materialService.GetAllMaterialTypesAsync();
        var sizes = await _sizeService.GetAllSizesAsync();

        var viewModel = new MaterialViewModel
        {
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            }),
        };

        return PartialView("_CreateMaterialPartial", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartial(MaterialViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Recargar tipos de materiales y tamaños en caso de error
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });
 /*           viewModel.Sizes = sizes.Select(sz => new SelectListItem
            {
                Value = sz.Size_Id.ToString(),
                Text = sz.Size_Name
            });
*/
            return PartialView("_CreateMaterialPartial", viewModel);
        }

        var newMaterial = new Material
        {
            Id = Guid.NewGuid(),
            Name = viewModel.Name,
            MaterialTypeId = viewModel.MaterialTypeId,
//            SizeId = viewModel.SizeId,
//            Stock = 0,
//            Cost = 0
        };

        await _materialService.AddMaterialAsync(newMaterial);

        // Guardar en sesión los valores seleccionados
        HttpContext.Session.SetString("SelectedMaterialType", viewModel.MaterialTypeId.ToString());
        HttpContext.Session.SetString("SelectedMaterial", newMaterial.Id.ToString());
        HttpContext.Session.SetString("SelectedSize", viewModel.SizeId.ToString());

        return Json(new { success = true });
    }
}
