﻿using System.Collections.Generic;
using System.Linq;
using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.Interfaces;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;

namespace ETicket.ApplicationServices.Services
{
    public class PrivilegeService: IPrivilegeService
    {
        #region private members

        private readonly IUnitOfWork uow;

        private readonly MapperService mapper;

        #endregion

        public PrivilegeService(IUnitOfWork uow)
        {
            this.uow = uow;
            mapper = new MapperService();
        }
        public IEnumerable<PrivilegeDto>GetPrivileges()
        {
            var privileges = uow.Privileges.GetAll();
            
            return mapper.Map<IQueryable<Privilege>, IEnumerable<PrivilegeDto>>(privileges).ToList();
        }
        public PrivilegeDto GetPrivilegeById(int id)
        {
            return mapper.Map<Privilege, PrivilegeDto>(uow.Privileges.Get(id));
        }

        public void Create(PrivilegeDto privilegeDto)
        {
            var privilege = mapper.Map<PrivilegeDto, Privilege>(privilegeDto);
            uow.Privileges.Create(privilege);
            uow.Save();
        }

        public void Update(PrivilegeDto privilegeDto)
        {
            var privilege = mapper.Map<PrivilegeDto, Privilege>(privilegeDto);
            uow.Privileges.Update(privilege);
            uow.Save();
        }

        public void Delete(int id)
        {
            uow.Privileges.Delete(id);
            uow.Save();
        }
    }
}
