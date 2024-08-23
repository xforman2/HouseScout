using HouseScout.Clients;
using HouseScout.Model;
using Advert = HouseScout.Clients.Advert;
using BezrealitkyResponse = HouseScout.Clients.BezrealitkyResponse;

namespace HouseScout.Mappers;

public class BezrealitkyMapper
{
    public List<Estate> MapResponseToModel(BezrealitkyResponse response)
    {
        var estatesFromResponse = response.ListAdverts.List;

        return estatesFromResponse.Select(MapSingleToModel).ToList();

    }
    private Estate MapSingleToModel(Advert estate)
    {
        return new Estate("bezrealitky", estate.Id, estate.Address, estate.Price, estate.Charges, estate.Uri, estate.Surface,
            estate.EstateType, estate.OfferType);

    }
}