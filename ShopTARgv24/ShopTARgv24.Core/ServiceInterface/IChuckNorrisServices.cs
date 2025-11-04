using ShopTARgv24.Core.Dto.ChuckNorris;

namespace ShopTARgv24.Core.ServiceInterface
{
    public interface IChuckNorrisServices
    {
        Task<ChuckNorrisResultDto> ChuckNorrisResult(ChuckNorrisResultDto dto);
    }
}
