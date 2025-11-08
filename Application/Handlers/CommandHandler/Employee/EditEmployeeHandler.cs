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
            applicationUser.UserID=app.UserID;
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _commandApplicationRepository.UpdateAsync(applicationUser);
            if (app != null)
            {
                bool isUpdtes = false;
                if (app.LoginID != request?.LoginID)
                {
                    isUpdtes = true;
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app.LoginID, request?.LoginID, request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LoginIDName", uid);
                }
                if (app.UserName != request?.FirstName)
                {
                    isUpdtes = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app.UserName, request?.FirstName, request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "UserName", uid);

                }
                if (app.UserCode != request?.SageID)
                {
                    isUpdtes = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app.UserCode, request?.SageID, request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "UserCode", uid);
                }
                if (app.DepartmentId != request?.DepartmentID)
                {
                    isUpdtes = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app.DepartmentId?.ToString(), request?.DepartmentID.ToString(), request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DepartmentID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app.DepartmentName, request?.DepartmentName.ToString(), request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DepartmentName", uid);
                }
                var pass=EncryptDecryptPassword.Decrypt(app?.LoginPassword);
                if (pass != request?.LoginPassword)
                {
                    isUpdtes = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", pass, request?.LoginPassword.ToString(), request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LoginPassword", uid);

                }
                if (isUpdtes)
                {
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app?.ModifiedByUserID?.ToString(), request?.ModifiedByUserID.ToString(), request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app?.ModifiedByUser, request?.ModifiedByUser.ToString(), request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedBy", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUser", "Update", app?.ModifiedDate != null ? app.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request?.ModifiedDate != null ? request.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request?.UserID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedDate", uid);

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
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", request?.RoleID?.ToString(), apuserRole?.RoleId.ToString(), apuserRole?.UserRoleId, guid, request?.ModifiedByUserID, DateTime.Now, false, "RoleID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", request?.RoleName?.ToString(), apuserRole?.RoleName, apuserRole?.UserRoleId, guid, request?.ModifiedByUserID, DateTime.Now, false, "RoleName", uid);
                }
            }
            else
            {
                var applicationUserRoles = new ApplicationUserRole();
                applicationUserRoles.UserId = request.UserID.Value;
                applicationUserRoles.RoleId = request.RoleID.Value;
                var applicationData = await _commandApplicationUserRoleRepository.AddAsync(applicationUserRoles);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", null, request?.RoleID?.ToString(), applicationData, guid, request?.AddedByUserID, DateTime.Now, false, "RoleID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", null, request?.RoleName?.ToString(), applicationData, guid, request?.AddedByUserID, DateTime.Now, false, "RoleName", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", null, applicationData.Value.ToString(), applicationData, guid, request?.AddedByUserID, DateTime.Now, false, "UserId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("ApplicationUserRole", "Update", null, request?.FirstName, applicationData, guid, request?.AddedByUserID, DateTime.Now, false, "UserName", uid);

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
                bool isUpdate = false;
                if (emp.FirstName != request.FirstName)
                {
                    isUpdate = false;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.FirstName, request?.FirstName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "FirstName", uid);

                }
                if (emp.LastName != request?.LastName)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.LastName, request?.LastName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LastName", uid);
                }
                if (emp.NickName != request?.NickName)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.NickName, request?.NickName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "NickName", uid);
                }
                if (emp.SageID != request?.SageID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SageID, request?.SageID, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SageIDName", uid);
                }
                if (emp.Gender != request?.Gender)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.Gender, request?.Gender, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "Gender", uid);
                }
                if (emp.Email != request?.Email)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.Email, request?.Email, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "Email", uid);
                }
                if (emp.JobTitle != request?.JobTitle)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.JobTitle, request?.JobTitle, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "JobTitle", uid);
                }
                if (request?.ExpectedJoiningDate != emp.ExpectedJoiningDate)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp?.ExpectedJoiningDate != null ? emp.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null, request?.ExpectedJoiningDate != null ? request.ExpectedJoiningDate.Value.ToString("dd-MMM-yyyy") : null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ExpectedJoiningDate", uid);
                }
                if (emp.Extension != request?.Extension)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.Extension, request?.Extension, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "Extension", uid);
                }
                if (emp.SpeedDial != request?.SpeedDial)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SpeedDial, request?.SpeedDial, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SpeedDial", uid);
                }
                if (request?.PlantID != emp.PlantID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.PlantID?.ToString(), request?.PlantID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "PlantID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.CompanyName, request?.CompanyName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "CompanyName", uid);
                }

                if (request?.DepartmentID != emp.DepartmentID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.DepartmentID?.ToString(), request?.DepartmentID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DepartmentID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.DepartmentName.ToString(), request?.DepartmentName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DepartmentName", uid);
                }
                if (request?.DesignationID != emp.DesignationID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.DesignationID?.ToString(), request?.DesignationID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DesignationID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.DesignationName, request?.DesignationName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "DesignationName", uid);
                }
                if (request?.SectionID != emp.SectionID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SectionID?.ToString(), request?.SectionID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SectionID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SectionName, request?.SectionName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SectionName", uid);
                }
                if (request?.SubSectionID != emp.SubSectionID)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SubSectionID?.ToString(), request?.SubSectionID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SubSectionID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.SubSectionName, request?.SubSectionName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "SubSectionName", uid);
                }
                if (request?.LevelID != emp.LevelID)
                {
                    isUpdate = true;
                    var Names = request?.LevelMasterItems.FirstOrDefault(x => x.LevelID == emp.LevelID)?.Name;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.LevelID?.ToString(), request?.LevelID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LevelID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", Names, request?.LevelName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LevelName", uid);
                }
                if (request?.TypeOfEmployeement != emp.TypeOfEmployeement)
                {
                    isUpdate = true;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", request?.TypeOfEmployeement?.ToString(), null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "TypeOfEmployeement", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", request?.TypeOfEmployeementName, null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "TypeOfEmployeementName", uid);
                }
                if (request?.AcceptanceStatus != emp.AcceptanceStatus)
                {
                    isUpdate = true;
                    var Names = request?.View_mainEmployeeStatusItems.FirstOrDefault(x => x.StatusCodeId == emp.AcceptanceStatus)?.Value;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.AcceptanceStatus?.ToString(), request?.AcceptanceStatus?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "AcceptanceStatus", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", Names, request?.AcceptanceStatusName, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "AcceptanceStatusName", uid);
                }
                if (request?.LanguageID != emp.LanguageID)
                {
                    isUpdate = true;
                    var Names = request?.LanguageMasterItems.FirstOrDefault(x => x.LanguageId == emp.LanguageID)?.Name;
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp.LanguageID?.ToString(), request?.LanguageID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LanguageID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", request?.LanguageName, null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "LanguageName", uid);
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
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", oldList?.Any() == true ? string.Join(",", oldList) : null, newList?.Any() == true ? string.Join(",", newList) : null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ReportToIds", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", oldValues, newValues, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ReportToIdNames", uid);
                }
                if (isUpdate)
                {
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp?.ModifiedByUserID?.ToString(), request?.ModifiedByUserID?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp?.ModifiedDate != null ? emp?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request?.ModifiedDate != null ? request?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Employee", "Update", emp?.ModifiedByUser?.ToString(), request?.ModifiedByUser?.ToString(), request?.EmployeeID, guid, request?.ModifiedByUserID, DateTime.Now, false, "ModifiedBy", uid);
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
