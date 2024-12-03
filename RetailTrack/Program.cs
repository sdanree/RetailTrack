using RetailTrack.Models;
using System;

class Program
{
    static void Main()
    {
        // Crear un diseño
        var design = new Design("Clásico", "Diseño elegante en negro", "https://example.com/design-image.jpg");

        // Crear un producto con diseño y talles específicos
        var product = new Product("Vestido", "Vestido de noche", 20, ProductSize.M, ProductStatus.Pending, design);

        // Agregar materiales al producto
        product.AddMaterial(new Material("Tela", 30.00m, 2));
        product.AddMaterial(new Material("Botones", 5.00m, 6));

        // Mostrar información del producto
        Console.WriteLine(product);

        // Mostrar materiales
        foreach (var material in product.Materials)
        {
            Console.WriteLine(material);
        }
    }
}
