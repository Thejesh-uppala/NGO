﻿using Microsoft.EntityFrameworkCore;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;

namespace NGO.Repository
{
    public class RoleRepository:Repository<Role>,IRoleRepository
    {
        private readonly NGOContext _nGOContext;
        public RoleRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _nGOContext = nGOContext;
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _nGOContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

    }
}
