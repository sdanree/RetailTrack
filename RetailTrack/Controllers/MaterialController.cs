using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System.Linq;
using System.Threading.Tasks;

public class MaterialController : Controller
{
    private readonly MaterialService _materialService;

    public MaterialController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? materialTypeId, string? name)
    {
        // Obtener todos los materiales
        var materials = await _materialService.GetAllMaterialsAsync();

        // Aplicar filtro por tipo de material
        if (!string.IsNullOrWhiteSpace(materialTypeId) && Guid.TryParse(materialTypeId, out var typeId))
        {
            materials = materials.Where(m => m.MaterialTypeId == typeId).ToList();
        }

        // Aplicar filtro por nombre
        if (!string.IsNullOrWhiteSpace(name))
        {
            materials = materials.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Obtener los tipos de materiales para el filtro
        var materialTypes = await _materialService.GetAllMaterialTypesAsync();

        // Crear el ViewModel
        var viewModel = new MaterialIndexViewModel
        {
            Materials = materials,
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            }),
            SelectedMaterialTypeId = materialTypeId,
            NameFilter = name
        };

        return View(viewModel);
    }


    // Crear Material - Vista
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

    // Crear Material - Acción
    [HttpPost]
    public async Task<IActionResult> Create(MaterialViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Recargar los tipos de materiales en caso de error
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });

            return View(viewModel);
        }

        await _materialService.AddMaterialAsync(viewModel.Material);
        TempData["Message"] = "Material creado con éxito.";
        return RedirectToAction("Index");
    }

    // Editar Material - Vista
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
            Material = material,
            MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name,
                Selected = mt.Id == material.MaterialTypeId
            })
        };

        return View(viewModel);
    }

    // Editar Material - Acción
    [HttpPost]
    public async Task<IActionResult> Edit(MaterialViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            // Recargar los tipos de materiales en caso de error
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name
            });

            return View(viewModel);
        }

        await _materialService.UpdateMaterialAsync(viewModel.Material);
        TempData["Message"] = "Material actualizado con éxito.";
        return RedirectToAction("Index");
    }

    // Eliminar Material - Confirmación
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var material = await _materialService.GetMaterialByIdAsync(id);
        if (material == null)
        {
            return NotFound();
        }

        return View(material);
    }

    // Eliminar Material - Acción
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _materialService.DeleteMaterialAsync(id);
        TempData["Message"] = "Material eliminado con éxito.";
        return RedirectToAction("Index");
    }
}
