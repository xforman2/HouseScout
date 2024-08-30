using HouseScout.Model;

namespace HouseScout.Mappers;

public interface IMapper
{
    List<Estate> MapResponseToModel(object rawData);
}
