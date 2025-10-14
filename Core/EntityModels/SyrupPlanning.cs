using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class SyrupPlanning : BaseEntity
    {
        public long Id { get; set; }
        public long DynamicFormDataItemID { get; set; }
        public long MethodCodeLineID { get; set; }
        public long DynamicFormDataID { get; set; }
        public string? MethodName { get; set; }
        public long MethodCodeID { get; set; }
        public string? ProfileNo { get; set; }
        public string? MethodCode { get; set; }
        public decimal BatchSizeInLiters { get; set; }
        public string? RestrictionOnPlanningDay { get; set; }
        public string? ProcessName { get; set; }

        // Syrup Simplex process
        public string? IsthereSyrupSimplextoproduce { get; set; }        
        public string? SyrupSimplexProcessName { get; set; }
        public string? SyrupSimplexLocation { get; set; }
        public string? SyrupSimplexPreparationHour { get; set; }
        public int SyrupSimplexManpower { get; set; }
        public string? SyrupSimplexLevel2CleaningHours { get; set; }
        public int SyrupSimplexLevel2CleaningManpower{ get; set; }
        public int SyrupSimplexNoofCampaign { get; set; }
        public string? SyrupSimplexNextProcessName { get; set; }


        // Syrup Simplex preparation
        public string? SyruppreparationProcessName { get; set; }
        public string? SyruppreparationLocation { get; set; }
        public string? SyruppreparationFirstVolumnHour { get; set; }
        public string? SyruppreparationFirstVolumnManpower { get; set; }
        public string? SyruppreparationIPQCTest { get; set; }
        public string? SyruppreparationTopupToVolumnHour { get; set; }
        public string? SyruppreparationTopupToVolumnManpower { get; set; }
        public string? SyruppreparationCampaignBatchesNumbers { get; set; }
        public string? SyruppreparationLevel1CleaningHours { get; set; }
        public string? SyruppreparationLevel1Cleaningmanpower { get; set; }
        public string? SyruppreparationLevel2Cleaninghours { get; set; }
        public string? SyruppreparationLevel2CleaningManpower { get; set; }
        public string? SyruppreparationNextProcessName { get; set; }

        public decimal PreparationPerHour { get; set; }
    

        // Cleaning Level 2
        public decimal Level2CleaningHours { get; set; }
        public int Level2CleaningManpower { get; set; }

        public string? NoOfCampaign { get; set; }
        public string? NextProcessName { get; set; }

        // Syrup Preparation
        public string? SyrupPreparationLocation { get; set; }
        public decimal PreparationFirstVolumePerHour { get; set; }
        public int PreparationFirstVolumeManpower { get; set; }

        public bool IpqcTestRequired { get; set; } // True/False instead of "2. IPQC test"

        public decimal PreparationTopUpPerHour { get; set; }
        public int PreparationTopUpManpower { get; set; }

        public int CampaignBatches { get; set; }

        // Cleaning Level 1
        public decimal Level1CleaningHours { get; set; }
        public int Level1CleaningManpower { get; set; }

        // Cleaning Level 2 (repeated section for Syrup Preparation)
        public decimal SyrupLevel2CleaningHours { get; set; }
        public int SyrupLevel2CleaningManpower { get; set; }

        public string? NextProcessNameAfterPreparation { get; set; }
        public List<SyrupOtherProcess> SyrupOtherProcesses { get; set; } = new List<SyrupOtherProcess>();
        public class SyrupProcessNameList
        {
            public long ID { get; set; }
            public string? ProcessName { get; set; }
        }

        public class SyrupFilling
        {
            public long ID { get; set; }
            public long SyrupPlanningID { get; set; }
            public long DynamicFormID { get; set; }
            public long DynamicFormDataID { get; set; }
            public long DynamicFormDataItemID { get; set; }
            public string ProcessName { get; set; } = "";
            public string ProfileNo { get; set; } = "";
       
            public string TypeOfPlanningProcess { get; set; } = "";
            public string FillingHours { get; set; } = "";
            public string FillingManpower { get; set; } = "";           
            public string ModifiedBy { get; set; } = "";
            public DateTime ModifiedDate { get; set; }
           

            // Primary Packing
            public string? ProcessName_Primary { get; set; }
            public string? NextProcessName_Primary { get; set; }
            public string? PlanningType_Primary { get; set; }

            // Machine Filling
            public string? PrimaryFillingMachine { get; set; }
            public decimal? FillingHours_Level1 { get; set; }
            public int? FillingManpower_Level1 { get; set; }
            public decimal? Speed_BottlePerMinute { get; set; }
            public decimal? ChangePackingFillingHours { get; set; }
            public decimal? FillingHours_Level2 { get; set; }
            public int? FillingManpower_Level2 { get; set; }

            // Secondary Packing
            public bool SecondarySameAsPrimaryTime { get; set; }
            public decimal? SecondaryPackingHours { get; set; }
            public int? SecondaryManpower { get; set; }
            public string? ProcessName_Secondary { get; set; }
            public string? NextProcessName_Secondary { get; set; }
            public string? RequireOfflinePacking { get; set; }
            public long? AddedByUserID { get; set; }
            public string? Description { get; set; }          


        }
        public class SyrupOtherProcess
        {
            public long ID { get; set; }
            public long DynamicFormID { get; set; }
            public long DynamicFormDataItemID { get;set; }
            public long SyrupPlanningID { get; set; }
            public long DynamicFormDataID { get; set; }
            public string? OtherJobsInformation { get; set; }
            public string? ProcessName { get; set; }
            public string? ProfileNo { get; set; }
            public string? SyrupOtherProcessNextProcess { get; set; }

            public string? MustCompleteForNext { get; set; }

            public string? LocationOfProcess { get; set; }

            public decimal? ManhoursOrHours { get; set; }

            public int? NoOfManpower { get; set; } 

            public string? ManpowerFromNextProcess { get; set; }

            public string? CanCarryoutOnNonWorkday { get; set; }

            public decimal? TimeGapHour { get; set; }
            public long? AddedByUserID { get; set; }
            public string? Description { get; set; }
            public string? ModifiedBy { get; set; } 
            public DateTime ModifiedDate { get; set; }

        }

    }



}
