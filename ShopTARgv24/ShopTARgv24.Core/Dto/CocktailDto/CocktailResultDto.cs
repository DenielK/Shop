namespace ShopTARgv24.Core.Dto.CocktailDto
{
    public class CocktailResultDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Glass { get; set; }
        public string? Instructions { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Measures { get; set; }
    }
}
