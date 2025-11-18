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
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeResponse>
    {
        private readonly IEmployeeCommandRepository _commandRepository;
        private readonly IApplicationUserCommandRepository _commandApplicationRepository;
        private readonly IApplicationUserRoleCommandRepository _commandApplicationUserRoleRepository;
        private readonly IEmployeeReportToCommandRepository _employeeReportToCommandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateEmployeeHandler(IEmployeeCommandRepository commandRepository, IApplicationUserCommandRepository applicationCommandRepository, IApplicationUserRoleCommandRepository applicationUserRoleCommandRepository, IEmployeeReportToCommandRepository employeeReportToCommandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _commandApplicationRepository = applicationCommandRepository;
            _commandApplicationUserRoleRepository = applicationUserRoleCommandRepository;
            _employeeReportToCommandRepository = employeeReportToCommandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
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
            applicationUser.InvalidAttempts = 0;
            applicationUser.LastPasswordChanged = DateTime.Now;
            applicationUser.Locked = false;
            var datas = await _commandApplicationRepository.AddAsync(applicationUser);
            request.UserID = datas;
            var applicationUserRole = new ApplicationUserRole();
            applicationUserRole.UserId = datas.Value;
            applicationUserRole.RoleId = request.RoleID.Value;
            await _commandApplicationUserRoleRepository.AddAsync(applicationUserRole);
            

            var employee = new Core.Entities.Employee();
            employee.UserId = datas;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.NickName = request.NickName;
            employee.SageId = request.SageID;
            employee.Gender = request.Gender;
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
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LoginID, data, guid, request?.AddedByUserID, DateTime.Now, false, "LoginIDName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SageID, data, guid, request?.AddedByUserID, DateTime.Now, false, "UserCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LoginPassword.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "LoginPassword", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.RoleID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "RoleID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.RoleName?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "RoleName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.FirstName, data, guid, request?.AddedByUserID, DateTime.Now, false, "FirstName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", request?.FirstName, request?.FirstName, data, guid, request?.AddedByUserID, DateTime.Now, false, "DisplayName", uid);

            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LastName, data, guid, request?.AddedByUserID, DateTime.Now, false, "LastName", uid);
            if (!string.IsNullOrEmpty(request?.NickName))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.NickName, data, guid, request?.AddedByUserID, DateTime.Now, false, "NickName", uid);
            }
            if (!string.IsNullOrEmpty(request?.SageID))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SageID, data, guid, request?.AddedByUserID, DateTime.Now, false, "SageIDName", uid);
            }
            if (!string.IsNullOrEmpty(request?.Gender))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.Gender, data, guid, request?.AddedByUserID, DateTime.Now, false, "Gender", uid);
            }
            if (!string.IsNullOrEmpty(request?.Email))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.Email, data, guid, request?.AddedByUserID, DateTime.Now, false, "Email", uid);
            }
            if (!string.IsNullOrEmpty(request?.JobTitle))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.JobTitle, data, guid, request?.AddedByUserID, DateTime.Now, false, "JobTitle", uid);
            }
            if (request?.ExpectedJoiningDate != null)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.ExpectedJoiningDate != null ? request.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null, data, guid, request?.AddedByUserID, DateTime.Now, false, "ExpectedJoiningDate", uid);
            }
            if (!string.IsNullOrEmpty(request?.Extension))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.Extension, data, guid, request?.AddedByUserID, DateTime.Now, false, "Extension", uid);
            }
            if (!string.IsNullOrEmpty(request?.SpeedDial))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SpeedDial, data, guid, request?.AddedByUserID, DateTime.Now, false, "SpeedDial", uid);
            }
            if (request?.PlantID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.PlantID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "PlantID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.CompanyName, data, guid, request?.AddedByUserID, DateTime.Now, false, "CompanyName", uid);
            }
            if (request?.DepartmentID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.DepartmentID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "DepartmentID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.DepartmentName, data, guid, request?.AddedByUserID, DateTime.Now, false, "DepartmentName", uid);
            }
            if (request?.DesignationID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.DesignationID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "DesignationID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.DesignationName, data, guid, request?.AddedByUserID, DateTime.Now, false, "DesignationName", uid);
            }
            if (request?.SectionID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SectionID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "SectionID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SectionName, data, guid, request?.AddedByUserID, DateTime.Now, false, "SectionName", uid);
            }
            if (request?.SubSectionID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SubSectionID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "SubSectionID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.SubSectionName, data, guid, request?.AddedByUserID, DateTime.Now, false, "SubSectionName", uid);
            }
            if (request?.LevelID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LevelID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "LevelID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LevelName, data, guid, request?.AddedByUserID, DateTime.Now, false, "LevelName", uid);
            }
            if (request?.TypeOfEmployeement > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.TypeOfEmployeement?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "TypeOfEmployeement", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.TypeOfEmployeementName, data, guid, request?.AddedByUserID, DateTime.Now, false, "TypeOfEmployeementName", uid);
            }
            if (request?.AcceptanceStatus > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.AcceptanceStatus?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "AcceptanceStatus", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.AcceptanceStatusName, data, guid, request?.AddedByUserID, DateTime.Now, false, "AcceptanceStatusName", uid);
            }
            if (request?.LanguageID > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LanguageID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "LanguageID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.LanguageName, data, guid, request?.AddedByUserID, DateTime.Now, false, "LanguageName", uid);
            }
            if (request?.ReportToIds != null && request?.ReportToIds?.ToList().Count > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, string.Join(",", request?.ReportToIds?.ToList()), data, guid, request?.AddedByUserID, DateTime.Now, false, "ReportToIds", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.ReportToIdNames, data, guid, request?.AddedByUserID, DateTime.Now, false, "ReportToIdNames", uid);
            }
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.AddedByUserID?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "AddedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.AddedDate != null ? request.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, data, guid, request?.AddedByUserID, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Add", null, request?.AddedByUser?.ToString(), data, guid, request?.AddedByUserID, DateTime.Now, false, "AddedBy", uid);

            var response = new EmployeeResponse
            {
                EmployeeID = (long)data,
                FirstName = request.FirstName,
                SessionId = applicationUser.SessionId,
                UserID = (long?)datas,
            };
            return response;
        }
    }
}
