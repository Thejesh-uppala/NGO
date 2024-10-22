using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Repository
{
    public class MemberShipTypesRepository : Repository<MemberShipType>,IMemberShipTypesRepository
    {
        private readonly NGOContext _nGOContext;
        public MemberShipTypesRepository(NGOContext nGODbContext, ApplicationContext applicationContext) : base(nGODbContext, applicationContext)
        {
            _nGOContext = nGODbContext;
        }
    }
}
