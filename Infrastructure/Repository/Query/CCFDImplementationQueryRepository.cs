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
                parameters.Add("SessionId", sessionId);
                var query = "SELECT * FROM View_GetCCFImplementation WHERE (SessionId = @SessionId) ";

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
                var query = @"select CCFA.CCFAInformationID,CCFA.IsInternalChanges,CCFA.InitiatorDate,CCFA.IsAuthorityDirectedChanges,CCFA.PIC,CCFA.PIA,CCFA.DepartmentID,CCFA.IsSiteTransfer,CCFA.IsProduct,CCFA.IsEquipment,
                                CCFA.IsComposition,CCFA.IsFacility,CCFA.IsLayout,CCFA.IsDocument,CCFA.IsProcess,CCFA.IsControlParameter,CCFA.IsBatchSize,CCFA.IsHoldingTime,CCFA.IsRawMeterial,CCFA.IsArtwork,CCFA.IsPackagingMaterial,CCFA.IsVendor,
                                CCFA.IsShelfLife,CCFA.IsRegulatory,CCFA.IsReTestPeriod,CCFA.Others,CCFA.DescriptionOfProposedChange ,CCFA.Justification,CCFA.ProposedImplementationAction,CCFA.RelatedDeviation,
                                CCFB.CCFBEvaluationID,CCFB.EvaluationDate,CCFB.IsAcceptable,CCFB.IsNotAcceptable,CCFB.IsMinor,CCFB.IsMajor,CCFB.IsCritical,CCFB.Comments,CCFB.IsProduction,CCFB.IsEAndM,CCFB.IsRequlatory,CCFB.IsQA,CCFB.IsQC,CCFB.IsStore,CCFB.RelatedDeparmentOthers,CCFB.EvaluatedBy,CCFB.IsRequlatoryApproval,
                                CCFB.IsNotificationRequired,CCFB.RegulatoryOthers,CCFB.RegulatoryDetails,CCFB.RequlatoryEvaluatedBy,CCFB.RegulatoryDate,CCFB.IsAnalyticalInstrument,CCFB.IsValidation,CCFB.IsEnvironmentalMonitoring,CCFB.IsRawMeterialSpec,CCFB.IsFinishedProductSpec,
                                CCFB.IsPackagingMaterialSpec,CCFB.IsCalibration,CCFB.IsAnalyticalTestMethod,CCFB.IsSamplingMethod,CCFB.IsVendor as EvaluationIsVendor,CCFB.IsStabilityStudy,CCFB.IsInProcess,CCFB.QualityControlOthers,CCFB.QualityControlDetails,CCFB.QualityControlEvaluatedBy,CCFB.QualityControlDate,CCFB.IsProductionProcess,CCFB.IsProductionValidation,CCFB.IsControlParameter as EvaluationIsControlParameter,
                                CCFB.ProductionOthers,CCFB.ProductionDetails,CCFB.ProductionEvaluatedBy,CCFB.ProductionDate,CCFB.IsPiping,CCFB.IsEquipment as EvaluationIsEquipment,CCFB.IsEngineeringCalibration,CCFB.IsPreventiveMaintenance,CCFB.IsUtilityParameter,CCFB.IsEngineeringFacility,CCFB.IsQualificationOfEquipment,CCFB.IsQualificationOfUtility,CCFB.EngineeringOthers,CCFB.EngineeringDetails,CCFB.EngineeringEvaluatedBy,CCFB.EngineeringMaintenanceDate,CCFB.IsQAQualification,CCFB.IsQAValidation,CCFB.IsQAVendor,
                                CCFB.QAOthers,CCFB.QADetails,CCFB.QAEvaluatedBy,CCFB.QADate,CCFB.IsStoreRawMaterial,CCFB.IsStorePackagingMaterial,CCFB.IsStoreLabel,CCFB.IsStorageCondition,CCFB.IsStoreFinishProduct,CCFB.StoreOthers,CCFB.StoreDetails,
                                CCFB.StoreEvaluatedBy,CCFB.StoreDate,CCFB.ProposedChangeImpactTo,CCFB.OthersDetails,CCFB.OthersEvaluatedBy,CCFB.OthersDate ,CCFC.CCFCAPprovalID ,CCFC.IsApproved ,CCFC.IsNotApproved ,CCFC.Comments as ApprovalComments ,CCFC.VerifiedBy ,CCFC.ApprovalDate ,
                                CCFD.CCFDImplementationID ,CCFD.ClassOfDocumentID , CCFD.HODComments ,CCFD.HODSignature ,CCFD.HODDate ,CCFD.IsAcceptable as ImplementationIsAcceptable ,CCFD.IsNotAcceptable as ImplementationIsNotAcceptable ,CCFD.VerifiedBy as ImplementationVerifiedBy , CCFD.VerifiedDate ,
                                CCFE.CCFEClosureID,CCFE.IsSatisfactory,CCFE.IsNotSatisfactory,CCFE.Comments as ClosureComments,CCFE.ClosedBy,CCFE.Date,CCFE.IsSatisfactoryForNotSatisfactory,CCFE.NotSatisfactoryClosedBy,CCFE.NotSatisfactoryDate,
                                CCFCD.CCFDImplementationDetailsID,CCFCD.CCFDImplementationID AS DetailCCFDImplementationID,CCFCD.IsRequired,CCFCD. ResponsibiltyTo,CCFCD.DoneBy,CCFCD.DoneByDate

                                 from ChangeControlForm  CCF 
                    Inner join CCFAInformation CCFA on CCFA.SessionId =CCF.SessionId
                    Inner Join CCFBEvaluation CCFB on CCFB.SessionId = CCF.SessionId
                    inner join CCFCAPproval CCFC on CCFC.SessionId = CCF.SessionId
                    Inner Join CCFDImplementation CCFD on CCFD.SessionId = CCF.SessionId
                    Inner Join CCFDImplementationDetails CCFCD on CCFCD.SessionId = CCF.SessionId
                    Inner Join CCFEClosure CCFE on CCFE.SessionId = CCF.SessionId
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
                    { 
                          

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("IsRequired", cCFDImplementation.IsRequired);
                            parameters.Add("DoneBy", cCFDImplementation.DoneBy);
                            parameters.Add("DoneByDate", cCFDImplementation.DoneByDate);
                            parameters.Add("ResponsibiltyTo", cCFDImplementation.ResponsibiltyTo);
                            parameters.Add("DoneByDate", cCFDImplementation.DoneByDate);
                            parameters.Add("AddedDate", cCFDImplementation.AddedDate);
                            parameters.Add("SessionId", cCFDImplementation.SessionId);
                            parameters.Add("AddedByUserID", cCFDImplementation.AddedByUserID);
                            parameters.Add("StatusCodeID", cCFDImplementation.StatusCodeID);

                            // var query = " Insert into  CCFDImplementationDetails (ClassOFDocumentID,IsRequired,DoneBy,DoneByDate,ResponsibiltyTo,AddedDate,SessionId,AddedByUserID,StatusCodeID)Values(@ClassOFDocumentID,@IsRequired,@DoneBy,@DoneByDate,@ResponsibiltyTo,@AddedDate,@SessionId,@AddedByUserID,@StatusCodeID)";
                            var query = @"insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(79,@SessionId,@AddedByUserID,@StatusCodeID)
                                        insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(80,@SessionId,@AddedByUserID,@StatusCodeID)
                                        insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(81,@SessionId,@AddedByUserID,@StatusCodeID)
                                       insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(82,@SessionId,@AddedByUserID,@StatusCodeID)
                                       insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(83,@SessionId,@AddedByUserID,@StatusCodeID)
                                       insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(84,@SessionId,@AddedByUserID,@StatusCodeID)
                                       insert into CCFDImplementationDetails(ClassOFDocumentID,SessionId,AddedByUserID,StatusCodeID)Values(85,@SessionId,@AddedByUserID,@StatusCodeID)";
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
                        parameterss.Add("ProductionDetails", _CCFInformationModels.ProductionDetails);
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
                        parameterss.Add("ModifiedByUserID", _CCFInformationModels.ModifiedByUserID);
                        parameterss.Add("ModifiedDate", _CCFInformationModels.ModifiedDate);
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
                           // parameters.Add("SessionId", cCFDImplementation.SessionId);
                            parameters.Add("ModifiedByUserID", cCFDImplementation.ModifiedByUserID);
                            parameters.Add("StatusCodeID", cCFDImplementation.StatusCodeID);
                            var query = " UPDATE CCFDImplementationDetails SET IsRequired = @IsRequired,DoneBy =@DoneBy,DoneByDate =@DoneByDate,ResponsibiltyTo =@ResponsibiltyTo,ModifiedDate =@ModifiedDate,StatusCodeID =@StatusCodeID, ModifiedByUserID =@ModifiedByUserID WHERE ClassOFDocumentID = @ClassOFDocumentID";

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
