using HouseScout.Clients;
using HouseScout.DTOs;
using HouseScout.Model;

namespace HouseScout.Mappers;

public class BezrealitkyMapper : IMapper
{
    private const string URL_PREFIX = "https://www.bezrealitky.cz/nemovitosti-byty-domy/";

    public List<Estate> MapResponseToModel(object response)
    {
        BezrealitkyResponseDTO responseDto = (BezrealitkyResponseDTO)response;
        var estatesFromResponse = responseDto.ListAdverts.List;

        return estatesFromResponse.Select(MapSingleToModel).ToList();
    }

    private Estate MapSingleToModel(BezrealitkyEstate estate)
    {
        return new Estate(
            ApiType.BEZREALITKY,
            estate.Id,
            estate.Address,
            estate.Price,
            CreateURL(estate.Uri),
            estate.Surface,
            EstateType.APARTMENT,
            OfferType.RENT
        );
    }

    private string CreateURL(string uncompleteUri)
    {
        return $"{URL_PREFIX}{uncompleteUri}";
    }
}
