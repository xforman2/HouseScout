using SharedDependencies.Model;

namespace Backend.Mappers;

public interface IMapper
{
    List<Estate> MapResponseToModel(object rawData);
}
