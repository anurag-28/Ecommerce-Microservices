using MediatR;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using Catalog.Application.Mappers;
using Catalog.Core.Entities;

namespace Catalog.Application.Handlers
{
    public class GetAllTypesHandler : IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
    {
        private readonly ITypesRepository _typesRepository;
        public GetAllTypesHandler(ITypesRepository typesRepository)
        {
            _typesRepository = typesRepository;
        }
        public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            var typesList =  await _typesRepository.GetAllTypes();
            var typesResponseList = ProductMapper.Mapper.Map<IList<ProductType>,IList<TypesResponse>>(typesList.ToList());
            return typesResponseList;
        }
    }
}
