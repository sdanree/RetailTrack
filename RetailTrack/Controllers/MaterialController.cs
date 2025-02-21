using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using RetailTrack.ViewModels;
using RetailTrack.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class MaterialController : Controller
{
    private readonly MaterialService _materialService;
    private readonly SizeService _sizeService;
    private readonly ReceiptService _receiptService;

    public MaterialController(MaterialService materialService, SizeService sizeService, ReceiptService receiptService)
    {
        _materialService = materialService;
        _sizeService = sizeService;
        _receiptService = receiptService;
    }

[HttpGet]
public async Task<IActionResult> Index(Guid? InMaterialId, Guid? InMaterialtypeId, int? InSizeId, bool? InOutOfStock, string InMaterialName)
{
    var materialsQuery = _materialService.GetMaterialQuery();

    // Aplicar filtros
    if (InMaterialtypeId.HasValue)
    {
        materialsQuery = materialsQuery.Where(m => m.MaterialTypeId == InMaterialtypeId.Value);
    }

    if (InMaterialId.HasValue)
    {
        materialsQuery = materialsQuery.Where(m => m.Id == InMaterialId.Value);
    }

    if (InOutOfStock.HasValue && InOutOfStock.Value)
    {
        materialsQuery = materialsQuery
            .Where(m => m.MaterialSizes.Any(ms => ms.Stock == 0))
            .Select(m => new Material
            {
                Id = m.Id,
                Name = m.Name,
                MaterialType = m.MaterialType,
                MaterialSizes = m.MaterialSizes.Where(ms => ms.Stock == 0).ToList()
            });
    }

    if (InSizeId.HasValue)
    {
        materialsQuery = materialsQuery
            .Where(m => m.MaterialSizes.Any(ms => ms.SizeId == InSizeId.Value))
            .Select(m => new Material
            {
                Id = m.Id,
                Name = m.Name,
                MaterialType = m.MaterialType,
                MaterialSizes = m.MaterialSizes.Where(ms => ms.SizeId == InSizeId.Value).ToList()
            });
    }

    if (!string.IsNullOrEmpty(InMaterialName))
    {
        materialsQuery = materialsQuery.Where(m => m.Name.Contains(InMaterialName));
    }

    var materials = await materialsQuery.ToListAsync();

    // Obtener listas para los filtros
    var materialTypes = await _materialService.GetAllMaterialTypesAsync();
    var sizes = await _sizeService.GetAllSizesAsync();

    var viewModel = new MaterialIndexViewModel
    {
        Materials = materials,
        MaterialTypes = materialTypes.Select(mt => new SelectListItem
        {
            Value = mt.Id.ToString(),
            Text = mt.Name
        }),
        Sizes = sizes.Select(sz => new SelectListItem
        {
            Value = sz.Size_Id.ToString(),
            Text = sz.Size_Name
        }),
        SelectedMaterialTypeId = InMaterialtypeId,
        SelectedSize = InSizeId,
        SelectedMaterial = InMaterialId,
        SelectedOutOfStock = InOutOfStock,
        MaterialNameFilter = InMaterialName
    };

    return View(viewModel);
}

[HttpGet]
public async Task<IActionResult> PurchaseHistory(Guid materialId)
{
    var material = await _materialService.GetMaterialByIdAsync(materialId);

    if (material == null)
    {
        TempData["ErrorMessage"] = "El material no existe.";
        return RedirectToAction("Index");
    }

    var receipts = await _receiptService.GetReceiptsByMaterialIdAsync(materialId);

    var viewModel = new PurchaseHistoryViewModel
    {
        MaterialName = material.Name,
        MaterialType = material.MaterialType.Name,
        Receipts = receipts.Select(gr => new ReceiptIndexDetailViewModel
        {
            ReceiptId = gr.ReceiptId,
            ReceiptDate = gr.ReceiptDate,
            ProviderName = gr.Provider?.BusinessName ?? "Sin Proveedor",
            PaymentMethods = string.Join(", ", gr.Payments.Select(p => p.PaymentMethod.Name)),
            Details = gr.Details
                .Where(d => d.MaterialId == materialId)
                .Select(detail => new ReceiptDetailViewModel
                {
                    MaterialId = detail.MaterialId,
                    MaterialName = detail.Material.Name,
                    MaterialTypeName = detail.Material.MaterialType.Name,
                    SizeId = detail.SizeId,
                    SizeName = detail.Size.Size_Name,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost
                }).ToList()
        }).ToList()
    };

    return View(viewModel);
}







    //Crear Material
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

            return PartialView("_CreateMaterialPartial", viewModel);
        }

        var newMaterial = new Material
        {
            Id = Guid.NewGuid(),
            Name = viewModel.Name,
            MaterialTypeId = viewModel.MaterialTypeId,

        };

        await _materialService.AddMaterialAsync(newMaterial);

        // Guardar en sesión los valores seleccionados
        HttpContext.Session.SetString("SelectedMaterialType", viewModel.MaterialTypeId.ToString());
        HttpContext.Session.SetString("SelectedMaterial", newMaterial.Id.ToString());
        HttpContext.Session.SetString("SelectedSize", viewModel.SizeId.ToString());

        return Json(new { success = true });
    }
}
