using Application.Command.Employees;
using Application.Commands;
using Application.Common;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.EntityModel;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.Employee
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeResponse>
    {
        private readonly IEmployeeCommandRepository _commandRepository;
        private readonly IApplicationUserCommandRepository _commandApplicationRepository;
        private readonly IApplicationUserRoleCommandRepository _commandApplicationUserRoleRepository;
        private readonly IEmployeeReportToCommandRepository _employeeReportToCommandRepository;
        public CreateEmployeeHandler(IEmployeeCommandRepository commandRepository, IApplicationUserCommandRepository applicationCommandRepository, IApplicationUserRoleCommandRepository applicationUserRoleCommandRepository, IEmployeeReportToCommandRepository employeeReportToCommandRepository)
        {
            _commandRepository = commandRepository;
            _commandApplicationRepository = applicationCommandRepository;
            _commandApplicationUserRoleRepository = applicationUserRoleCommandRepository;
            _employeeReportToCommandRepository = employeeReportToCommandRepository;
        }
        public async Task<EmployeeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {

            var applicationUser = new Core.Entities.ApplicationUser();
            applicationUser.LoginID = request.LoginID;
            applicationUser.UserCode = request.SageID;
            applicationUser.DepartmentId = request.DepartmentID;
            applicationUser.LoginPassword = EncryptDecryptPassword.Encrypt(request.LoginPassword);
            applicationUser.SessionId = Guid.NewGuid();
            applicationUser.AddedByUserID = request.AddedByUserID;
            applicationUser.AddedDate = DateTime.Now;
            applicationUser.ModifiedDate = DateTime.Now;
            applicationUser.ModifiedByUserID = request.ModifiedByUserID;
            applicationUser.ModifiedByUserID = request.AddedByUserID;
            applicationUser.UserName = request.FirstName;
            applicationUser.StatusCodeID = request.StatusCodeID;
            applicationUser.IsPasswordChanged = false;
            applicationUser.AuthenticationType = 1;
            var applicationData = await _commandApplicationRepository.AddAsync(applicationUser);
            request.UserID = applicationData;

            var applicationUserRole = new ApplicationUserRole();
            applicationUserRole.UserId = applicationData.Value;
            applicationUserRole.RoleId = request.RoleID.Value;
            await _commandApplicationUserRoleRepository.AddAsync(applicationUserRole);


            var employee = new Core.Entities.Employee();
            employee.UserId = applicationData;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.NickName = request.NickName;
            employee.SageId = request.SageID;
            employee.Email = request.Email;
            employee.PlantId = request.PlantID;
            employee.LevelId = request.LevelID;
            employee.DepartmentId = request.DepartmentID;
            employee.DesignationId = request.DesignationID;
            employee.JobTitle = request.JobTitle;
            employee.TypeOfEmployeement = request.TypeOfEmployeement;
            employee.LanguageId = request.LanguageID;
            employee.SectionId = request.SectionID;
            employee.SubSectionId = request.SubSectionID;
            employee.CityId = request.CityID;
            employee.RegionId = request.RegionID;
            employee.DateOfEmployeement = request.DateOfEmployeement;
            employee.LastWorkingDate = request.LastWorkingDate;
            employee.Extension = request.Extension;
            employee.SpeedDial = request.SpeedDial;
            employee.Mobile = request.Mobile;
            employee.SkypeAddress = request.SkypeAddress;
            employee.HeadCount = request.HeadCount;
            employee.DivisionId = request.DivisionID;
            employee.ModifiedByUserId = request.ModifiedByUserID;
            employee.AddedByUserId = request.AddedByUserID;
            employee.StatusCodeId = request.StatusCodeID;
            employee.AddedDate = DateTime.Now;
            employee.ModifiedDate = DateTime.Now;
            employee.ExpectedJoiningDate = request.ExpectedJoiningDate;
            employee.AcceptanceStatus = request.AcceptanceStatus;
            employee.Nricno = request.Nricno;

            var data = await _commandRepository.AddAsync(employee);
            request.EmployeeID = data.Value;
            if (request.ReportToIds != null)
            {
                var listData = request.ReportToIds.ToList();
                if (listData.Count > 0)
                {
                    request.ReportToIds.ToList().ForEach(a =>
                    {
                        var employeeReportTo = new EmployeeReportTo();
                        employeeReportTo.EmployeeId = data;
                        employeeReportTo.ReportToId = a;
                        _employeeReportToCommandRepository.AddAsync(employeeReportTo);
                    });
                }
            }
            var response = new EmployeeResponse
            {
                EmployeeID = (long)data,
                FirstName = request.FirstName,
            };
            return response;
        }
    }
}
