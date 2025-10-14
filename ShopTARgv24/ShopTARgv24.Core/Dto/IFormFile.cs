
namespace ShopTARgv24.Core.Dto
{
    public interface IFormFile
    {
        string Name { get; set; }

        void CopyTo(FileStream fileStream);
    }
}