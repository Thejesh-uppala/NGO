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
    public class OrgChapterRepository:Repository<OrganizationChapter>, IOrgChapterRepository
    {
        private readonly NGOContext _nGOContext;
        public OrgChapterRepository(NGOContext nGODbContext, ApplicationContext applicationContext) : base(nGODbContext, applicationContext)
        {
            _nGOContext = nGODbContext;
        }
    }
}
