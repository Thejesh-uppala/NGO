using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class PaymentRepository:Repository<Payment>, IPaymentRepository
    {
        private readonly NGOContext _nGOContext;
        public PaymentRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }
    }
}
