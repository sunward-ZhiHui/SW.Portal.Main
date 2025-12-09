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
using System.Security.Cryptography;
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
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeHandler(IEmployeeCommandRepository commandRepository, IApplicationUserCommandRepository applicationCommandRepository, IApplicationUserRoleCommandRepository applicationUserRoleCommandRepository, IEmployeeReportToCommandRepository employeeReportToCommandRepository, IApplicationUserQueryRepository applicationUserQueryRepository, IEmployeeQueryRepository employeeQueryRepository, IApplicationUserRoleQueryRepository applicationUserRoleQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _commandApplicationRepository = applicationCommandRepository;
            _commandApplicationUserRoleRepository = applicationUserRoleCommandRepository;
            _employeeReportToCommandRepository = employeeReportToCommandRepository;
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _employeeQueryRepository = employeeQueryRepository;
            _applicationUserRoleQueryRepository = applicationUserRoleQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeResponse> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            var app = await _employeeQueryRepository.GetAllAppUserByIDAsync(request.UserID);
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
            applicationUser.UserID = app.UserID;

            await _commandApplicationRepository.UpdateAsync(applicationUser);
            bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            if (app != null)
            {

                if (app.LoginID != request?.LoginID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = app?.LoginID, CurrentValue = request?.LoginID, ColumnName = "LoginIDName" });
                }
                if (app.UserName != request?.FirstName)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = app?.UserName, CurrentValue = request?.FirstName, ColumnName = "UserName" });

                }
                if (app.UserCode != request?.SageID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = app?.UserCode, CurrentValue = request?.SageID, ColumnName = "UserCode" });
                }
                if (app.DepartmentId != request?.DepartmentID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = app?.DepartmentId?.ToString(), CurrentValue = request?.DepartmentID?.ToString(), ColumnName = "DepartmentId" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = app?.DepartmentName.ToString(), CurrentValue = request?.DepartmentName.ToString(), ColumnName = "DepartmentName" });
                }
                var pass = EncryptDecryptPassword.Decrypt(app?.LoginPassword);
                if (pass != request?.LoginPassword)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = pass, CurrentValue = request?.LoginPassword.ToString(), ColumnName = "LoginPassword" });

                }
            }

            request.UserID = request.UserID;
            var apuserRole = await _applicationUserRoleQueryRepository.GetByIdAsync(request.UserID.Value);
            if (apuserRole != null)
            {
                var applicationUserRole = new ApplicationUserRole();
                applicationUserRole.UserRoleId = apuserRole.UserRoleId;
                applicationUserRole.UserId = request.UserID.Value;
                applicationUserRole.RoleId = request.RoleID.Value;
                await _commandApplicationUserRoleRepository.UpdateAsync(applicationUserRole);

                if (apuserRole.RoleId != request.RoleID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = apuserRole?.RoleId.ToString(), CurrentValue = request?.RoleID?.ToString(), ColumnName = "RoleID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = apuserRole?.RoleName.ToString(), CurrentValue = request?.RoleName.ToString(), ColumnName = "RoleName" });
                }
            }
            else
            {
                var applicationUserRoles = new ApplicationUserRole();
                applicationUserRoles.UserId = request.UserID.Value;
                applicationUserRoles.RoleId = request.RoleID.Value;
                var applicationData = await _commandApplicationUserRoleRepository.AddAsync(applicationUserRoles);
                auditList.Add(new HRMasterAuditTrail { CurrentValue = request?.RoleID?.ToString(), ColumnName = "RoleID" });
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = request?.RoleName.ToString(), ColumnName = "RoleName" });
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
            if (emp != null)
            {
                // bool isUpdate = false;
                if (emp.FirstName != request.FirstName)
                {
                    isUpdate = false;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.FirstName.ToString(), CurrentValue = request?.FirstName.ToString(), ColumnName = "FirstName" });

                }
                if (emp.LastName != request?.LastName)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.LastName.ToString(), CurrentValue = request?.LastName.ToString(), ColumnName = "LastName" });
                }
                if (emp.NickName != request?.NickName)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.NickName.ToString(), CurrentValue = request?.NickName.ToString(), ColumnName = "NickName" });
                }
                if (emp.SageID != request?.SageID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.SageID.ToString(), CurrentValue = request?.SageID.ToString(), ColumnName = "SageIDName" });
                }
                if (emp.Gender != request?.Gender)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.Gender.ToString(), CurrentValue = request?.Gender.ToString(), ColumnName = "Gender" });
                }
                if (emp.Email != request?.Email)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.Email.ToString(), CurrentValue = request?.Email.ToString(), ColumnName = "Email" });
                }
                if (emp.JobTitle != request?.JobTitle)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.JobTitle.ToString(), CurrentValue = request?.JobTitle.ToString(), ColumnName = "JobTitle" });
                }
                if (request?.ExpectedJoiningDate != emp.ExpectedJoiningDate)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.ExpectedJoiningDate != null ? emp.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null,CurrentValue = request?.ExpectedJoiningDate != null ? request.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "ExpectedJoiningDate" });

                }
                if (emp.Extension != request?.Extension)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.Extension.ToString(), CurrentValue = request?.Extension.ToString(), ColumnName = "Extension" });
                }
                if (emp.SpeedDial != request?.SpeedDial)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.SpeedDial.ToString(), CurrentValue = request?.SpeedDial.ToString(), ColumnName = "SpeedDial" });
                }
                if (request?.PlantID != emp.PlantID)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.PlantID?.ToString(), CurrentValue = request?.PlantID?.ToString(), ColumnName = "PlantID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.CompanyName.ToString(), CurrentValue = request?.CompanyName.ToString(), ColumnName = "CompanyName" });
                }

                if (request?.DepartmentID != emp.DepartmentID)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.DepartmentID?.ToString(), CurrentValue = request?.DepartmentID?.ToString(), ColumnName = "DepartmentID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.DepartmentName.ToString(), CurrentValue = request?.DepartmentName, ColumnName = "DepartmentName" });
                }
                if (request?.DesignationID != emp.DesignationID)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.DesignationID?.ToString(), CurrentValue = request?.DesignationID?.ToString(), ColumnName = "DesignationID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.DesignationName.ToString(), CurrentValue = request?.DesignationName.ToString(), ColumnName = "DesignationName" });
                }
                if (request?.SectionID != emp.SectionID)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.SectionID?.ToString(), CurrentValue = request?.SectionID?.ToString(), ColumnName = "SectionID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.SectionName.ToString(), CurrentValue = request?.SectionName.ToString(), ColumnName = "SectionName" });
                }
                if (request?.SubSectionID != emp.SubSectionID)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.SubSectionID?.ToString(), CurrentValue = request?.SubSectionID?.ToString(), ColumnName = "SubSectionID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.SubSectionName.ToString(), CurrentValue = request?.SubSectionName.ToString(), ColumnName = "SubSectionName" });
                }
                if (request?.LevelID != emp.LevelID)
                {
                    isUpdate = true;
                    var Names = request?.LevelMasterItems.FirstOrDefault(x => x.LevelID == emp.LevelID)?.Name;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.LevelID?.ToString(), CurrentValue = request?.LevelID?.ToString(), ColumnName = "LevelID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = Names, CurrentValue = request?.LevelName, ColumnName = "LevelName" });
                }
                if (request?.TypeOfEmployeement != emp.TypeOfEmployeement)
                {
                    isUpdate = true;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = request?.TypeOfEmployeement?.ToString(), CurrentValue = null, ColumnName = "TypeOfEmployeement" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = request?.TypeOfEmployeementName, CurrentValue = null, ColumnName = "TypeOfEmployeementName" });
                }
                if (request?.AcceptanceStatus != emp.AcceptanceStatus)
                {
                    isUpdate = true;
                    var Names = request?.View_mainEmployeeStatusItems.FirstOrDefault(x => x.StatusCodeId == emp.AcceptanceStatus)?.Value;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.AcceptanceStatus?.ToString(), CurrentValue = request?.AcceptanceStatus?.ToString(), ColumnName = "AcceptanceStatus" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = Names, CurrentValue = request?.AcceptanceStatusName, ColumnName = "AcceptanceStatusName" });
                }
                if (request?.LanguageID != emp.LanguageID)
                {
                    isUpdate = true;
                    var Names = request?.LanguageMasterItems.FirstOrDefault(x => x.LanguageId == emp.LanguageID)?.Name;
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp.LanguageID?.ToString(), CurrentValue = request?.LanguageID?.ToString(), ColumnName = "LanguageID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = request?.LanguageName, CurrentValue = null, ColumnName = "LanguageName" });
                }
                var newList = request?.ReportToIds.ToList();
                newList = newList != null ? newList : new List<long?>();
                var oldList = request?.OldReportToIds.ToList();
                oldList = oldList != null ? oldList : new List<long?>();
                var added = newList.Except(oldList).ToList();     // items in new but not old
                var removed = oldList.Except(newList).ToList();   // items in old but not new
                string oldValues = string.Empty; string newValues = string.Empty;
                if (added.Any() || removed.Any() && request.EmployeeItems?.Any() == true)
                {
                    isUpdate = true;
                    oldValues = string.Join(",", request.EmployeeItems.Where(w => oldList.Contains(w.UserID)).Select(w => w.FirstName));
                    newValues = string.Join(",", request.EmployeeItems.Where(w => newList.Contains(w.UserID)).Select(w => w.FirstName));
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = oldList?.Any() == true ? string.Join(",", oldList) : null, CurrentValue = newList?.Any() == true ? string.Join(",", newList) : null, ColumnName = "ReportToIds" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = oldValues, CurrentValue = newValues, ColumnName = "ReportToIdNames" });
                }
                if (isUpdate)
                {
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = request?.FirstName, CurrentValue = request?.FirstName, ColumnName = "DisplayName" });

                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.ModifiedByUserID?.ToString(), CurrentValue = request?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.ModifiedDate != null ? emp?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = request?.ModifiedDate != null ? request?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    
                    auditList.Add(new HRMasterAuditTrail { PreValue = emp?.ModifiedByUser?.ToString(), CurrentValue = request?.ModifiedByUser?.ToString(), ColumnName = "ModifiedBy" });
                }
                if (auditList.Count() > 0)
                {
                    HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                    {
                        HRMasterAuditTrailItems = auditList,
                        Type = "Employee",
                        FormType = "Update",
                        HRMasterSetId = request?.EmployeeID,
                        AuditUserId = request?.ModifiedByUserID,
                    };
                    await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
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
