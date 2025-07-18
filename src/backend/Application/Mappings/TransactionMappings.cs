using FiTrack.Application.Models.Transaction;
using FiTrack.Domain.Entities.Business;

namespace FiTrack.Application.Mappings;
public class TransactionMappings : Profile
{
    public TransactionMappings()
    {
        CreateMap<FinTransaction, FinanceTransactionResponseModel>();

        CreateMap<FinTransaction, FinanceMiniTransactionResponseModel>()
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name));
    }
}
