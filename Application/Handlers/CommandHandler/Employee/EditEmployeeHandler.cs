using Application.Command.Employees;
using Application.Commands;
using Application.Common;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.EntityModel;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.Employee
{
    public class EditEmployeeHandler : IRequestHandler<EditEmployeeCommand, EmployeeResponse>
    {
        private readonly IEmployeeCommandRepository _commandRepository;
        private readonly IApplicationUserCommandRepository _commandApplicationRepository;
        private readonly IApplicationUserRoleCommandRepository _commandApplicationUserRoleRepository;
        private readonly IEmployeeReportToCommandRepository _employeeReportToCommandRepository;
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IEmployeeQueryRepository _employeeQueryRepository;
        private readonly IApplicationUserRoleQueryRepository _applicationUserRoleQueryRepository;
        public EditEmployeeHandler(IEmployeeCommandRepository commandRepository, IApplicationUserCommandRepository applicationCommandRepository, IApplicationUserRoleCommandRepository applicationUserRoleCommandRepository, IEmployeeReportToCommandRepository employeeReportToCommandRepository, IApplicationUserQueryRepository applicationUserQueryRepository, IEmployeeQueryRepository employeeQueryRepository, IApplicationUserRoleQueryRepository applicationUserRoleQueryRepository)
        {
            _commandRepository = commandRepository;
            _commandApplicationRepository = applicationCommandRepository;
            _commandApplicationUserRoleRepository = applicationUserRoleCommandRepository;
            _employeeReportToCommandRepository = employeeReportToCommandRepository;
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _employeeQueryRepository = employeeQueryRepository;
            _applicationUserRoleQueryRepository = applicationUserRoleQueryRepository;
        }
        public async Task<EmployeeResponse> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            var app = await _applicationUserQueryRepository.GetAsync((int)request.UserID);
            var applicationUser = new Core.Entities.ApplicationUser();
            applicationUser.LoginID = request.LoginID;
            applicationUser.UserCode = request.SageID;
            applicationUser.DepartmentId = request.DepartmentID;
            applicationUser.LoginPassword = EncryptDecryptPassword.Encrypt(request.LoginPassword);
            applicationUser.SessionId = app.SessionId;
            applicationUser.AddedByUserID = request.AddedByUserID;
            applicationUser.AddedDate = app.AddedDate;
            applicationUser.ModifiedDate = DateTime.Now;
            applicationUser.ModifiedByUserID = request.ModifiedByUserID;
            applicationUser.ModifiedByUserID = request.AddedByUserID;
            applicationUser.UserName = request.FirstName;
            applicationUser.StatusCodeID = request.StatusCodeID;
            applicationUser.IsPasswordChanged = false;
            applicationUser.AuthenticationType = 1;
            applicationUser.InvalidAttempts = app.InvalidAttempts;
            applicationUser.LastPasswordChanged = app.LastPasswordChanged;
            applicationUser.LastAccessDate = app.LastAccessDate;
            applicationUser.Locked = app.Locked;


            await _commandApplicationRepository.UpdateAsync(applicationUser);
            request.UserID = request.UserID;
            var apuserRole = await _applicationUserRoleQueryRepository.GetByIdAsync(request.UserID.Value);
            if (apuserRole != null)
            {
                var applicationUserRole = new ApplicationUserRole();
                applicationUserRole.UserRoleId = apuserRole.UserRoleId;
                applicationUserRole.UserId = request.UserID.Value;
                applicationUserRole.RoleId = request.RoleID.Value;
                await _commandApplicationUserRoleRepository.UpdateAsync(applicationUserRole);
            }
            else
            {
                var applicationUserRoles = new ApplicationUserRole();
                applicationUserRoles.UserId = request.UserID.Value;
                applicationUserRoles.RoleId = request.RoleID.Value;
                await _commandApplicationUserRoleRepository.AddAsync(applicationUserRoles);
            }

            var emp = await _employeeQueryRepository.GetAllByIdAsync(request.EmployeeID);
            var employee = new Core.Entities.Employee();
            employee.EmployeeId = request.EmployeeID;
            employee.UserId = request.UserID;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.NickName = request.NickName;
            employee.SageId = request.SageID;
            employee.Email = request.Email;
            employee.Gender = request.Gender;
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
            employee.AddedByUserId = emp.AddedByUserID;
            employee.StatusCodeId = request.StatusCodeID;
            employee.AddedDate = emp.AddedDate;
            employee.ModifiedDate = DateTime.Now;
            employee.ExpectedJoiningDate = request.ExpectedJoiningDate;
            employee.AcceptanceStatus = request.AcceptanceStatus;
            employee.Nricno = request.Nricno;

            await _commandRepository.UpdateAsync(employee);

            await _employeeQueryRepository.DeleteEmployeeReportAsync(request.EmployeeID);
            if (request.ReportToIds != null)
            {
                var listData = request.ReportToIds.ToList();
                if (listData.Count > 0)
                {
                    request.ReportToIds.ToList().ForEach(a =>
                    {
                        var employeeReportTo = new EmployeeReportTo();
                        employeeReportTo.EmployeeId = request.EmployeeID;
                        employeeReportTo.ReportToId = a;
                        _employeeReportToCommandRepository.AddAsync(employeeReportTo);
                    });
                }
            }
            var response = new EmployeeResponse
            {
                EmployeeID = request.EmployeeID,
                FirstName = request.FirstName,
                SessionId = applicationUser.SessionId,
                UserID = request.UserID,
            };
            return response;
        }
    }
}
