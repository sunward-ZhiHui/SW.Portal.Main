using Application.Common;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class GenerateDocumentNoSeriesService : QueryRepository<DocumentNoSeriesModel>, IGenerateDocumentNoSeriesSeviceQueryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IGenerateDocumentNoSeriesQueryRepository _generateDocumentNoSeriesQueryRepository;
        public GenerateDocumentNoSeriesService(IConfiguration configuration, IGenerateDocumentNoSeriesQueryRepository generateDocumentNoSeriesQueryRepository) : base(configuration)
        {
            _configuration = configuration;
            _generateDocumentNoSeriesQueryRepository = generateDocumentNoSeriesQueryRepository;
        }
        public async Task<string> GenerateDocumentProfileAutoNumber(DocumentNoSeriesModel noSeriesModel)
        {
            var documentNo = await GenerateDocumentNoAsync(new DocumentNoSeriesModel
            {
                ProfileID = noSeriesModel.ProfileID > 0 ? noSeriesModel.ProfileID : null,
                AddedByUserID = noSeriesModel.AddedByUserID,
                StatusCodeID = noSeriesModel.StatusCodeID,
                DepartmentName = noSeriesModel.DepartmentName,
                CompanyCode = noSeriesModel.CompanyCode,
                SectionName = noSeriesModel.SectionName,
                SubSectionName = noSeriesModel.SubSectionName,
                DepartmentId = noSeriesModel.DepartmentId > 0 ? noSeriesModel.DepartmentId : null,
                PlantID = noSeriesModel.PlantID > 0 ? noSeriesModel.PlantID : null,
                SectionId = noSeriesModel.SectionId > 0 ? noSeriesModel.SectionId : null,
                SubSectionId = noSeriesModel.SubSectionId > 0 ? noSeriesModel.SubSectionId : null,
                ScreenID = noSeriesModel.ScreenID,
                ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,
                CompanyId = noSeriesModel.PlantID > 0 ? noSeriesModel.PlantID : null,
                SessionId = noSeriesModel.SessionId,
                FileProfileTypeId = noSeriesModel.FileProfileTypeId,

                RequestorId = noSeriesModel.RequestorId,
                Title = noSeriesModel.Title,
                Description = noSeriesModel.Description,
                IsUpload = noSeriesModel.IsUpload,
                ModifiedByUserID = noSeriesModel.ModifiedByUserID,
                ModifiedDate = noSeriesModel.ModifiedDate,
                VersionNo = noSeriesModel.VersionNo,
                EffectiveDate = noSeriesModel.EffectiveDate,
                NextReviewDate = noSeriesModel.NextReviewDate,
                Date = noSeriesModel.Date,
                Link = noSeriesModel.Link,
                ReasonToVoid = noSeriesModel.ReasonToVoid,

            });

            return documentNo;
        }
        public async Task<string> GenerateDocumentNoAsync(DocumentNoSeriesModel noSeriesModel)
        {
            bool isCompanyDepartmentExist = false;
            string documentNo = string.Empty;
            string deptProfileCode = "";
            string sectionProfileCode = "";
            string subSectionProfileCode = "";
            var masterList = await _generateDocumentNoSeriesQueryRepository.GetMasterLists(noSeriesModel);
            var profileSettings = masterList.DocumentProfileNoSeries.FirstOrDefault(s => s.ProfileId == noSeriesModel.ProfileID);
            GenerateDocumentNoSeriesModel generateDocumentNoSeriesModel = new GenerateDocumentNoSeriesModel();
            if (profileSettings != null)
            {
                generateDocumentNoSeriesModel.DocumentProfileNoSeries = profileSettings != null ? profileSettings : new DocumentProfileNoSeries();
                if (masterList.ProfileAutoNumber != null && masterList.ProfileAutoNumber.Count > 0)
                {
                    var profileAutoNo = masterList.ProfileAutoNumber.FirstOrDefault();
                    var incrementalNo = profileSettings.IncrementalNo > 0 ? profileSettings.IncrementalNo : 0;

                    int lastNoUsed;
                    int lnused = 0 ;
                    if (int.TryParse(profileAutoNo.LastNoUsed, out lastNoUsed))
                    {
                        lnused = lastNoUsed > 0 ? lastNoUsed : 0;                        
                    }

                    var LastNoUsed = (Convert.ToInt32(lnused) + Convert.ToInt32(incrementalNo)) * Convert.ToInt32(noSeriesModel.NoOfCounts);
                    ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                    {
                        ProfileId = noSeriesModel.ProfileID,
                        LastNoUsed = LastNoUsed.ToString("D" + profileSettings.NoOfDigit),
                    };
                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(newProfileAutoNumber);
                }
                else
                {
                    var lastNo = noSeriesModel.NoOfCounts * (profileSettings.NoOfDigit > 0 ? profileSettings.NoOfDigit : 0);
                    ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                    {
                        ProfileId = noSeriesModel.ProfileID,
                        CompanyId = noSeriesModel.PlantID,
                        DepartmentId = noSeriesModel.DepartmentId,
                        SectionId = noSeriesModel.SectionId,
                        SubSectionId = noSeriesModel.SubSectionId,
                        LastNoUsed = lastNo > 0 ? lastNo.ToString() : "",
                        ProfileYear = profileSettings.StartWithYear.GetValueOrDefault(false) ? DateTime.Now.Year : 0,
                        ScreenId = noSeriesModel.ScreenID,
                        ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,

                    };
                    await _generateDocumentNoSeriesQueryRepository.InsertProfileAutoNumber(newProfileAutoNumber);
                }

            }
            if (noSeriesModel.CompanyId > 0 && masterList.Plants != null && masterList.Plants.Count > 0)
            {
                noSeriesModel.CompanyCode = masterList.Plants.FirstOrDefault(p => p.PlantID == noSeriesModel.CompanyId)?.PlantCode;
            }
            if (noSeriesModel.DepartmentId > 0 && masterList.Departments != null && masterList.Departments.Count > 0)
            {
                deptProfileCode = masterList.Departments.FirstOrDefault(s => s.DepartmentId == noSeriesModel.DepartmentId)?.ProfileCode;
            }
            if (noSeriesModel.SectionId > 0 && masterList.Sections != null && masterList.Sections.Count > 0)
            {
                noSeriesModel.SectionName = masterList.Sections.FirstOrDefault(s => s.SectionId == noSeriesModel.SectionId)?.ProfileCode;
                if (noSeriesModel.SectionName == null && masterList.Sections != null && masterList.Sections.Count > 0)
                {
                    noSeriesModel.SectionName = masterList.Sections.FirstOrDefault(s => s.SectionId == noSeriesModel.SectionId)?.Code;
                }
            }
            if (noSeriesModel.SubSectionId > 0 && masterList.SubSections != null && masterList.SubSections.Count > 0)
            {
                noSeriesModel.SubSectionName = masterList.SubSections.FirstOrDefault(s => s.SubSectionId == noSeriesModel.SubSectionId)?.ProfileCode;
                if (noSeriesModel.SubSectionName == null)
                {
                    noSeriesModel.SubSectionName = masterList.SubSections.FirstOrDefault(s => s.SubSectionId == noSeriesModel.SubSectionId)?.Code;
                }

            }
            if (noSeriesModel.ProfileID == null || noSeriesModel.ProfileID <= 0)
            {
                return null;
            }
            else
            {

                var profileAutoNumbers = await _generateDocumentNoSeriesQueryRepository.GetProfileAutoNumber(profileSettings.ProfileId, noSeriesModel);

                if (profileSettings != null && profileSettings.CompanyId > 0 && masterList.Plants != null && masterList.Plants.Count > 0 && (noSeriesModel.CompanyId == null || noSeriesModel.PlantID == null))
                {
                    noSeriesModel.CompanyCode = masterList.Plants.Where(s => s.PlantID == profileSettings.CompanyId).FirstOrDefault().PlantCode;
                }
                if (profileSettings != null && profileSettings.DeparmentId > 0 && noSeriesModel.DepartmentId == null && masterList.Departments != null && masterList.Departments.Count > 0)
                {
                    var department = masterList.Departments.Where(s => s.DepartmentId == profileSettings.DeparmentId)?.FirstOrDefault();
                    if (department != null)
                    {
                        if (department.ProfileCode != null)
                        {
                            noSeriesModel.DepartmentName = department.ProfileCode;
                        }
                        else
                        {
                            noSeriesModel.DepartmentName = department.Code;
                        }
                    }
                }

                List<string> numberSeriesCodes = new List<string> { "Company", "Department" };
                List<NumberSeriesCodeModel> numberSeriesCodeModels = new List<NumberSeriesCodeModel>();

                List<Seperator> seperators = new List<Seperator>();
                seperators.Add(new Seperator { SeperatorSymbol = "/", SeperatorValue = 1 });
                seperators.Add(new Seperator { SeperatorSymbol = "-", SeperatorValue = 2 });

                var seperator = seperators.FirstOrDefault(s => s.SeperatorValue == profileSettings.SeperatorToUse.GetValueOrDefault(0));
                var seperatorSymbol = seperator != null ? seperator.SeperatorSymbol : "/";

                if (!String.IsNullOrEmpty(profileSettings.Abbreviation1))
                {
                    numberSeriesCodeModels = JsonConvert.DeserializeObject<List<NumberSeriesCodeModel>>(profileSettings.Abbreviation1).ToList();
                    isCompanyDepartmentExist = numberSeriesCodeModels.Any(c => numberSeriesCodes.Contains(c.Name));
                    numberSeriesCodeModels.OrderBy(n => n.Index).ToList().ForEach(n =>
                    {
                        if (n.Name == "Company" && !string.IsNullOrEmpty(noSeriesModel.CompanyCode))
                        {
                            documentNo += noSeriesModel.CompanyCode + seperatorSymbol;
                        }
                        if (n.Name == "Department" && !string.IsNullOrEmpty(noSeriesModel.DepartmentName))
                        {
                            noSeriesModel.DepartmentName = string.IsNullOrEmpty(deptProfileCode) ? noSeriesModel.DepartmentName : deptProfileCode;
                            string[] departmentDetails = noSeriesModel.DepartmentName.Split(" ");
                            if (departmentDetails.Length > 1 && !string.IsNullOrEmpty(departmentDetails[1]))
                            {
                                documentNo += departmentDetails[1] + seperatorSymbol;
                            }
                            else if (!string.IsNullOrEmpty(noSeriesModel.DepartmentName))
                            {
                                documentNo += noSeriesModel.DepartmentName + seperatorSymbol;
                            }
                        }
                        if (n.Name == "Section" && !string.IsNullOrEmpty(noSeriesModel.SectionName))
                        {
                            noSeriesModel.SectionName = string.IsNullOrEmpty(sectionProfileCode) ? noSeriesModel.SectionName : sectionProfileCode;
                            documentNo += noSeriesModel.SectionName + seperatorSymbol;
                        }
                        if (n.Name == "SubSection" && !string.IsNullOrEmpty(noSeriesModel.SubSectionName))
                        {
                            noSeriesModel.SubSectionName = string.IsNullOrEmpty(subSectionProfileCode) ? noSeriesModel.SubSectionName : subSectionProfileCode;
                            documentNo += noSeriesModel.SubSectionName + seperatorSymbol;
                        }
                    });
                }

                if (profileSettings.AbbreviationRequired.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.Abbreviation))
                {
                    documentNo += profileSettings.Abbreviation + seperatorSymbol;
                }

                if (profileSettings.IsGroupAbbreviation.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.GroupAbbreviation))
                {
                    documentNo += profileSettings.GroupAbbreviation + seperatorSymbol;
                }

                if (profileSettings.IsCategoryAbbreviation.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.CategoryAbbreviation))
                {
                    documentNo += profileSettings.CategoryAbbreviation + seperatorSymbol;
                }

                if (!String.IsNullOrEmpty(profileSettings.SpecialWording))
                {
                    documentNo += profileSettings.SpecialWording + seperatorSymbol;
                }
                if (profileSettings.StartWithYear.GetValueOrDefault(false))
                {
                    documentNo += DateTime.Now.Year.ToString().Substring(2, 2) + seperatorSymbol;
                }
                generateDocumentNoSeriesModel.ProfileNo = documentNo;
                if (profileSettings.NoOfDigit.HasValue && profileSettings.NoOfDigit > 0)
                {
                    if (isCompanyDepartmentExist)
                    {
                        if (profileAutoNumbers != null && profileAutoNumbers.Count > 0)
                        {
                            if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId == null)
                            {

                                ProfileAutoNumber profileAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == null && p.SectionId == null && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileAutoNumber, profileSettings, documentNo);

                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId == null)
                            {

                                ProfileAutoNumber profileAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == null && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileAutoNumber, profileSettings, documentNo);

                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId == null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == null && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId != null)
                            {

                                ProfileAutoNumber profileAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == null && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileAutoNumber, profileSettings, documentNo);

                            }
                            else if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId == null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == noSeriesModel.PlantID && p.DepartmentId == null && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId == null)
                            {

                                ProfileAutoNumber profileAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == null && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileAutoNumber, profileSettings, documentNo);

                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId == null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == noSeriesModel.DepartmentId && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId != null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == null && p.SubSectionId == noSeriesModel.SubSectionId);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId == null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == noSeriesModel.SectionId && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }

                            else if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId == null)
                            {
                                ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == null && p.SubSectionId == null);
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);
                            }
                        }
                    }

                    else
                    {
                        if (profileAutoNumbers != null && profileAutoNumbers.Count > 0)
                        {
                            ProfileAutoNumber profileSectionAutoNumber = profileAutoNumbers.OrderByDescending(s => s.ProfileAutoNumberId).FirstOrDefault(p => p.ProfileId == profileSettings.ProfileId && p.CompanyId == null && p.DepartmentId == null && p.SectionId == null && p.SubSectionId == null);
                            if (String.IsNullOrEmpty(profileSettings.LastNoUsed))
                            {
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);

                            }
                            else
                            {
                                documentNo = await GenerateProfileAutoAsync(noSeriesModel, profileSectionAutoNumber, profileSettings, documentNo);

                            }
                        }
                    }

                    profileSettings.LastCreatedDate = DateTime.Now;
                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastCreateDate(profileSettings);
                }

                var SessionId = noSeriesModel.SessionId == null ? Guid.NewGuid() : noSeriesModel.SessionId;
                if (noSeriesModel.RequestorId == null)
                {
                    noSeriesModel.RequestorId = noSeriesModel.AddedByUserID;
                }
                var documentNoSeries = new DocumentNoSeries
                {
                    ProfileId = profileSettings.ProfileId,
                    DocumentNo = documentNo,
                    AddedDate = DateTime.Now,
                    AddedByUserId = noSeriesModel.AddedByUserID,
                    StatusCodeId = noSeriesModel.StatusCodeID.Value,
                    SessionId = SessionId,
                    RequestorId = noSeriesModel.RequestorId,
                    Title = noSeriesModel.Title,
                    ModifiedDate = DateTime.Now,
                    ModifiedByUserId = noSeriesModel.AddedByUserID,
                    FileProfileTypeId = noSeriesModel.FileProfileTypeId,
                    Description = noSeriesModel.Description,
                    IsUpload = noSeriesModel.IsUpload,
                    VersionNo = noSeriesModel.VersionNo,
                    EffectiveDate = noSeriesModel.EffectiveDate,
                    NextReviewDate = noSeriesModel.NextReviewDate,
                    Date = noSeriesModel.Date,
                    Link = noSeriesModel.Link,
                    ReasonToVoid = noSeriesModel.ReasonToVoid,
                };
                await _generateDocumentNoSeriesQueryRepository.InsertDocumentNoSeries(documentNoSeries);
                return documentNo;
            }
        }
        private async Task<string> GenerateProfileAutoAsync(DocumentNoSeriesModel noSeriesModel, ProfileAutoNumber profileAutonumber, DocumentProfileNoSeries profilesettings, string documentNo)
        {

            string LastNoUsed = "";
            if (profileAutonumber == null)
            {
                LastNoUsed = (Convert.ToInt32(profilesettings.StartingNo) + profilesettings.IncrementalNo.GetValueOrDefault(0)).ToString("D" + profilesettings.NoOfDigit);
                if (noSeriesModel.PlantID == 0)
                {
                    noSeriesModel.PlantID = null;
                }
                ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                {
                    ProfileId = profilesettings.ProfileId,
                    CompanyId = noSeriesModel.PlantID,
                    DepartmentId = noSeriesModel.DepartmentId,
                    SectionId = noSeriesModel.SectionId,
                    SubSectionId = noSeriesModel.SubSectionId,
                    LastNoUsed = LastNoUsed,
                    ProfileYear = profilesettings.StartWithYear.GetValueOrDefault(false) ? DateTime.Now.Year : 0,
                    ScreenId = noSeriesModel.ScreenID,
                    ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,

                };
                documentNo += LastNoUsed;
                await _generateDocumentNoSeriesQueryRepository.InsertProfileAutoNumber(newProfileAutoNumber);

            }
            else if (profilesettings.StartWithYear.GetValueOrDefault(false) && profileAutonumber.ProfileYear.GetValueOrDefault(0) > 0 && profileAutonumber.ProfileYear.Value != DateTime.Now.Year)
            {
                var profileAutoNumbers = await _generateDocumentNoSeriesQueryRepository.GetProfileAutoNumber(profilesettings.ProfileId, noSeriesModel);
                var yearchecking = profileAutoNumbers != null && profileAutoNumbers.Count > 0 ? profileAutoNumbers.Where(p => p.ProfileId == profilesettings.ProfileId && p.ProfileYear > 0 && p.ProfileYear == DateTime.Now.Year).FirstOrDefault() : null;
                if (yearchecking == null)
                {
                    LastNoUsed = (Convert.ToInt32(profilesettings.StartingNo) + profilesettings.IncrementalNo.GetValueOrDefault(0)).ToString("D" + profilesettings.NoOfDigit);

                    ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                    {
                        ProfileId = profilesettings.ProfileId,
                        CompanyId = noSeriesModel.PlantID,
                        DepartmentId = noSeriesModel.DepartmentId,
                        SectionId = noSeriesModel.SectionId,
                        SubSectionId = noSeriesModel.SubSectionId,
                        LastNoUsed = LastNoUsed,
                        ScreenId = noSeriesModel.ScreenID,
                        ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,
                        ProfileYear = profilesettings.StartWithYear.GetValueOrDefault(false) ? DateTime.Now.Year : 0
                    };
                    documentNo += LastNoUsed;
                    await _generateDocumentNoSeriesQueryRepository.InsertProfileAutoNumber(newProfileAutoNumber);
                }
                else if (yearchecking != null)
                {
                    LastNoUsed = (Convert.ToInt32(yearchecking.LastNoUsed) + profilesettings.IncrementalNo.GetValueOrDefault(0)).ToString("D" + profilesettings.NoOfDigit);
                    var profileAutoNumberData = await _generateDocumentNoSeriesQueryRepository.GetProfileAutoNumber(profilesettings.ProfileId, noSeriesModel);


                    documentNo += LastNoUsed;
                    if (noSeriesModel != null)
                    {
                        if (profileAutoNumberData != null && profileAutoNumberData.Count > 0)
                        {
                            if (noSeriesModel.PlantID != null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                var existingprofileAutonumber = profileAutoNumberData.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == noSeriesModel.DepartmentId && t.CompanyId == noSeriesModel.PlantID && t.SectionId == noSeriesModel.SectionId && t.SubSectionId == noSeriesModel.SubSectionId && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                                if (existingprofileAutonumber != null)
                                {
                                    existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                                }
                            }
                            if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId != null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                var existingprofileAutonumber = profileAutoNumberData.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == noSeriesModel.DepartmentId && t.CompanyId == null && t.SectionId == noSeriesModel.SectionId && t.SubSectionId == noSeriesModel.SubSectionId && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                                if (existingprofileAutonumber != null)
                                {
                                    existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                                }
                            }
                            if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId != null && noSeriesModel.SubSectionId != null)
                            {
                                var existingprofileAutonumber = profileAutoNumberData.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == null && t.CompanyId == null && t.SectionId == noSeriesModel.SectionId && t.SubSectionId == noSeriesModel.SubSectionId && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                                if (existingprofileAutonumber != null)
                                {
                                    existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                                }
                            }
                            if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId != null)
                            {
                                var existingprofileAutonumber = profileAutoNumberData.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == null && t.CompanyId == null && t.SectionId == null && t.SubSectionId == noSeriesModel.SubSectionId && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                                if (existingprofileAutonumber != null)
                                {
                                    existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                                }
                            }
                            if (noSeriesModel.PlantID == null && noSeriesModel.DepartmentId == null && noSeriesModel.SectionId == null && noSeriesModel.SubSectionId == null)
                            {
                                var existingprofileAutonumber = profileAutoNumberData.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == null && t.CompanyId == null && t.SectionId == null && t.SubSectionId == null && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                                if (existingprofileAutonumber != null)
                                {
                                    existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                    await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                                }
                            }
                        }
                    }
                    else if (noSeriesModel == null)
                    {
                        if (profileAutoNumbers != null && profileAutoNumbers.Count > 0)
                        {
                            var existingprofileAutonumber = profileAutoNumbers.Where(t => t.ProfileId == profilesettings.ProfileId && t.DepartmentId == null && t.CompanyId == null && t.SectionId == null && t.SubSectionId == null && t.ProfileYear == DateTime.Now.Year).FirstOrDefault();
                            if (existingprofileAutonumber != null)
                            {
                                existingprofileAutonumber.LastNoUsed = LastNoUsed;
                                await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(existingprofileAutonumber);
                            }
                        }
                    }
                }
                else
                {
                    LastNoUsed = (Convert.ToInt32(profilesettings.StartingNo) + profilesettings.IncrementalNo.GetValueOrDefault(0)).ToString("D" + profilesettings.NoOfDigit);

                    ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                    {
                        ProfileId = profilesettings.ProfileId,
                        CompanyId = noSeriesModel.PlantID,
                        DepartmentId = noSeriesModel.DepartmentId,
                        SectionId = noSeriesModel.SectionId,
                        SubSectionId = noSeriesModel.SubSectionId,
                        LastNoUsed = LastNoUsed,
                        ScreenId = noSeriesModel.ScreenID,
                        ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,
                        ProfileYear = profilesettings.StartWithYear.GetValueOrDefault(false) ? DateTime.Now.Year : 0
                    };
                    documentNo += LastNoUsed;
                    await _generateDocumentNoSeriesQueryRepository.InsertProfileAutoNumber(newProfileAutoNumber);
                }
            }
            else
            {
                int lastNoUsed;
                int lnused = 0;
                if (int.TryParse(profileAutonumber.LastNoUsed, out lastNoUsed))
                {
                    lnused = lastNoUsed > 0 ? lastNoUsed : 0;
                }


                LastNoUsed = (Convert.ToInt32(lnused) + profilesettings.IncrementalNo.GetValueOrDefault(0)).ToString("D" + profilesettings.NoOfDigit);
                profileAutonumber.LastNoUsed = LastNoUsed;
                documentNo += LastNoUsed;
                await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(profileAutonumber);
            }
            return documentNo;
        }

        public async Task<GenerateDocumentNoSeriesModel> GenerateDocumentProfileAutoNumberAllAsync(DocumentNoSeriesModel noSeriesModel)
        {
            var documentNo = await GenerateDocumentNosAsync(new DocumentNoSeriesModel
            {
                ProfileID = noSeriesModel.ProfileID > 0 ? noSeriesModel.ProfileID : null,
                AddedByUserID = noSeriesModel.AddedByUserID,
                StatusCodeID = noSeriesModel.StatusCodeID,
                DepartmentName = noSeriesModel.DepartmentName,
                CompanyCode = noSeriesModel.CompanyCode,
                SectionName = noSeriesModel.SectionName,
                SubSectionName = noSeriesModel.SubSectionName,
                DepartmentId = noSeriesModel.DepartmentId > 0 ? noSeriesModel.DepartmentId : null,
                PlantID = noSeriesModel.PlantID > 0 ? noSeriesModel.PlantID : null,
                SectionId = noSeriesModel.SectionId > 0 ? noSeriesModel.SectionId : null,
                SubSectionId = noSeriesModel.SubSectionId > 0 ? noSeriesModel.SubSectionId : null,
                ScreenID = noSeriesModel.ScreenID,
                ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,
                CompanyId = noSeriesModel.PlantID > 0 ? noSeriesModel.PlantID : null,
                SessionId = noSeriesModel.SessionId,
                FileProfileTypeId = noSeriesModel.FileProfileTypeId,
                RequestorId = noSeriesModel.RequestorId,
                Title = noSeriesModel.Title,
                Description = noSeriesModel.Description,
                IsUpload = noSeriesModel.IsUpload,
                ModifiedByUserID = noSeriesModel.ModifiedByUserID,
                ModifiedDate = noSeriesModel.ModifiedDate,
                VersionNo = noSeriesModel.VersionNo,
                EffectiveDate = noSeriesModel.EffectiveDate,
                NextReviewDate = noSeriesModel.NextReviewDate,
                Date = noSeriesModel.Date,
                Link = noSeriesModel.Link,
                ReasonToVoid = noSeriesModel.ReasonToVoid,
                NoOfCounts = noSeriesModel.NoOfCounts
            });

            return documentNo;
        }
        public async Task<GenerateDocumentNoSeriesModel> GenerateDocumentNosAsync(DocumentNoSeriesModel noSeriesModel)
        {
            bool isCompanyDepartmentExist = false;
            string documentNo = string.Empty;
            string deptProfileCode = "";
            string sectionProfileCode = "";
            string subSectionProfileCode = "";
            var masterList = await _generateDocumentNoSeriesQueryRepository.GetMasterLists(noSeriesModel);
            var profileSettings = masterList.DocumentProfileNoSeries.FirstOrDefault(s => s.ProfileId == noSeriesModel.ProfileID);
            GenerateDocumentNoSeriesModel generateDocumentNoSeriesModel = new GenerateDocumentNoSeriesModel();
            if (profileSettings != null)
            {
                generateDocumentNoSeriesModel.DocumentProfileNoSeries = profileSettings != null ? profileSettings : new DocumentProfileNoSeries();
                if (masterList.ProfileAutoNumber != null && masterList.ProfileAutoNumber.Count > 0)
                {
                    var profileAutoNo = masterList.ProfileAutoNumber.FirstOrDefault();
                    if (profileAutoNo!=null && !string.IsNullOrEmpty(profileAutoNo.LastNoUsed))
                    {
                        var incrementalNo = (profileSettings.IncrementalNo > 0 ? profileSettings.IncrementalNo : 0) * Convert.ToInt32(noSeriesModel.NoOfCounts);
                        var LastNoUsed = (Convert.ToInt32(profileAutoNo.LastNoUsed) + Convert.ToInt32(incrementalNo));
                        generateDocumentNoSeriesModel.ProfileAutoNumber = profileAutoNo != null ? profileAutoNo : new ProfileAutoNumber();
                        ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                        {
                            ProfileAutoNumberId = profileAutoNo.ProfileAutoNumberId,
                            LastNoUsed = LastNoUsed.ToString("D" + profileSettings.NoOfDigit),
                        };
                        await _generateDocumentNoSeriesQueryRepository.UpdateDocumentProfileNoSeriesLastNoUsed(newProfileAutoNumber);
                    }
                }
                else
                {
                    var lastNo = noSeriesModel.NoOfCounts * (profileSettings.IncrementalNo > 0 ? profileSettings.IncrementalNo : 0);
                    ProfileAutoNumber newProfileAutoNumber = new ProfileAutoNumber
                    {
                        ProfileId = noSeriesModel.ProfileID,
                        CompanyId = noSeriesModel.PlantID,
                        DepartmentId = noSeriesModel.DepartmentId,
                        SectionId = noSeriesModel.SectionId,
                        SubSectionId = noSeriesModel.SubSectionId,
                        LastNoUsed = lastNo > 0 ? lastNo.ToString() : "",
                        ProfileYear = profileSettings.StartWithYear.GetValueOrDefault(false) ? DateTime.Now.Year : 0,
                        ScreenId = noSeriesModel.ScreenID,
                        ScreenAutoNumberId = noSeriesModel.ScreenAutoNumberId,

                    };
                    await _generateDocumentNoSeriesQueryRepository.InsertProfileAutoNumber(newProfileAutoNumber);
                }

            }
            if (noSeriesModel.CompanyId > 0 && masterList.Plants != null && masterList.Plants.Count > 0)
            {
                noSeriesModel.CompanyCode = masterList.Plants.FirstOrDefault(p => p.PlantID == noSeriesModel.CompanyId)?.PlantCode;
            }
            if (noSeriesModel.DepartmentId > 0 && masterList.Departments != null && masterList.Departments.Count > 0)
            {
                deptProfileCode = masterList.Departments.FirstOrDefault(s => s.DepartmentId == noSeriesModel.DepartmentId)?.ProfileCode;
            }
            if (noSeriesModel.SectionId > 0 && masterList.Sections != null && masterList.Sections.Count > 0)
            {
                noSeriesModel.SectionName = masterList.Sections.FirstOrDefault(s => s.SectionId == noSeriesModel.SectionId)?.ProfileCode;
                if (noSeriesModel.SectionName == null && masterList.Sections != null && masterList.Sections.Count > 0)
                {
                    noSeriesModel.SectionName = masterList.Sections.FirstOrDefault(s => s.SectionId == noSeriesModel.SectionId)?.Code;
                }
            }
            if (noSeriesModel.SubSectionId > 0 && masterList.SubSections != null && masterList.SubSections.Count > 0)
            {
                noSeriesModel.SubSectionName = masterList.SubSections.FirstOrDefault(s => s.SubSectionId == noSeriesModel.SubSectionId)?.ProfileCode;
                if (noSeriesModel.SubSectionName == null)
                {
                    noSeriesModel.SubSectionName = masterList.SubSections.FirstOrDefault(s => s.SubSectionId == noSeriesModel.SubSectionId)?.Code;
                }

            }
            if (noSeriesModel.ProfileID == null || noSeriesModel.ProfileID <= 0)
            {
                return null;
            }
            else
            {

                var profileAutoNumbers = await _generateDocumentNoSeriesQueryRepository.GetProfileAutoNumber(profileSettings.ProfileId, noSeriesModel);

                if (profileSettings != null && profileSettings.CompanyId > 0 && masterList.Plants != null && masterList.Plants.Count > 0 && (noSeriesModel.CompanyId == null || noSeriesModel.PlantID == null))
                {
                    noSeriesModel.CompanyCode = masterList.Plants.Where(s => s.PlantID == profileSettings.CompanyId).FirstOrDefault().PlantCode;
                }
                if (profileSettings != null && profileSettings.DeparmentId > 0 && noSeriesModel.DepartmentId == null && masterList.Departments != null && masterList.Departments.Count > 0)
                {
                    var department = masterList.Departments.Where(s => s.DepartmentId == profileSettings.DeparmentId)?.FirstOrDefault();
                    if (department != null)
                    {
                        if (department.ProfileCode != null)
                        {
                            noSeriesModel.DepartmentName = department.ProfileCode;
                        }
                        else
                        {
                            noSeriesModel.DepartmentName = department.Code;
                        }
                    }
                }

                List<string> numberSeriesCodes = new List<string> { "Company", "Department" };
                List<NumberSeriesCodeModel> numberSeriesCodeModels = new List<NumberSeriesCodeModel>();

                List<Seperator> seperators = new List<Seperator>();
                seperators.Add(new Seperator { SeperatorSymbol = "/", SeperatorValue = 1 });
                seperators.Add(new Seperator { SeperatorSymbol = "-", SeperatorValue = 2 });

                var seperator = seperators.FirstOrDefault(s => s.SeperatorValue == profileSettings.SeperatorToUse.GetValueOrDefault(0));
                var seperatorSymbol = seperator != null ? seperator.SeperatorSymbol : "/";

                if (!String.IsNullOrEmpty(profileSettings.Abbreviation1))
                {
                    numberSeriesCodeModels = JsonConvert.DeserializeObject<List<NumberSeriesCodeModel>>(profileSettings.Abbreviation1).ToList();
                    isCompanyDepartmentExist = numberSeriesCodeModels.Any(c => numberSeriesCodes.Contains(c.Name));
                    numberSeriesCodeModels.OrderBy(n => n.Index).ToList().ForEach(n =>
                    {
                        if (n.Name == "Company" && !string.IsNullOrEmpty(noSeriesModel.CompanyCode))
                        {
                            documentNo += noSeriesModel.CompanyCode + seperatorSymbol;
                        }
                        if (n.Name == "Department" && !string.IsNullOrEmpty(noSeriesModel.DepartmentName))
                        {
                            noSeriesModel.DepartmentName = string.IsNullOrEmpty(deptProfileCode) ? noSeriesModel.DepartmentName : deptProfileCode;
                            string[] departmentDetails = noSeriesModel.DepartmentName.Split(" ");
                            if (departmentDetails.Length > 1 && !string.IsNullOrEmpty(departmentDetails[1]))
                            {
                                documentNo += departmentDetails[1] + seperatorSymbol;
                            }
                            else if (!string.IsNullOrEmpty(noSeriesModel.DepartmentName))
                            {
                                documentNo += noSeriesModel.DepartmentName + seperatorSymbol;
                            }
                        }
                        if (n.Name == "Section" && !string.IsNullOrEmpty(noSeriesModel.SectionName))
                        {
                            noSeriesModel.SectionName = string.IsNullOrEmpty(sectionProfileCode) ? noSeriesModel.SectionName : sectionProfileCode;
                            documentNo += noSeriesModel.SectionName + seperatorSymbol;
                        }
                        if (n.Name == "SubSection" && !string.IsNullOrEmpty(noSeriesModel.SubSectionName))
                        {
                            noSeriesModel.SubSectionName = string.IsNullOrEmpty(subSectionProfileCode) ? noSeriesModel.SubSectionName : subSectionProfileCode;
                            documentNo += noSeriesModel.SubSectionName + seperatorSymbol;
                        }
                    });
                }

                if (profileSettings.AbbreviationRequired.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.Abbreviation))
                {
                    documentNo += profileSettings.Abbreviation + seperatorSymbol;
                }

                if (profileSettings.IsGroupAbbreviation.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.GroupAbbreviation))
                {
                    documentNo += profileSettings.GroupAbbreviation + seperatorSymbol;
                }

                if (profileSettings.IsCategoryAbbreviation.GetValueOrDefault(false) && !String.IsNullOrEmpty(profileSettings.CategoryAbbreviation))
                {
                    documentNo += profileSettings.CategoryAbbreviation + seperatorSymbol;
                }

                if (!String.IsNullOrEmpty(profileSettings.SpecialWording))
                {
                    documentNo += profileSettings.SpecialWording + seperatorSymbol;
                }
                if (profileSettings.StartWithYear.GetValueOrDefault(false))
                {
                    documentNo += DateTime.Now.Year.ToString().Substring(2, 2) + seperatorSymbol;
                }
                generateDocumentNoSeriesModel.ProfileNo = documentNo;

                return generateDocumentNoSeriesModel;
            }
        }
    }
}
