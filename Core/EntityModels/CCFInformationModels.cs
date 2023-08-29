using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class CCFInformationModels:BaseEntity
    {
        [Key]
        public long CCFAInformationID { get; set; }
       
        public bool IsInternalChanges { get; set; }
     
        public bool IsAuthorityDirectedChanges { get; set; }
        public long? PIC { get; set; }
        public long? PIA { get; set; }
        public long? DepartmentID { get; set; }
        public bool IsSiteTransfer { get; set; }
        public bool IsProduct { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsComposition { get; set; }
        public bool IsFacility { get; set; }
        public bool IsLayout { get; set; }
        public bool IsDocument { get; set; }
        public bool IsProcess { get; set; }
        public bool IsControlParameter { get; set; }
        public bool IsBatchSize { get; set; }
        public bool IsHoldingTime { get; set; }
        public bool IsRawMeterial { get; set; }
        public bool IsArtwork { get; set; }
        public bool IsPackagingMaterial { get; set; }
        public bool IsVendor { get; set; }
        public bool IsShelfLife { get; set; }
        public bool IsRegulatory { get; set; }
        public bool IsReTestPeriod { get; set; }
        public string? Others { get; set; }

        public string? DescriptionOfProposedChange { set; get; }
        public string? Description { get; set; }
        public string? Justification { get; set; }
        public string? ProposedImplementationAction { get; set; }
        public string? RelatedDeviation { get; set; }
        [Key]
        public long CCFBEvaluationID { get; set; }
        public bool IsAcceptable { get; set; }
        public bool IsNotAcceptable { get; set; }
        public bool IsMinor { get; set; }
        public bool IsMajor { get; set; }
        public bool IsCritical { get; set; }
        public string? Comments { get; set; }
        public bool IsProduction { get; set; }
        public bool IsEAndM { get; set; }
        public bool IsRequlatory { get; set; }
        public bool IsQA { get; set; }
        public bool IsQC { get; set; }
        public bool IsStore { get; set; }
        public string? RelatedDeparmentOthers { get; set; }
        public long EvaluatedBy { get; set; }
        public bool IsRequlatoryApproval { get; set; }
        public bool IsNotificationRequired { get; set; }
        public string? RegulatoryOthers { get; set; }
        public string? RegulatoryDetails { get; set; }
        public long RequlatoryEvaluatedBy { get; set; }
        public DateTime? RegulatoryDate { get; set; }
        public bool IsAnalyticalInstrument { get; set; }
        public bool IsValidation { get; set; }
        public bool IsEnvironmentalMonitoring { get; set; }
        public bool IsRawMeterialSpec { get; set; }
        public bool IsFinishedProductSpec { get; set; }
        public bool IsPackagingMaterialSpec { get; set; }
        public bool IsCalibration { get; set; }
        public bool IsAnalyticalTestMethod { get; set; }
        public bool IsSamplingMethod { get; set; }
        public bool EvaluationIsVendor { get; set; }
        public bool IsStabilityStudy { get; set; }
        public bool IsInProcess { get; set; }
        public string? QualityControlOthers { get; set; }
        public string? QualityControlDetails { get; set; }
        public long QualityControlEvaluatedBy { get; set; }
        public DateTime? QualityControlDate { get; set; }
        public bool IsProductionProcess { get; set; }
        public bool IsProductionValidation { get; set; }
        public bool EvaluationIsControlParameter { get; set; }
        public string? ProductionOthers { get; set; }
        public string? ProductionDetails { get; set; }
        public long? ProductionEvaluatedBy { get; set; }
        public DateTime? ProductionDate { get; set; }
        public bool IsPiping { get; set; }
        public bool EvaluationIsEquipment { get; set; }
        public bool IsEngineeringCalibration { get; set; }
        public bool IsPreventiveMaintenance { get; set; }
        public bool IsUtilityParameter { get; set; }
        public bool IsEngineeringFacility { get; set; }
        public bool IsQualificationOfEquipment { get; set; }
        public bool IsQualificationOfUtility { get; set; }
        public string? EngineeringOthers { get; set; }
        public string? EngineeringDetails { get; set; }
        public long? EngineeringEvaluatedBy { get; set; }

        public DateTime? EngineeringMaintenanceDate { get; set; }
        public bool IsQAQualification { get; set; }
        public bool IsQAValidation { get; set; }
        public bool IsQAVendor { get; set; }

        public string? QAOthers { get; set; }
        public string? QADetails { get; set; }
        public long? QAEvaluatedBy { get; set; }
        public DateTime? QADate { get; set; }
        public bool IsStoreRawMaterial { get; set; }
        public bool IsStorePackagingMaterial { get; set; }
        public bool IsStoreLabel { get; set; }
        public bool IsStorageCondition { get; set; }
        public bool IsStoreFinishProduct { get; set; }
        public string? StoreOthers { get; set; }
        public string? StoreDetails { get; set; }
        public long? StoreEvaluatedBy { get; set; }
        public DateTime? StoreDate { get; set; }
        public string? ProposedChangeImpactTo { get; set; }
        public string? OthersDetails { get; set; }
        public long? OthersEvaluatedBy { get; set; }
        public DateTime? OthersDate { get; set; }

        [Key]
        public long CCFCAPprovalID { get; set; }
        public bool IsApproved { get; set; }
        public bool IsNotApproved { get; set; }

        public string? ApprovalComments { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        [Key]
        public long? CCFDImplementationID { get; set; }
        public long? ClassOfDocumentID { get; set; }
        public string? HODComments { get; set; }
        public string? HODSignature { get; set; }
        public DateTime? HODDate { get; set; }
        public bool ImplementationIsAcceptable { get; set; }
        public bool ImplementationIsNotAcceptable { get; set; }
        public long? ImplementationVerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        [Key]
        public long? CCFEClosureID { get; set; }
        public bool IsSatisfactory { get; set; }
        public bool IsNotSatisfactory { get; set; }
        public string? ClosureComments { get; set; }
        public long? ClosedBy { get; set; }
        public DateTime? Date { get; set; }
        public bool IsSatisfactoryForNotSatisfactory { get; set; }
        public long? NotSatisfactoryClosedBy { get; set; }
        public DateTime? NotSatisfactoryDate { get; set; }


        [Key]
        public long CCFDImplementationDetailsID { get; set; }
        public long DetailCCFDImplementationID { get; set; }
        public bool IsRequired { get; set; }
        public long? ResponsibiltyTo { get; set; }
        public long? DoneBy { get; set; }
        public DateTime? DoneByDate { get; set; }
    }
}
