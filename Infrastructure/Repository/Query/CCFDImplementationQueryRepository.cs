using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class CCFDImplementationQueryRepository : QueryRepository<View_GetCCFImplementation>, ICCDFImplementationQueryRepository
    {
        public CCFDImplementationQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<View_GetCCFImplementation>> GetAllAsync(Guid? sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId);
                var query = "SELECT * FROM View_GetCCFImplementation WHERE (SessionID = @SessionID) OR (SessionID IS NULL)";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_GetCCFImplementation>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<CCFInformationModels> GetAllBySessionAsync(Guid? SessionId)
        {
            try
            {
                var query = @"select * from ChangeControlForm  CCF 
                    Inner join CCFAInformation CCFA on CCFA.SessionID =CCF.SessionID
                    Inner Join CCFBEvaluation CCFB on CCFB.SessionID = CCF.SessionID
                    inner join CCFCAPproval CCFC on CCFC.SessionID = CCF.SessionID
                    Inner Join CCFDImplementation CCFD on CCFD.SessionID = CCF.SessionID
                    Inner Join CCFDImplementationDetails CCFCD on CCFCD.SessionID = CCF.SessionID
                    Inner Join CCFEClosure CCFE on CCFE.SessionID = CCF.SessionID
                    where CCF.SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<CCFInformationModels>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> InsertDetail(CCFDImplementationDetails cCFDImplementation)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    { var parameters = new DynamicParameters();
                            parameters.Add("ClassOFDocumentID", cCFDImplementation.ClassOFDocumentID);

                        try
                        {
                           
                            parameters.Add("IsRequired", cCFDImplementation.IsRequired);
                            parameters.Add("DoneBy", cCFDImplementation.DoneBy);
                            parameters.Add("DoneByDate", cCFDImplementation.DoneByDate);
                            parameters.Add("ResponsibiltyTo", cCFDImplementation.ResponsibiltyTo);
                            parameters.Add("DoneByDate", cCFDImplementation.DoneByDate);
                            parameters.Add("AddedDate", cCFDImplementation.AddedDate);
                            parameters.Add("SessionId", cCFDImplementation.SessionId);
                            parameters.Add("AddedByUserID", cCFDImplementation.AddedByUserID);
                            parameters.Add("StatusCodeID", cCFDImplementation.StatusCodeID);

                            var query = " Insert into  CCFDImplementationDetails (ClassOFDocumentID,IsRequired,DoneBy,DoneByDate,ResponsibiltyTo,AddedDate,SessionId,AddedByUserID,StatusCodeID)Values(@ClassOFDocumentID,@IsRequired,@DoneBy,@DoneByDate,@ResponsibiltyTo,@AddedDate,@SessionId,@AddedByUserID,@StatusCodeID)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public long Insert(CCFInformationModels _CCFInformationModels)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameterss = new DynamicParameters();
                        parameterss.Add("CCFAInformationID", _CCFInformationModels.CCFAInformationID);
                        parameterss.Add("CCFBEvaluationID", _CCFInformationModels.CCFBEvaluationID);
                        parameterss.Add("CCFCAPprovalID", _CCFInformationModels.CCFCAPprovalID);
                        parameterss.Add("CCFDImplementationID", _CCFInformationModels.CCFDImplementationID);
                        parameterss.Add("CCFEClosureID", _CCFInformationModels.CCFEClosureID);

                        parameterss.Add("IsInternalChanges", _CCFInformationModels.IsInternalChanges);
                        parameterss.Add("IsAuthorityDirectedChanges", _CCFInformationModels.IsAuthorityDirectedChanges);
                        parameterss.Add("PIC", _CCFInformationModels.PIC);
                        parameterss.Add("PIA", _CCFInformationModels.PIA);
                        parameterss.Add("DepartmentID", _CCFInformationModels.DepartmentID);
                        parameterss.Add("IsSiteTransfer", _CCFInformationModels.IsSiteTransfer);
                       parameterss.Add("EvaluationDate", _CCFInformationModels.EvaluationDate);
                  
                        parameterss.Add("IsProduct", _CCFInformationModels.IsProduct);
                        parameterss.Add("IsEquipment", _CCFInformationModels.IsEquipment);
                        parameterss.Add("IsComposition", _CCFInformationModels.IsComposition);
                        parameterss.Add("IsFacility", _CCFInformationModels.IsFacility);
                        parameterss.Add("IsLayout", _CCFInformationModels.IsLayout);
                        parameterss.Add("IsDocument", _CCFInformationModels.IsDocument);
                        parameterss.Add("IsProcess", _CCFInformationModels.IsProcess);


                        parameterss.Add("IsControlParameter", _CCFInformationModels.IsControlParameter);
                        parameterss.Add("IsBatchSize", _CCFInformationModels.IsBatchSize);
                        parameterss.Add("IsHoldingTime", _CCFInformationModels.IsHoldingTime);
                        parameterss.Add("IsRawMeterial", _CCFInformationModels.IsRawMeterial);
                        parameterss.Add("IsArtwork", _CCFInformationModels.IsArtwork);


                        parameterss.Add("IsPackagingMaterial", _CCFInformationModels.IsPackagingMaterial);
                        parameterss.Add("IsVendor", _CCFInformationModels.IsVendor);
                        parameterss.Add("IsShelfLife", _CCFInformationModels.IsShelfLife);

                        parameterss.Add("IsRegulatory", _CCFInformationModels.IsRegulatory);
                        parameterss.Add("IsReTestPeriod", _CCFInformationModels.IsReTestPeriod);


                        parameterss.Add("Others", _CCFInformationModels.Others);
                        parameterss.Add("Justification", _CCFInformationModels.Justification);
                        parameterss.Add("DescriptionOfProposedChange", _CCFInformationModels.DescriptionOfProposedChange);
                        parameterss.Add("ProposedImplementationAction", _CCFInformationModels.ProposedImplementationAction);
                        parameterss.Add("RelatedDeviation", _CCFInformationModels.RelatedDeviation);
                        parameterss.Add("IsAcceptable", _CCFInformationModels.IsAcceptable);
                       parameterss.Add("InitiatorDate", _CCFInformationModels.InitiatorDate);
                       
                        parameterss.Add("IsNotAcceptable", _CCFInformationModels.IsNotAcceptable);
                        parameterss.Add("IsMinor", _CCFInformationModels.IsMinor);
                        parameterss.Add("IsMajor", _CCFInformationModels.IsMajor);
                        parameterss.Add("IsCritical", _CCFInformationModels.IsCritical);
                        parameterss.Add("Comments", _CCFInformationModels.Comments);
                        parameterss.Add("IsProduction", _CCFInformationModels.IsProduction);
                        parameterss.Add("IsEAndM", _CCFInformationModels.IsEAndM);


                        parameterss.Add("IsRequlatory", _CCFInformationModels.IsRequlatory);
                        parameterss.Add("IsQA", _CCFInformationModels.IsQA);
                        parameterss.Add("IsQC", _CCFInformationModels.IsQC);
                        parameterss.Add("IsStore", _CCFInformationModels.IsStore);
                        parameterss.Add("RelatedDeparmentOthers", _CCFInformationModels.RelatedDeparmentOthers);


                        parameterss.Add("EvaluatedBy", _CCFInformationModels.EvaluatedBy);
                        parameterss.Add("IsRequlatoryApproval", _CCFInformationModels.IsRequlatoryApproval);
                        parameterss.Add("IsNotificationRequired", _CCFInformationModels.IsNotificationRequired);

                        parameterss.Add("RegulatoryOthers", _CCFInformationModels.RegulatoryOthers);
                        parameterss.Add("RegulatoryDetails", _CCFInformationModels.RegulatoryDetails);

                        parameterss.Add("RequlatoryEvaluatedBy", _CCFInformationModels.RequlatoryEvaluatedBy);
                        parameterss.Add("RegulatoryDate", _CCFInformationModels.RegulatoryDate);
                        parameterss.Add("IsAnalyticalInstrument", _CCFInformationModels.IsAnalyticalInstrument);
                        parameterss.Add("IsValidation", _CCFInformationModels.IsValidation);
                        parameterss.Add("IsEnvironmentalMonitoring", _CCFInformationModels.IsEnvironmentalMonitoring);


                        parameterss.Add("IsRawMeterialSpec", _CCFInformationModels.IsRawMeterialSpec);
                        parameterss.Add("IsFinishedProductSpec", _CCFInformationModels.IsFinishedProductSpec);
                        parameterss.Add("IsPackagingMaterialSpec", _CCFInformationModels.IsPackagingMaterialSpec);

                        parameterss.Add("IsCalibration", _CCFInformationModels.IsCalibration);
                        parameterss.Add("IsAnalyticalTestMethod", _CCFInformationModels.IsAnalyticalTestMethod);

                        parameterss.Add("IsSamplingMethod", _CCFInformationModels.IsSamplingMethod);
                        parameterss.Add("EvaluationIsVendor", _CCFInformationModels.EvaluationIsVendor);
                        parameterss.Add("IsStabilityStudy", _CCFInformationModels.IsStabilityStudy);
                        parameterss.Add("IsInProcess", _CCFInformationModels.IsInProcess);
                        parameterss.Add("QualityControlOthers", _CCFInformationModels.QualityControlOthers);


                        parameterss.Add("QualityControlDetails", _CCFInformationModels.QualityControlDetails);
                        parameterss.Add("QualityControlEvaluatedBy", _CCFInformationModels.QualityControlEvaluatedBy);
                        parameterss.Add("QualityControlDate", _CCFInformationModels.QualityControlDate);

                        parameterss.Add("IsProductionProcess", _CCFInformationModels.IsProductionProcess);
                        parameterss.Add("IsProductionValidation", _CCFInformationModels.IsProductionValidation);



                        parameterss.Add("EvaluationIsControlParameter", _CCFInformationModels.EvaluationIsControlParameter);
                        parameterss.Add("ProductionOthers", _CCFInformationModels.ProductionOthers);
                        parameterss.Add("ProductionEvaluatedBy", _CCFInformationModels.ProductionEvaluatedBy);
                        parameterss.Add("ProductionDate", _CCFInformationModels.ProductionDate);
                        parameterss.Add("IsPiping", _CCFInformationModels.IsPiping);
                        parameterss.Add("EvaluationIsEquipment", _CCFInformationModels.EvaluationIsEquipment);


                        parameterss.Add("IsEngineeringCalibration", _CCFInformationModels.IsEngineeringCalibration);
                        parameterss.Add("IsPreventiveMaintenance", _CCFInformationModels.IsPreventiveMaintenance);
                        parameterss.Add("IsUtilityParameter", _CCFInformationModels.IsUtilityParameter);
                        parameterss.Add("IsEngineeringFacility", _CCFInformationModels.IsEngineeringFacility);
                        parameterss.Add("IsQualificationOfEquipment", _CCFInformationModels.IsQualificationOfEquipment);


                        parameterss.Add("IsQualificationOfUtility", _CCFInformationModels.IsQualificationOfUtility);
                        parameterss.Add("EngineeringOthers", _CCFInformationModels.EngineeringOthers);
                        parameterss.Add("EngineeringDetails", _CCFInformationModels.EngineeringDetails);

                        parameterss.Add("EngineeringEvaluatedBy", _CCFInformationModels.EngineeringEvaluatedBy);
                        parameterss.Add("EngineeringMaintenanceDate", _CCFInformationModels.EngineeringMaintenanceDate);

                        parameterss.Add("IsQAQualification", _CCFInformationModels.IsQAQualification);
                        parameterss.Add("IsQAValidation", _CCFInformationModels.IsQAValidation);
                        parameterss.Add("IsQAVendor", _CCFInformationModels.IsQAVendor);
                        parameterss.Add("QAOthers", _CCFInformationModels.QAOthers);
                        parameterss.Add("QADetails", _CCFInformationModels.QADetails);


                        parameterss.Add("QAEvaluatedBy", _CCFInformationModels.QAEvaluatedBy);
                        parameterss.Add("QADate", _CCFInformationModels.QADate);
                        parameterss.Add("IsStoreRawMaterial", _CCFInformationModels.IsStoreRawMaterial);

                        parameterss.Add("IsStorePackagingMaterial", _CCFInformationModels.IsStorePackagingMaterial);
                        parameterss.Add("IsStoreLabel", _CCFInformationModels.IsStoreLabel);

                        parameterss.Add("IsStorageCondition", _CCFInformationModels.IsStorageCondition);
                        parameterss.Add("IsStoreFinishProduct", _CCFInformationModels.IsStoreFinishProduct);
                        parameterss.Add("StoreOthers", _CCFInformationModels.StoreOthers);
                        parameterss.Add("StoreDetails", _CCFInformationModels.StoreDetails);
                        parameterss.Add("StoreEvaluatedBy", _CCFInformationModels.StoreEvaluatedBy);


                        parameterss.Add("StoreDate", _CCFInformationModels.StoreDate);
                        parameterss.Add("ProposedChangeImpactTo", _CCFInformationModels.ProposedChangeImpactTo);
                        parameterss.Add("OthersDetails", _CCFInformationModels.OthersDetails);

                        parameterss.Add("OthersEvaluatedBy", _CCFInformationModels.OthersEvaluatedBy);
                        parameterss.Add("OthersDate", _CCFInformationModels.OthersDate);



                        parameterss.Add("IsApproved", _CCFInformationModels.IsApproved);
                        parameterss.Add("IsNotApproved", _CCFInformationModels.IsNotApproved);
                        parameterss.Add("ApprovalComments", _CCFInformationModels.ApprovalComments);

                        parameterss.Add("VerifiedBy", _CCFInformationModels.VerifiedBy);
                        parameterss.Add("ApprovalDate", _CCFInformationModels.ApprovalDate);

                        parameterss.Add("HODComments", _CCFInformationModels.HODComments);
                        parameterss.Add("HODSignature", _CCFInformationModels.HODSignature);
                        parameterss.Add("HODDate", _CCFInformationModels.HODDate);
                        parameterss.Add("ImplementationIsAcceptable", _CCFInformationModels.ImplementationIsAcceptable);
                        parameterss.Add("ImplementationIsNotAcceptable", _CCFInformationModels.ImplementationIsNotAcceptable);


                        parameterss.Add("ImplementationVerifiedBy", _CCFInformationModels.ImplementationVerifiedBy);
                        parameterss.Add("VerifiedDate", _CCFInformationModels.VerifiedDate);
                        parameterss.Add("IsSatisfactory", _CCFInformationModels.IsSatisfactory);

                        parameterss.Add("IsNotSatisfactory", _CCFInformationModels.IsNotSatisfactory);
                        parameterss.Add("ClosureComments", _CCFInformationModels.ClosureComments);

                        parameterss.Add("ClosedBy", _CCFInformationModels.ClosedBy);
                        parameterss.Add("Date", _CCFInformationModels.Date);
                        parameterss.Add("IsSatisfactoryForNotSatisfactory", _CCFInformationModels.IsSatisfactoryForNotSatisfactory);
                        parameterss.Add("NotSatisfactoryClosedBy", _CCFInformationModels.NotSatisfactoryClosedBy);
                        parameterss.Add("NotSatisfactoryDate", _CCFInformationModels.NotSatisfactoryDate);


                        parameterss.Add("StatusCodeID", _CCFInformationModels.StatusCodeID);
                        parameterss.Add("AddedByUserID", _CCFInformationModels.AddedByUserID);
                        parameterss.Add("AddedDate", _CCFInformationModels.AddedDate);

                        parameterss.Add("SessionId", _CCFInformationModels.SessionId);
                       
                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_CCF_SavePro", parameterss, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<long> UpdateDetail(CCFDImplementationDetails cCFDImplementation)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ClassOFDocumentID", cCFDImplementation.ClassOFDocumentID);
                            parameters.Add("IsRequired", cCFDImplementation.IsRequired);
                            parameters.Add("DoneBy", cCFDImplementation.DoneBy);
                            parameters.Add("DoneByDate", cCFDImplementation.DoneByDate);
                            parameters.Add("ResponsibiltyTo", cCFDImplementation.ResponsibiltyTo);
                          
                            parameters.Add("ModifiedDate", cCFDImplementation.ModifiedDate);
                            parameters.Add("SessionId", cCFDImplementation.SessionId);
                            parameters.Add("ModifiedByUserID", cCFDImplementation.ModifiedByUserID);
                            parameters.Add("StatusCodeID", cCFDImplementation.StatusCodeID);
                            var query = " UPDATE CCFDImplementationDetails SET IsRequired = @IsRequired,DoneBy =@DoneBy,DoneByDate =@DoneByDate,ResponsibiltyTo =@ResponsibiltyTo,ModifiedDate =@ModifiedDate,SessionId =@SessionId,StatusCodeID =@StatusCodeID, ModifiedByUserID =@ModifiedByUserID WHERE ClassOFDocumentID = @ClassOFDocumentID";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
   
}
