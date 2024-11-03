using NGO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Repository.Contracts
{
    public interface IOrgRepository:IRepository<Organization>
    {
        Task<Organization> GetByIdAsync(int orgId);
    }
}
