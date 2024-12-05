namespace RetailTrack.Models
{
    public enum ProductSize
    {
        XS,
        S,
        M,
        L,
        XL,
        XXL
    }

    public enum ProductStatus
    {
        Pending,
        ReadyToMake,
        Made,
        Sold
    }

    public enum MovementType
    {
        PorCobrar,
        Vendido
    }
}
