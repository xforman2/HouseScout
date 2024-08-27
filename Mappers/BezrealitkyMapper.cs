using HouseScout.Clients;
using HouseScout.DTOs;
using HouseScout.Model;

namespace HouseScout.Mappers;

public class BezrealitkyMapper
{
    public List<Estate> MapResponseToModel(BezrealitkyResponseDTO response)
    {
        var estatesFromResponse = response.ListAdverts.List;

        return estatesFromResponse.Select(MapSingleToModel).ToList();

    }
    private Estate MapSingleToModel(BezrealitkyEstate estate)
    {
        return new Estate(ApiType.BEZREALITKY, estate.Id, estate.Address, estate.Price, CreateURL(estate.Uri), estate.Surface,
            EstateType.APARTMENT, OfferType.RENT);

    }

    private string CreateURL(string uncompleteUri)
    {
        return $"https://www.bezrealitky.cz/nemovitosti-byty-domy/{uncompleteUri}";
    }
}