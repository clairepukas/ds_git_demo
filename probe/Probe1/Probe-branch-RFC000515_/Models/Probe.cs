using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Web.Mvc;
using System.Data;
using System.Web.Configuration;
using WebMatrix.Data;


namespace Probe.Models
{
    public class ProjectInfo
    {
        ProbeData data = ProbeDataContext.GetDataContext();
        public ProjectInfo(){}

        public int ProjectId {get; set;}

        public ProjectInfoBudget spank { get; set; } 

        [DisplayName("Project #")]
        public string ProjectNumber {get; set;}

        [DisplayName("Selected Project")]
        public string ProjectName {get; set;}

        [DisplayName("Project Name")]
        public string DisplayName {get; set;}

        [DisplayName("Budget #")]
        public string BudgetNum {get; set;}
        public bool Priority {get; set;}
        public Nullable<int> Health {get; set;}
        public string Scope {get; set;}
        public string Comments {get; set;}
        public IEnumerable<SelectListItem> ManagerList { get; set; }

        [DisplayName("Project Manager")]
        public string Manager { get; set; }
        public IEnumerable<SelectListItem> SponsorList { get; set; }

        [DisplayName("Project Sponsor")]
        public string Sponsor { get; set; }
        public IEnumerable<SelectListItem> EngineerList { get; set; }

        [DisplayName("Project Engineer")]
        public string Engineer { get; set; }
        public IEnumerable<SelectListItem> ContributorList { get; set; }
        public string Creator { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public string Customer { get; set; }
        public bool CarryOver {get; set;}

        public bool InitApproved { get; set; }

        [DisplayName("Budget Type")]
        public string ProjectType { get; set; }
        [DisplayName("Warranty")]
        public string warrantyAgreement { get; set; }
        [DisplayName("Expiration")]
        public Nullable<DateTime> warrantyAgreementDate { get; set; }
       
        public IEnumerable<SelectListItem> warrantyAgreementTypes { get; set; }
        public Nullable<int> RiskLikelihood {get; set;}
        public Nullable<int> RiskImpact { get; set; }
       
        [DisplayName("Approval Status")]
        public string ApprovalStatus {get; set;}
        public IEnumerable<SelectListItem> ActivityStatusList { get; set; }
        
        [DisplayName("Activity Status")]
        public string ActivityStatus { get; set; }
        public IEnumerable<SelectListItem> BudgetTypeList { get; set; }

        [DisplayName("Proposed Start Date")]
        public Nullable<DateTime> ProposedStart { get; set; }

        [DisplayName("Proposed End Date")]
        public Nullable<DateTime> ProposedEnd { get; set; }

        [DisplayName("Projected Start Date")]
        public Nullable<DateTime> ProjectedStart { get; set; }

        [DisplayName("Projected End Date")]
        public Nullable<DateTime> ProjectedEnd { get; set; }

        [DisplayName("Actual Start Date")]
        public Nullable<DateTime> ActualStart { get; set; }

        [DisplayName("Actual End Date")]
        public Nullable<DateTime> ActualEnd { get; set; }
        public ProjectInfoBudget ProjectBudgets { get; set; }
        public List<ProjectAttachments> ProjectAttachments { get; set; }

        [DisplayName("Total % Complete")]
        public int pct_complete { get; set; }
        public IEnumerable<SelectListItem> GroupList { get; set; }

        [DisplayName("Group")]
        public Group ProjectGroup { get; set; }

        public IEnumerable<ProjectInfo> projectGroups { get; set; }

        public int GroupId { get; set; }

        public decimal BudgetTotal { get; set; }

        public decimal Budgeted { get; set; }

    }

    public class DashboardProject
    {
        public DashboardProject(){}
        public int ProjectId { get; set;}
        public string DisplayName {get; set;}
        public string ApprovalStatus {get; set;}
        public string ActivityStatus { get; set; }
        public Nullable<int> Health {get; set;}
        public string ProjectType { get; set; }
        public Nullable<DateTime> StartDate{ get; set;}
        public Nullable<DateTime> EndDate{ get; set;}        
        
    }

    public class ProbeTemplates
    {
        public ProbeTemplates() { }

        public string TemplateFileName { get; set; }
        public string TemplateDesc { get; set; }
        public string FileType { get; set; }
        public string TemplateType { get; set; }
        public string DocumentType { get; set; }
        public byte[] Template { get; set; }

    }
    public class ProbeUser
    {
        public ProbeUser(){}
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public int active { get; set; }
        public bool IsAdmin { get; set; }
        public int? ResourceId { get; set; }
        public List<string> ProjectRoleList { get; set; }
        public List<string> UserRoleList { get; set; }
    }
    public class ProjectRoles
    {
        public ProjectRoles() { }
        public string ProjectRole { get; set; }

    }

    public class UserRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }

    }
    public class GuestUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Extension { get; set; }
        
    }
    public class NewProj
    {
        [Required][DisplayName("Project Name")]
        public string ProjectName { get; set; }
        [DisplayName("Budget #")]
        public string BudgetNum { get; set; }
        [Required]
        public string Scope { get; set; }
        [DisplayName("Project Type")]
        public string ProjectType { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        public string updateBy { get; set; }
        public string Comments { get; set; }
        public string Customer { get; set; }
        public string Manager { get; set; }
        public string ServiceAgreementType { get; set; }
        public Nullable<DateTime> ServiceAgreementExpDt { get; set; } 
        
        public string Sponsor { get; set; }
        public string Engineer { get; set; }
        public string ServiceAgreement { get; set; }
        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDate { get; set; }
        [DisplayName("Add this project to a Group")]
        public string Group { get; set; }
        public IEnumerable<SelectListItem> ProjectTypes { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; }
        public IEnumerable<SelectListItem> Groups { get; set; }
        public IEnumerable<SelectListItem> Agreements { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> RequiredUsers { get; set; }

        public GuestUser RequestCreator { get; set; }
        public NewProj() { }
    }

    
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public Group() { }
    }
    public class ChangeLog 
    {
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime LogDt { get; set; }
        public string UserName { get; set; }
        public string LogMsg { get; set; }

        public ChangeLog() { }
    }
    public class ProjectImpact
    {
        public int ImpactId { get; set; }
        public string ImpactName { get; set; }
        public string OtherImpactDesc { get; set; }
        public int Selected { get; set; }
        public int ProjImpactId { get; set; }

        public ProjectImpact() { }
    }

    public class ProjectBenefit
    {
        public int BenefitId { get; set; }
        public string BenefitName { get; set; }
        public string BenefitGroupName { get; set; }
        public int BenefitGroupOrder { get; set; }
        public int Selected { get; set; }

        public ProjectBenefit() { }
    }

    public class ProjectCostSaving
    {
        public int CostSavingsTypeId { get; set; }
        public string CostSavingsType { get; set; }
        public decimal CostSaving { get; set; }

        public ProjectCostSaving() { }
    }

    public class Logs
    {
        public virtual List<ChangeLog> AllLogs { get; set; }        
        public Logs() { }
    }

    public class ProjectInfoBudget
    {
        public ProjectInfoBudget() { }

        [DisplayName("Current Budgeted Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> BudgetedTotal { get; set; }

        [DisplayName("Projected Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ProjectedTotal { get; set; }

        [DisplayName("Actual Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ActualTotal { get; set; }

        [DisplayName("Total Proposed Budget")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ProposedTotal { get; set; }

        [DisplayName("Proposed Capital Budget")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ProposedCapital { get; set; }

        [DisplayName("Proposed Expense Budget")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ProposedExpense { get; set; }

        [DisplayName("Proposed Expense E to P")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ProposedExpenseEP { get; set; }
        //public Nullable<decimal> Budgeted { get; set; }
        //public Nullable<decimal> Projected { get; set; }
        [DisplayName("Current Budgeted Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> tot { get; set; }

    }

    public class ProjectAttachments
    {
        public ProjectAttachments() { }

        public int ProjectAttachmentId { get; set; }

        public string AttachmentTitle { get; set; }

        public string AttachmentType { get; set; }

        public byte[] AttachmentBinary { get; set; }

        public string FileType { get; set; }

        public DateTime createdDt { get; set; }
    }

    public class ProjectPhase
    {
        public int ProjectId { get; set;}
        public int PhaseId { get; set;}        
        public string PhaseName {get; set;}
        public DateTime StartDate{ get; set;}
        public DateTime EndDate{ get; set;}
        public string color{ get; set;}
        public Boolean IsCurrent {get; set;}
        public int pct_complete { get; set;}   
        public string updateBy{ get; set;}

    }

    public class ResourceManHrs
    {
        public int ResourceId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Comments { get; set; }
        public decimal BaseManHours { get; set; }
        public decimal CoreManHours { get; set; }
        public string updateBy { get; set; }



    }
    public class ResourceEntity
    {
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public int ParentId { get; set; }
        public int EntityId { get; set; }
        public int UserId { get; set; }
        public string Notes { get; set; }

        public string updateBy { get; set; }

    }

    public class ProjectResource
    {
        public int ProjectId { get; set; }
        public int ResourceId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
        public decimal ProposedManHrs { get; set; }
        public decimal ActualManHrs { get; set; }
        public string Comments { get; set; }

        public int EntityId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceTitle { get; set; }
        public string ResourceEmail { get; set; }
        public string ResourcePhone { get; set; }
        public string updateBy { get; set; }

    }

    public class ProjectResources
    {
        public int ProjectId { get; set; }
        public int ResourceId { get; set; }
        public int Year { get; set; }
        public string Comments { get; set; }
        public decimal ProposedManHrs { get; set; }
        public decimal ActualManHrs { get; set; }
        public string updateBy { get; set; }
    }

    public class OrigApprovedBudget
    {
        public string BudgetType { get; set; }
        public decimal Total { get; set; }

    }

    public class AgreementTypes
    {
        public string ServiceAgreementType { get; set; }
        public decimal Total { get; set; }

    }
    public class BudgetRecord
    {
        public int ProjectId { get; set; }
        public string BudgetType { get; set; }
        public string BudgetStatus { get; set; }   
        public decimal Amount { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
    }

    public class BudgetViewRecord
    {
        public int ProjectId { get; set; }
        public string BudgetType { get; set; }
        public decimal Budgeted { get; set; }
        public decimal Projected { get; set; }
        public decimal Actual { get; set; }
        public decimal CurrentBudgetAmount { get; set; }
        
    }

    public class ApprovalActivityStatuses{
        public string ApprovalStatus { get; set; }
        public string ActivityStatus { get; set; }
        public string ReportDefault { get; set; }
    }
    public class BenefitGroups{

        public int BenefitGroupId { get; set; }
        public string BenefitGroupName { get; set; }
        public int BenefitGroupOrder { get; set; }
    }
    public class Benefits{
        public int BenefitId { get; set; }
        public string BenefitName { get; set; }
        public int BenefitGroupId { get; set; }
    }
    public class BudgetStatuses{
        public string BudgetStatus { get; set; }
    }
    public class BudgetTypes{
        public string BudgetType { get; set; }
    }
    public class CostSavingsTypes{
        public int CostSavingsTypeId { get; set; }
        public string CostSavingsType { get; set; }
    }
    public class DocumentTypes{
        public string DocumentType { get; set; }    
    }
    public class EntityTypes{
        public string EntityType { get; set; }   
    }
    public class Groups{
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
    public class Impacts{
        public int ImpactId { get; set; }
        public string ImpactName { get; set; }
        public string ImpactDesc { get; set; }
    }
    public class IssueStatuses{
        public string IssueStatus { get; set; }
    }
    public class Phases{
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }
        public string PhaseDesc { get; set; }
        public int PhaseOrder{ get; set; }
        public string color { get; set; }
        public Int16 weeks_duration { get; set; }
    }
    public class ProjectTypes{
        public string ProjectType { get; set; }
    }
    public class TemplateTypes
    {
        public string TemplateType { get; set; }
    }

    public partial class ProbeData : DataContext
    {
        #region temporary

        public int GetProjectId()
        {
            try
            {
                var sql = @"SELECT min(ProjectId) FROM Projects WHERE ApprovalStatus = 'Approved'";
                IEnumerable<int> results = this.ExecuteQuery<int>(sql);
                return results.ToList<int>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region List Functions
        public List<string> projTyps()
        {
            try
            {
                var sql = @"SELECT ProjectType From ProjectTypes";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> custmrs()
        {
            try
            {
                var sql = @"SELECT Customer FROM Customers";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Group> grps()
        {
            try
            {
                var sql = @"SELECT GroupId, GroupName FROM Groups";
                IEnumerable<Group> results = this.ExecuteQuery<Group>(sql);
                return results.ToList<Group>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProbeUser> usrs()
        {
            try
            {
                var sql = @"SELECT UserId, UserName, DisplayName FROM Users where Active != 0 order by DisplayName";
                IEnumerable<ProbeUser> results = this.ExecuteQuery<ProbeUser>(sql);
                return results.ToList<ProbeUser>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> approvalStatuses()
        {
            try
            {
                var sql = @"SELECT ApprovalStatus From ApprovalStatuses";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> activityStatuses(string status)
        {
            try
            {
                var sql = @"SELECT ActivityStatus From ApprovalActivityStatuses where ApprovalStatus = {0}";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql, status);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region User Functions
        public ProbeUser GetUser(string username)
        {
            try
            {
                var sql = @"Select UserId, 
			                    UserName, 
			                    DisplayName, 
			                    Email, 
                                Phone,
			                    FirstName, 
			                    LastName,
                                MiddleInitial,
                                Active 
	                        From probe.dbo.Users Where UserName = {0} and Active != 0";

                IEnumerable<ProbeUser> results = this.ExecuteQuery<ProbeUser>(sql, username);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProbeUser GetUser(int id)
        {
            try
            {
                var sql = @"Select UserId, 
			                    UserName, 
			                    DisplayName, 
			                    Email, 
                                Phone,
			                    FirstName, 
			                    LastName,
                                MiddleInitial,
                                Active 
	                        From probe.dbo.Users Where UserId = {0} and Active != 0";

                IEnumerable<ProbeUser> results = this.ExecuteQuery<ProbeUser>(sql, id);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<ProbeUser> GetAdminUsers()
        {
            try
            {
                var sql = @"Select u.UserId, 
			                    UserName, 
			                    DisplayName, 
			                    Email, 
                                Phone,
			                    FirstName, 
			                    LastName,
                                MiddleInitial,
                                Active 
	                        From probe.dbo.Users u
                            join UserRoles ur on ur.UserId = u.UserId
                            join Roles r on r.RoleId = ur.RoleId
                            Where u.Active != 0 and r.RoleName = 'Admin'";

                IEnumerable<ProbeUser> results = this.ExecuteQuery<ProbeUser>(sql, "");
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ProjectUserRoles

        [FunctionAttribute(Name = "probe_AddProjectUserRoles")]
        [return: Parameter(DbType = "Int")]
        public int AddProjectUserRole(int ProjectId, int UserId, string ProjectRole, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, UserId, ProjectRole, updateBy);
            return (int)result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_UpdProjectUserRoles")]
        public int UpdProjectUserRole(int ProjectId, int UserId, string ProjectRole, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, UserId, ProjectRole, updateBy);
            return (int)result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_DelProjectUserRoles")]
        public int DelProjectUserRole(int ProjectId, int UserId, string ProjectRole)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, UserId, ProjectRole);
            return (int)result.ReturnValue;
        }

        public List<string> GetProjectRolesForUser(int projId, string uName)
        {
            string[] parms =  {projId.ToString(), uName};
            try
            {
                var sql = @"SELECT ProjectRole
                            FROM ProjectUserRoles a
                            join Users b
                            on a.UserId = b.UserId
                            WHERE ProjectId = {0}
	                        and b.UserName = {1}";

                IEnumerable<string> results = this.ExecuteQuery<string>(sql, parms);
                return results.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsUserAdmin(int uid)
        {
            bool isAdmin = false;
            try
            {
                var sql = @"SELECT  UserId
                              FROM UserRoles a
                              join Roles b
                              on a.RoleId = b.RoleId
                              where UserId  = {0}
                              and b.RoleName = 'Admin'";

                IEnumerable<ProjectRoles> results = this.ExecuteQuery<ProjectRoles>(sql, uid);

                if (results.Count() > 0)
                    isAdmin = true;
                
                return isAdmin;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region ProjectGroups

        [FunctionAttribute(Name = "probe_AddProjectGroups")]
        [return: Parameter(DbType = "Int")]
        public int AddProjectGroup(int ProjectId, int GroupId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, GroupId, UpdateBy);
            return (int)result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_UpdProjectGroups")]
        public int UpdProjectGroup(int ProjectId,int Old_GroupId, int New_GroupId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, Old_GroupId, New_GroupId, UpdateBy);            
            return (int) result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_DelProjectGroups")]
        [return: Parameter(DbType = "Int")]
        public int DelProjectGroup(int ProjectId, int GroupId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, GroupId, UpdateBy);
            return (int)result.ReturnValue;
        }

        public string GetGroupName(int groupId)
        {
            try
            {
                var sql = @"Select GroupName
                          FROM  Groups 
                          where GroupId = {0}";

                IEnumerable<Group> results = this.ExecuteQuery<Group>(sql, groupId);
                
                return results.FirstOrDefault().GroupName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ProjectDates
        
        [FunctionAttribute(Name = "probe_AddProjectDates")]
        [return: Parameter(DbType = "Int")]
        public int AddProjectDates(int ProjectId, int ProjectDateTypeId, DateTime StartDate, DateTime EndDate, string updateBy, [Parameter(Name = "ProjectDateId", DbType = "Int")] ref System.Nullable<Int32> ProjectDateId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ProjectDateTypeId, StartDate, EndDate, updateBy, ProjectDateId);
            ProjectDateId = (System.Nullable<Int32>)result.GetParameterValue(5);
            return (int)result.ReturnValue;
        }

        #endregion

        #region ChangeLog

        [FunctionAttribute(Name = "probe_AddChangeLog")]
        [return: Parameter(DbType = "Int")]
        public int AddChangeLog(int ProjectId, DateTime LogDt, int UserId, string LogMsg, [Parameter(Name = "LogId", DbType = "Int")] ref System.Nullable<Int32> LogId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, LogDt, UserId, LogMsg, LogId);
            LogId = (System.Nullable<Int32>)result.GetParameterValue(5);
            return (int)result.ReturnValue;
        }

        public List<ChangeLog> ChangeLogs(int ProjectId)
        {
            try
            {
                var sql = @"SELECT LogDt, UserName, LogMsg FROM ChangeLog a join Users b on a.UserId = b.UserId where ProjectId = {0} order by LogDt desc";
                IEnumerable<ChangeLog> results = this.ExecuteQuery<ChangeLog>(sql, ProjectId);
                return results.ToList<ChangeLog>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Project Phases
        public int GetPhaseIdByName(string phaseName)
        {
            var sql = @"select PhaseId from Phases where PhaseName={0}";
            IEnumerable<ProjectPhase> results = this.ExecuteQuery<ProjectPhase>(sql, phaseName);
            return results.FirstOrDefault().PhaseId;
        }
        public ProjectPhase GetProjectPhaseById(int ProjId, int phaseId)
        {            
            var sql = @"select * from ProjectPhases where ProjectId={0} and PhaseId = {1}";
            IEnumerable<ProjectPhase> results = this.ExecuteQuery<ProjectPhase>(sql, ProjId, phaseId);
            return results.FirstOrDefault();
        }
        public IEnumerable<ProjectPhase> GetProjectPhases(int ProjId)
        {
                        var sql = @"SELECT 
                            p.ProjectId
                            ,p.PhaseId
                            ,ph.PhaseName
                            ,p.StartDate
                            ,p.EndDate
                            ,ph.color
                            ,p.IsCurrent
                            ,p.pct_complete
                        FROM ProjectPhases p
                        join Phases ph on p.PhaseId = ph.PhaseId
                        where p.ProjectId = {0}";

            IEnumerable<ProjectPhase> results = this.ExecuteQuery<ProjectPhase>(sql, ProjId);
            return results;
        }
        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdProjectPhases")]
        public int UpdProjectPhase(int ProjectId,int PhaseId, DateTime StartDate, DateTime EndDate, int pct_complete, Boolean IsCurrent, string updateBy )
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),  ProjectId, PhaseId, StartDate,EndDate,pct_complete,IsCurrent,updateBy);
            return (int)result.ReturnValue;
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddProjectPhases")]
        public int AddProjectPhase(int ProjectId, int PhaseId, DateTime StartDate, DateTime EndDate, int pct_complete, Boolean IsCurrent, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, PhaseId, StartDate, EndDate, pct_complete, IsCurrent, updateBy);
            return (int)result.ReturnValue;
        }
        #endregion

        #region New Project

        [FunctionAttribute(Name = "probe_AddProjects")]
        public int AddProjectTwo(string ProjectName, string BudgetNum, string Scope, string Comments, string Customer, string ProjectType, DateTime? ProposedStart, DateTime? ProposedEnd, string updateBy, [Parameter(Name = "ProjectId", DbType = "Int")] ref System.Nullable<Int32> ProjectId, [Parameter(Name = "UserId", DbType = "Int")] ref System.Nullable<Int32> UserId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectName, BudgetNum, Scope, Comments, Customer, ProjectType, ProposedStart, ProposedEnd, updateBy, ProjectId, UserId);
            ProjectId = (System.Nullable<Int32>)result.GetParameterValue(9);
            UserId = (System.Nullable<Int32>)result.GetParameterValue(10);
            string ret = ProjectId.ToString() + "|" + UserId.ToString();
            return (int)ProjectId;
        }

        public List<int> AddProjectTwoOutputs(string ProjectName, string BudgetNum, string Scope, string Comments, string Customer, string ProjectType, DateTime? ProposedStart, DateTime? ProposedEnd, string updateBy, int ProjectId, int UserId, string serviceAgreements, DateTime? SAExpirationdate)
        {
            List<int> ret = new List<int>();
            try
            {
                string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(cnStr))
                {
                    using (SqlCommand cmd = new SqlCommand("probe_AddProjects", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                        cmd.Parameters.AddWithValue("@BudgetNum", BudgetNum);
                        cmd.Parameters.AddWithValue("@Scope", Scope);
                        cmd.Parameters.AddWithValue("@Comments", Comments);
                        cmd.Parameters.AddWithValue("@Customer", Customer);
                        cmd.Parameters.AddWithValue("@ProjectType", ProjectType);
                        cmd.Parameters.AddWithValue("@ProposedStart", ProposedStart);
                        cmd.Parameters.AddWithValue("@ProposedEnd", ProposedEnd);
                        cmd.Parameters.AddWithValue("@updateBy", updateBy);
                        cmd.Parameters.AddWithValue("@ServiceAgreementType", serviceAgreements);
                        cmd.Parameters.AddWithValue("@ServiceAgreementExpDt", SAExpirationdate);

                        SqlParameter projid = cmd.CreateParameter();
                        projid.ParameterName = "@ProjectId";
                        projid.Direction = System.Data.ParameterDirection.Output;
                        projid.DbType = System.Data.DbType.Int32;
                        cmd.Parameters.Add(projid);

                        SqlParameter usrid = cmd.CreateParameter();
                        usrid.ParameterName = "@UserId";
                        usrid.Direction = System.Data.ParameterDirection.Output;
                        usrid.DbType = System.Data.DbType.Int32;
                        cmd.Parameters.Add(usrid);

                        cn.Open();
                        int result = cmd.ExecuteNonQuery();
                        cn.Close();
                        ret.Add(int.Parse(cmd.Parameters["@ProjectId"].Value.ToString()));
                        ret.Add(int.Parse(cmd.Parameters["@UserId"].Value.ToString()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }
        
        [FunctionAttribute(Name = "probe_AddProjects")]
        [return: Parameter(DbType = "Int")]
        public int AddProject(string ProjectName, string BudgetNum, string Scope, string Comments, string Customer, string ProjectType, DateTime? ProposedStart, DateTime? ProposedEnd, string updateBy, [Parameter(Name = "ProjectId", DbType = "Int")] ref System.Nullable<Int32> ProjectId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectName, BudgetNum, Scope, Comments, Customer, ProjectType, ProposedStart, ProposedEnd, updateBy, ProjectId);
            ProjectId = (System.Nullable<Int32>)result.GetParameterValue(9);
            return (int)ProjectId;
        }

        public int UpdateProject(ProjectInfo oldModel, ProjectInfo model, string changedField, int userId, string updateBy)
        {
            int ret = -1;
            int? newLogId = -1;
            int log = -1;
            string msg = "";

            //################################# GREAT BIG NOTE ############################################################
            //Due to the requirement of needing to set the Actual Start or Actual End dates from a value to a NULL 
            //The ActualStart and ActualEnd values must be included in every call to the UpdProject stored procedure calls.
            //Richard Eaton - 10-13-2015
            //#############################################################################################################
            try
            {
            switch (changedField)
            {
                case "DisplayName":
                    if (oldModel.DisplayName != model.DisplayName && !string.IsNullOrEmpty(model.DisplayName))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter pnum = new SqlParameter();
                                pnum.ParameterName = "@ProjectNumber";
                                pnum.Value = oldModel.ProjectNumber;
                                pnum.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(pnum);

                                SqlParameter dname = new SqlParameter();
                                dname.ParameterName = "@DisplayName";
                                dname.Value = model.DisplayName;
                                dname.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(dname);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }                        

                        msg = "Project Display Name changed from " + oldModel.DisplayName + " to " + model.DisplayName;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }

                    break;

                case "BudgetNum":
                    if (oldModel.BudgetNum != model.BudgetNum && !string.IsNullOrEmpty(model.BudgetNum))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@BudgetNum";
                                x.Value = model.BudgetNum;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();

                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }   
                     
                        msg = "Project Budget Number changed from " + oldModel.BudgetNum + " to " + model.BudgetNum;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "Manager":
                    if (oldModel.Manager != model.Manager && !string.IsNullOrEmpty(model.Manager))
                    {
                        ProbeUser mgr = GetUser(model.Manager);
                        if (string.IsNullOrEmpty(oldModel.Manager))
                        {
                            ret = AddProjectUserRole(model.ProjectId, mgr.UserId, "Manager", updateBy);

                            msg = "Project Manager added";
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                        else
                        {
                        ret = UpdProjectUserRole(model.ProjectId, mgr.UserId, "Manager", updateBy);

                        msg = "Project Manager changed from " + oldModel.Manager + " to " + model.Manager;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    }
                    break;
                    
                case "Sponsor":
                    if (oldModel.Sponsor != model.Sponsor && !string.IsNullOrEmpty(model.Sponsor))
                    {
                        ProbeUser sponsor = GetUser(model.Sponsor);
                        if (string.IsNullOrEmpty(oldModel.Sponsor))
                        {
                            ret = AddProjectUserRole(model.ProjectId, sponsor.UserId, "Sponsor", updateBy);

                            msg = "Project Sponsor added";
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                        else
                        {
                            
                        ret = UpdProjectUserRole(model.ProjectId, sponsor.UserId, "Sponsor", updateBy);

                        msg = "Project Sponsor changed from " + oldModel.Sponsor + " to " + model.Sponsor;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    }
                    break;

                case "Engineer":
                    if (oldModel.Engineer != model.Engineer && !string.IsNullOrEmpty(model.Engineer))
                    {
                        ProbeUser engineer = GetUser(model.Engineer);

                        if (string.IsNullOrEmpty(oldModel.Engineer))
                        {
                            ret = AddProjectUserRole(model.ProjectId, engineer.UserId, "Engineer", updateBy);

                            msg = "Project Engineer added";
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                        else
                        {
                        ret = UpdProjectUserRole(model.ProjectId, engineer.UserId, "Engineer", updateBy);

                        msg = "Project Engineer changed from " + oldModel.Engineer + " to " + model.Engineer;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    }
                    break;

                case "ProjectGroup":
                    if (oldModel.ProjectGroup != model.ProjectGroup)
                    {                        
                        if (oldModel.ProjectGroup.GroupId == 0)
                        {
                            ret = AddProjectGroup(model.ProjectId, model.ProjectGroup.GroupId, updateBy);
                            msg = "Project Group added";
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                        else if (model.ProjectGroup.GroupId == 0 || model.ProjectGroup.GroupName == null)
                        {
                            ret = DelProjectGroup(model.ProjectId, oldModel.ProjectGroup.GroupId, updateBy);
                            msg = "Project Group deleted";
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                        else
                        {
                            model.ProjectGroup.GroupName = GetGroupName(model.ProjectGroup.GroupId);
                            ret = UpdProjectGroup(model.ProjectId, oldModel.ProjectGroup.GroupId, model.ProjectGroup.GroupId, updateBy);
                            msg = "Project Group changed from " + oldModel.ProjectGroup.GroupName + " to " + model.ProjectGroup.GroupName;
                            log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                        }
                    }
                    break;

                case "Priority":

                        string cnnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@Priority";
                                x.Value = model.Priority;
                                x.SqlDbType = SqlDbType.Bit;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }        
                    
                    msg = "Project Priority changed from " + oldModel.Priority + " to " + model.Priority;
                    log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    
                    break;

                case "Customer":
                    if (oldModel.Customer != model.Customer && !string.IsNullOrEmpty(model.Customer))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@Customer";
                                x.Value = model.Customer;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Customer changed from " + oldModel.Customer + " to " + model.Customer;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "Scope":
                    if (oldModel.Scope != model.Scope && !string.IsNullOrEmpty(model.Scope))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@Scope";
                                x.Value = model.Scope;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Scope changed from " + oldModel.Scope + " to " + model.Scope;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "Comments":
                    if (oldModel.Comments != model.Comments && !string.IsNullOrEmpty(model.Comments))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@Comments";
                                x.Value = model.Comments;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Comments changed from " + oldModel.Comments + " to " + model.Comments;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "ProjectType":
                    if (oldModel.ProjectType != model.ProjectType && !string.IsNullOrEmpty(model.ProjectType))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ProjectType";
                                x.Value = model.ProjectType;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Budget Type changed from " + oldModel.ProjectType + " to " + model.ProjectType;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;
                case "ApprovalStatus":
                    
                    string cnSt = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                    using (SqlConnection cn = new SqlConnection(cnSt))
                    {
                        using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter projId = new SqlParameter();
                            projId.ParameterName = "@ProjectId";
                            projId.Value = model.ProjectId;
                            projId.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(projId);

                            SqlParameter x = new SqlParameter();
                            x.ParameterName = "@ApprovalStatus";
                            x.Value = model.ApprovalStatus;
                            x.SqlDbType = SqlDbType.VarChar;
                            cmd.Parameters.Add(x);

                            SqlParameter upby = new SqlParameter();
                            upby.ParameterName = "@updateBy";
                            upby.Value = updateBy;
                            upby.SqlDbType = SqlDbType.VarChar;
                            cmd.Parameters.Add(upby);

                            SqlParameter y = new SqlParameter();
                            y.ParameterName = "@ActualStart";
                            y.Value = oldModel.ActualStart;
                            y.SqlDbType = SqlDbType.DateTime;
                            cmd.Parameters.Add(y);

                            SqlParameter z = new SqlParameter();
                            z.ParameterName = "@ActualEnd";
                            z.Value = oldModel.ActualEnd;
                            z.SqlDbType = SqlDbType.DateTime;
                            cmd.Parameters.Add(z);

                            SqlParameter retVal = new SqlParameter();
                            retVal.Direction = ParameterDirection.ReturnValue;
                            cmd.Parameters.Add(retVal);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();

                            ret = (int)retVal.Value;
                        }
                    }

                    msg = "Project Approval Status changed from " + oldModel.ApprovalStatus + " to " + model.ApprovalStatus;
                    log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    
                    break;

                case "ActivityStatus":
                    if (oldModel.ActivityStatus != model.ActivityStatus && !string.IsNullOrEmpty(model.ActivityStatus))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ActivityStatus";
                                x.Value = model.ActivityStatus;
                                x.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(x);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = model.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = model.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Activity Status changed from " + oldModel.ActivityStatus + " to " + model.ActivityStatus;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;
                case "ProposedStart":
                    if (oldModel.ProposedStart != model.ProposedStart && !string.IsNullOrEmpty(model.ProposedStart.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ProposedStart";
                                x.Value = model.ProposedStart;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ProjectedStart";
                                y.Value = model.ProjectedStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter yy = new SqlParameter();
                                yy.ParameterName = "@ActualStart";
                                yy.Value = oldModel.ActualStart;
                                yy.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(yy);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Projected Start Date changed from " + oldModel.ProjectedStart + " to " + model.ProjectedStart;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "ProposedEnd":
                    if (oldModel.ProposedEnd != model.ProposedEnd && !string.IsNullOrEmpty(model.ProposedEnd.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ProposedEnd";
                                x.Value = model.ProposedEnd;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ProjectedEnd";
                                y.Value = model.ProjectedEnd;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter yy = new SqlParameter();
                                yy.ParameterName = "@ActualStart";
                                yy.Value = oldModel.ActualStart;
                                yy.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(yy);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Projected End Date changed from " + oldModel.ProjectedEnd + " to " + model.ProjectedEnd;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;
                case "ProjectedStart":
                    if (oldModel.ProjectedStart != model.ProjectedStart && !string.IsNullOrEmpty(model.ProjectedStart.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ProjectedStart";
                                x.Value = model.ProjectedStart;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);
                                
                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Projected Start Date changed from " + oldModel.ProjectedStart + " to " + model.ProjectedStart;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "ProjectedEnd":
                    if (oldModel.ProjectedEnd != model.ProjectedEnd && !string.IsNullOrEmpty(model.ProjectedEnd.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ProjectedEnd";
                                x.Value = model.ProjectedEnd;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Projected End Date changed from " + oldModel.ProjectedEnd + " to " + model.ProjectedEnd;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "ActualStart":
                    if (oldModel.ActualStart != model.ActualStart && !string.IsNullOrEmpty(model.ActualStart.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ActualStart";
                                x.Value = model.ActualStart;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualEnd";
                                y.Value = model.ActualEnd;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActivityStatus";
                                z.Value = model.ActivityStatus;
                                z.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(z);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Actual Start Date changed from " + oldModel.ActualStart + " to " + model.ActualStart;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "ActualEnd":
                    if (oldModel.ActualEnd != model.ActualEnd) // && !string.IsNullOrEmpty(model.ActualEnd.ToString()))
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@ActualEnd";
                                x.Value = model.ActualEnd;
                                x.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(x);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = model.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);


                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActivityStatus";
                                z.Value = model.ActivityStatus;
                                z.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(z);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Actual End Date changed from " + oldModel.ActualEnd + " to " + model.ActualEnd;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;
                case "pct_complete":
                    if (oldModel.pct_complete != model.pct_complete )
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@pct_complete";
                                x.Value = model.pct_complete;
                                x.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);


                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Total Percent Complete changed from " + oldModel.pct_complete + " to " + model.pct_complete;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;
                case "InitApproved":
                    if (oldModel.InitApproved != model.InitApproved)
                    {
                        string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                        using (SqlConnection cn = new SqlConnection(cnStr))
                        {
                            using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter projId = new SqlParameter();
                                projId.ParameterName = "@ProjectId";
                                projId.Value = model.ProjectId;
                                projId.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(projId);

                                SqlParameter x = new SqlParameter();
                                x.ParameterName = "@InitApproved";
                                x.Value = model.InitApproved;
                                x.SqlDbType = SqlDbType.Bit;
                                cmd.Parameters.Add(x);

                                SqlParameter upby = new SqlParameter();
                                upby.ParameterName = "@updateBy";
                                upby.Value = updateBy;
                                upby.SqlDbType = SqlDbType.VarChar;
                                cmd.Parameters.Add(upby);

                                SqlParameter y = new SqlParameter();
                                y.ParameterName = "@ActualStart";
                                y.Value = oldModel.ActualStart;
                                y.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(y);

                                SqlParameter z = new SqlParameter();
                                z.ParameterName = "@ActualEnd";
                                z.Value = oldModel.ActualEnd;
                                z.SqlDbType = SqlDbType.DateTime;
                                cmd.Parameters.Add(z);

                                SqlParameter retVal = new SqlParameter();
                                retVal.Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add(retVal);

                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                ret = (int)retVal.Value;
                            }
                        }

                        msg = "Project Total Percent Complete changed from " + oldModel.pct_complete + " to " + model.pct_complete;
                        log = AddChangeLog(model.ProjectId, DateTime.Now, userId, msg, ref newLogId);
                    }
                    break;

                case "warrantyAgreement":
                    if (oldModel.warrantyAgreement != model.warrantyAgreement)
                    { 
                    
                    
                    }

                    break;
            }
            }catch(Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        public int UpdateProjectHealth(int projectId, int health, string updateBy)
        {
            int ret = -1;
            string cnnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(cnnStr))
            {
                using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter projId = new SqlParameter();
                    projId.ParameterName = "@ProjectId";
                    projId.Value = projectId;
                    projId.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(projId);

                    SqlParameter x = new SqlParameter();
                    x.ParameterName = "@Health";
                    x.Value = health;
                    x.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(x);

                    SqlParameter upby = new SqlParameter();
                    upby.ParameterName = "@updateBy";
                    upby.Value = updateBy;
                    upby.SqlDbType = SqlDbType.VarChar;
                    cmd.Parameters.Add(upby);

                    SqlParameter retVal = new SqlParameter();
                    retVal.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(retVal);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();

                    ret = (int)retVal.Value;
                }
            }

            return ret;
        }

        [FunctionAttribute(Name = "probe_DelProjects")]
        public int DeleteProject(int ProjectId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, UpdateBy);
            return ((int)(result.ReturnValue));
        }

        [FunctionAttribute(Name = "probe_UpdProjects")]
        public int UpdProjectRisk(int ProjectId, DateTime? ActualStart, DateTime? ActualEnd, int RiskLikelihood, int RiskImpact, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ActualStart, ActualEnd, RiskLikelihood, RiskImpact, updateBy);
            return (int)result.ReturnValue;
        }
        #endregion

        #region Project Info Updates

        [FunctionAttribute(Name = "probe_AddProjectUpdates")]
        [return: Parameter(DbType = "Int")]
        public int addProjectUpdate(int ProjectId, string Explanation, int UpdateOwner, string updateBy, [Parameter(Name = "ProjectUpdateId", DbType = "Int")] ref System.Nullable<Int32> ProjectUpdateId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, Explanation, UpdateOwner, updateBy, ProjectUpdateId);
            ProjectUpdateId = (System.Nullable<Int32>)result.GetParameterValue(4);
            return (int) result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_UpdProjectUpdates")]
        public int updProjectUpdate(int ProjectUpdateId, string Explanation, int UpdateOwner, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectUpdateId, Explanation, UpdateOwner, updateBy);
            return ((int)(result.ReturnValue));
        }

        [FunctionAttribute(Name = "probe_DelProjectUpdates")]
        public int delProjectUpdate(int ProjectUpdateId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectUpdateId, UpdateBy);
            return ((int)(result.ReturnValue));
        }

        #endregion

        #region Project Info Issues

        [FunctionAttribute(Name = "probe_AddProjectIssues")]
        [return: Parameter(DbType = "Int")]
        public int addProjectIssue(int ProjectId, string Explanation, DateTime TargetDate, string Resolution, string IssueStatus, int IssueOwner, string updateBy, [Parameter(Name = "ProjectIssueId", DbType = "Int")] ref System.Nullable<Int32> ProjectIssueId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, Explanation, TargetDate, Resolution, IssueStatus, IssueOwner, updateBy, ProjectIssueId);
            ProjectIssueId = (System.Nullable<Int32>)result.GetParameterValue(7);
            return (int)result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_UpdProjectIssues")]
        public int updProjectIssue(int ProjectIssueId, int ProjectId, string Explanation, DateTime TargetDate, string Resolution, string IssueStatus, int IssueOwner, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectIssueId, ProjectId, Explanation, TargetDate, Resolution, IssueStatus, IssueOwner, updateBy);
            return ((int)(result.ReturnValue));
        }

        [FunctionAttribute(Name = "probe_DelProjectIssues")]
        public int delProjectIssue(int ProjectIssueId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectIssueId, UpdateBy);
            return ((int)(result.ReturnValue));
        }

        #endregion

        #region Project Info Attachments

        public int addProjectAttachment(int ProjectId, string AttachmentTitle, byte[] AttachmentBinary, string FileType, string updateBy, ref int? newProjectAttachmentId)
        {
            string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(cnStr))
            {
                
                using (SqlCommand cmd = new SqlCommand("probe_AddProjectAttachments", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter projId = new SqlParameter();
                    projId.ParameterName = "@ProjectId";
                    projId.Value = ProjectId;
                    projId.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(projId);

                    SqlParameter title = new SqlParameter();
                    title.ParameterName = "@AttachmentTitle";
                    title.Value = AttachmentTitle;
                    title.SqlDbType = SqlDbType.VarChar;
                    cmd.Parameters.Add(title);

                    SqlParameter attachType = new SqlParameter();
                    attachType.ParameterName = "@AttachmentType";
                    attachType.Value = "Supporting Docs";
                    attachType.SqlDbType = SqlDbType.VarChar;
                    cmd.Parameters.Add(attachType);

                    SqlParameter bin = new SqlParameter();
                    bin.ParameterName = "@AttachmentBinary";
                    bin.Value = AttachmentBinary;
                    bin.SqlDbType = SqlDbType.VarBinary;
                    cmd.Parameters.Add(bin);

                    SqlParameter fType = new SqlParameter();
                    fType.ParameterName = "@FileType";
                    fType.Value = FileType;
                    fType.SqlDbType = SqlDbType.VarChar;
                    cmd.Parameters.Add(fType);

                    SqlParameter upby = new SqlParameter();
                    upby.ParameterName = "@updateBy";
                    upby.Value = updateBy;
                    upby.SqlDbType = SqlDbType.VarChar;
                    cmd.Parameters.Add(upby);

                    SqlParameter newId = new SqlParameter();
                    newId.ParameterName = "@ProjectAttachmentId";
                    newId.SqlDbType = SqlDbType.Int;
                    newId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(newId);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();

                    newProjectAttachmentId = (int)newId.Value;
                }
            }

            return (int)newProjectAttachmentId;
        }

        [FunctionAttribute(Name = "probe_DelProjectAttachments")]
        public int delProjectAttachment(int ProjectAttachmentId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectAttachmentId, UpdateBy);
            return ((int)(result.ReturnValue));
        }

        public IEnumerable<ProjectAttachments> GetProjectAttachments(int projectId)
        {

            var sql = @"SELECT ProjectAttachmentId
                          ,AttachmentTitle
                          ,AttachmentType
                          ,AttachmentBinary
                          ,FileType
                          ,createdDt
                      FROM ProjectAttachments
                      where ProjectId = {0}";

            IEnumerable<ProjectAttachments> results = this.ExecuteQuery<ProjectAttachments>(sql, projectId);
            return results;
        }

        public ProjectAttachments GetSelectedProjectAttachment(int attachmentId)
        {
            var sql = @"SELECT ProjectAttachmentId
                          ,AttachmentTitle
                          ,AttachmentType
                          ,AttachmentBinary
                          ,FileType
                          ,createdDt
                      FROM ProjectAttachments
                      where ProjectAttachmentId = {0}";

            IEnumerable<ProjectAttachments> results = this.ExecuteQuery<ProjectAttachments>(sql, attachmentId);
            return results.FirstOrDefault();
        }

        #endregion

        #region Templates

        public string GetTemplateHTML(string url)
        {
            string html = "";
            string disabletxt = "disabled style='color:gray' ";

            html += "<div id=\"accordion\" class=\"accordion\"><h2>Business Case</h2><div><p><h4>Required Documents</h4></p>";

            List<ProbeTemplates> templates = GetTemplatesBySection("Required", "Business Case");

            foreach(var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Business Case");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }
            //Ends Business Case section
            html += "</div>";

            //Starts Initiation Section
            html += "<h2>Initiation</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Initiation");

            foreach(var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Initiation section
            html += "</div>";

            //Starts Planning
            html += "<h2>Planning</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Planning");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Planning");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Planning section
            html += "</div>";

            //Starts Development
            html += "<h2>Development</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Development");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Development");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Development section
            html += "</div>";

            //Starts Testing
            html += "<h2>Testing</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Testing");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Testing");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Testing section
            html += "</div>";

            //Starts Implementation
            html += "<h2>Implementation</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Implementation");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Implementation");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Implementation section
            html += "</div>";

            //Starts Training
            html += "<h2>Training</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Training");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Training section
            html += "</div>";


            //Starts Closeout
            html += "<h2>Closeout</h2><div><p><h4>Required Documents</h4></p>";

            templates = GetTemplatesBySection("Required", "Closeout");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            html += "<p><h4>Supplemental Documents</h4></p>";

            templates = GetTemplatesBySection("Supplemental", "Closeout");

            foreach (var item in templates)
            {
                disabletxt = "disabled style='color:gray' ";
                if (item.Template.Length > 0)
                {
                    disabletxt = "";
                }
                html += "<p><a " + disabletxt + " href='" + url + "Home/doTemplateDownload?templateFileName=" + item.TemplateFileName + "&templateType=" + item.TemplateType + "'>" + item.TemplateFileName + "</a></p>";
                html += "<p>" + item.TemplateDesc + "</p>";
            }

            //Ends Closeout section
            html += "</div>";

            //Ends Accordion
            html += "</div>";


            return html;
        }
        public List<ProbeTemplates> GetTemplatesBySection(string docType, string tempType)
        { 
            var sql = @"SELECT TemplateFileName
                              ,TemplateDesc
                              ,FileType
                              ,TemplateType
                              ,DocumentType
                              ,Template
                          FROM Templates
                          where DocumentType = {0}
                          and TemplateType = {1} ";

            string[] parms = { docType, tempType };

            IEnumerable<ProbeTemplates> results = this.ExecuteQuery<ProbeTemplates>(sql, parms);            

            return results.ToList();
        }

        public ProbeTemplates GetSelectedTemplate(string fname, string type)
        {
            var sql = @"SELECT TemplateFileName
                              ,TemplateDesc
                              ,FileType
                              ,TemplateType
                              ,DocumentType
                              ,Template
                          FROM Templates
                          where TemplateFileName = {0}
                          and TemplateType = {1} ";

            string [] parms = {fname, type};

            IEnumerable<ProbeTemplates> results = this.ExecuteQuery<ProbeTemplates>(sql, parms);

            return results.FirstOrDefault();
        }

        #endregion

        public ProjectInfo GetProjectInfo(int projectId)
        {
            var sql = @"SELECT ProjectId
                            ,ProjectNumber
                            ,ProjectName
                            ,DisplayName
                            ,BudgetNum
                            ,Priority
                            ,Health
                            ,Scope
                            ,Comments
                            ,Customer
                            ,CarryOver
                            ,ProjectType
                            ,RiskLikelihood
                            ,RiskImpact
                            ,ApprovalStatus
                            ,ActivityStatus
                            ,ProposedStart 
                            ,ProposedEnd 
                            ,ProjectedStart 
                            ,ProjectedEnd
                            ,ActualStart
                            ,ActualEnd 
                            ,pct_complete
                            ,InitApproved
                        FROM Projects
                        where ProjectId = {0}";

            IEnumerable<ProjectInfo> results = this.ExecuteQuery<ProjectInfo>(sql, projectId);
            return results.FirstOrDefault();
        }

        public List<DashboardProject> GetDashboardProjects()
        {
            var sql = @"SELECT p.ProjectId
                                ,p.DisplayName
                                ,p.ApprovalStatus
                                ,p.ActivityStatus
                                ,p.ProjectType
                                ,p.Health
                                ,ProjectedStart as StartDate
                                ,ProjectedEnd as EndDate
                                FROM Projects p
                                WHERE p.ActivityStatus not in ('Completed', 'Cancelled')
                                order by p.DisplayName";

            IEnumerable<DashboardProject> results = this.ExecuteQuery<DashboardProject>(sql);
            return results.ToList();
        }

        [FunctionAttribute(Name = "probe_UpdProjects")]
        public int UpdateProjectRecord(int ProjectId, string ServiceAgreementType, string updateBy,   string Scope, string Comments, string ProjectType, DateTime? ProposedStart, string Customer, DateTime? ProposedEnd, DateTime? ServiceAgreementExpDt, int UserId)
        {
            int g = 7;
            try
            {

                string cnStr = WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(cnStr))
                {
                    using (SqlCommand cmd = new SqlCommand("probe_UpdProjects", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                      
                      
                        //cmd.Parameters.AddWithValue("@Scope", Scope);
                        //cmd.Parameters.AddWithValue("@UserId", UserId);
                        //cmd.Parameters.AddWithValue("@Comments", Comments);
                        //cmd.Parameters.AddWithValue("@Customer", Customer);
                        //cmd.Parameters.AddWithValue("@ProjectType", ProjectType);
                        cmd.Parameters.AddWithValue("@ProposedStart", ProposedStart);
                        cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                        cmd.Parameters.AddWithValue("@ProposedEnd", ProposedEnd);
                        cmd.Parameters.AddWithValue("@updateBy", updateBy);
                        cmd.Parameters.AddWithValue("@ServiceAgreementType", ServiceAgreementType);
                        cmd.Parameters.AddWithValue("@ServiceAgreementExpDt", ServiceAgreementExpDt);
                        try { 
                            cn.Open();
                            int result = cmd.ExecuteNonQuery();
                            cn.Close();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    
                }

               

                //string updateBy = updateBy1;
                //IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ServiceAgreementType, updateBy);
               // return (int)result.ReturnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return g;
        }

        public string GetProjectManager(int projectId)
        {
            try
            {
                string manager = "";
                var sql = @"Select b.UserName 
	                    FROM  ProjectUserRoles a
	                    join Users b
	                    on a.UserId = b.UserId 
                        where ProjectId = {0}
	                    and ProjectRole = 'Manager'";

                IEnumerable<string> results = this.ExecuteQuery<string>(sql, projectId);

                foreach (var x in results)
                {
                    manager = x.ToString();
                }

                return manager;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public NewProj getWarrantyInfo(int projectId)
        {

            var sql = @"Select ServiceAgreementType, ServiceAgreementExpDt from Projects where
ProjectId ={0}";
            try
            {
                IEnumerable<NewProj> results = this.ExecuteQuery<NewProj>(sql, projectId);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

            

        }

        public string GetProjectSponsor(int projectId)
        {
            try
            {
                string sponsor = "";
                var sql = @"Select b.UserName 
	                    FROM  ProjectUserRoles a
	                    join Users b
	                    on a.UserId = b.UserId 
                        where ProjectId = {0}
	                    and ProjectRole = 'Sponsor'";

                IEnumerable<string> results = this.ExecuteQuery<string>(sql, projectId);

                foreach (var x in results)
                {
                    sponsor = x.ToString();
                }

                return sponsor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetProjectEngineer(int projectId)
        {
            try
            {
                string engineer = "";
                var sql = @"Select b.UserName
	                    FROM  ProjectUserRoles a
	                    join Users b
	                    on a.UserId = b.UserId 
                        where ProjectId = {0}
	                    and ProjectRole = 'Engineer'";

                IEnumerable<string> results = this.ExecuteQuery<string>(sql, projectId);

                foreach (var x in results)
                {
                    engineer = x.ToString();
                }

                return engineer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<ProjectInfo> GetEachProjectGroup(int projectId)
        {
            
            try {
                var sql = @"Select GroupId from ProjectGroups where ProjectId = {0}";
                IEnumerable<ProjectInfo> results = this.ExecuteQuery<ProjectInfo>(sql, projectId);
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }

           
            
        }

        public string GetProjectCreator(int projectId)
        {
            try
            {
                var sql = @"Select b.DisplayName 
	                    FROM  ProjectUserRoles a
	                    join Users b
	                    on a.UserId = b.UserId 
                        where ProjectId = {0}
	                    and ProjectRole = 'Creator'";

                IEnumerable<string> results = this.ExecuteQuery<string>(sql, projectId);

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProbeUser> GetListofAvailableContributors(int projectId)
        {
            try
            {
                var sql = @"Select a.UserId,
                                    a.UserName,
		                            a.DisplayName
                            from Users a
                            where a.Active != 0
                            and a.UserId not in (select UserId 
						                            from ProjectUserRoles 
						                            where ProjectId = {0} 
						                            and ProjectRole = 'Contributor') order by DisplayName";

                IEnumerable<ProbeUser> results = this.ExecuteQuery<ProbeUser>(sql, projectId);

                return results.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ProjectInfoBudget getTotalBudgetCost(int projectId)
        {
            try
            {

                var sql = @"SELECT Projected as Projectedtotal,Budgeted as BudgetedTotal,Actual as ActualTotal, CurrentBudgetAmount as tot FROM ProjectBudgetsDetailsWTotals_v 
                            WHERE ProjectId = {0} and BudgetType='Totals' ";
                IEnumerable<ProjectInfoBudget> results = this.ExecuteQuery<ProjectInfoBudget>(sql, projectId);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProjectInfoBudget GetProjectInfoBudget(int projectId)
        {
            var sql = @"SELECT BudgetedTotal
                                ,ProjectedTotal
                                ,ActualTotal
	                            ,ProposedTotal
	                            ,ProposedCapital
	                            ,ProposedExpense
                                ,ProposedExpenseEP
                                
                        FROM
                        (SELECT ProjectId
                                ,CurrentBudgetAmount as BudgetedTotal
                                ,Projected as ProjectedTotal
                                ,Actual as ActualTotal
                                
                            FROM ProjectBudgetsDetailsWTotals_v
                            WHERE BudgetType = 'Totals'
                            AND ProjectId = {0})sub1,
                        (SELECT SUM(CASE WHEN BudgetStatus = 'Proposed' and BudgetType in ('Capital', 'Expense', 'Expense E to P') THEN Amount END) ProposedTotal
		                        ,SUM(CASE WHEN BudgetStatus = 'Proposed' AND BudgetType = 'Capital' THEN Amount END) ProposedCapital
		                        ,SUM(CASE WHEN BudgetStatus = 'Proposed' AND BudgetType = 'Expense' THEN Amount END) ProposedExpense
                                ,SUM(CASE WHEN BudgetStatus = 'Proposed' AND BudgetType = 'Expense E to P' THEN Amount END) ProposedExpenseEP
                            FROM ProjectBudgets
                            WHERE ProjectId = {0})sub2";

            IEnumerable<ProjectInfoBudget> results = this.ExecuteQuery<ProjectInfoBudget>(sql, projectId);
            return results.FirstOrDefault();
        }



        public Group GetProjectGroup(int projectId)
        {
            try
            {
                var sql = @"Select a.GroupId,
		                         b.GroupName
                          FROM ProjectGroups a
                          join Groups b
                          on a.GroupId = b.GroupId
                          where ProjectId = {0}";

                IEnumerable<Group> results = this.ExecuteQuery<Group>(sql, projectId);

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region Project Impacts

        public List<ProjectImpact> GetProjectImpacts(int ProjectId)
        {
            try
            {
                var sql = @"SELECT a.ImpactId
                            ,ImpactName
                            ,OtherImpactDesc
                            ,CASE b.ImpactId WHEN a.ImpactId THEN 1 ELSE 0 END as Selected
                            FROM Impacts a 
                            left join ProjectImpacts b
                            on a.ImpactId = b.ImpactId
                            and ProjectId = {0}
                            order by ImpactId";
                IEnumerable<ProjectImpact> results = this.ExecuteQuery<ProjectImpact>(sql, ProjectId);
                return results.ToList<ProjectImpact>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddProjectImpacts")]
        public int AddProjectImpact(int ProjectId, int ImpactId, string OtherImpactDesc, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ImpactId, OtherImpactDesc, updateBy);
            return (int)result.ReturnValue;
        }

        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdProjectImpacts")]
        public int UpdProjectImpact(int ProjectId, int ImpactId, string OtherImpactDesc, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ImpactId, OtherImpactDesc, updateBy);
            return (int)result.ReturnValue;
        }

        //DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelProjectImpacts")]
        public int DelProjectImpact(int ProjectId, int ImpactId, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ImpactId, updateBy);
            return (int)result.ReturnValue;
        }

        #endregion

        #region Benefits

        public List<ProjectBenefit> GetProjectBenefits(int ProjectId)
        {
            try
            {
                var sql = @"SELECT a.BenefitId,
                            a.BenefitName,
                            b.BenefitGroupName,
                            b.BenefitGroupOrder,
                            CASE c.BenefitId WHEN a.BenefitId THEN 1 ELSE 0 END as Selected
                            FROM Benefits a
                            JOIN BenefitGroups b
                            ON a.BenefitGroupId = b.BenefitGroupId
                            LEFT JOIN ProjectBenefits c
                            ON a.BenefitId = c.BenefitId
                            and ProjectId = {0}
                            ORDER BY b.BenefitGroupName";
                IEnumerable<ProjectBenefit> results = this.ExecuteQuery<ProjectBenefit>(sql, ProjectId);
                return results.ToList<ProjectBenefit>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetBenefitGroups()
        {
            try
            {
                var sql = @"SELECT BenefitGroupName
                            FROM dbo.BenefitGroups
                            order by BenefitGroupOrder";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProjectCostSaving> GetProjectSavings(int ProjectId)
        {
            try
            {
                var sql = @"SELECT a.CostSavingsTypeId
                            ,a.CostSavingsType
                            ,CASE b.CostSavingsTypeId WHEN a.CostSavingsTypeId THEN CostSavings ELSE 0 END as CostSaving
                            from CostSavingsTypes a
                            left join ProjectCostSavings b
                            on a.CostSavingsTypeId = b.CostSavingsTypeId
                            and ProjectId = {0}";
                IEnumerable<ProjectCostSaving> results = this.ExecuteQuery<ProjectCostSaving>(sql, ProjectId);
                return results.ToList<ProjectCostSaving>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddProjectBenefits")]
        public int AddProjectBenefit(int ProjectId, int BenefitId, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, BenefitId, updateBy);
            return (int)result.ReturnValue;
        }

        //DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelProjectBenefits")]
        public int DelProjectBenefit(int ProjectId, int BenefitId, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, BenefitId, updateBy);
            return (int)result.ReturnValue;
        }

        //UPDATE PROJECT COST SAVINGS~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdProjectCostSavings")]
        public int UpdProjectCostSavings(int ProjectId, int CostSavingsTypeId, decimal CostSavings, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, CostSavingsTypeId, CostSavings, updateBy);
            return (int)result.ReturnValue;
        }

        #endregion

        #region Budget

        public List<BudgetRecord> GetProposedBudgetRecords(int ProjectId)
        {
            try
            {
                var sql = @"SELECT ProjectId, BudgetType,BudgetStatus,Amount,Year,Month,DateName( month , DateAdd( month , month , 0 ) - 1 ) as MonthName
                            FROM dbo.ProjectBudgets
                            where ProjectId = {0}
                            and BudgetStatus = 'proposed'
                            order by BudgetType, BudgetStatus, Year, Month";
                IEnumerable<BudgetRecord> results = this.ExecuteQuery<BudgetRecord>(sql, ProjectId);
                return results.ToList<BudgetRecord>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BudgetRecord> GetAllBudgetRecords(int ProjectId)
        {
            try
            {
                var sql = @"SELECT ProjectId, BudgetType,BudgetStatus,Amount,Year,Month,DateName( month , DateAdd( month , month , 0 ) - 1 ) as MonthName
                            FROM dbo.ProjectBudgets
                            where ProjectId = {0}
                            and BudgetStatus in ('Budgeted', 'Projected', 'Actual')
                            order by BudgetType, BudgetStatus, Year, Month";
                IEnumerable<BudgetRecord> results = this.ExecuteQuery<BudgetRecord>(sql, ProjectId);
                return results.ToList<BudgetRecord>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OrigApprovedBudget> GetOrigApprovedBudgetRecords(int ProjectId)
        {
            try
            {
                var sql = @"select BudgetType, sum(Amount) as Total
                            from ProjectBudgets 
                            where ProjectId = {0} and BudgetStatus = 'Proposed'
                            group by BudgetType";
                IEnumerable<OrigApprovedBudget> results = this.ExecuteQuery<OrigApprovedBudget>(sql, ProjectId);
                return results.ToList<OrigApprovedBudget>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<string> GetServiceAgreementTypes()
        {
            try
            {
                string sql = @"SELECT * from ServiceAgreementTypes";
                IEnumerable<string> results = this.ExecuteQuery<string>(sql);
                return results.ToList<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BudgetViewRecord> GetBudgetsDetailsWTotalsRecords(int ProjectId)
        {
            try
            {
                var sql = @"select BudgetType,Budgeted,Projected,Actual,CurrentBudgetAmount
                            from ProjectBudgetsDetailsWTotals_v
                            where ProjectId = {0}
                            order by order_by, BudgetType";
                IEnumerable<BudgetViewRecord> results = this.ExecuteQuery<BudgetViewRecord>(sql, ProjectId);
                return results.ToList<BudgetViewRecord>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FunctionAttribute(Name = "probe_UpdProjectBudgets")]
        public int UpdProjectBudgets(int ProjectId, string BudgetType, string BudgetStatus, decimal Amount, int Year, int Month, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, BudgetType, BudgetStatus, Amount, Year, Month, updateBy);
            return (int)result.ReturnValue;
        }

        [FunctionAttribute(Name = "probe_UpdProjectCurrentBudgets")]
        public int UpdProjectCurrentBudgets(int ProjectId, string BudgetType, decimal CurrentBudgetAmount, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, BudgetType, CurrentBudgetAmount, updateBy);
            return (int)result.ReturnValue;
        }

        #endregion

        #region Risk

        //[FunctionAttribute(Name = "probe_UpdProjects")]
        //public int UpdProjectRisk(int ProjectId, int RiskLikelihood, int RiskImpact, string updateBy)
        //{
        //    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, RiskLikelihood, RiskImpact, updateBy);
        //    return (int)result.ReturnValue;
        //}

        #endregion

        #region Project Resources

        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdProjectResources")]
        public int UpdProjectResource(int ProjectId, int ResourceId, int Year,int Month, decimal ProposedManHrs, decimal ActualManHrs, string Comments, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ResourceId, Year, Month, ProposedManHrs, ActualManHrs, Comments, updateBy);
            return (int)result.ReturnValue;
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddProjectResources")]
        public int AddProjectResource(int ProjectId, int ResourceId, int Year, int Month,decimal ProposedManHrs, decimal ActualManHrs, string Comments, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ResourceId, Year, Month, ProposedManHrs, ActualManHrs, Comments, updateBy);
            return (int)result.ReturnValue;
        }
        //DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelProjectResources")]
        public int DelProjectResource(int ProjectId, int ResourceId,string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectId, ResourceId,UpdateBy);
            return (int)result.ReturnValue;
        }

        #endregion

        #region Resources Admin

        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdResourceManHours")]
        public int UpdResourceManHours(int ResourceId, int Year,int Month, decimal BaseManHours, decimal CoreManHours, string Comments, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ResourceId, Year,Month, BaseManHours, CoreManHours, Comments, updateBy);
            return (int)result.ReturnValue;
        }

        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdResources")]
        public int UpdResources(int ResourceId, string ResourceName, string ResourceTitle, string ResourceEmail, string ResourcePhone, string Comments, int EntityId, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ResourceId, ResourceName, ResourceTitle, ResourceEmail, ResourcePhone, Comments, EntityId, updateBy);
            return (int)result.ReturnValue;
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddResources")]
        public int AddResources(string  ResourceName,string ResourceTitle,string ResourceEmail,String ResourcePhone, string Comments, int EntityId, string updateBy,ref int ResourceId)
        {
            
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ResourceName, ResourceTitle, ResourceEmail, ResourcePhone, Comments, EntityId, updateBy, ResourceId);
            return (int)result.ReturnValue;
        }

        ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelResources")]
        public int DelResources(int ResourceId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ResourceId, UpdateBy);
            return (int)result.ReturnValue;
        }

        #endregion


        #region Entities Admin

        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdEntities")]
        public int UpdEntities(int EntityId, string EntityName, string EntityType, int ParentId, string Notes, int UserId, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), EntityId, EntityName, EntityType, ParentId, Notes, UserId, updateBy);
            return (int)result.ReturnValue;
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddEntities")]
        public int AddEntities(string EntityName, string EntityType, int ParentId, string Notes,  int UserId, string updateBy, ref int EntityId)
        {

            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), EntityName, EntityType, ParentId, Notes, UserId, updateBy, EntityId);
            return (int)result.ReturnValue;
        }

        ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelEntities")]
        public int DelEntities(int EntityId, string UpdateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), EntityId, UpdateBy);
            return (int)result.ReturnValue;
        }

        #endregion


        #region User Has Edit
        public bool UserHasEdit(int ProjId,ProbeUser usr)
        {
            bool hasEdit = false;

            if (usr.IsAdmin)
            {
                hasEdit = true;
            }

            if (!hasEdit)
            {
                try
                {
                    ProbeData data = ProbeDataContext.GetDataContext();

                    ProjectInfo pinfo = data.GetProjectInfo(ProjId);
                    usr.ProjectRoleList = data.GetProjectRolesForUser(ProjId, usr.UserName);

                    if (pinfo.ApprovalStatus == "Proposed")
                    {
                        if(pinfo.ActivityStatus.ToLower() == "pending")
                        {
                            if(usr.ProjectRoleList.Contains("Creator") ||
                                usr.ProjectRoleList.Contains("Sponsor") ||
                                usr.ProjectRoleList.Contains("Manager") ||
                                usr.ProjectRoleList.Contains("Engineer") 
                                )
                            {
                                hasEdit = true;
                            }
                        }
                    }

                    if(pinfo.ApprovalStatus == "Approved")
                    {
                        if(!pinfo.InitApproved)
                        {
                            if(usr.ProjectRoleList.Contains("Sponsor"))
                            {
                                hasEdit = true;
                            }
                        }
                        else
                        {
                            if(pinfo.ActivityStatus.ToLower() == "not started" || pinfo.ActivityStatus.ToLower() == "in progress" || pinfo.ActivityStatus.ToLower() == "on hold")
                            {
                               if(usr.ProjectRoleList.Contains("Sponsor") ||
                                    usr.ProjectRoleList.Contains("Manager") ||
                                    usr.ProjectRoleList.Contains("Engineer") 
                                    )
                                {
                                    hasEdit = true;
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

            

            return hasEdit;
        }
        public bool UserHasResourcesEdit(ProbeUser usr)
        {
            bool hasEdit = false;

            if (usr.IsAdmin)
            {
                hasEdit = true;
            }

            if (!hasEdit)
            {
                try
                {

                    var udb = Database.Open("probedb");
                    var usrEnts = udb.Query("select UserId,EntityId,ParentId from Entities where UserId = " + usr.UserId);
                    if (usrEnts.Count() > 0)
                    {
                        hasEdit = true;
                    }                    

                }
                catch (Exception ex)
                {

                }
            }



            return hasEdit;
        }

        #endregion


        #region AdminUser
        //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_UpdUsers")]
        public int UpdUsers(int UserId, string UserName, string Password, string DisplayName, string Email, string Phone, string FirstName,string LastName,string MiddleInitial,int? ResourceId,string LastActivityDt,int Active, string updateBy)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), UserId, UserName, Password,DisplayName, Email, Phone, FirstName,LastName,MiddleInitial, ResourceId, LastActivityDt,Active, updateBy);
            return (int)result.ReturnValue;
        }

        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddUsers")]
        public int AddUsers(string UserName, string Password, string DisplayName, string Email, string Phone, string FirstName,string LastName,string MiddleInitial,int? ResourceId,string LastActivityDt,int Active, string updateBy,ref int UserId)
        {
            
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), UserName, Password, DisplayName, Email, Phone, FirstName, LastName, MiddleInitial, ResourceId, LastActivityDt, Active, updateBy, UserId);
            return (int)result.ReturnValue;
        }

        ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelUsers")]
        public int DelUsers(int UserId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), UserId);
            return (int)result.ReturnValue;
        }
        #endregion

        #region User Roles
        //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_AddUserRoles")]
        public int AddUserRoles(int UserId,int RoleId, string updateBy)
        {

            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), UserId, RoleId, updateBy);
            return (int)result.ReturnValue;
        }
        ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [FunctionAttribute(Name = "probe_DelUserRoles")]
        public int DelUserRoles(int UserId, int RoleId)
        {

            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), UserId, RoleId);
            return (int)result.ReturnValue;
        }
        #endregion

        #region Templates
            //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [FunctionAttribute(Name = "probe_UpdTemplates")]
            public int UpdTemplates(string TemplateFileName, string TemplateDesc, string FileType, string TemplateType, string DocumentType, byte[] Template, string updateBy)
            {
                IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), TemplateFileName,  TemplateDesc,  FileType,  TemplateType,  DocumentType, Template,  updateBy);
                return (int)result.ReturnValue;
            }

            //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [FunctionAttribute(Name = "probe_AddTemplates")]
            public int AddTemplates(string TemplateFileName, string TemplateDesc, string FileType, string TemplateType, string DocumentType, byte[] Template, string updateBy)
            {

                IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), TemplateFileName, TemplateDesc, FileType, TemplateType, DocumentType, Template, updateBy);
                return (int)result.ReturnValue;
            }

            ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [FunctionAttribute(Name = "probe_DelTemplates")]
            public int DelTemplates(string TemplateFileName,string TemplateType,string UpdateBy)
            {
                IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), TemplateFileName,  TemplateType,UpdateBy);
                return (int)result.ReturnValue;
            }
        #endregion

        #region Table Maintenance Admin

        
            #region ApprovalActivityStatuses
                public IEnumerable<ApprovalActivityStatuses> GetApprovalActivityStatusesData()
                {
                    try
                    {
                        var sql = @"SELECT [ApprovalStatus]
                              ,[ActivityStatus]
                              ,[ReportDefault]
                          FROM [ApprovalActivityStatuses] ";                       

                        IEnumerable<ApprovalActivityStatuses> results = this.ExecuteQuery<ApprovalActivityStatuses>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetApprovalActivityStatusesData method. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public ApprovalActivityStatuses GetApprovalActivityStatusesRecord(ApprovalActivityStatuses recobj)
                {
                    try
                    {
                        var sql = @"SELECT [ApprovalStatus]
                              ,[ActivityStatus]
                              ,[ReportDefault]
                          FROM [ApprovalActivityStatuses] ";
                        sql += " where ApprovalStatus='" + recobj.ApprovalStatus + "' and ActivityStatus='" + recobj.ActivityStatus + "'";


                        IEnumerable<ApprovalActivityStatuses> results = this.ExecuteQuery<ApprovalActivityStatuses>(sql, "");
                        return results.FirstOrDefault<ApprovalActivityStatuses>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetApprovalActivityStatusesRecord method. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public IEnumerable<ApprovalActivityStatuses> GetApprovalStatusesList()
                {
                    try
                    {
                        var sql = @"SELECT [ApprovalStatus]
                          FROM [ApprovalStatuses] ";

                        IEnumerable<ApprovalActivityStatuses> results = this.ExecuteQuery<ApprovalActivityStatuses>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetApprovalActivityStatusesData method. : " + ex.ToString()));
                        throw ex;
                    }
                }

                public IEnumerable<ApprovalActivityStatuses> GetActivityStatusesList()
                {
                    try
                    {
                        var sql = @"SELECT [ActivityStatus]
                          FROM [ActivityStatuses] ";

                        IEnumerable<ApprovalActivityStatuses> results = this.ExecuteQuery<ApprovalActivityStatuses>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetApprovalActivityStatusesData method. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdApprovalActivityStatuses")]
                public int UpdApprovalActivityStatuses(string ApprovalStatus, string ActivityStatus, string ReportDefault,string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ApprovalStatus, ActivityStatus, ReportDefault,updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddApprovalActivityStatuses")]
                public int AddApprovalActivityStatuses(string ApprovalStatus, string ActivityStatus, string ReportDefault, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ApprovalStatus, ActivityStatus, ReportDefault, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelApprovalActivityStatuses")]
                public int DelApprovalActivityStatuses(string ApprovalStatus, string ActivityStatus, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ApprovalStatus,ActivityStatus, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region BenefitGroups
                public IEnumerable<BenefitGroups> GetBenefitGroupsData()
                {
                    try
                    {
                        var sql = @"SELECT [BenefitGroupId]
                              ,[BenefitGroupName]
                              ,[BenefitGroupOrder]
                          FROM [BenefitGroups] ";
                        
                        IEnumerable<BenefitGroups> results = this.ExecuteQuery<BenefitGroups>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBenefitGroupsData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public BenefitGroups GetBenefitGroupsRecord(BenefitGroups recobj)
                {
                    try
                    {
                        var sql = @"SELECT [BenefitGroupId]
                              ,[BenefitGroupName]
                              ,[BenefitGroupOrder]
                          FROM [BenefitGroups] ";
                        sql += " where BenefitGroupId=" + recobj.BenefitGroupId;


                        IEnumerable<BenefitGroups> results = this.ExecuteQuery<BenefitGroups>(sql, "");
                        return results.FirstOrDefault<BenefitGroups>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBenefitGroupsRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdBenefitGroups")]
                public int UpdBenefitGroups(int BenefitGroupId, string BenefitGroupName, int BenefitGroupOrder,string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BenefitGroupId, BenefitGroupName, BenefitGroupOrder, updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddBenefitGroups")]
                public int AddBenefitGroups(string BenefitGroupName, int BenefitGroupOrder, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),  BenefitGroupName,  BenefitGroupOrder, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelBenefitGroups")]
                public int DelBenefitGroups(int BenefitGroupId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BenefitGroupId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region Benefits
                public IEnumerable<Benefits> GetBenefitsData()
                {
                    try
                    {
                        var sql = @"SELECT [BenefitId]
                              ,[BenefitName]
                              ,[BenefitGroupId]
                          FROM [Benefits] ";

                        IEnumerable<Benefits> results = this.ExecuteQuery<Benefits>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBenefitsData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public Benefits GetBenefitsRecord(Benefits recobj)
                {
                    try
                    {
                        var sql = @"SELECT [BenefitId]
                              ,[BenefitName]
                              ,[BenefitGroupId]
                          FROM [Benefits] ";
                        sql += " where BenefitId=" + recobj.BenefitId;


                        IEnumerable<Benefits> results = this.ExecuteQuery<Benefits>(sql, "");
                        return results.FirstOrDefault<Benefits>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBenefitsRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdBenefits")]
                public int UpdBenefits(int BenefitId, string BenefitName, int BenefitGroupId, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),  BenefitId,  BenefitName,  BenefitGroupId, updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddBenefits")]
                public int AddBenefits(string BenefitName, int BenefitGroupId, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BenefitName, BenefitGroupId, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelBenefits")]
                public int DelBenefits(int BenefitId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BenefitId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region BudgetStatuses
                public IEnumerable<BudgetStatuses> GetBudgetStatusesData()
                {
                    try
                    {
                        var sql = @"SELECT [BudgetStatus]
                          FROM [BudgetStatuses] ";

                        IEnumerable<BudgetStatuses> results = this.ExecuteQuery<BudgetStatuses>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBudgetStatusesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public BudgetStatuses GetBudgetStatusesRecord(BudgetStatuses recobj)
                {
                    try
                    {
                        var sql = @"SELECT [BudgetStatus]
                          FROM [BudgetStatuses] ";
                        sql += " where BudgetStatus='" + recobj.BudgetStatus + "'";


                        IEnumerable<BudgetStatuses> results = this.ExecuteQuery<BudgetStatuses>(sql, "");
                        return results.FirstOrDefault<BudgetStatuses>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBudgetStatusesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddBudgetStatuses")]
                public int AddBudgetStatuses(string BudgetStatus, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BudgetStatus, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelBudgetStatuses")]
                public int DelBudgetStatuses(string BudgetStatus, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BudgetStatus, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region BudgetTypes
                public IEnumerable<BudgetTypes> GetBudgetTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [BudgetType]
                          FROM [BudgetTypes] ";

                        IEnumerable<BudgetTypes> results = this.ExecuteQuery<BudgetTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBudgetTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public BudgetTypes GetBudgetTypesRecord(BudgetTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [BudgetType]
                          FROM [BudgetTypes] ";
                        sql += " where BudgetType='" + recobj.BudgetType + "'";


                        IEnumerable<BudgetTypes> results = this.ExecuteQuery<BudgetTypes>(sql, "");
                        return results.FirstOrDefault<BudgetTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetBudgetTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddBudgetTypes")]
                public int AddBudgetTypes(string BudgetType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BudgetType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelBudgetTypes")]
                public int DelBudgetTypes(string BudgetType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), BudgetType, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region CostSavingsTypes
                public IEnumerable<CostSavingsTypes> GetCostSavingsTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [CostSavingsTypeId]
                                    ,[CostSavingsType]
                          FROM [CostSavingsTypes] ";

                        IEnumerable<CostSavingsTypes> results = this.ExecuteQuery<CostSavingsTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetCostSavingsTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public CostSavingsTypes GetCostSavingsTypesRecord(CostSavingsTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [CostSavingsTypeId]
                                    ,[CostSavingsType]
                          FROM [CostSavingsTypes] ";
                        sql += " where CostSavingsTypeId=" + recobj.CostSavingsTypeId;


                        IEnumerable<CostSavingsTypes> results = this.ExecuteQuery<CostSavingsTypes>(sql, "");
                        return results.FirstOrDefault<CostSavingsTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetCostSavingsTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdCostSavingsTypes")]
                public int UpdCostSavingsTypes(int CostSavingsTypeId, string CostSavingsType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), CostSavingsTypeId, CostSavingsType, updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddCostSavingsTypes")]
                public int AddCostSavingsTypes(string CostSavingsType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), CostSavingsType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelCostSavingsTypes")]
                public int DelCostSavingsTypes(int CostSavingsTypeId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), CostSavingsTypeId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region DocumentTypes
                public IEnumerable<DocumentTypes> GetDocumentTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [DocumentType]
                          FROM [DocumentTypes] ";

                        IEnumerable<DocumentTypes> results = this.ExecuteQuery<DocumentTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetDocumentTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public DocumentTypes GetDocumentTypesRecord(DocumentTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [DocumentType]
                          FROM [DocumentTypes] ";
                        sql += " where DocumentType ='" + recobj.DocumentType + "'";


                        IEnumerable<DocumentTypes> results = this.ExecuteQuery<DocumentTypes>(sql, "");
                        return results.FirstOrDefault<DocumentTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetDocumentTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddDocumentTypes")]
                public int AddDocumentTypes(string DocumentType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), DocumentType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelDocumentTypes")]
                public int DelDocumentTypes(string DocumentType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), DocumentType, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region EntityTypes
                public IEnumerable<EntityTypes> GetEntityTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [EntityType]
                          FROM [EntityTypes] ";

                        IEnumerable<EntityTypes> results = this.ExecuteQuery<EntityTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetEntityTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public EntityTypes GetEntityTypesRecord(EntityTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [EntityType]
                          FROM [EntityTypes] ";
                        sql += " where EntityType ='" + recobj.EntityType + "'";


                        IEnumerable<EntityTypes> results = this.ExecuteQuery<EntityTypes>(sql, "");
                        return results.FirstOrDefault<EntityTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetEntityTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddEntityTypes")]
                public int AddEntityTypes(string EntityType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), EntityType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelEntityTypes")]
                public int DelEntityTypes(string EntityType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), EntityType, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region Groups
                public IEnumerable<Groups> GetGroupsData()
                {
                    try
                    {
                        var sql = @"SELECT [GroupId]
                            ,[GroupName]
                          FROM [Groups] ";

                        IEnumerable<Groups> results = this.ExecuteQuery<Groups>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetGroupsData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public Groups GetGroupsRecord(Groups recobj)
                {
                    try
                    {
                        var sql = @"SELECT [GroupId]
                            ,[GroupName]
                          FROM [Groups] ";
                        sql += " where GroupId =" + recobj.GroupId;


                        IEnumerable<Groups> results = this.ExecuteQuery<Groups>(sql, "");
                        return results.FirstOrDefault<Groups>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetGroupsRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdGroups")]
                public int UpdGroups(int GroupId, string GroupName, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), GroupId, GroupName, updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddGroups")]
                public int AddGroups(string GroupName, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), GroupName, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelGroups")]
                public int DelGroups(int GroupId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), GroupId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region Impacts
                public IEnumerable<Impacts> GetImpactsData()
                {
                    try
                    {
                        var sql = @"SELECT [ImpactId]
                            ,[ImpactName]
                            ,[ImpactDesc]
                          FROM [Impacts] ";

                        IEnumerable<Impacts> results = this.ExecuteQuery<Impacts>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetImpactsData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public Impacts GetImpactsRecord(Impacts recobj)
                {
                    try
                    {
                        var sql = @"SELECT [ImpactId]
                            ,[ImpactName]
                            ,[ImpactDesc]
                          FROM [Impacts] ";
                        sql += " where ImpactId =" + recobj.ImpactId;


                        IEnumerable<Impacts> results = this.ExecuteQuery<Impacts>(sql, "");
                        return results.FirstOrDefault<Impacts>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetImpactsRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdImpacts")]
                public int UpdImpacts(int ImpactId, string ImpactName, string ImpactDesc, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ImpactId, ImpactName, ImpactDesc, updateBy);
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddImpacts")]
                public int AddImpacts(string ImpactName, string ImpactDesc, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ImpactName, ImpactDesc, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelImpacts")]
                public int DelImpacts(int ImpactId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ImpactId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region IssueStatuses
                public IEnumerable<IssueStatuses> GetIssueStatusesData()
                {
                    try
                    {
                        var sql = @"SELECT [IssueStatus]
                          FROM [IssueStatuses] ";

                        IEnumerable<IssueStatuses> results = this.ExecuteQuery<IssueStatuses>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetIssueStatusesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public IssueStatuses GetIssueStatusesRecord(IssueStatuses recobj)
                {
                    try
                    {
                        var sql = @"SELECT [IssueStatus]
                          FROM [IssueStatuses] ";
                        sql += " where IssueStatus = '" + recobj.IssueStatus + "'";


                        IEnumerable<IssueStatuses> results = this.ExecuteQuery<IssueStatuses>(sql, "");
                        return results.FirstOrDefault<IssueStatuses>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetIssueStatusesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddIssueStatuses")]
                public int AddIssueStatuses(string IssueStatus, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), IssueStatus, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelIssueStatuses")]
                public int DelIssueStatuses(string IssueStatus, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), IssueStatus, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region Phases
                public IEnumerable<Phases> GetPhasesData()
                {
                    try
                    {
                        var sql = @"SELECT [PhaseId]
                            ,[PhaseName]
                            ,[PhaseDesc]
                            ,[PhaseOrder]
                            ,[color]
                            ,[weeks_duration]
                          FROM [Phases] ";

                        IEnumerable<Phases> results = this.ExecuteQuery<Phases>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetPhasesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public Phases GetPhasesRecord(Phases recobj)
                {
                    try
                    {
                        var sql = @"SELECT [PhaseId]
                            ,[PhaseName]
                            ,[PhaseDesc]
                            ,[PhaseOrder]
                            ,[color]
                            ,[weeks_duration]
                          FROM [Phases] ";
                        sql += " where PhaseId = " + recobj.PhaseId;


                        IEnumerable<Phases> results = this.ExecuteQuery<Phases>(sql, "");
                        return results.FirstOrDefault<Phases>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetPhasesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }
                //UPDATE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_UpdPhases")]
                public int UpdPhases(int PhaseId, string PhaseName, string PhaseDesc, int PhaseOrder, string color, Int16 weeks_duration, string updateBy) 
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),  PhaseId,  PhaseName,  PhaseDesc,  PhaseOrder,  color,  weeks_duration, updateBy) ;
                    return (int)result.ReturnValue;
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddPhases")]
                public int AddPhases(string PhaseName, string PhaseDesc, int PhaseOrder, string color, Int16 weeks_duration, string updateBy) 
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), PhaseName, PhaseDesc, PhaseOrder, color, weeks_duration, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelPhases")]
                public int DelPhases(int PhaseId, string UpdateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), PhaseId, UpdateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region ProjectTypes
                public IEnumerable<ProjectTypes> GetProjectTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [ProjectType]
                          FROM [ProjectTypes] ";

                        IEnumerable<ProjectTypes> results = this.ExecuteQuery<ProjectTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetProjectTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public ProjectTypes GetProjectTypesRecord(ProjectTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [ProjectType]
                          FROM [ProjectTypes] ";
                        sql += " where ProjectType = '" + recobj.ProjectType +"'";


                        IEnumerable<ProjectTypes> results = this.ExecuteQuery<ProjectTypes>(sql, "");
                        return results.FirstOrDefault<ProjectTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetProjectTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }

                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddProjectTypes")]
                public int AddProjectTypes(string ProjectType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelProjectTypes")]
                public int DelProjectTypes(string ProjectType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ProjectType, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion
            #region TemplateTypes
                public IEnumerable<TemplateTypes> GetTemplateTypesData()
                {
                    try
                    {
                        var sql = @"SELECT [TemplateType]
                          FROM [TemplateTypes] ";

                        IEnumerable<TemplateTypes> results = this.ExecuteQuery<TemplateTypes>(sql, "");
                        return results;
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetTemplateTypesData. : " + ex.ToString()));
                        throw ex;
                    }
                }
                public TemplateTypes GetTemplateTypesRecord(TemplateTypes recobj)
                {
                    try
                    {
                        var sql = @"SELECT [TemplateType]
                          FROM [TemplateTypes] ";
                        sql += " where TemplateType = '" + recobj.TemplateType + "'";


                        IEnumerable<TemplateTypes> results = this.ExecuteQuery<TemplateTypes>(sql, "");
                        return results.FirstOrDefault<TemplateTypes>();
                    }
                    catch (Exception ex)
                    {
                        //logEntry(new Logging.log("Error", "GetTemplateTypesRecord. : " + ex.ToString()));
                        throw ex;
                    }
                }


                //INSERT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_AddTemplateTypes")]
                public int AddTemplateTypes(string TemplateType, string updateBy)
                {

                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), TemplateType, updateBy);
                    return (int)result.ReturnValue;
                }

                ////DELETE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                [FunctionAttribute(Name = "probe_DelTemplateTypes")]
                public int DelTemplateTypes(string TemplateType, string updateBy)
                {
                    IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), TemplateType, updateBy);
                    return (int)result.ReturnValue;
                }
            #endregion

        #endregion

    }
}