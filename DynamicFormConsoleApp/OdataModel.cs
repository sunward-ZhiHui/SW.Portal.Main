using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormConsoleApp
{

    public class Rootobject
    {
        public int responseCode { get; set; }
        public object result { get; set; }
        public Result[] results { get; set; }
        public object[] errorMessages { get; set; }
    }

    public class Result
    {
        public int dynamicFormDataId { get; set; }
        public object dynamicFormItem { get; set; }
        public bool isSendApproval { get; set; }
        public int dynamicFormId { get; set; }
        public object fileProfileSessionID { get; set; }
        public object profileId { get; set; }
        public string profileNo { get; set; }
        public object attributeHeader { get; set; }
        public object objectData { get; set; }
        public object dynamicFormCurrentItem { get; set; }
        public string name { get; set; }
        public string screenID { get; set; }
        public object isApproval { get; set; }
        public object isApproved { get; set; }
        public object approvalStatus { get; set; }
        public object approvalStatusId { get; set; }
        public object rejectedUser { get; set; }
        public object rejectedDate { get; set; }
        public object rejectedUserId { get; set; }
        public object pendingUser { get; set; }
        public object pendingUserId { get; set; }
        public object approvedUser { get; set; }
        public object approvedDate { get; set; }
        public object approvedUserId { get; set; }
        public object dynamicFormApproved { get; set; }
        public object statusName { get; set; }
        public object fileProfileTypeId { get; set; }
        public object fileProfileTypeName { get; set; }
        public int isDocuments { get; set; }
        public int isFileprofileTypeDocument { get; set; }
        public object currentUserName { get; set; }
        public object currentUserId { get; set; }
        public Dynamicformprofile dynamicFormProfile { get; set; }
        public object dynamicFormDataGridId { get; set; }
        public bool isDynamicFormDataGrid { get; set; }
        public object backUrl { get; set; }
        public object emailTopicSessionId { get; set; }
        public bool? isDraft { get; set; }
        public object dynamicFormName { get; set; }
        public object dynamicFormSessionID { get; set; }
        public int sortOrderByNo { get; set; }
        public object sortOrderAnotherBy { get; set; }
        public object gridSortOrderByNo { get; set; }
        public object gridSortOrderAnotherByNo { get; set; }
        public object companyId { get; set; }
        public object plantCode { get; set; }
        public object dynamicFormDataGridFormId { get; set; }
        public object dynamicFormDataGridProfileNo { get; set; }
        public object dynamicFormSectionGridAttributeId { get; set; }
        public Objectdatalist[] objectDataList { get; set; }
        public Objectdataitems objectDataItems { get; set; }
        public Dynamicformreportitem[] dynamicFormReportItems { get; set; }
        public DateTime addedDate { get; set; }
        public object modifiedByUserID { get; set; }
        public DateTime modifiedDate { get; set; }
        public string sessionId { get; set; }
        public object addedByUserID { get; set; }
        public object statusCodeID { get; set; }
        public int index { get; set; }
        public object addedBy { get; set; }
        public string modifiedBy { get; set; }
        public object statusCode { get; set; }
    }

    public class Dynamicformprofile
    {
        public int plantId { get; set; }
        public int departmentId { get; set; }
        public int sectionId { get; set; }
        public int subSectionId { get; set; }
        public int divisionId { get; set; }
        public int profileId { get; set; }
        public object userId { get; set; }
        public object profileNo { get; set; }
    }

    public class Objectdataitems
    {
        public string _35_AttrJob_Require_for_ { get; set; }
        public string _43_AttrDescription_of_the_Job { get; set; }
        public string _45_AttrAssignment_of_Job { get; set; }
        public string _86_AttrAssignment_of_Job { get; set; }
        public string _87_AttrAssignment_of_Job { get; set; }
        public string _54_AttrICT_Job_Type { get; set; }
        public string _44_AttrDetail_requirement { get; set; }
        public string _32_AttrType_of_Job_Request { get; set; }
        public string _32_33_46_Sub_SubAttrType_of_Solution { get; set; }
        public string _32_66_44_Sub_SubAttrRequest_No { get; set; }
        public string _32_66_45_Sub_SubAttrRequest_Description { get; set; }
    }

    public class Objectdatalist
    {
        public _35_Attr _35_Attr { get; set; }
        public _43_Attr _43_Attr { get; set; }
        public _45_Attr _45_Attr { get; set; }
        public _86_Attr _86_Attr { get; set; }
        public _87_Attr _87_Attr { get; set; }
        public _54_Attr _54_Attr { get; set; }
        public _44_Attr _44_Attr { get; set; }
        public _32_Attr _32_Attr { get; set; }
        public _32_33_46_Sub_Subattr _32_33_46_Sub_SubAttr { get; set; }
        public _32_66_44_Sub_Subattr _32_66_44_Sub_SubAttr { get; set; }
        public _32_66_45_Sub_Subattr _32_66_45_Sub_SubAttr { get; set; }
    }

    public class _35_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _43_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _45_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsGrid { get; set; }
        public string DynamicFormSessionId { get; set; }
        public string DynamicFormDataSessionId { get; set; }
        public string DynamicFormDataGridSessionId { get; set; }
        public string Url { get; set; }
    }

    public class _86_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsGrid { get; set; }
        public string DynamicFormSessionId { get; set; }
        public string DynamicFormDataSessionId { get; set; }
        public string DynamicFormDataGridSessionId { get; set; }
        public string Url { get; set; }
    }

    public class _87_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsGrid { get; set; }
        public string DynamicFormSessionId { get; set; }
        public string DynamicFormDataSessionId { get; set; }
        public string DynamicFormDataGridSessionId { get; set; }
        public string Url { get; set; }
    }

    public class _54_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _44_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _32_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _32_33_46_Sub_Subattr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _32_66_44_Sub_Subattr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _32_66_45_Sub_Subattr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class Dynamicformreportitem
    {
        public string attrId { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public bool isGrid { get; set; }
        public string dynamicFormSessionId { get; set; }
        public string dynamicFormDataSessionId { get; set; }
        public string dynamicFormDataGridSessionId { get; set; }
        public string url { get; set; }
        public Griditem[] gridItems { get; set; }
        public bool isSubForm { get; set; }
    }

    public class Griditem
    {
        public int dynamicFormDataId { get; set; }
        public object dynamicFormItem { get; set; }
        public bool isSendApproval { get; set; }
        public int dynamicFormId { get; set; }
        public object fileProfileSessionID { get; set; }
        public object profileId { get; set; }
        public string profileNo { get; set; }
        public object attributeHeader { get; set; }
        public object objectData { get; set; }
        public object dynamicFormCurrentItem { get; set; }
        public string name { get; set; }
        public string screenID { get; set; }
        public object isApproval { get; set; }
        public object isApproved { get; set; }
        public object approvalStatus { get; set; }
        public object approvalStatusId { get; set; }
        public object rejectedUser { get; set; }
        public object rejectedDate { get; set; }
        public object rejectedUserId { get; set; }
        public object pendingUser { get; set; }
        public object pendingUserId { get; set; }
        public object approvedUser { get; set; }
        public object approvedDate { get; set; }
        public object approvedUserId { get; set; }
        public object dynamicFormApproved { get; set; }
        public object statusName { get; set; }
        public object fileProfileTypeId { get; set; }
        public object fileProfileTypeName { get; set; }
        public int isDocuments { get; set; }
        public int isFileprofileTypeDocument { get; set; }
        public object currentUserName { get; set; }
        public object currentUserId { get; set; }
        public Dynamicformprofile1 dynamicFormProfile { get; set; }
        public object dynamicFormDataGridId { get; set; }
        public bool isDynamicFormDataGrid { get; set; }
        public object backUrl { get; set; }
        public object emailTopicSessionId { get; set; }
        public object isDraft { get; set; }
        public object dynamicFormName { get; set; }
        public object dynamicFormSessionID { get; set; }
        public int sortOrderByNo { get; set; }
        public object sortOrderAnotherBy { get; set; }
        public object gridSortOrderByNo { get; set; }
        public object gridSortOrderAnotherByNo { get; set; }
        public object companyId { get; set; }
        public object plantCode { get; set; }
        public object dynamicFormDataGridFormId { get; set; }
        public object dynamicFormDataGridProfileNo { get; set; }
        public object dynamicFormSectionGridAttributeId { get; set; }
        public Objectdatalist1[] objectDataList { get; set; }
        public Objectdataitems1 objectDataItems { get; set; }
        public Dynamicformreportitem1[] dynamicFormReportItems { get; set; }
        public DateTime addedDate { get; set; }
        public object modifiedByUserID { get; set; }
        public DateTime modifiedDate { get; set; }
        public string sessionId { get; set; }
        public object addedByUserID { get; set; }
        public object statusCodeID { get; set; }
        public int index { get; set; }
        public object addedBy { get; set; }
        public string modifiedBy { get; set; }
        public object statusCode { get; set; }
    }

    public class Dynamicformprofile1
    {
        public int plantId { get; set; }
        public int departmentId { get; set; }
        public int sectionId { get; set; }
        public int subSectionId { get; set; }
        public int divisionId { get; set; }
        public int profileId { get; set; }
        public object userId { get; set; }
        public object profileNo { get; set; }
    }

    public class Objectdataitems1
    {
        public DateTime _36_AttrDate_of_Entry { get; set; }
        public string _37_AttrJob_Number { get; set; }
        public string _38_AttrJob_description { get; set; }
        public string _39_AttrAssigned_staff__Company { get; set; }
        public string _39_1_Employee_DependencyEmployee { get; set; }
        public DateTime _40_AttrExpected_Completion_date_for_assignee { get; set; }
        public string _41_AttrTask_Status { get; set; }
    }

    public class Objectdatalist1
    {
        public _36_Attr _36_Attr { get; set; }
        public _37_Attr _37_Attr { get; set; }
        public _38_Attr _38_Attr { get; set; }
        public _39_Attr _39_Attr { get; set; }
        public _39_1_Employee_Dependency _39_1_Employee_Dependency { get; set; }
        public _40_Attr _40_Attr { get; set; }
        public _41_Attr _41_Attr { get; set; }
    }

    public class _36_Attr
    {
        public string Label { get; set; }
        public DateTime Value { get; set; }
    }

    public class _37_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _38_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _39_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _39_1_Employee_Dependency
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class _40_Attr
    {
        public string Label { get; set; }
        public DateTime Value { get; set; }
    }

    public class _41_Attr
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class Dynamicformreportitem1
    {
        public string attrId { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public bool isGrid { get; set; }
        public object dynamicFormSessionId { get; set; }
        public object dynamicFormDataSessionId { get; set; }
        public object dynamicFormDataGridSessionId { get; set; }
        public string url { get; set; }
        public object[] gridItems { get; set; }
        public bool isSubForm { get; set; }
    }

}
