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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LoginID, ColumnName = "LoginIDName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.UserCode, ColumnName = "UserCode" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LoginPassword, ColumnName = "LoginPassword" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.RoleID?.ToString(), ColumnName = "RoleID" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.RoleName, ColumnName = "RoleName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.FirstName, ColumnName = "FirstName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.FirstName, ColumnName = "FirstName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LastName, ColumnName = "LastName" });
            if (!string.IsNullOrEmpty(request?.NickName))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.NickName, ColumnName = "NickName" });
            }
            if (!string.IsNullOrEmpty(request?.SageID))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SageID, ColumnName = "SageIDName" });
            }
            if (!string.IsNullOrEmpty(request?.Gender))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.Gender, ColumnName = "Gender" });
            }
            if (!string.IsNullOrEmpty(request?.Email))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.Email, ColumnName = "Email" });
            }
            if (!string.IsNullOrEmpty(request?.JobTitle))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.JobTitle, ColumnName = "JobTitle" });
            }
            if (request?.ExpectedJoiningDate != null)
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.ExpectedJoiningDate != null ? request.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "ExpectedJoiningDate" });
            }
            if (!string.IsNullOrEmpty(request?.Extension))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.Extension, ColumnName = "Extension" });
            }
            if (!string.IsNullOrEmpty(request?.SpeedDial))
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SpeedDial, ColumnName = "SpeedDial" });
            }
            if (request?.PlantID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.PlantID?.ToString(), ColumnName = "PlantID" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.CompanyName, ColumnName = "CompanyName" });
            }
            if (request?.DepartmentID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.DepartmentID?.ToString(), ColumnName = "DepartmentID" });
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.DepartmentName, ColumnName = "DepartmentName" });
            }
            if (request?.DesignationID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.DesignationID?.ToString(), ColumnName = "DesignationID" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.DesignationName, ColumnName = "DesignationName" });
            }
            if (request?.SectionID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SectionID?.ToString(), ColumnName = "SectionID" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SectionName, ColumnName = "SectionName" });
            }
            if (request?.SubSectionID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SubSectionID?.ToString(), ColumnName = "SubSectionID" });
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.SubSectionName, ColumnName = "SubSectionName" });
            }
            if (request?.LevelID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LevelID?.ToString(), ColumnName = "LevelID" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LevelName, ColumnName = "LevelName" });
            }
            if (request?.TypeOfEmployeement > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.TypeOfEmployeement?.ToString(), ColumnName = "TypeOfEmployeement" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.TypeOfEmployeementName, ColumnName = "TypeOfEmployeementName" });
            }
            if (request?.AcceptanceStatus > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.AcceptanceStatus?.ToString(), ColumnName = "AcceptanceStatus" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.AcceptanceStatusName, ColumnName = "AcceptanceStatusName" });
            }
            if (request?.LanguageID > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LanguageID?.ToString(), ColumnName = "LanguageID" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.LanguageName, ColumnName = "LanguageName" });
            }
            if (request?.ReportToIds != null && request?.ReportToIds?.ToList().Count > 0)
            {
                
                auditList.Add(new HRMasterAuditTrail { CurrentValue = string.Join(",", request?.ReportToIds?.ToList()), ColumnName = "ReportToIds" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.ReportToIdNames, ColumnName = "ReportToIdNames" });
            }

            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.AddedByUserID?.ToString(), ColumnName = "AddedByUserId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.AddedDate != null ? request.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.AddedByUser?.ToString(), ColumnName = "AddedBy" }); if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "Employee",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = request?.AddedByUserID,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
