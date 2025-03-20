using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System.Collections.Generic;

namespace RetailTrack.ViewModels
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new Product();

        // Variantes del producto (ProductStock)
        public List<ProductStockViewModel> Variants { get; set; } = new List<ProductStockViewModel>();

        // Listas de selección para la vista
        public IEnumerable<SelectListItem> Designs { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Materials { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();

        // Persistencia de datos, para el cabezal de producto.
        public string SelectedProductName { get; set; } = string.Empty;
        public decimal? SelectedGeneralPrice { get; set; }
        public string SelectedDescription { get; set; } = string.Empty;
        public Guid? SelectedDesignId { get; set; }
        public Design SelectedDesignDetails {get; set;}
        public Guid? SelectedMaterialTypeId { get; set; } 
        public Guid? SelectedMaterialId { get; set; }
        public int? SelectedSizeId { get; set; }        
    }

    public class ProductStockViewModel
    {

        public Guid MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int SizeId { get; set; }
        public string MaterialSizeName { get; set; } = string.Empty;
        public string MaterialTypeName { get; set; } = string.Empty;
        public decimal? CustomPrice { get; set; } = 0;
        public decimal Cost { get; set; } = 0; 
        public int Stock { get; set; } = 0;
    }

    public class ProductDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? DesignName { get; set; }
        public Design Design {get; set;}
        public string? Status { get; set; }

        public List<ProductStockViewModel> Variants { get; set; } = new List<ProductStockViewModel>();
    }

    public class ProductFilterViewModel
    {
        // Filtros seleccionados por el usuario
        public string? ProductName { get; set; }
        public Guid? SelectedDesignId { get; set; }
        public Guid? SelectedMaterialTypeId { get; set; }
        public Guid? SelectedMaterialId { get; set; }
        public int? SelectedSizeId { get; set; }
        public int? SelectedStatusId { get; set; }

        // Listas de selección para los filtros
        public IEnumerable<SelectListItem> Designs { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Materials { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> sizesList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();

        // Lista de productos filtrados
        public IEnumerable<ProductDetailsViewModel> Products { get; set; } = new List<ProductDetailsViewModel>();
    }    

    public class DeleteVariantRequest
    {
        public Guid MaterialId { get; set; }
    }

    public class ProductHeaderViewModel
    {
        public string SelectedProductName { get; set; } = string.Empty;
        public decimal? SelectedGeneralPrice { get; set; }
        public string SelectedDescription { get; set; } = string.Empty;
        public Guid? SelectedDesignId { get; set; }
        public Design SelectedDesignDetails {get; set;}
    }
}
