using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Services;
using Probe.Models;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using WebMatrix.Data;
using System.Data.Linq;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Globalization;
using System.Web.Script.Serialization;




namespace Probe.Controllers
{

    public class HomeController : Controller
    {
        ProbeData data = ProbeDataContext.GetDataContext();
        ProbeUser usr = null;
        int? logId = -1;

        public List<int> NewProj;
        public ActionResult GuestUser()
        {

            GuestUser gusr = new GuestUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            return View(gusr);

        }


        [HttpPost]
        public JsonResult UpdateProjectApprovalStatus(BudgetRecord mod)
        {
            string username = User.Identity.Name;
            username = username.Substring(username.IndexOf("\\") + 1);

            int t = data.roleBackProjectBudgets(mod.ProjectId, "Y", username);
            //int b = data.roleBackProjectPhases(mod.ProjectId, 5, "Y", username);
            int d = data.roleBackApprovalStatus(mod.ProjectId, "Proposed", "Pending", null, null, username);

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult GuestUser(GuestUser gusr = null)
        {
            try
            {
                ViewBag.AddMessage = "";
                ViewBag.MsgClass = "RedMessage";
                ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
                List<int> ret = new List<int>();
                if (ModelState.IsValid)
                {
                    ProbeUser usr = new ProbeUser();
                    usr.UserName = User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1);
                    usr.DisplayName = gusr.FirstName + " " + gusr.LastName;
                    usr.Phone = gusr.Extension;
                    usr.Email = gusr.Email;
                    int retUserId = -1;
                    data.AddUsers(usr.UserName, "", usr.DisplayName, usr.Email, usr.Phone, gusr.FirstName, gusr.LastName, "", null, "", 1, "Guest User Registration", ref retUserId);

                    MailMessage mail = new MailMessage("dewane.daley@gasoc.com", "dewane.daley@gasoc.com");
                    SmtpClient client = new SmtpClient();
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "mailrelay.gafoc.com";
                    mail.Subject = "PROBE Guest User Registration";
                    mail.IsBodyHtml = true;
                    string bodytxt = @"<html><body><h3>The following user has registered as a new PROBE user.</h3><table>";
                    bodytxt += "<tr><td>First Name:</td><td>" + usr.FirstName + "</td></tr>";
                    bodytxt += "<tr><td>Last Name:</td><td>" + usr.LastName + "</td></tr>";
                    bodytxt += "<tr><td>Email:</td><td>" + usr.Email + "</td></tr>";
                    bodytxt += "<tr><td>Extension:</td><td>" + usr.Phone + "</td></tr></table></body></html>";

                    mail.Body = bodytxt;
                    client.Send(mail);

                    Response.Redirect("~/Home");
                }
                else
                {
                    return View(gusr);
                }
            }
            catch (Exception ex)
            {
                ViewBag.AddMessage = "An Error occurred in attempting to complete Guest User Registration.";
            }
            return View(gusr);
        }
        public ActionResult Default()
        {

            

            /*string torig = "select pb.BudgetStatus, sum(pb.Amount) as budget_total";
            torig = torig + " from Projects p join ProjectBudgets pb on p.ProjectId = pb.ProjectId";
            torig = torig + " and    pb.BudgetType = 'Capital' where  datepart(yyyy,getDate())    between datepart(yyyy, dbo.MinSmallDatetime_f(p.ProjectedStart, p.ActualStart))";
            torig = torig + " and datepart(yyyy, isnull(dbo.MaxSmallDatetime_f(p.ProjectedEnd, p.ActualEnd), '1/1/2050' ) ) group by";
            torig = torig + " pb.BudgetStatus order by pb.BudgetStatus";*/


            string torig = "select pb.BudgetStatus, sum(pb.Amount) as budget_total";
            torig = torig + " from Projects p join ProjectBudgets pb on p.ProjectId = pb.ProjectId";
            torig = torig + " and    pb.BudgetType = 'Capital'";
            torig = torig + " and pb.Year = datepart(yy, getdate())";
            torig = torig + " where datepart(yy, getdate() )   between datepart(yyyy, dbo.MinSmallDatetime_f(p.ProjectedStart, p.ActualStart))";
            torig = torig + " and datepart(yyyy, isnull(dbo.MaxSmallDatetime_f(p.ProjectedEnd, p.ActualEnd), '1/1/2050' ) ) ";
            torig = torig + " and p.ApprovalStatus = 'Approved'";
            torig = torig + " group by";
            torig = torig + " pb.BudgetStatus order by pb.BudgetStatus";




            var db1 = WebMatrix.Data.Database.Open("probedb");
            bool hasEdit = false;
            List<chartQuotes> u = new List<chartQuotes>();


            int cnt = 0;
            IEnumerable<dynamic> t = db1.Query(torig);
            foreach (var typ in t)
            {
                string h = typ.BudgetStatus;
                string labStatus = "Projected";
                cnt = 2;
                switch (h)
                {
                    case "Actual":
                        labStatus = "Actual Spend";
                        cnt = 3;
                        break;

                    case "Proposed":
                        labStatus = "Original Approved";
                        cnt = 0;
                        break;

                    case "Budgeted":
                        labStatus = "Budgeted Capital";
                        cnt = 1;
                        break;

                }

                u.Add(new chartQuotes() { x = cnt, y = typ.budget_total, label = labStatus, legendText = labStatus });

            }
            ViewBag.json = JsonConvert.SerializeObject(u);

            string username = User.Identity.Name;
            username = username.Substring(username.IndexOf("\\") + 1);
            ViewBag.username = username;
            usr = data.GetUser(username);
            //usr = data.GetUser("itcntr95") - Rick Beasley;
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (usr != null)
            {

                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
               //usr.IsAdmin = false;
                Session["CurrProbeUser"] = usr;
                Session["UserIsAdmin"] = usr.IsAdmin;

                //int ret = -1;

                //List<DashboardProject> dbProjects = data.GetDashboardProjects();

                //foreach (var proj in dbProjects)
                //{
                //    ret = data.UpdateProjectHealth(proj.ProjectId, 0, usr.UserName);

                //    if (proj.EndDate < DateTime.Today) //set health to 2 (red)
                //    {                        
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 2, usr.UserName);
                //    }

                //    if (proj.StartDate < DateTime.Today && proj.ActivityStatus == "Not Started")
                //    {                        
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 1, usr.UserName);
                //    }
                //}

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasResourcesEdit = false;
                bool hasResourcesEdit = false;

                if (usr.IsAdmin)
                {
                    hasResourcesEdit = true;
                    hasEdit = true;
                }

                if (!hasResourcesEdit)
                {
                    try
                    {
                        //check to see if user is associated with an entity in order to edit the Resource Base and Core hours. 
                        var udb = Database.Open("probedb");
                        var usrEnts = udb.Query("select UserId,EntityId,ParentId from Entities where UserId = " + usr.UserId);
                        if (usrEnts.Count() > 0)
                        {
                            hasResourcesEdit = true;

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (hasResourcesEdit)
                {
                    ViewBag.hasResourcesEdit = hasResourcesEdit;
                }

                return View();
            }
            else
            {

                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");

            }
        }

        public ActionResult Approved()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }

        }

        public ActionResult Proposed()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
               
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public ActionResult Reports()
        {

            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                ViewBag.IsAdmin = false;
                if (usr.IsAdmin)
                    ViewBag.IsAdmin = true;

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public ActionResult Completed()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public ActionResult Cancelled()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        #region Templates
        public ActionResult Templates()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string baseUrl = string.Concat(Request.Url.Scheme + "://", Request.Url.Authority, Request.Url.Segments[0], string.Concat(Request.Url.Segments[1].TrimEnd('/'), "/"));
                string templateHtml = data.GetTemplateHTML(baseUrl);
                ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);

                ViewBag.TemplateHTML = templateHtml;

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }


        }

        public ActionResult doTemplateDownload(string templateFileName, string templateType)
        {
            try
            {
                ProbeTemplates myFile = data.GetSelectedTemplate(templateFileName, templateType);
                return File(myFile.Template, "application/octet-stream", myFile.TemplateFileName + "." + myFile.FileType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Budget

        public ActionResult ApprovedBudget()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    GetHtmlForApprovedBudget(int.Parse(Session["CurrProjectId"].ToString()));
                    usr = (ProbeUser)Session["CurrProbeUser"];
                    ViewBag.ID = Session["CurrProjectId"];
                    ViewBag.username = usr.UserName;
                    ViewBag.Access = BudgetAccess();

                }
                catch (Exception)
                {

                }
                ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);

                return View();

            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public JsonResult ajax_changeLog(ChangeLog_set mod)
        {
            try
            {
                List<ChangeLog_set> cLog = new List<ChangeLog_set>();
                cLog = data.GetProjectsLogs(mod.interVal);
                return Json(new { success = cLog });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }




        [HttpPost]
        public JsonResult rollbackBudget(ProjectInfo mod)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                int h = data.budgetRollBack(mod.ProjectId, usr.UserName);

                return Json(new { success = true, h });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        public ActionResult ProposedBudget()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    GetHtmlForProposedBudget(int.Parse(Session["CurrProjectId"].ToString()));
                    ViewBag.Access = BudgetAccess();
                }
                catch (Exception)
                {

                }
                ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public void UpdBudget(int ProjectId, string BudgetType, string BudgetStatus, decimal Amount, decimal oldAmount, int Year, int Month, string MonthName, string updateBy)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.UpdProjectBudgets(ProjectId, BudgetType, BudgetStatus, Amount, Year, Month, usr.UserName);
                string msg = BudgetStatus + " " + BudgetType + " " + MonthName + " " + Year.ToString() + " changed from $" + Decimal.Parse(oldAmount.ToString("0.00"), NumberStyles.Currency) + " to $" + Double.Parse(Amount.ToString("0.00"), NumberStyles.Currency);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, msg, ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }

        }

        public void UpdCurrentBudget(int ProjectId, string BudgetType, decimal CurrentBudgetAmount, decimal OldAmount, string updateBy)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.UpdProjectCurrentBudgets(ProjectId, BudgetType, CurrentBudgetAmount, usr.UserName);
                string msg = "Current Budgeted " + BudgetType + " changed from $" + Decimal.Parse(OldAmount.ToString("0.00"), NumberStyles.Currency) + " to $" + Decimal.Parse(CurrentBudgetAmount.ToString("0.00"), NumberStyles.Currency);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, msg, ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }



        [HttpPost]
        public JsonResult UpdOriginalBudget(BudgetRecord mod)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            string msg = "";
            int chk;
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                int y = -1;
                chk = data.UpdProjectBudgets(mod.ProjectId, mod.BudgetType, mod.BudgetStatus, mod.Amount, mod.Year, mod.Month, usr.UserName);
                //msg = "Current Budgeted " + mod.BudgetType + " changed from $" + Decimal.Parse(OldAmount.ToString("0.00"), NumberStyles.Currency) + " to $" + Decimal.Parse(CurrentBudgetAmount.ToString("0.00"), NumberStyles.Currency);
                data.AddChangeLog(mod.ProjectId, DateTime.Now, usr.UserId, msg, ref logId);

            }
            catch (Exception)
            {
                //Handle Error
            }
            return Json(new { success = true });
        }





        public void GetHtmlForApprovedBudget(int ProjectId)
        {
            try
            {
                ResourcesData rsrcdata = new ResourcesData();
                DateTime projStartDt = rsrcdata.getProjectStart(ProjectId);
                DateTime projEndDt = rsrcdata.getProjectEnd(ProjectId);

                //get project actual start date
                ProjectInfo proj = data.GetProjectInfo(ProjectId);
                DateTime projDt = proj.ActualStart ?? DateTime.Now;
                DateTime projectActualStart = DateTime.Parse(projDt.Month.ToString() + "/1/" + projDt.Year.ToString());

                //determine current Year, Month and Day
                int cYear = DateTime.Now.Year;
                int cMth = DateTime.Now.Month;
                int cDay = DateTime.Now.Day;

                List<BudgetRecord> Records1 = data.GetAllBudgetRecords(ProjectId);
                List<BudgetViewRecord> Details = data.GetBudgetsDetailsWTotalsRecords(ProjectId);
                List<BudgetRecord> Proposalrecs = data.GetProposedBudgetRecords(ProjectId);

                List<BudgetRecord> Records = Records1.Concat(Proposalrecs).ToList();

                IEnumerable<int> years = Records.Select(x => x.Year).Distinct().OrderBy(x => x); //.Where(x => x.BudgetStatus == "Projected")
                IEnumerable<string> types = Records.Select(x => x.BudgetType).Distinct();
                //IEnumerable<string> statuses = Records.Select(x => x.BudgetStatus).Distinct();

                string newTxtBx = "<input type=\"text\" onkeypress=\"return isNumberKey(event)\" class=\"ui-corner-all currency budgetTextbox\" value=\"";

                //~~~~~~~~~~~~~~~~~~~~~~
                //CREATE MONTHLY SECTION
                //~~~~~~~~~~~~~~~~~~~~~~
                StringBuilder sb = new StringBuilder();
                //Create labels
                sb.Append("<span style=\"visibility:hidden\" class=\"budgetLbl\">empty</span>");
                foreach (string typ in types)
                {

                    //sb.Append("<label class=\"budgetLbl\"><em>Budgeted</em> " + typ + "</label>");
                    //sb.Append("<label class=\"budgetLbl\"><em>Projected</em> " + typ + "</label>");
                    //sb.Append("<label class=\"budgetLbl\"><em>Actual</em> " + typ + "</label>");
                    string edittyp;
                    edittyp = typ;

                    if (typ == "Capital")
                    {
                        edittyp = "Current Budgeted Capital";
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">Original\n" + typ + "\nApproved</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">" + edittyp + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Projected \n" + typ + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Actual \n" + typ + "</textarea>");
                        //Change
                        //sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">" + typ + " \nOriginal \nApproved</textarea>");
                    }
                    else if (typ == "Expense")
                    {
                        edittyp = "Current Budgeted Expense";
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">Original\n" + typ + "\nApproved</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">" + edittyp + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Projected \n" + typ + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Actual \n" + typ + "</textarea>");
                        //Change sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">" + typ + " \nOriginal \nApproved</textarea>");
                    }
                    else
                    {
                        edittyp = "E to P";
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:60px;\">Original\n" + edittyp + " \nApproved</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:50px;\">Current\n" + edittyp + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Projected \n" + edittyp + "</textarea>");
                        sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + "Actual \n" + edittyp + "</textarea>");
                        //Change sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;height:60px;\">" +typ + " \nOriginal \nApproved</textarea>");
                    }
                }
                sb.Append("<br/><br/><br/><br/><br/>");

                foreach (int yr in years)
                {
                    string dsply = string.Empty;
                    IEnumerable<BudgetRecord> yearRecords = Records.Where(x => x.Year == yr);
                    //if (yr == years.First()) { dsply = ""; } else { dsply = "style=\"display:none;\""; }
                    sb.Append("<div>");
                    sb.Append("<label class=\"budgetYearLbl\" onclick=\"javascript:runEffect('#" + yr.ToString() + "')\">" + yr.ToString() + "</label>");
                    foreach (string btype in types)
                    {
                        decimal bdg = yearRecords.Where(x => x.BudgetStatus == "Budgeted" && x.BudgetType == btype).Select(x => x.Amount).Sum();
                        decimal prj = yearRecords.Where(x => x.BudgetStatus == "Projected" && x.BudgetType == btype).Select(x => x.Amount).Sum();
                        decimal act = yearRecords.Where(x => x.BudgetStatus == "Actual" && x.BudgetType == btype).Select(x => x.Amount).Sum();
                        decimal pro = yearRecords.Where(x => x.BudgetStatus == "Proposed" && x.BudgetType == btype).Select(x => x.Amount).Sum();

                        //sb.Append("<label class=\"budgetLbl\">" + yr.ToString() + "</label>");

                        string id = "Totals|" + btype + "Original|" + yr.ToString();
                        sb.Append(newTxtBx + pro.ToString() + "\" id=\"" + id + "\" disabled/>");

                        id = "Totals|" + btype + "Budgeted|" + yr.ToString();
                        sb.Append(newTxtBx + bdg.ToString() + "\" id=\"" + id + "\" disabled/>");

                        id = "Totals|" + btype + "Projected|" + yr.ToString();
                        sb.Append(newTxtBx + prj.ToString() + "\" id=\"" + id + "\" disabled/>");

                        id = "Totals|" + btype + "Actual|" + yr.ToString();
                        sb.Append(newTxtBx + act.ToString() + "\" id=\"" + id + "\" disabled/>");



                    }
                    sb.Append("</div>");
                    sb.Append("<br/><br/>");


                    sb.Append("<div id=\"" + yr.ToString() + "\" class=\"ui-widget-content ui-corner-all\" " + dsply + ">");

                    IEnumerable<int> monthRecs = yearRecords.Select(x => x.Month).Distinct().OrderBy(x => x);


                    int swtch = 0;
                    foreach (var mnth in monthRecs)
                    {
                        IEnumerable<BudgetRecord> monthRecords = yearRecords.Where(x => x.Month == mnth);
                        if (swtch == 0) { sb.Append("<div class=\"odd\">"); } else { sb.Append("<div class=\"even\">"); }
                        sb.Append("<label class=\"budgetLbl\">" + monthRecords.First().MonthName + "</label>");

                        foreach (string typ in types)
                        {
                            BudgetRecord bdgtd = monthRecords.Where(x => x.BudgetType == typ && x.BudgetStatus == "Budgeted").FirstOrDefault();
                            BudgetRecord prjctd = monthRecords.Where(x => x.BudgetType == typ && x.BudgetStatus == "Projected").FirstOrDefault();
                            BudgetRecord actual = monthRecords.Where(x => x.BudgetType == typ && x.BudgetStatus == "Actual").FirstOrDefault();
                            BudgetRecord propos = monthRecords.Where(x => x.BudgetType == typ && x.BudgetStatus == "Proposed").FirstOrDefault();


                            string tooltip = "";
                            string id = "";



                            //Proposed textbox
                            string ProposedDisabled = "";
                            if (propos != null)
                            {
                                if (yr < cYear) { ProposedDisabled = "disabled"; }
                                if (yr == cYear && propos.Month < cMth) { ProposedDisabled = "disabled"; }
                                if (yr == cYear && propos.Month == cMth && cDay > 10) { ProposedDisabled = "disabled"; }


                                usr = (ProbeUser)Session["CurrProbeUser"];

                                if (usr.IsAdmin == true)
                                {
                                    tooltip = propos.ProjectId.ToString() + "|" + propos.BudgetType + "|" + propos.BudgetStatus + "|" + propos.Amount.ToString() + "|" + propos.Year.ToString() + "|" + propos.Month.ToString() + "|" + propos.MonthName;
                                    id = propos.BudgetType + propos.BudgetStatus + "|" + yr.ToString() + propos.MonthName;
                                    sb.Append(newTxtBx + propos.Amount.ToString() + "\" id=\"" + id + "\" title=\"" + tooltip + "\" " + ProposedDisabled + " onchange=\"javascript:updOriginalBudget(this)\"/>");
                                }
                                else
                                {
                                    decimal totAmt = Math.Round(propos.Amount, 2);
                                    string amt = totAmt.ToString("C");
                                    string newTxtBx1 = "<span style='float:left;width:90px;margin: 0px 5px 0px 5px;color:#787878 !important;font-size:smaller!important;'>" + amt + "</span>";
                                    sb.Append(newTxtBx1);

                                }

                            }
                            else
                            {
                                sb.Append(newTxtBx + "\" id=\"No Id available\" title=\"No value available\" disabled />");
                            }








                            //Budgeted textbox
                            string BudgetedDisabled = "";
                            if (bdgtd != null)
                            {
                                if (yr < cYear) { BudgetedDisabled = "disabled"; }
                                if (yr == cYear && bdgtd.Month < cMth) { BudgetedDisabled = "disabled"; }
                                if (yr == cYear && bdgtd.Month == cMth && cDay > 10) { BudgetedDisabled = "disabled"; }

                                tooltip = bdgtd.ProjectId.ToString() + "|" + bdgtd.BudgetType + "|" + bdgtd.BudgetStatus + "|" + bdgtd.Amount.ToString() + "|" + bdgtd.Year.ToString() + "|" + bdgtd.Month.ToString() + "|" + bdgtd.MonthName;
                                id = bdgtd.BudgetType + bdgtd.BudgetStatus + "|" + yr.ToString() + bdgtd.MonthName;
                                sb.Append(newTxtBx + bdgtd.Amount.ToString() + "\" id=\"" + id + "\" title=\"" + tooltip + "\" " + BudgetedDisabled + " onchange=\"javascript:updApprovedBudget(this)\"/>");
                            }
                            else
                            {
                                sb.Append(newTxtBx + "\" id=\"No Id available\" title=\"No value available\" disabled />");
                            }

                            //Projected textbox
                            string ProjectedDisabled = "";
                            if (prjctd != null)
                            {
                                if (yr < cYear) { ProjectedDisabled = "disabled"; }
                                if (yr == cYear && prjctd.Month < cMth) { ProjectedDisabled = "disabled"; }
                                if (yr == cYear && prjctd.Month == cMth && cDay > 10) { ProjectedDisabled = "disabled"; }

                                tooltip = prjctd.ProjectId.ToString() + "|" + prjctd.BudgetType + "|" + prjctd.BudgetStatus + "|" + prjctd.Amount.ToString() + "|" + prjctd.Year.ToString() + "|" + prjctd.Month.ToString() + "|" + prjctd.MonthName;
                                id = prjctd.BudgetType + prjctd.BudgetStatus + "|" + yr.ToString() + prjctd.MonthName;
                                sb.Append(newTxtBx + prjctd.Amount.ToString() + "\" id=\"" + id + "\" title=\"" + tooltip + "\" " + ProjectedDisabled + " onchange=\"javascript:updApprovedBudget(this)\"/>");
                            }
                            else
                            {
                                sb.Append(newTxtBx + "\" id=\"No Id available\" title=\"No value available\" disabled />");
                            }

                            //Actual textbox
                            string ActualDisabled = "disabled";
                            if (actual != null)
                            {
                                if (yr < cYear) { ActualDisabled = ""; }
                                if (yr == cYear && actual.Month <= cMth) { ActualDisabled = ""; }

                                DateTime ActualDate = DateTime.Parse(actual.Month.ToString() + "/1/" + actual.Year.ToString());
                                if (ActualDate < projectActualStart) { ActualDisabled = "disabled"; }

                                tooltip = actual.ProjectId.ToString() + "|" + actual.BudgetType + "|" + actual.BudgetStatus + "|" + actual.Amount.ToString() + "|" + actual.Year.ToString() + "|" + actual.Month.ToString() + "|" + actual.MonthName;
                                id = actual.BudgetType + actual.BudgetStatus + "|" + yr.ToString() + actual.MonthName;
                                sb.Append(newTxtBx + actual.Amount.ToString() + "\" id=\"" + id + "\" title=\"" + tooltip + "\" " + ActualDisabled + " onchange=\"javascript:updApprovedBudget(this)\"/>");
                            }
                            else
                            {
                                sb.Append(newTxtBx + "\" id=\"No Id available\" title=\"No value available\" disabled />");
                            }
                        }
                        sb.Append("</div>");
                        if (swtch == 0) { swtch = 1; } else { swtch = 0; }
                    }
                    sb.Append("</div><br/>");
                }
                ViewBag.DynamicBudgetDetails = sb.ToString();


                //~~~~~~~~~~~~~~~~~~~~~
                //CREATE ANNUAL SECTION
                //~~~~~~~~~~~~~~~~~~~~~
                StringBuilder sbAnnual = new StringBuilder();

                sbAnnual.Append("<label class=\"budgetLbl\">Year</label>");
                sbAnnual.Append("<label class=\"budgetLbl\">Current</label>");
                sbAnnual.Append("<label class=\"budgetLbl\">Projected</label>");
                sbAnnual.Append("<label class=\"budgetLbl\">Actual</label>");

                sbAnnual.Append("<br/><br/>");
                foreach (int yr in years)
                {
                    IEnumerable<BudgetRecord> yearRecords = Records.Where(x => x.Year == yr);
                    decimal bdgtd = yearRecords.Where(x => x.BudgetStatus == "Budgeted").Select(x => x.Amount).Sum();
                    decimal prjctd = yearRecords.Where(x => x.BudgetStatus == "Projected").Select(x => x.Amount).Sum();
                    decimal actual = yearRecords.Where(x => x.BudgetStatus == "Actual").Select(x => x.Amount).Sum();

                    sbAnnual.Append("<label class=\"budgetLbl\">" + yr.ToString() + "</label>");

                    string id = "Current|" + yr.ToString();
                    sbAnnual.Append(newTxtBx + bdgtd.ToString() + "\" id=\"" + id + "\" disabled />");

                    id = "Projected|" + yr.ToString();
                    sbAnnual.Append(newTxtBx + prjctd.ToString() + "\" id=\"" + id + "\" disabled />");

                    id = "Actual|" + yr.ToString();
                    sbAnnual.Append(newTxtBx + actual.ToString() + "\" id=\"" + id + "\" disabled />");

                    sbAnnual.Append("<br/><br/>");
                }
                ViewBag.DynamicBudgetAnnualTotals = sbAnnual.ToString();


                //~~~~~~~~~~~~~~~~~~~~~
                //CREATE TOTALS SECTION
                //~~~~~~~~~~~~~~~~~~~~~

                List<OrigApprovedBudget> OrigApprRecs = data.GetOrigApprovedBudgetRecords(ProjectId);

                StringBuilder sbTotal = new StringBuilder();
                IEnumerable<string> viewTypes = Details.Select(x => x.BudgetType).Distinct();

                sbTotal.Append("<span style=\"visibility:hidden\" class=\"budgetLbl\">empty</span>");
                sbTotal.Append("<label class=\"budgetLbl\">Original Approved</label>");
                sbTotal.Append("<label class=\"budgetLbl\">Current</label>");
                sbTotal.Append("<label class=\"budgetLbl\">Projected</label>");
                sbTotal.Append("<label class=\"budgetLbl\">Actual</label>");


                sbTotal.Append("<br/><br/>");

                foreach (BudgetViewRecord rcd in Details)
                {
                    string origapprtot = "0.00";
                    decimal y = 0;
                    foreach (OrigApprovedBudget oarec in OrigApprRecs)
                    {
                        if (oarec.BudgetType == rcd.BudgetType)
                        {
                            origapprtot = oarec.Total.ToString();
                        }
                        y = y + oarec.Total;
                    }
                    string t = rcd.BudgetType.ToString();
                    if (t == "Totals")
                    {
                        origapprtot = y.ToString();
                    }
                    string tooltip = ProjectId.ToString() + "|" + rcd.BudgetType + "|" + y;
                    sbTotal.Append("<label class=\"budgetLbl\">" + rcd.BudgetType + "</label>");
                    string id = rcd.BudgetType + "OriginalApproved";
                    sbTotal.Append(newTxtBx + origapprtot + "\" id=\"" + id + "\"  disabled \"/>");
                    id = rcd.BudgetType + "Budgeted";
                    sbTotal.Append(newTxtBx + rcd.BudgetedTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
                    id = rcd.BudgetType + "Projected";
                    sbTotal.Append(newTxtBx + rcd.ProjectedTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
                    id = rcd.BudgetType + "Actual";
                    sbTotal.Append(newTxtBx + rcd.ActualTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
                    sbTotal.Append("<br/><br/>");
                }
                ViewBag.DynamicBudgetTotals = sbTotal.ToString();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public void GetHtmlForProposedBudget(int ProjectId)
        {
            List<BudgetRecord> Records = data.GetProposedBudgetRecords(ProjectId);

            IEnumerable<int> years = Records.Select(x => x.Year).Distinct().OrderBy(x => x);
            IEnumerable<string> types = Records.Select(x => x.BudgetType).Distinct();

            string newTxtBx = "<input type=\"text\" onkeypress=\"return isNumberKey(event)\" class=\"ui-corner-all currency budgetTextbox\" value=\"";

            //~~~~~~~~~~~~~~~~~~~~~~
            //CREATE MONTHLY SECTION
            //~~~~~~~~~~~~~~~~~~~~~~
            StringBuilder sb = new StringBuilder();
            //Create labels
            sb.Append("<span style=\"visibility:hidden\" class=\"budgetLbl\">empty</span>");
            foreach (string typ in types)
            {
                //sb.Append("<label class=\"budgetLbl\">" + typ + "</label>");
                sb.Append("<textarea class=\"budgetTextArea\" disabled style=\"overflow:hidden;text-align:left;color:black!important;\">" + typ + "</textarea>");
            }
            sb.Append("<br/><br/>");

            foreach (int yr in years)
            {
                string dsply = string.Empty;
                //if (yr == years.First()) { dsply = ""; } else { dsply = "style=\"display:none;\""; }
                sb.Append("<div>");
                sb.Append("<label class=\"budgetYearLbl\" onclick=\"javascript:runEffect('#" + yr.ToString() + "')\">" + yr.ToString() + "</label>");
                //CREATES TOTALS BESIDE YEAR
                IEnumerable<BudgetRecord> yearRecords = Records.Where(x => x.Year == yr);
                decimal total = Records.Where(x => x.Year == yr).Select(x => x.Amount).Sum();
                //sb.Append("<label class=\"budgetLbl\">" + yr.ToString() + "</label>");
                foreach (string typ in types)
                {
                    decimal typTotal = Records.Where(x => x.Year == yr && x.BudgetType == typ).Select(x => x.Amount).Sum();
                    string id = "Total|Proposed" + typ + "|" + yr.ToString();
                    sb.Append(newTxtBx + typTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
                }
                //END TOTALS BESIDE YEAR
                sb.Append("</div>");
                sb.Append("<br/><br/>");
                //sb.Append("<br/>");
                sb.Append("<div id=\"" + yr.ToString() + "\" class=\"ui-widget-content ui-corner-all\" " + dsply + ">");
                IEnumerable<BudgetRecord> yearRecs = Records.Where(x => x.Year == yr);
                IEnumerable<string> months = yearRecs.Select(x => x.MonthName).Distinct();
                int swtch = 0;
                foreach (string mnth in months)
                {
                    IEnumerable<BudgetRecord> monthRecords = yearRecs.Where(x => x.MonthName == mnth);
                    if (swtch == 0) { sb.Append("<div class=\"odd\">"); } else { sb.Append("<div class=\"even\">"); }
                    sb.Append("<label class=\"budgetLbl\">" + mnth + "</label>");
                    foreach (BudgetRecord rcd in monthRecords)
                    {
                        string tooltip = rcd.ProjectId.ToString() + "|" + rcd.BudgetType + "|" + rcd.BudgetStatus + "|" + rcd.Amount.ToString() + "|" + rcd.Year.ToString() + "|" + rcd.Month.ToString() + "|" + rcd.MonthName;
                        string id = "Proposed" + rcd.BudgetType + "|" + yr.ToString() + rcd.MonthName;
                        sb.Append(newTxtBx + rcd.Amount.ToString() + "\" id=\"" + id + "\" title=\"" + tooltip + "\" onchange=\"javascript:updBudget(this)\"/>");
                    }
                    sb.Append("</div>");
                    if (swtch == 0) { swtch = 1; } else { swtch = 0; }
                }
                sb.Append("</div><br/>");
            }
            ViewBag.DynamicBudgetDetails = sb.ToString();


            //~~~~~~~~~~~~~~~~~~~~~
            //CREATE ANNUAL SECTION
            //~~~~~~~~~~~~~~~~~~~~~
            StringBuilder sbAnnual = new StringBuilder();

            sbAnnual.Append("<label class=\"budgetLbl\">Year</label>");
            foreach (string typ in types)
            {
                sbAnnual.Append("<label class=\"budgetLbl\">Proposed " + typ + "</label>");
            }
            sbAnnual.Append("<br/><br/>");
            foreach (int yr in years)
            {
                IEnumerable<BudgetRecord> yearRecords = Records.Where(x => x.Year == yr);
                decimal total = Records.Where(x => x.Year == yr).Select(x => x.Amount).Sum();
                sbAnnual.Append("<label class=\"budgetLbl\">" + yr.ToString() + "</label>");
                foreach (string typ in types)
                {
                    decimal typTotal = Records.Where(x => x.Year == yr && x.BudgetType == typ).Select(x => x.Amount).Sum();
                    string id = "Proposed" + typ + "|" + yr.ToString();
                    sbAnnual.Append(newTxtBx + typTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
                }
                sbAnnual.Append("<br/><br/>");
            }
            ViewBag.DynamicBudgetAnnualTotals = sbAnnual.ToString();


            //~~~~~~~~~~~~~~~~~~~~~
            //CREATE TOTALS SECTION
            //~~~~~~~~~~~~~~~~~~~~~
            StringBuilder sbTotal = new StringBuilder();
            sbTotal.Append("<label class=\"budgetLbl\">Total</label>");
            foreach (string typ in types)
            {
                sbTotal.Append("<label class=\"budgetLbl\">Proposed " + typ + "</label>");
            }
            sbTotal.Append("<br/><br/>");
            decimal tot = Records.Select(x => x.Amount).Sum();
            sbTotal.Append(newTxtBx + tot.ToString() + "\" id=\"Total\" disabled/>");
            foreach (string typ in types)
            {
                decimal typTotal = Records.Where(x => x.BudgetType == typ).Select(x => x.Amount).Sum();
                string id = "Proposed" + typ + "|";
                sbTotal.Append(newTxtBx + typTotal.ToString() + "\" id=\"" + id + "\" disabled/>");
            }
            ViewBag.DynamicBudgetTotals = sbTotal.ToString();
        }

        private string BudgetAccess()
        {
            string Access = string.Empty;
            usr = (ProbeUser)Session["CurrProbeUser"];

            if (usr.IsAdmin)
            {
                Access = "Admin";
            }
            else
            {
                int pid = int.Parse(Session["CurrProjectId"].ToString());
                ProjectInfo pinfo = data.GetProjectInfo(pid);

                if (usr.ProjectRoleList == null)
                {
                    usr.ProjectRoleList = data.GetProjectRolesForUser(pid, usr.UserName);
                }

                if (pinfo.InitApproved)
                {
                    foreach (string pr in usr.ProjectRoleList)
                    {
                        Access += pr + "|";
                    }
                    if (!string.IsNullOrEmpty(Access))
                    {
                        Access = Access.Substring(0, Access.Length - 1);
                    }
                }
                else
                {
                    foreach (string pr in usr.ProjectRoleList)
                    {
                        if (usr.ProjectRoleList.Contains("Sponsor") || usr.ProjectRoleList.Contains("Creator") || usr.ProjectRoleList.Contains("Manager") || usr.ProjectRoleList.Contains("Engineer"))
                        {
                            Access += pr + "|";
                        }
                    }
                    if (!string.IsNullOrEmpty(Access))
                    {
                        Access = Access.Substring(0, Access.Length - 1);
                    }

                }

            }

            return Access;
        }


        #endregion

        #region Resources
        public ActionResult Resources()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                bool hasEdit = false;
                if (Session["CurrProjectId"] != null)
                {
                    var ProjId = Session["CurrProjectId"].ToString();
                    if (!String.IsNullOrEmpty(ProjId))
                    {
                        ProjectInfo pi = data.GetProjectInfo(int.Parse(ProjId));
                        ViewBag.ProjectName = pi.ProjectName;

                        ViewBag.CurrProjId = ProjId;
                        if (Session["CurrProbeUser"] != null)
                        {
                            usr = (ProbeUser)Session["CurrProbeUser"];
                            if (usr.IsAdmin)
                            {
                                hasEdit = true;
                            }
                            else
                            {
                                hasEdit = data.UserHasEdit(int.Parse(ProjId), usr);
                            }

                        }
                    }
                }
                ViewBag.HasEdit = hasEdit;
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }


        public ActionResult ProjectTemplate()
        {
            ViewBag.hasRole = true;


            var items = new List<SelectListItem>();

            items.Add(new SelectListItem { Value = null, Text = null, Selected = true });

            IEnumerable<ProbeUser> usrs = data.usrs();

            NewProj proj = new NewProj();
            foreach (var x in usrs)
            {
                if (x.UserName == proj.Manager)
                {
                    items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName, Selected = true });
                }
                else
                    items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName });
            }


            proj.ResourceList = items;






            ViewBag.Sent = false;
            try
            {
                proj = PopulateProjectLists(proj);
                usr = (ProbeUser)Session["CurrProbeUser"];
                string sqltxt = @"SELECT 
	                              r.RoleName
                                  ,u.UserName
                              FROM Roles r
                              left join [probe].[dbo].[UserRoles] ur on r.RoleId = ur.RoleId
                              left join Users u on u.UserId = ur.UserId
                              where u.Active != 0 and u.UserName = '" + usr.UserName + "' and r.RoleName in('Project Creator','Admin')";

                Database db = Database.Open("probedb");

                var urol = db.Query(sqltxt);
                if (urol.Count() == 0)
                {
                    ViewBag.hasRole = false;
                    GuestUser reqcreat = new GuestUser();
                    reqcreat.Email = usr.Email;
                    reqcreat.FirstName = usr.FirstName;
                    reqcreat.LastName = usr.LastName;
                    reqcreat.Extension = usr.Phone;

                    proj.RequestCreator = reqcreat;
                }
                else
                {
                    ViewBag.hasRole = true;
                }


                if (string.IsNullOrEmpty(usr.UserName))
                { proj.updateBy = User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1); }
                else { proj.updateBy = usr.UserName; }
            }
            catch (Exception ex)
            {
                //Handle Error
            }
            return View(proj);




        }




        [HttpPost]
        public string ResourceHandler(string ProjId = null, string RsrcId = null, string EntId = null, string ActTyp = null, string comments = null, string yr = null, string mhrs = null)
        {

            bool hasEdit = false;
            if (String.IsNullOrEmpty(ProjId))
            {
                ProjId = Session["CurrProjectId"].ToString();
            }
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                hasEdit = data.UserHasEdit(int.Parse(ProjId), usr);

                if (Session["lastPage"] != null)
                {

                    if (Session["lastPage"].ToString() == "proposed")
                    {

                        foreach (string pr in usr.ProjectRoleList)
                        {
                            if (pr == "Sponsor" || pr == "Manager" || pr == "Engineer" || pr == "Contributor" || pr == "Creator")
                                hasEdit = true;
                        }
                    }

                    if (Session["lastPage"].ToString() == "approved")
                    {

                        foreach (string pr in usr.ProjectRoleList)
                        {
                            if (pr == "Sponsor" || pr == "Manager" || pr == "Engineer" || pr == "Contributor")
                                hasEdit = true;
                        }
                    }
                }

            }
            bool hh = hasEdit;
            string retVal = "";
            string cl_msg = "";
            try
            {
                var db = Database.Open("probedb");

                string sqltxt = "";
                string retval = "";
                int? logid = -1;
                if (ActTyp != null)
                {

                    switch (ActTyp)
                    {
                        case "info":
                            sqltxt = @"select r.ResourceId,
                                r.Comments,
                                r.ResourceName,
                                r.ResourceTitle,
                                r.ResourceEmail,
                                r.ResourcePhone,
                                rmh.BaseManHours,
                                rmh.CoreManHours,
                                r.EntityId                                
                                from Resources r left outer join ResourceManHours rmh on rmh.ResourceId = r.ResourceId
                                where r.EntityId = " + EntId +
                                            @" and r.ResourceId=" + RsrcId;

                            var recinfo = db.Query(sqltxt, null);
                            retVal = "<table><tr>";
                            retVal += "<td><span>Title:</span><span></td><td>" + recinfo.FirstOrDefault().ResourceTitle + "</td></tr>";
                            retVal += "<tr><td><span>Email:</span></td><td>" + recinfo.FirstOrDefault().ResourceEmail + "</td></tr>";
                            retVal += "<tr><td><span>Phone:</span></td><td>" + recinfo.FirstOrDefault().ResourcePhone + "</td></tr>";
                            //retVal += "<tr><td><span>Base Hours:</span></td><td>" + recinfo.FirstOrDefault().BaseManHours + "</td></tr>";
                            //retVal += "<tr><td><span>Core Hours:</span></td><td>" + recinfo.FirstOrDefault().CoreManHours + "</td></tr>";
                            retVal += "<tr><td><span>Comments:</span></td><td>" + recinfo.FirstOrDefault().Comments + "</td></tr>";
                            retVal += "<tr><td>&nbsp;</td><td><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeInfoDialog()'/></td></tr></table>";

                            break;

                        case "edit":
                            Dictionary<int, string> mnlist = new Dictionary<int, string>();
                            mnlist.Add(1, "Jan");
                            mnlist.Add(2, "Feb");
                            mnlist.Add(3, "Mar");
                            mnlist.Add(4, "Apr");
                            mnlist.Add(5, "May");
                            mnlist.Add(6, "Jun");
                            mnlist.Add(7, "Jul");
                            mnlist.Add(8, "Aug");
                            mnlist.Add(9, "Sep");
                            mnlist.Add(10, "Oct");
                            mnlist.Add(11, "Nov");
                            mnlist.Add(12, "Dec");
                            sqltxt = @"select p.ApprovalStatus,
                                        p.DisplayName,
                                        r.ResourceName,  
								        pr.ProposedManHrs,
								        pr.ActualManHrs,                              
                                        pr.Comments,
                                        pr.Year,
								        pr.Month                              
                                        from ProjectResources pr  
                                        join Resources r on r.ResourceId = pr.ResourceId
								        join Projects p on p.ProjectId = pr.ProjectId
                                        where pr.ProjectId = " + ProjId + " and pr.ResourceId= " + RsrcId + " and pr.[Year] = " + yr;

                            var rec = db.Query(sqltxt, null);
                            if (rec.Count() > 0)
                            {
                                retVal += "<form id='frmRsrcEdit' name='frmRsrcEdit'>";
                                retVal += "<h5>Project: " + rec.FirstOrDefault().DisplayName + "</h5>";
                                retVal += "<h5>Resource Man Hours for : " + rec.FirstOrDefault().ResourceName + "</h5>";
                                retVal += "<h5>Year: " + rec.FirstOrDefault().Year + "</h5>";
                                retVal += "<br/><span>Total Proposed:</span><input id='totProposed' name='totProposed'  type='text' readonly />&nbsp;&nbsp;";

                                if (rec.First().ApprovalStatus == "Approved")
                                {
                                    retVal += "<span>Total Actual:</span><input type=text' id='totActual' name='totActual'  readonly />";

                                }
                                retVal += "<br/><span style='font-size:.75em'>(enter values here to populate all months. -->)</span>&nbsp;&nbsp;<span>All Proposed:</span><input id='allProp' name='allProp'  type='text' value='0'   onkeypress='return isNumberKey(event)' onkeyup='getKey();' />&nbsp;&nbsp;";
                                if (rec.First().ApprovalStatus == "Approved")
                                {
                                    retVal += "<span>All Actual:</span><input type=text' id='allAct' name='allAct' value='0'   onkeypress='return isNumberKey(event)' onkeyup='getKey();' />";
                                }

                                retVal += "<br/> <br/><div class='mhrWrapper'>";
                                foreach (var rsrcrec in rec)
                                {
                                    retVal += "<div class='mnthWrapper'><div class='iBlock'>";
                                    retVal += "<div class='mnthLbl'>" + mnlist[rsrcrec.Month] + "</div>";
                                    retVal += "<div><input class='mhrLbl' value='Proposed:' readonly />";
                                    retVal += "<input type ='text' id='prop_" + rsrcrec.Month + "' name='prop_" + rsrcrec.Month + "' value='" + rsrcrec.ProposedManHrs + "'  onkeypress='return isNumberKey(event)' onkeyup='getKey();' /></div>";
                                    if (rsrcrec.ApprovalStatus == "Approved")
                                    {
                                        retVal += "<div><input class='mhrLbl' value='Actual:' readonly />";
                                        retVal += "<input class='mhrLbl' type ='text' id='act_" + rsrcrec.Month + "' name='act_" + rsrcrec.Month + "' value='" + rsrcrec.ActualManHrs + "' onkeypress='return isNumberKey(event)' onkeyup='getKey();' /></div>";
                                    }
                                    retVal += "</div><div class='cmtBlock'><input type='text' readonly class='mhrLbl' value='Comments:' /><br/>";
                                    retVal += "<textarea id='cmts_" + rsrcrec.Month + "' name='cmts_" + rsrcrec.Month + "'  rows='5' cols='40'  >" + rsrcrec.Comments + "</textarea></div></div>";
                                }
                                retVal += "</div>";
                                if (hasEdit)
                                {
                                    retVal += "<br/><div style='float:left'><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveRsrcEdits()' /></div>";
                                    retVal += "<div style='float:left'><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeEditDialog()'/></div>";
                                }
                                else
                                {
                                    retVal += "<br/><div style='float:left'><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/></div>";
                                }

                                retVal += "<input id='rsrcId' name='rsrcId' type='hidden' value='" + RsrcId + "' />";
                                retVal += "<input id='projId' name='projId' type='hidden' value='" + ProjId + "' />";
                                retVal += "<input id='entId' name='entId' type='hidden' value='" + EntId + "' />";
                                retVal += "<input id='year' name='year' type='hidden' value='" + yr + "' />";
                                retVal += "</form>";
                            }
                            else
                            {
                                retVal = "<p> No Record found for this resource and year. Contact the Administrator if this error persists.</p><br/><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/>";
                            }
                            break;

                        case "updateEdits":
                            ProjectResource rsrcObj = (ProjectResource)JsonConvert.DeserializeObject(mhrs, typeof(ProjectResource));
                            var rsrcUpdN = db.Query("select pr.*, r.ResourceName from ProjectResources pr join Resources r on pr.ResourceId = r.ResourceId where pr.ProjectId=" + ProjId + " and pr.ResourceId=" + RsrcId + " and pr.[Year] = " + yr + " and pr.[Month] = " + rsrcObj.Month, null);
                            var rsrcRecN = rsrcUpdN.FirstOrDefault();
                            data.UpdProjectResource(rsrcRecN.ProjectId, rsrcRecN.ResourceId, rsrcRecN.Year, rsrcRecN.Month, rsrcObj.ProposedManHrs, rsrcObj.ActualManHrs, rsrcObj.Comments, usr.UserName);
                            //add changlog record
                            string mhrsChange = "";
                            mhrsChange = (rsrcRecN.ProposedManHrs != rsrcObj.ProposedManHrs ? " , Proposed from " + rsrcRecN.ProposedManHrs + " to " + rsrcObj.ProposedManHrs : "");
                            mhrsChange += (rsrcRecN.ActualManHrs != rsrcObj.ActualManHrs ? " , Actual from " + rsrcRecN.ActualManHrs + " to " + rsrcObj.ActualManHrs : "");
                            mhrsChange += (rsrcRecN.Comments != rsrcObj.Comments ? " , Updated Comments " : "");
                            cl_msg = "Updated the Project Resource for " + rsrcRecN.ResourceName + " - Year " + yr + " - Month " + rsrcObj.Month + mhrsChange;
                            data.AddChangeLog(rsrcRecN.ProjectId, DateTime.Now, usr.UserId, cl_msg, ref logid);
                            retVal = "success";
                            break;
                        case "addResource":
                            ResourcesData rsdata = new ResourcesData();
                            //get project years
                            List<int> years = new List<int>();
                            DateTime startDt = rsdata.getProjectStart(int.Parse(ProjId));
                            DateTime endDt = rsdata.getProjectEnd(int.Parse(ProjId));

                            for (int addyr = startDt.Year; endDt.Year >= addyr; addyr++)
                            {

                                for (int m = 1; m < 13; m++)
                                {
                                    if (addyr == startDt.Year)
                                    {
                                        if (m >= startDt.Month)
                                        {
                                            data.AddProjectResource(int.Parse(ProjId), int.Parse(RsrcId), addyr, m, 0, 0, "", usr.UserName);
                                        }
                                    }
                                    else if (addyr == endDt.Year)
                                    {
                                        if (m <= endDt.Month)
                                        {
                                            data.AddProjectResource(int.Parse(ProjId), int.Parse(RsrcId), addyr, m, 0, 0, "", usr.UserName);
                                        }
                                    }
                                    else
                                    {
                                        data.AddProjectResource(int.Parse(ProjId), int.Parse(RsrcId), addyr, m, 0, 0, "", usr.UserName);
                                    }
                                }
                                ////check if resource already exists for this year
                                //var rsrcExist = db.Query("select * from ProjectResources where ProjectId=" + ProjId + " and ResourceId=" + RsrcId + " and [Year] = " + addyr, null);                                
                                //if (rsrcExist.Count() == 0)
                                //{

                                //}
                            }

                            //add changlog record
                            var rsrcAddName = db.Query("select r.ResourceName from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecAddNm = rsrcAddName.FirstOrDefault();
                            cl_msg = "Added Resource " + rsrcRecAddNm.ResourceName + " to the Project.";
                            data.AddChangeLog(int.Parse(ProjId), DateTime.Now, usr.UserId, cl_msg, ref logid);
                            retVal = "success";
                            break;
                        case "delResource":
                            data.DelProjectResource(int.Parse(ProjId), int.Parse(RsrcId), usr.UserName);
                            //add changlog record
                            var rsrcDelName = db.Query("select r.ResourceName from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecDelNm = rsrcDelName.FirstOrDefault();
                            cl_msg = "Removed Resource " + rsrcRecDelNm.ResourceName + " from the Project.";
                            data.AddChangeLog(int.Parse(ProjId), DateTime.Now, usr.UserId, cl_msg, ref logid);
                            retVal = "success";
                            break;
                        case "all":
                            ResourcesData rsrcAlldata = new ResourcesData();
                            //hasEdit = true;
                            rsrcAlldata.AllResources = true;
                            rsrcAlldata.IsReadonly = (hasEdit ? false : true);
                            retVal = rsrcAlldata.GetResourcesTree(int.Parse(ProjId));

                            break;
                        case "project":
                            ResourcesData rsrcdata = new ResourcesData();
                            rsrcdata.AllResources = false;
                            rsrcdata.IsReadonly = (hasEdit ? false : true);
                            retVal = rsrcdata.GetResourcesTree(int.Parse(ProjId));

                            break;

                    }
                }

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }
            return retVal;
        }


        #endregion

        #region Schedule Details
        public ActionResult ScheduleDetails()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                bool hasEdit = false;
                
                if (Session["CurrProjectId"] != null)
                {
                    var ProjId = Session["CurrProjectId"].ToString();
                    if (!String.IsNullOrEmpty(ProjId))
                    {
                        ProjectInfo pi = data.GetProjectInfo(int.Parse(ProjId));
                        ViewBag.ProjectName = pi.ProjectName;

                        ViewBag.CurrProjId = ProjId;
                        if (Session["CurrProbeUser"] != null)
                        {
                            usr = (ProbeUser)Session["CurrProbeUser"];
                            ViewBag.username=usr.UserName;
                            hasEdit = data.UserHasEdit(int.Parse(ProjId), usr);
                        }
                    }
                }
                ViewBag.HasEdit = hasEdit;
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string GetPhaseData(string ProjId = null)
        {
            string retVal = "";
            try
            {
                if (!String.IsNullOrEmpty(ProjId))
                {
                    IEnumerable<ProjectPhase> phaseData = data.GetProjectPhases(int.Parse(ProjId));

                    string phaseJSObj = "({";

                    foreach (ProjectPhase phs in phaseData)
                    {
                        phaseJSObj += phs.PhaseName + ":{ start:'" + phs.StartDate.ToShortDateString() + "' , end:'" + phs.EndDate.ToShortDateString() + "',color:'" + phs.color + "'},";
                        //sample output
                        //Initiation: { start: '1/1/2015', end: '3/28/2015', color: 'green' },
                    }
                    phaseJSObj += "})";
                    retVal = phaseJSObj;
                }
            }
            catch (Exception ex)
            {
                retVal = ex.Message;
            }
            return retVal;
        }

        [HttpPost]
        public string UpdatePhase()
        {
            usr = (ProbeUser)Session["CurrProbeUser"];

            string retVal = "";
            try
            {

                if (Request.Form != null)
                {

                    var projId = Request.Form["projId"];
                    var editField = Request["editFld"];
                    var fieldVal = Request.Form[editField];
                    ProjectPhase oldPhase = null;
                    ProjectPhase newPhase = null;

                    if (editField != null)
                    {
                        string logMsg = "";
                        int? LogId = -1;
                        int phaseId = -1;

                        switch (editField)
                        {
                            case "CurrentPhase":
                                phaseId = data.GetPhaseIdByName(fieldVal);
                                var projPhases = data.GetProjectPhases(int.Parse(projId));

                                //update all phases to IsCurrent = 0
                                foreach (ProjectPhase phs in projPhases)
                                {
                                    if (phs.PhaseId == phaseId)
                                    {
                                        //update this phase to be the current phase
                                        newPhase = new ProjectPhase();
                                        newPhase.PhaseName = phs.PhaseName;
                                        newPhase.IsCurrent = true;
                                        newPhase.updateBy = usr.UserName;
                                        data.UpdProjectPhase(phs.ProjectId, phs.PhaseId, phs.StartDate, phs.EndDate, phs.pct_complete, newPhase.IsCurrent, newPhase.updateBy);
                                    }
                                    else
                                    {
                                        if (phs.IsCurrent)
                                        {
                                            //get the old current phase for the change log message
                                            oldPhase = phs;
                                            phs.IsCurrent = false;
                                            phs.updateBy = usr.UserName;
                                            data.UpdProjectPhase(phs.ProjectId, phs.PhaseId, phs.StartDate, phs.EndDate, phs.pct_complete, phs.IsCurrent, phs.updateBy);
                                        }
                                    }
                                }

                                //add to change log if not the same phase name in case the user selected the same one as the value before in the drop down.
                                if (oldPhase != null)
                                {
                                    if (oldPhase.PhaseName != newPhase.PhaseName)
                                    {
                                        logMsg = "Updated Schedule Phases Current Phase from " + oldPhase.PhaseName + " to " + newPhase.PhaseName + ".";
                                        data.AddChangeLog(int.Parse(projId), DateTime.Now, usr.UserId, logMsg, ref LogId);
                                    }
                                }
                                else
                                {
                                    logMsg = "Updated Schedule Phases Current Phase set to " + newPhase.PhaseName + ".";
                                    data.AddChangeLog(int.Parse(projId), DateTime.Now, usr.UserId, logMsg, ref LogId);
                                }
                                retVal = "Success";
                                break;

                            case "TotPctComplete":
                                var oldProjData = data.GetProjectInfo(int.Parse(projId));
                                var newProjData = new ProjectInfo();
                                int oldPctTot = oldProjData.pct_complete;
                                int newPctTot = int.Parse(fieldVal);
                                if (newPctTot != oldPctTot)
                                {
                                    newProjData.ProjectId = oldProjData.ProjectId;
                                    newProjData.pct_complete = newPctTot;

                                    data.UpdateProject(oldProjData, newProjData, "pct_complete", usr.UserId, usr.UserName);
                                    //change log entry is done in the UpdateProject method

                                }
                                retVal = "Success";
                                break;

                            default:
                                string phase = "";
                                newPhase = null;
                                if (editField.Contains("End"))
                                {
                                    phase = editField.Substring(0, editField.Length - 3);
                                    phaseId = data.GetPhaseIdByName(phase);
                                    oldPhase = data.GetProjectPhaseById(int.Parse(projId), phaseId);
                                    newPhase = new ProjectPhase();
                                    DateTime newDt = DateTime.Parse(fieldVal);
                                    if (newDt != oldPhase.EndDate)
                                    {
                                        newPhase.EndDate = newDt;
                                        newPhase.updateBy = usr.UserName;
                                        data.UpdProjectPhase(oldPhase.ProjectId, oldPhase.PhaseId, oldPhase.StartDate, newPhase.EndDate, oldPhase.pct_complete, oldPhase.IsCurrent, newPhase.updateBy);

                                        logMsg = "Updated Schedule " + oldPhase.PhaseName + " Phase End Date from " + oldPhase.EndDate.ToShortDateString() + " to " + newPhase.EndDate.ToShortDateString() + ".";
                                        data.AddChangeLog(int.Parse(projId), DateTime.Now, usr.UserId, logMsg, ref LogId);
                                    }

                                }
                                if (editField.Contains("Start"))
                                {
                                    phase = editField.Substring(0, editField.Length - 5);
                                    phaseId = data.GetPhaseIdByName(phase);
                                    oldPhase = data.GetProjectPhaseById(int.Parse(projId), phaseId);
                                    newPhase = new ProjectPhase();
                                    DateTime newDt = DateTime.Parse(fieldVal);
                                    if (newDt != oldPhase.StartDate)
                                    {
                                        newPhase.StartDate = newDt;
                                        newPhase.updateBy = usr.UserName;
                                        data.UpdProjectPhase(oldPhase.ProjectId, oldPhase.PhaseId, newPhase.StartDate, oldPhase.EndDate, oldPhase.pct_complete, oldPhase.IsCurrent, newPhase.updateBy);

                                        logMsg = "Updated Schedule " + oldPhase.PhaseName + " Phase Start Date from " + oldPhase.StartDate.ToShortDateString() + " to " + newPhase.StartDate.ToShortDateString() + ".";
                                        data.AddChangeLog(int.Parse(projId), DateTime.Now, usr.UserId, logMsg, ref LogId);
                                    }
                                }
                                if (editField.Contains("Pct"))
                                {
                                    phase = editField.Substring(0, editField.Length - 3);
                                    phaseId = data.GetPhaseIdByName(phase);
                                    oldPhase = data.GetProjectPhaseById(int.Parse(projId), phaseId);
                                    newPhase = new ProjectPhase();
                                    int newPct = int.Parse(fieldVal);
                                    if (newPct != oldPhase.pct_complete)
                                    {
                                        newPhase.pct_complete = newPct;
                                        newPhase.updateBy = usr.UserName;
                                        data.UpdProjectPhase(oldPhase.ProjectId, oldPhase.PhaseId, oldPhase.StartDate, oldPhase.EndDate, newPhase.pct_complete, oldPhase.IsCurrent, newPhase.updateBy);

                                        logMsg = "Updated Schedule " + oldPhase.PhaseName + " Phase Percent Complete from " + oldPhase.pct_complete + "% to " + newPhase.pct_complete + "%.";
                                        data.AddChangeLog(int.Parse(projId), DateTime.Now, usr.UserId, logMsg, ref LogId);
                                    }
                                }
                                retVal = "Success";
                                break;

                        }
                    }

                }
                else
                {
                    //retMsg = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    retVal = "No form data was found.";
                }
            }
            catch (Exception ex)
            {
                //retMsg = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //retMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                //retMsg.Content = new StringContent(ex.Message);
                retVal = ex.Message;
            }

            return retVal;
        }
        #endregion

        #region Impact To Operations

        public ActionResult ImpactToOperations()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    ViewBag.Access = UserHasAccess(int.Parse(Session["CurrProjectId"].ToString()));
                    ViewBag.DynamicImpacts = GetHtmlForProjectImpacts(int.Parse(Session["CurrProjectId"].ToString()));
                }
                catch (Exception)
                {
                    //Handle Error
                }
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public void AddImpact(int ProjectId, int ImpactId, string ImpactName)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.AddProjectImpact(ProjectId, ImpactId, null, usr.UserName);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "Impact: " + ImpactName + " added", ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public void UpdImpact(int ProjectId, int ImpactId, string desc)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.UpdProjectImpact(ProjectId, ImpactId, desc, usr.UserName);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "Other Impact description edited", ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public void DelImpact(int ProjectId, int ImpactId, string ImpactName)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.DelProjectImpact(ProjectId, ImpactId, usr.UserName);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "Impact: " + ImpactName + " removed", ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public string GetHtmlForProjectImpacts(int ProjectId)
        {
            List<ProjectImpact> impacts = data.GetProjectImpacts(ProjectId);

            StringBuilder sb = new StringBuilder();
            string newChkBx = "<input type=\"checkbox\" class=\"ui-corner-all chkBx\" id=\"";

            sb.Append("<div id=\"bubba\" class=\"ui-corner-all colWrap2\">");
            foreach (ProjectImpact impact in impacts)
            {
                if (impact.ImpactName != "Other Impacts")
                {
                    if (impact.Selected == 0)
                    {
                        sb.Append(newChkBx + impact.ImpactId + "\" onclick=\"javascript:UpdateImpact(" + ProjectId + ", " + impact.ImpactId + ", '" + impact.ImpactName + "', this)\" />  " + impact.ImpactName + "<br/>");
                    }
                    else
                    {
                        sb.Append(newChkBx + impact.ImpactId + "\" onclick=\"javascript:UpdateImpact(" + ProjectId + ", " + impact.ImpactId + ", '" + impact.ImpactName + "', this)\" checked />  " + impact.ImpactName + "<br/>");
                    }
                }
            }
            sb.Append("</div>");

            //Add textarea for Other Impacts
            sb.Append("<div style=\"width:200px\">");

            ProjectImpact other = impacts.Where(x => x.ImpactName == "Other Impacts").FirstOrDefault();
            if (other.Selected == 0)
            {
                sb.Append(newChkBx + other.ImpactId + "\" onclick=\"javascript:UpdateOtherImpact(" + ProjectId + ", " + other.ImpactId + ", '" + other.ImpactName + "', this)\" />  " + other.ImpactName + "<br/><textarea id=\"TextArea\" onchange=\"javascript:EditTextArea(" + ProjectId + ", " + other.ImpactId + ")\" style=\"height:100px;resize:none;\" disabled></textarea><input type=\"button\" id=\"Save\" style=\"display:none;\" value=\"Save edits to text area\" />");
            }
            else
            {
                sb.Append(newChkBx + other.ImpactId + "\" onclick=\"javascript:UpdateOtherImpact(" + ProjectId + ", " + other.ImpactId + ", '" + other.ImpactName + "', this)\" checked />  " + other.ImpactName + "<br/><textarea id=\"TextArea\" onchange=\"javascript:EditTextArea(" + ProjectId + ", " + other.ImpactId + ")\" style=\"height:100px;resize:none;\">" + other.OtherImpactDesc + "</textarea><input type=\"button\" id=\"Save\" style=\"display:inline;\" value=\"Save edits to text area\" />");
            }
            sb.Append("</div>");

            return sb.ToString();
        }

        #endregion

        #region Benefits

        public ActionResult Benefits()
        {
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    ViewBag.Access = UserHasAccess(int.Parse(Session["CurrProjectId"].ToString()));
                    ViewBag.DynamicBenefits = GetHtmlForProjectBenefits(int.Parse(Session["CurrProjectId"].ToString()));
                }
                catch (Exception)
                {
                    //Handle Error
                }
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }


        public void AddTask(phaseTasks proj)
        {
            int? TaskId = -1;
            data.AddProjectPhaseTasks(proj.ProjectId, proj.PhaseId, proj.TaskTitle, proj.StartDate, proj.EndDate, proj.pct_complete, proj.updateBy, ref TaskId);
           
        }
        public void updateTask(phaseTasks proj)
        {
           
            data.UpdProjectPhaseTasks(proj.ProjectId, proj.PhaseId, proj.TaskId, proj.TaskTitle, proj.StartDate, proj.EndDate, proj.pct_complete, proj.updateBy);
        }
        public void DeleteTask(phaseTasks proj)
        {
            data.deleteProjectPhaseTasks(proj.ProjectId, proj.PhaseId,  proj.TaskId, proj.updateBy);

        }

        public void AddBenefit(int ProjectId, int BenefitId, string BenefitName)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.AddProjectBenefit(ProjectId, BenefitId, 1,1,usr.UserName);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "Benefit: " + BenefitName + " added", ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public void DelBenefit(int ProjectId, int BenefitId, string BenefitName)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                data.DelProjectBenefit(ProjectId, BenefitId, usr.UserName);
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "Benefit: " + BenefitName + " removed", ref logId);
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public void UpdateCostSaving(int ProjectId, int CostSavingsTypeId, decimal CostSaving)
        {
            usr = (ProbeUser)Session["CurrProbeUser"];
            //get current CostSavings
            List<ProjectCostSaving> savings = data.GetProjectSavings(ProjectId);
            ProjectCostSaving OrigSaving = savings.Where(x => x.CostSavingsTypeId == CostSavingsTypeId).FirstOrDefault();

            //UpdProjectCostSavings
            data.UpdProjectCostSavings(ProjectId, CostSavingsTypeId, CostSaving, usr.UserName);
            data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, OrigSaving.CostSavingsType + " changed from $" + OrigSaving.CostSaving.ToString() + " to $" + CostSaving.ToString(), ref logId);
        }


        public string GetHtmlForProjectBenefits(int ProjectId)
        {
            List<ProjectCostSaving> savings = new List<ProjectCostSaving>();

            try {
                savings = data.GetProjectSavings(ProjectId);
            } catch (Exception ex) {
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, " ProjectSavings Error is  " + ex.ToString(), ref logId);
            }

            List<ProjectBenefit> benefits = new List<ProjectBenefit>();
            try {
                benefits = data.GetProjectBenefits(ProjectId);
            } catch (Exception ex)
            {
                data.AddChangeLog(ProjectId, DateTime.Now, usr.UserId, "GetProjectBenefits Error is  " + ex.ToString(), ref logId);
            }
            
            



            List<string> grps = data.GetBenefitGroups();

            IEnumerable<string> groups = benefits.Where(x => x.BenefitGroupName != "Cost Savings").Select(x => x.BenefitGroupName).Distinct();

            StringBuilder sb = new StringBuilder();
            string newChkBx = "<input type=\"checkbox\" class=\"ui-corner-all chkBx\" id=\"";
            string newTxtBx = "<input type=\"text\" onkeypress=\"return isNumberKey(event)\" class=\"ui-corner-all currency\" id=\"";

            //Add textboxes for cost savings
            sb.Append("<div class=\"ui-corner-all colWrap2\">");
            foreach (ProjectCostSaving saving in savings)
            {
                sb.Append("<div class=\"dontsplit\">" + saving.CostSavingsType + "($)<br/>");
                sb.Append(newTxtBx + saving.CostSavingsTypeId + "\" value=\"" + saving.CostSaving.ToString() + "\" onchange=\"javascript:UpdateCostSaving(" + ProjectId + ", " + saving.CostSavingsTypeId + ", this)\" /><br/></div>");
            }
            sb.Append("</div>");

            //Add all other Benefit sections
            sb.Append("<div style='float:left;width:450px;' class=\"ui-corner-all colWrap3\">");
            foreach (string group in grps)
            {
                if (group == "Compliance")
                {
                    sb.Append("</div>");
                    sb.Append("<div style='float:left;width:450px;' class=\"ui-corner-all colWrap4\">");
                }
                sb.Append("<div ><div style='float:left;width:300px;'><h5>" + group + "</h5></div><div style='float:left;padding-top:10px;'><b>LVL</b></div><div style='float:left;padding-top:10px;margin-left:16px;'><b>Value</b></div><div style='clear:both;'></div>");
                foreach (ProjectBenefit benefit in benefits)
                {

                    if (benefit.BenefitGroupName == group)
                    {
                        string[] bVal = new string[11];
                        int leng = bVal.Length;
                        for (int f = 0; f < 11; f++)
                        {
                            bVal[f] = "";
                        }

                        if (benefit.BenefitValue != null)
                        {
                            int y = Convert.ToInt32(benefit.BenefitValue);
                            bVal[y] = "selected";
                        }



                        string[] bLvL = new string[6];
                        int leng1 = bLvL.Length;
                        for (int f = 0; f < 6; f++)
                        {
                            bLvL[f] = "";
                        }

                        if (benefit.BenefitLevel != null)
                        {
                            int y1 = Convert.ToInt32(benefit.BenefitLevel);
                            bLvL[y1] = "selected";
                        }

                        string disAble = "disabled";

                        if (usr.IsAdmin)
                        {
                            disAble = "";
                        }

                        if (benefit.Selected == 0)
                        {
                            sb.Append("<div style='float:left;width:300px'>" + newChkBx + benefit.BenefitId + "\" onclick=\"javascript:UpdateBenefit(" + ProjectId + ", " + benefit.BenefitId + ", '" + benefit.BenefitName + "', this)\" />  " + benefit.BenefitName + "</div><div style='float:left;'><select style='width:35px' " + disAble + " id='benLevel" + benefit.BenefitId + "'><option value=1 " + bLvL[1] + ">1</option><option value=2 " + bLvL[2] + ">2</option><option value=3 " + bLvL[3] + ">3</option><option value=4 " + bLvL[4] + ">4</option><option value=5 " + bLvL[5] + ">5</option></select>&nbsp;<select class='benValue' id='benValue" + benefit.BenefitId + "' style='width:40px' " + disAble + "><option value=1 " + bVal[1] + "> 1</option><option value=2 " + bVal[2] + ">2</option><option value=3 " + bVal[3] + ">3</option><option value=4 " + bVal[4] + ">4</option><option value=5 " + bVal[5] + ">5</option><option value=6 " + bVal[6] + ">6</option><option value=7 " + bVal[7] + ">7</option><option value=8 " + bVal[8] + ">8</option><option value=9 " + bVal[9] + ">9</option><option value=10 " + bVal[10] + ">10</option></select></div><div style='clear:both;'></div>");
                        }
                        else
                        {
                            sb.Append("<div style='float:left;width:300px'>" + newChkBx + benefit.BenefitId + "\" onclick=\"javascript:UpdateBenefit(" + ProjectId + ", " + benefit.BenefitId + ", '" + benefit.BenefitName + "', this)\" checked/>  " + benefit.BenefitName + "</div><div style='float:left;'><select style='width:35px' " + disAble + "  id='benLevel" + benefit.BenefitId + "'><option value=1 " + bLvL[1] + ">1</option><option value=2 " + bLvL[2] + ">2</option><option value=3 " + bLvL[3] + ">3</option><option value=4 " + bLvL[4] + ">4</option><option value=5 " + bLvL[5] + ">5</option></select>&nbsp;<select class='benValue' id='benValue" + benefit.BenefitId + "' style='width:40px' " + disAble + "><option value=1 " + bVal[1] + " > 1</option><option value=2 " + bVal[2] + ">2</option><option value=3 " + bVal[3] + ">3</option><option value=4 " + bVal[4] + ">4</option><option value=5 " + bVal[5] + ">5</option><option value=6 " + bVal[6] + ">6</option><option value=7 " + bVal[7] + ">7</option><option value=8 " + bVal[8] + ">8</option><option value=9 " + bVal[9] + ">9</option><option value=10 " + bVal[10] + ">10</option></select></div><div style='clear:both;'></div>");
                        }
                    }
                }
                sb.Append("</div>");
                ViewBag.PROJiD = int.Parse(Session["CurrProjectId"].ToString());
            }
            sb.Append("</div>");
            return sb.ToString();
        }








        #endregion

        #region Risks

        public ActionResult Risks()
        {
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    ViewBag.Access = UserHasAccess(int.Parse(Session["CurrProjectId"].ToString()));
                    ProjectInfo selProject = data.GetProjectInfo(int.Parse(Session["CurrProjectId"].ToString()));
                    ViewBag.Likelihood = selProject.RiskLikelihood;
                    ViewBag.Impact = selProject.RiskImpact;
                }
                catch (Exception)
                {
                    //Handle Error
                }
                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public void UpdateRisk(string UpdateType, int Likelihood, int Impact)
        {
            try
            {
                ProjectInfo selProject = data.GetProjectInfo(int.Parse(Session["CurrProjectId"].ToString()));
                string OriginalLikelihood = RiskName(Convert.ToInt32(selProject.RiskLikelihood));
                string OriginalImpact = RiskName(Convert.ToInt32(selProject.RiskImpact));

                usr = (ProbeUser)Session["CurrProbeUser"];

                data.UpdProjectRisk(selProject.ProjectId, selProject.ActualStart, selProject.ActualEnd, Likelihood, Impact, usr.UserName);
                string desc = string.Empty;
                if (UpdateType == "LikelihoodRadios")
                {
                    string NewLikelihood = RiskName(Likelihood);
                    data.AddChangeLog(selProject.ProjectId, DateTime.Now, usr.UserId, "Risk Likelihood changed from " + OriginalLikelihood + " to " + NewLikelihood, ref logId);
                }
                else
                {
                    string NewImpact = RiskName(Impact);
                    data.AddChangeLog(selProject.ProjectId, DateTime.Now, usr.UserId, "Risk Impact changed from " + OriginalImpact + " to " + NewImpact, ref logId);
                }
            }
            catch (Exception)
            {
                //Handle Error
            }
        }

        public string RiskName(int level)
        {
            string desc = string.Empty;
            switch (level)
            {
                case 0:
                    desc = "None";
                    break;
                case 1:
                    desc = "Low";
                    break;
                case 2:
                    desc = "Medium";
                    break;
                case 3:
                    desc = "High";
                    break;
            }
            return desc;
        }

        #endregion

        private string UserHasAccess(int ProjectId)
        {
            ProjectInfo proj = data.GetProjectInfo(ProjectId);
            string HasAccess = "no";
            usr = (ProbeUser)Session["CurrProbeUser"];
            usr.ProjectRoleList = data.GetProjectRolesForUser(ProjectId, usr.UserName);
            if (proj.ApprovalStatus == "Proposed")
            {
                foreach (string pr in usr.ProjectRoleList)
                {
                    if (pr == "Sponsor" || pr == "Manager" || pr == "Engineer" || pr == "Creator" || pr == "Contributor")
                        HasAccess = "yes";
                }
            }
            else
            {
                if (!proj.InitApproved)
                {
                    if (usr.ProjectRoleList.Contains("Sponsor"))
                    {
                        HasAccess = "yes";
                    }
                }
                else
                {
                    foreach (string pr in usr.ProjectRoleList)
                    {
                        if (pr == "Sponsor" || pr == "Manager" || pr == "Engineer")
                            HasAccess = "yes";
                    }
                }
            }
            if (usr.IsAdmin)
                HasAccess = "yes";

            return HasAccess;
        }

        public ActionResult ChangeLog()
        {
            if (Session["CurrProbeUser"] != null)
            {
                Logs logs = new Logs();
                ViewBag.Srch = "";
                try
                {
                    logs.AllLogs = data.ChangeLogs(int.Parse(Session["CurrProjectId"].ToString()));

                }
                catch (Exception ex)
                {
                    string errMsg = ex.Message;
                    //Handle Error
                }

                return View(logs);
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string FilterChangeLog(string srchtxt = null)
        {
            string retval = "";
            Logs logs = new Logs();
            try
            {
                logs.AllLogs = data.ChangeLogs(int.Parse(Session["CurrProjectId"].ToString()));

            }
            catch (Exception ex)
            {
                retval = "Error: " + ex.Message;
                //Handle Error
            }
            if (!String.IsNullOrEmpty(srchtxt))
            {
                ViewBag.Srch = srchtxt;
                srchtxt = srchtxt.ToLower();
                //LogDt, UserName, LogMsg 
                logs.AllLogs = logs.AllLogs.Where(p => p.LogDt.ToShortDateString().ToLower().Contains(srchtxt) || (p.UserName != null ? p.UserName : "").ToLower().Contains(srchtxt) || (p.LogMsg != null ? p.LogMsg : "").ToLower().Contains(srchtxt)).ToList<ChangeLog>();

            }

            string tblbody = @"<tbody>
                                <tr>
                                    <th width='150px'>Created</th>
                                    <th width='150px'>User</th>
                                    <th width='925px'>Log</th>
                                </tr>";

            foreach (var item in logs.AllLogs)
            {
                tblbody += "<tr><td style='text-align:left'>" + item.LogDt + "</td>";
                tblbody += "<td style='text-align:left'>" + item.UserName + "</td>";
                tblbody += "<td style='text-align:left'>" + item.LogMsg + "</td></tr>";
            }
            tblbody += "</tbody>";

            retval = tblbody;


            return retval;
        }

        [HttpPost]
        public ActionResult AddToChangeLogInitiation()
        {
            usr = (ProbeUser)Session["CurrProbeUser"];
            string pId = Session["CurrProjectId"].ToString();
            int PID = Int32.Parse(pId);
            data.AddChangeLog(PID, DateTime.Now, usr.UserId, "Initiation Approved", ref logId);
            return View();
        }



        public ActionResult NewProject()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                NewProj proj = new NewProj();
                ViewBag.hasRole = false;
                ViewBag.Sent = false;
                try
                {
                    ViewBag.test = "test";
                    proj = PopulateProjectLists(proj);
                    usr = (ProbeUser)Session["CurrProbeUser"];
                    string sqltxt = @"SELECT 
	                              r.RoleName
                                  ,u.UserName
                              FROM Roles r
                              left join [probe].[dbo].[UserRoles] ur on r.RoleId = ur.RoleId
                              left join Users u on u.UserId = ur.UserId
                              where u.Active != 0 and u.UserName = '" + usr.UserName + "' and r.RoleName in('Project Creator','Admin')";

                    Database db = Database.Open("probedb");

                    var urol = db.Query(sqltxt);
                    ViewBag.urol = urol.Count();
                    ViewBag.sql = sqltxt;


                    if (urol.Count() == 0)
                    {
                        ViewBag.hasRole = false;
                        GuestUser reqcreat = new GuestUser();
                        reqcreat.Email = usr.Email;
                        reqcreat.FirstName = usr.FirstName;
                        reqcreat.LastName = usr.LastName;
                        reqcreat.Extension = usr.Phone;

                        proj.RequestCreator = reqcreat;
                    }
                    else
                    {
                        ViewBag.hasRole = true;
                    }


                    if (string.IsNullOrEmpty(usr.UserName))
                    { proj.updateBy = User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1); }
                    else { proj.updateBy = usr.UserName; }
                }
                catch (Exception ex)
                {
                    ViewBag.test = ex.ToString();
                    //Handle Error
                }

                return View(proj);
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public ActionResult NewProject(NewProj proj)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            try
            {
                ViewBag.AddMessage = "";
                ViewBag.MsgClass = "RedMessage";
                ViewBag.hasRole = false;
                ViewBag.Sent = false;
                string hasRoletxt = Request.Form["hasRole"].ToString();
                if (!String.IsNullOrEmpty(hasRoletxt))
                {
                    ViewBag.hasRole = bool.Parse(hasRoletxt);
                }

                List<int> ret = new List<int>();
                if (ModelState.IsValid)
                {
                    //Save to Projects
                    ret = data.AddProjectTwoOutputs(proj.ProjectName, proj.BudgetNum, proj.Scope, proj.Comments, proj.Customer, proj.ProjectType, proj.StartDate, proj.EndDate, proj.updateBy, 0, 0, proj.ServiceAgreementType, proj.ExpirationDate);
                    int projId = ret[0];
                    int usrid = ret[1];

                    //Save to ProjectUserRoles (Manager, Sponsor & Engineer if not null)
                    if (proj.Manager != null) { data.AddProjectUserRole(projId, int.Parse(proj.Manager), "Manager", proj.updateBy); }
                    if (proj.Sponsor != null) { data.AddProjectUserRole(projId, int.Parse(proj.Sponsor), "Sponsor", proj.updateBy); }
                    if (proj.Engineer != null) { data.AddProjectUserRole(projId, int.Parse(proj.Engineer), "Engineer", proj.updateBy); }

                    //Save to ProjectGroups if not null
                    if (proj.Group != null) { data.AddProjectGroup(projId, int.Parse(proj.Group), proj.updateBy); }

                    //Write a record to ChangeLog
                    data.AddChangeLog(projId, DateTime.Now, usrid, "Project Proposed", ref logId);

                    return RedirectToAction("SelectedProject", "Home", new { id = projId });
                }
                else
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                    if (proj.RequestCreator != null)
                    {
                        if (proj.RequestCreator.FirstName != null && proj.RequestCreator.LastName != null && proj.RequestCreator.Email != null && proj.RequestCreator.Extension != null)
                        {
                            GuestUser requsr = proj.RequestCreator;

                            MailMessage mail = new MailMessage("dewane.daley@gasoc.com", "dewane.daley@gasoc.com");
                            SmtpClient client = new SmtpClient();
                            client.Port = 25;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.UseDefaultCredentials = false;
                            client.Host = "mailrelay.gafoc.com";
                            mail.Subject = "PROBE User Request for the 'Create Project' role";
                            mail.IsBodyHtml = true;
                            string bodytxt = @"<html><body><h3>The following user has requested the Create Project role</h3><table>";

                            bodytxt += "<tr><td>First Name:</td><td>" + proj.RequestCreator.FirstName + "</td></tr>";
                            bodytxt += "<tr><td>Last Name:</td><td>" + proj.RequestCreator.LastName + "</td></tr>";
                            bodytxt += "<tr><td>Email:</td><td>" + proj.RequestCreator.Email + "</td></tr>";
                            bodytxt += "<tr><td>Extension:</td><td>" + proj.RequestCreator.Extension + "</td></tr></table></body></html>";

                            mail.Body = bodytxt;
                            client.Send(mail);

                            ViewBag.Sent = true;
                            ViewBag.AddMessage = "Your request has been sent.";
                        }
                        else
                        {
                            ViewBag.AddMessage = "All fields are required";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                ViewBag.AddMessage = "An Error was encountered while trying to add the project.";
            }
            proj = PopulateProjectLists(proj);
            return View(proj);
        }



        [HttpPost]
        public int NewProject_Temp(NewProj proj)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            try
            {
                ViewBag.AddMessage = "";
                ViewBag.MsgClass = "RedMessage";
                ViewBag.hasRole = false;
                ViewBag.Sent = false;
                ProbeUser pusr = (ProbeUser)Session["CurrProbeUser"];
                string hasRoletxt = "false";
                int y = pusr.ProjectRoleList.Count;
                if (y > 0)
                {
                    hasRoletxt = "true";
                }

                if (!String.IsNullOrEmpty(hasRoletxt))
                {
                    ViewBag.hasRole = bool.Parse(hasRoletxt);
                }

                List<int> ret = new List<int>();
         
                    //Save to Projects
                    ret = data.AddProjectTwoOutputs_Template(proj.ProjectName, proj.BudgetNum, proj.Scope, proj.Comments, proj.Customer, proj.ProjectType, proj.StartDate, proj.EndDate, pusr.UserName, 0, 0, proj.ServiceAgreementType, proj.ExpirationDate);
                    int projId = ret[0];
                    int usrid = ret[1];

                    //Save to ProjectUserRoles (Manager, Sponsor & Engineer if not null)
                    if (proj.Manager != null) { data.AddProjectUserRole(projId, int.Parse(proj.Manager), "Manager", pusr.UserName); }
                    if (proj.Sponsor != null) { data.AddProjectUserRole(projId, int.Parse(proj.Sponsor), "Sponsor", pusr.UserName); }
                    if (proj.Engineer != null) { data.AddProjectUserRole(projId, int.Parse(proj.Engineer), "Engineer", pusr.UserName); }

                    //Save to ProjectGroups if not null
                    if (proj.Group != null) { data.AddProjectGroup(projId, int.Parse(proj.Group), proj.updateBy); }

                    //Write a record to ChangeLog
                    data.AddChangeLog(projId, DateTime.Now, usrid, "Template Proposed", ref logId);
                    ViewBag.pid = projId;
                    //return RedirectToAction("SelectedProject", "Home", new { id = projId });
                
               
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return 1;
                ViewBag.AddMessage = "An Error was encountered while trying to add the project.";
            }
            proj = PopulateProjectLists(proj);
            return ViewBag.pid;
        }













        public ActionResult SelectedProject(string id, string SelectedTab)
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            ProbeUser pusr = (ProbeUser)Session["CurrProbeUser"];
            string username = pusr.UserName;
            if (Session["CurrProbeUser"] != null)
            {
                try
                {
                    //Check CurrProjectId session value.  If null then set to 0
                    if (Session["CurrProjectId"] == null)
                        Session["CurrProjectId"] = 0;
                    //Check CurrProjectId session value.  If IsNullOrEmpty then set to 0
                    if (string.IsNullOrEmpty(Session["CurrProjectId"].ToString()))
                    {
                        Session["CurrProjectId"] = 0;
                    }

                    //If id has been passed in set CurrProjectId session value.
                    if (!string.IsNullOrEmpty(id))
                    {
                        Session["CurrProjectId"] = id;
                    }

                    //Set ViewBag.SelectedTab (Default is 0)
                    ViewBag.SelectedTab = "0";
                    if (!string.IsNullOrEmpty(SelectedTab))
                    {
                        ViewBag.SelectedTab = SelectedTab;
                    }

                    //Populate the ProjectInfo model
                    //ProjectInfo selProject1 = data.GetProjectInfo(Convert.ToInt32(Session["CurrProjectId"].ToString()));

                    
                    ProjectInfo selProject = data.GetProjectInfo1(Convert.ToInt32(Session["CurrProjectId"].ToString()),username);


                   
                    //show or hide InitApprove button based on if sponsor or admin and the InitApprove flag is false or true
                    ViewBag.ShowInitApprove = false;
                    if (!selProject.InitApproved)
                    {
                       
                        if (pusr.ProjectRoleList == null)
                        {
                            pusr.ProjectRoleList = data.GetProjectRolesForUser(int.Parse(id), pusr.UserName);
                        }
                        if (pusr.IsAdmin || pusr.ProjectRoleList.Contains("Sponsor"))
                        {
                            ViewBag.ShowInitApprove = true;
                        }
                    }

                    return View(selProject);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public ActionResult ApprovedProjectInfo()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            Session["lastPage"] = "approved";
            if (Session["CurrProbeUser"] != null)
            {
                ProjectInfo selProject = new ProjectInfo();
                try
                {
                    int pid = int.Parse(Session["CurrProjectId"].ToString());

                    List<BudgetViewRecord> Details = data.GetBudgetsDetailsWTotalsRecords(pid);
                    IEnumerable<string> viewTypes = Details.Select(x => x.BudgetType).Distinct();

                    usr = (ProbeUser)Session["CurrProbeUser"];
                    usr.ProjectRoleList = data.GetProjectRolesForUser(pid, usr.UserName);
                    Session["CurrProbeUser"] = usr;

                    ViewBag.UserDisplayName = usr.DisplayName;
                    ViewBag.UserName = usr.UserName;

                    selProject = data.GetProjectInfo1(pid, usr.DisplayName);
                    //selProject.Vendors = data.getVendorInfo();
                    BudgetViewRecord r = new BudgetViewRecord();
                    foreach (BudgetViewRecord rcd in Details)
                    {
                        selProject.BudgetTotal = rcd.Budgeted;
                    }


                    if (selProject.ProjectedStart < Convert.ToDateTime("1/1/2000"))
                    { 
                        selProject.ProjectedStart = null;
                    }

                    if (selProject.ProjectedEnd < Convert.ToDateTime("1/1/2000"))
                    {
                        selProject.ProjectedEnd = null;
                    }

                    if (selProject.ActualStart < Convert.ToDateTime("1/1/2000"))
                    {
                        selProject.ActualStart = null;
                    }

                    if (selProject.ActualEnd < Convert.ToDateTime("1/1/2000"))
                    {
                        selProject.ActualEnd = null;
                    }
                    selProject = PopulateProjectInfoLists(selProject);
                    selProject.projectGroups = data.GetEachProjectGroup(pid);

                    IEnumerable<string> listofAgrees = data.GetServiceAgreementTypes();

                    var aggs = new List<SelectListItem>();
                    { aggs.Add(new SelectListItem { Value = "Unknown", Text = "" }); }
                    foreach (var x in listofAgrees)
                    {
                        aggs.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() });
                    }
                    selProject.warrantyAgreementTypes = aggs;
                    ViewBag.pid = pid;

                    IEnumerable<NewProj> listofVendors = data.GetVendorLists();
                    var vends = new List<SelectListItem>();
                    { vends.Add(new SelectListItem { Value = "Unknown", Text = "" }); }
                    foreach (var x in listofVendors)
                    {
                        string u = x.ServiceAgreementVendorId.ToString();
                        vends.Add(new SelectListItem { Value = u, Text = x.ServiceAgreementVendorName });
                    }
                    selProject.vendorList = vends;

                    int? permisson_flg = -1;
                    string permisson_msg = null;
                    //verify department admin
                    int check = data.verifyDeptAdmin(pid, usr.UserName, ref permisson_flg, ref permisson_msg);

                    List<string> gname = new List<string>();
                    List<string> val = new List<string>();
                    List<SelectListItem> groupName1 = new List<SelectListItem>();

                    List<string> newvals = new List<string>();
                    List<string> newvalsIndx = new List<string>();
                    foreach (var key in selProject.GroupList)
                    {
                        val.Add(key.Value);
                        gname.Add(key.Text);
                    }
                    var modifiedGlobalList = new List<SelectListItem>();
                    int indx = gname.Count;
                    foreach (var key in selProject.projectGroups)
                    {
                        int lengo = val.Count;
                        for (var i = 0; i < lengo; i++)
                        {
                            if (key.GroupId.ToString() == val[i])
                            {
                                newvals.Add(gname[i]);
                                newvalsIndx.Add(val[i]);
                            }
                        }

                    }

                    int newIndx = newvalsIndx.Count;
                    int valIndx = val.Count;
                    try
                    {
                        for (var t = 0; t < valIndx; t++)
                        {
                            for (var e = 0; e < newIndx; e++)
                            {
                                if (newvalsIndx[e] == val[t])
                                {
                                    Console.WriteLine(val[t]);
                                    gname.RemoveAt(t);
                                    val.RemoveAt(t);
                                    valIndx = valIndx - 1;
                                    t = t - 1;
                                }

                            }


                        }
                        //val = gname;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    int indx1 = gname.Count;
                    for (var d = 0; d < indx1; d++)
                    {
                        string gName = "LG" + val[d];
                        groupName1.Add(new SelectListItem { Value = gName, Text = gname[d] });
                    }
                    selProject.ProposedBudgetList = data.getApprovedTotalBudgetCost(pid);
                    selProject.BudgetedTotal = selProject.ProposedBudgetList[3].BudgetedTotal;
                    selProject.ProjectedTotal = selProject.ProposedBudgetList[3].ProjectedTotal;
                    selProject.ActualTotal = selProject.ProposedBudgetList[3].ActualTotal;

                    selProject.GroupList = groupName1;
                    selProject.ProjectGroupList = newvals;
                    selProject.ProjectGroupListIndex = newvalsIndx;
                    selProject.Creator = data.GetProjectCreator(pid);
                    selProject.ProjectBudgets = data.GetProjectInfoBudget(pid);
                   
                    List<projectAttachments> attchGroup = new List<projectAttachments>();
                    try
                    {
                        selProject.ProjectAttachments = data.GetProjectAttachments(pid).ToList();
                        try
                        {

                            attchGroup = data.GetProjectAttachmentsGroup().ToList();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        ViewBag.attachmentgroups = JsonConvert.SerializeObject(attchGroup);
                        ViewBag.attachments = JsonConvert.SerializeObject(selProject.ProjectAttachments);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    #region Role Permissions

                    bool isdisabled = true;
                    bool commentsdisabled = true;
                    bool DeletesDisabled = true;
                    bool securityset = false;

                    string CrudDisabledStr = "true";
                    string HideContribDisabledStr = "true";
                    if (usr.ProjectRoleList == null)
                    {
                        usr.ProjectRoleList = data.GetProjectRolesForUser(pid, usr.UserName);
                    }

                    if (selProject.ActivityStatus == "Completed" || selProject.ActivityStatus == "Cancelled")
                    {
                        if (usr.IsAdmin)
                        {
                            isdisabled = false;
                            CrudDisabledStr = "false";
                            HideContribDisabledStr = "false";
                            commentsdisabled = false;
                            DeletesDisabled = false;

                        }
                    }
                    else
                    {
                        if (usr.IsAdmin)
                        {
                            isdisabled = false;
                            CrudDisabledStr = "false";
                            HideContribDisabledStr = "false";
                            commentsdisabled = false;
                            securityset = true;
                            DeletesDisabled = false;
                        }

                        //checking here to see if the flag for approval to start the project after being Budget Approved is set to false or true
                        if (!securityset)
                        {
                            if (!selProject.InitApproved)
                            {
                                if (usr.ProjectRoleList.Contains("Sponsor"))
                                {
                                    isdisabled = false;
                                    CrudDisabledStr = "false";
                                    HideContribDisabledStr = "false";
                                    commentsdisabled = false;
                                    securityset = true;
                                }
                            }

                        }

                        if (!securityset)
                        {
                            if (usr.ProjectRoleList.Contains("Manager") || usr.ProjectRoleList.Contains("Engineer") || usr.ProjectRoleList.Contains("Sponsor"))
                            {
                                isdisabled = false;
                                CrudDisabledStr = "false";
                                HideContribDisabledStr = "false";
                                commentsdisabled = false;
                                securityset = true;
                                DeletesDisabled = false;
                            }
                        }

                        if (!securityset)
                        {
                            if (usr.ProjectRoleList.Contains("Contributor"))
                            {
                                commentsdisabled = false;
                                CrudDisabledStr = "false";
                                HideContribDisabledStr = "true";
                            }
                        }

                    }

                    ViewBag.DisplayNameDisabled = isdisabled;
                    ViewBag.BudgetNumDisabled = isdisabled;
                    ViewBag.PriorityDisabled = isdisabled;
                    ViewBag.ScopeDisabled = isdisabled;
                    ViewBag.JustificationDisabled = isdisabled;
                    ViewBag.ManagerDisabled = isdisabled;
                    ViewBag.SponsorDisabled = isdisabled;
                    ViewBag.EngineerDisabled = isdisabled;
                    ViewBag.ProjectTypeDisabled = isdisabled;
                    ViewBag.CustomerDisabled = isdisabled;
                    ViewBag.ActivityStatusDisabled = isdisabled;
                    ViewBag.ProjectedStartDisabled = isdisabled;
                    ViewBag.ProjectedEndDisabled = isdisabled;
                    ViewBag.ActualStartDisabled = isdisabled;
                    ViewBag.ActualEndDisabled = isdisabled;
                    ViewBag.GroupsDisabled = isdisabled;

                    ViewBag.CommentsDisabled = commentsdisabled;
                    ViewBag.DeletesDisabled = DeletesDisabled;
                    ViewBag.CrudDisabled = CrudDisabledStr;
                    ViewBag.HideContributors = HideContribDisabledStr;

                    #endregion

                    return View(selProject);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        public ActionResult ProposedProjectInfo()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            ViewBag.construction = "false";
            Session["lastPage"] = "proposed";
            ViewBag.DeletesDisabled = "true";
            if (Session["CurrProbeUser"] != null)
            {
                ProjectInfo selProject = new ProjectInfo();
                try
                {
                    int pid = int.Parse(Session["CurrProjectId"].ToString());
                    selProject = data.GetProjectInfo(pid);

                    List<projectAttachments> attchGroup = new List<projectAttachments>();
                    try
                    {
                        selProject.ProjectAttachments = data.GetProjectAttachments(pid).ToList();
                        try
                        {

                            attchGroup = data.GetProjectAttachmentsGroup().ToList();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        ViewBag.attachmentgroups = JsonConvert.SerializeObject(attchGroup);
                        ViewBag.attachments = JsonConvert.SerializeObject(selProject.ProjectAttachments);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    IEnumerable<NewProj> listofVendors = data.GetVendorLists();
                    var vends = new List<SelectListItem>();
                    { vends.Add(new SelectListItem { Value = "Unknown", Text = "" }); }
                    foreach (var x in listofVendors)
                    {
                        string u = x.ServiceAgreementVendorId.ToString();
                        vends.Add(new SelectListItem { Value = u, Text = x.ServiceAgreementVendorName });
                    }
                    selProject.vendorList = vends;

                    usr = (ProbeUser)Session["CurrProbeUser"];
                    usr.ProjectRoleList = data.GetProjectRolesForUser(pid, usr.UserName);
                    Session["CurrProbeUser"] = usr;
                    int? permisson_flg = -1;
                    string permisson_msg = null;
                    //verify department admin
                    int check = data.verifyDeptAdmin(pid, usr.UserName, ref permisson_flg, ref permisson_msg);
                    ViewBag.UserDisplayName = usr.DisplayName;
                    ViewBag.UserName = usr.UserName;

                    if (selProject.ProjectedStart < Convert.ToDateTime("1/1/2000"))
                    {
                        selProject.ProjectedStart = null;
                    }

                    if (selProject.ProjectedEnd < Convert.ToDateTime("1/1/2000"))
                    {
                        selProject.ProjectedEnd = null;
                    }

                    selProject = PopulateProjectInfoLists(selProject);
                    selProject.projectGroups = data.GetEachProjectGroup(pid);
                    List<string> gname = new List<string>();
                    List<SelectListItem> groupName1 = new List<SelectListItem>();
                    List<string> val = new List<string>();
                    List<string> newvals = new List<string>();
                    List<string> newvalsIndx = new List<string>();
                    foreach (var key in selProject.GroupList)
                    {
                        val.Add(key.Value);
                        gname.Add(key.Text);
                    }
                    var modifiedGlobalList = new List<SelectListItem>();
                    int indx = gname.Count;
                    foreach (var key in selProject.projectGroups)
                    {
                        int lengo = val.Count;
                        for (var i = 0; i < lengo; i++)
                        {
                            if (key.GroupId.ToString() == val[i])
                            {
                                newvals.Add(gname[i]);
                                newvalsIndx.Add(val[i]);
                            }
                        }

                    }

                    int newIndx = newvalsIndx.Count;
                    int valIndx = val.Count;
                    for (var t = 0; t < valIndx; t++)
                    {
                        for (var e = 0; e < newIndx; e++)
                        {
                            if (newvalsIndx[e] == val[t])
                            {
                                gname.RemoveAt(t);
                                val.RemoveAt(t);
                                valIndx = valIndx - 1;
                            }
                        }
                    }


                    int indx1 = gname.Count;
                    for (var d = 0; d < indx1; d++)
                    {
                        string gName = "LG" + val[d];
                        groupName1.Add(new SelectListItem { Value = gName, Text = gname[d] });
                    }

                    selProject.GroupList = groupName1;
                    try
                    {
                        selProject.ProposedBudgetList = data.getTotalBudgetCost(pid);
                        selProject.ProposedCapital = selProject.ProposedBudgetList[0].ProposedTotal;
                        selProject.ProposedExpense = selProject.ProposedBudgetList[1].ProposedTotal;
                        selProject.ProposedEtoP = selProject.ProposedBudgetList[2].ProposedTotal;

                        selProject.ProposedTotal= selProject.ProposedCapital+ selProject.ProposedExpense+ selProject.ProposedEtoP;


                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                    
                    selProject.ProjectGroupList = newvals;
                    selProject.ProjectGroupListIndex = newvalsIndx;
                    selProject.Creator = data.GetProjectCreator(pid);
                    selProject.ProjectBudgets = data.GetProjectInfoBudget(pid);
                    ViewBag.pid = pid;
                    selProject.ProjectAttachments = data.GetProjectAttachments(pid).ToList();
                    IEnumerable<string> listofAgrees = data.GetServiceAgreementTypes();

                    var aggs = new List<SelectListItem>();
                    { aggs.Add(new SelectListItem { Value = "Unknown", Text = "" }); }
                    foreach (var x in listofAgrees)
                    {
                        aggs.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() });
                    }
                    selProject.warrantyAgreementTypes = aggs;
                    #region Role Permissions


                    





                    if (usr.IsAdmin || check == 1)
                    {
                        ViewBag.DisplayNameDisabled = false;
                        ViewBag.BudgetNumDisabled = false;
                        ViewBag.PriorityDisabled = false;
                        ViewBag.ScopeDisabled = false;
                        ViewBag.ManagerDisabled = false;
                        ViewBag.SponsorDisabled = false;
                        ViewBag.EngineerDisabled = false;
                        ViewBag.ProjectTypeDisabled = false;
                        ViewBag.JustificationDisabled = false;
                        ViewBag.ProposedStartDisabled = false;
                        ViewBag.ProposedEndDisabled = false;
                        ViewBag.CustomerDisabled = false;
                        ViewBag.CommentsDisabled = false;
                        ViewBag.GroupsDisabled = false;
                        ViewBag.CrudDisabled = "false";
                        ViewBag.HideContributors = "false";
                        ViewBag.ApproveBtnDisabled = "false";
                        ViewBag.ActivityStatusDisabled = false;
                        ViewBag.DeleteBtnDisabled = "false";
                        ViewBag.DeletesDisabled = "false";
                    }
                    else
                    {
                        if (usr.ProjectRoleList.Count() > 0)
                        {
                            foreach (string role in usr.ProjectRoleList)
                            {
                                if (role == "Manager" || role == "Sponsor" || role == "Engineer" || role == "Creator")
                                {
                                    ViewBag.DisplayNameDisabled = false;
                                    ViewBag.BudgetNumDisabled = false;
                                    ViewBag.PriorityDisabled = true;
                                    ViewBag.ScopeDisabled = false;
                                    ViewBag.ManagerDisabled = false;
                                    ViewBag.SponsorDisabled = false;
                                    ViewBag.EngineerDisabled = false;
                                    ViewBag.ProjectTypeDisabled = false;
                                    ViewBag.ProposedStartDisabled = false;
                                    ViewBag.ProposedEndDisabled = false;
                                    ViewBag.CustomerDisabled = false;
                                    ViewBag.CommentsDisabled = false;
                                    ViewBag.GroupsDisabled = false;
                                    ViewBag.CrudDisabled = "false";
                                    ViewBag.HideContributors = "false";
                                    ViewBag.ApproveBtnDisabled = "true";
                                    ViewBag.ActivityStatusDisabled = true;
                                    ViewBag.DeletesDisabled = "false";

                                    if (role == "Creator")
                                    {
                                        ViewBag.DeleteBtnDisabled = "false";
                                        ViewBag.DeletesDisabled = "false";
                                    }
                                    else
                                    {
                                        ViewBag.DeleteBtnDisabled = "true";
                                    }

                                    break;
                                }
                                else if (role == "Contributor")
                                {
                                    ViewBag.DisplayNameDisabled = true;
                                    ViewBag.BudgetNumDisabled = true;
                                    ViewBag.PriorityDisabled = true;
                                    ViewBag.ScopeDisabled = true;
                                    ViewBag.ManagerDisabled = true;
                                    ViewBag.SponsorDisabled = true;
                                    ViewBag.EngineerDisabled = true;
                                    ViewBag.ProjectTypeDisabled = true;
                                    ViewBag.ProposedStartDisabled = true;
                                    ViewBag.ProposedEndDisabled = true;
                                    ViewBag.CustomerDisabled = true;
                                    ViewBag.CommentsDisabled = false;
                                    ViewBag.GroupsDisabled = true;
                                    ViewBag.CrudDisabled = "false";
                                    ViewBag.HideContributors = "true";
                                    ViewBag.ApproveBtnDisabled = "true";
                                    ViewBag.ActivityStatusDisabled = true;
                                    ViewBag.DeleteBtnDisabled = "true";
                                    ViewBag.DeletesDisabled = "false";
                                }
                                else
                                {
                                    ViewBag.DisplayNameDisabled = true;
                                    ViewBag.BudgetNumDisabled = true;
                                    ViewBag.PriorityDisabled = true;
                                    ViewBag.ScopeDisabled = true;
                                    ViewBag.ManagerDisabled = true;
                                    ViewBag.SponsorDisabled = true;
                                    ViewBag.EngineerDisabled = true;
                                    ViewBag.ProjectTypeDisabled = true;
                                    ViewBag.ProposedStartDisabled = true;
                                    ViewBag.ProposedEndDisabled = true;
                                    ViewBag.CustomerDisabled = true;
                                    ViewBag.CommentsDisabled = true;
                                    ViewBag.GroupsDisabled = true;
                                    ViewBag.CrudDisabled = "true";
                                    ViewBag.HideContributors = "true";
                                    ViewBag.ApproveBtnDisabled = "true";
                                    ViewBag.ActivityStatusDisabled = true;
                                    ViewBag.DeleteBtnDisabled = "true";
                                    ViewBag.DeletesDisabled = "false";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.DisplayNameDisabled = true;
                            ViewBag.BudgetNumDisabled = true;
                            ViewBag.PriorityDisabled = true;
                            ViewBag.ScopeDisabled = true;
                            ViewBag.ManagerDisabled = true;
                            ViewBag.SponsorDisabled = true;
                            ViewBag.EngineerDisabled = true;
                            ViewBag.ProjectTypeDisabled = true;
                            ViewBag.ProposedStartDisabled = true;
                            ViewBag.ProposedEndDisabled = true;
                            ViewBag.CustomerDisabled = true;
                            ViewBag.CommentsDisabled = true;
                            ViewBag.GroupsDisabled = true;
                            ViewBag.CrudDisabled = "true";
                            ViewBag.HideContributors = "true";
                            ViewBag.ApproveBtnDisabled = "true";
                            ViewBag.ActivityStatusDisabled = true;
                            ViewBag.DeleteBtnDisabled = "true";
                            ViewBag.DeletesDisabled = "true";
                        }

                        #endregion

                    }

                    return View(selProject);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }


        [HttpPost]
        public JsonResult getAttachments(int projectId)
        {
            projectAttachments ProjectAttachments = new projectAttachments();
            ProjectInfo selProject = new ProjectInfo();
            selProject.ProjectAttachments = data.GetProjectAttachments(projectId).ToList();
            //string h = JsonConvert.SerializeObject(selProject.ProjectAttachments);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var results = serializer.Serialize(selProject.ProjectAttachments);
            //var jsonResult = Json(selProject.ProjectAttachments, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            foreach (ProjectAttachments item in selProject.ProjectAttachments)
            {
                Console.WriteLine(item);
            }

            //serializer.MaxJsonLength = Int32.MaxValue;
            return Json(new { success = true });
        }









        [HttpPost]
        public JsonResult updateDisplay(configType mod)
        {
            int h = data.configupdate(mod.ProjectId,mod.ConfigType,mod.ConfigValue,mod.updateBy);
            return Json(new { success = true });
        }



        [HttpPost]
        public JsonResult addDisplay(configType mod)
        {
            int h = data.configAdd(mod.ProjectId, mod.ConfigType, mod.ConfigValue, mod.updateBy);
            return Json(new { success = "raise" });
        }
        

        [HttpPost]
        public JsonResult UpdateGroupData(ProjectInfo mod)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                int verify = data.AddProjectGroup(mod.ProjectId, mod.GroupId, usr.UserName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { success = true });
        }



        [HttpPost]
        public JsonResult DeleteProjectData(ProjectInfo mod)
        {
            usr = (ProbeUser)Session["CurrProbeUser"];

            try
            {
                int re;
                re = data.DelProjectGroup(mod.ProjectId, mod.GroupId, usr.UserName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { success = true });
        }
        

        [HttpPost]
        public JsonResult UpdateProjectData(ProjectInfo mod)
        {
            int h;
            try
            {
                ProjectInfo oldModel = data.GetProjectInfo(mod.ProjectId);
                usr = (ProbeUser)Session["CurrProbeUser"];
                DateTime? dt = new DateTime();
                DateTime? dt1 = oldModel.ProposedStart;
                

                
                string newtd = mod.warrantyAgreementDate.ToString();
                if (newtd == null)
                {
                    dt = null;
                }else
                {
                    dt = mod.warrantyAgreementDate;
                }
                bool? k = mod.TerminateServiceAgreementAlert;
                ProjectInfo mod1 = mod;
                ProjectInfo gr = oldModel;
                string VendI = mod.ServiceAgreementVendorId.ToString();
                int? newId = mod.ServiceAgreementVendorId;
                if (VendI == "Unknown" || VendI == "0")
                {
                    newId = null;
                }

                if (mod.update_Field_Info== "warrantyAgreement")
                {
                    string Fmsg = oldModel.ServiceAgreementType;
                    string Tmsg = mod.ServiceAgreementType;
                    if (Fmsg==null)
                    {
                        Fmsg = "No record selected";
                    }

                    if (Tmsg == null || Tmsg=="Unknown")
                    {
                        Tmsg = "No record selected";
                    }
                    string msg = "Contract agreement type changed to "+ Tmsg + " from "+ Fmsg;
                    int log = data.AddChangeLog(oldModel.ProjectId, DateTime.Now, usr.UserId, msg, ref logId);
                }
                
                h = data.UpdateProjectRecord(mod.ProjectId, mod.warrantyAgreement, usr.UserName, oldModel.Scope, oldModel.Comments, oldModel.ProjectType, oldModel.ProposedStart, oldModel.Customer, oldModel.ProposedEnd, dt, ref newId, usr.UserId, mod.TerminateServiceAgreementAlert,mod.ProjectedStart, mod.ProjectedEnd, mod.ActualStart, mod.ActualEnd);
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public int updateProjectBenefits(ProjectBenefit mod)
        {
            int y1 = Convert.ToInt32(mod.BenefitValue);
            int y2 = Convert.ToInt32(mod.BenefitLevel);
            int h = data.probe_UpdProjectBenefits(mod.ProjectId, mod.BenefitId, y1, y2, "estes");
            return h;
        }

        [HttpPost]
        public JsonResult UpdateProjectAttachment(projectAttachments mod)
        {
            int h;
            try
            {
                //int ProjectAttachmentId, int ProjectId, string AttachmentTitle, string AttachmentType
                Byte[] u = null;
                h = data.UpdAttachmentGroups(mod.ProjectAttachmentId, mod.ProjectId, null, null, u, null, mod.updateBy, mod.AttachmentGroupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
            return Json(new { success = true });
        }

        public int deleteProject(int projectId)
        {
            usr = (ProbeUser)Session["CurrProbeUser"];
            int ret = data.DeleteProject(projectId, usr.UserName);
            //string msg = "Project was deleted";
            //int log = data.AddChangeLog(projectId, DateTime.Now, usr.UserId, msg, ref logId);
            return ret;
        }

        #region Approve Project Init
        [HttpPost]
        public string approveProjectInit()
        {
            string retmsg = "success";
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                int projID = int.Parse(Session["CurrProjectId"].ToString());

                int? newUpdateId = -1;

                ProjectInfo oldpi = data.GetProjectInfo(projID);
                ProjectInfo newpi = new ProjectInfo();
                newpi.ProjectId = oldpi.ProjectId;
                newpi.ProjectName = oldpi.ProjectName;
                newpi.InitApproved = true;

                int ret = data.UpdateProject(oldpi, newpi, "InitApproved", usr.UserId, usr.UserName);
                //int log = data.AddChangeLog(projID, DateTime.Now, usr.UserId, "Project initiation has been approved.", ref logId);

                //send email to project manager and cc Admin and sponsor
                List<string> recips = new List<string>();


                string projMgr = data.GetProjectManager(projID);
                ProbeUser mgr = data.GetUser(projMgr);
                if (mgr != null)
                {
                    if (!String.IsNullOrEmpty(mgr.Email))
                    {
                        recips.Add(mgr.Email);
                    }
                }

                string projSpnsr = data.GetProjectManager(projID);
                ProbeUser spnsr = data.GetUser(projSpnsr);
                if (spnsr != null)
                {
                    if (!String.IsNullOrEmpty(spnsr.Email))
                    {
                        recips.Add(spnsr.Email);
                    }
                }

                if (!usr.IsAdmin)
                {
                    List<ProbeUser> admins = data.GetAdminUsers().ToList<ProbeUser>();
                    foreach (var adm in admins)
                    {
                        if (!String.IsNullOrEmpty(adm.Email))
                        {
                            recips.Add(adm.Email);
                        }

                    }
                }

                if (!String.IsNullOrEmpty(usr.Email))
                {
                    if (!recips.Contains(usr.Email))
                    {
                        recips.Add(usr.Email);
                    }
                }

                try
                {

                    MailMessage mail = new MailMessage("dewane.daley@gasoc.com", "");
                    foreach (string addr in recips)
                    {
                        mail.To.Add(addr);
                    }
                    SmtpClient client = new SmtpClient();

                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "mailrelay.gafoc.com";
                    mail.Subject = "PROBE Project : " + newpi.ProjectName + " has been approved for Initiation in PROBE.";
                    mail.IsBodyHtml = true;
                    string bodytxt = @"<html><body><h3>The following Project has been approved for Initiation in PROBE.</h3><table>";
                    bodytxt += "<tr><td>Project Name:</td><td>" + newpi.ProjectName + "</td></tr>";
                    bodytxt += "<tr><td>Approved By:</td><td>" + usr.DisplayName + "</td></tr>";
                    bodytxt += "<tr><td>Email:</td><td>" + usr.Email + "</td></tr>";
                    bodytxt += "<tr><td>Extension:</td><td>" + usr.Phone + "</td></tr></table></body></html>";

                    mail.Body = bodytxt;
                    client.Send(mail);
                }
                catch (Exception mailex)
                {
                    retmsg = "Error: Project was approved but with an error in sending email notifications. Please contact the site administrator about this error. " + mailex.Message;
                }
                return retmsg;

            }
            catch (Exception ex)
            {
                //throw ex;
                return "Error: " + ex.Message;
            }
        }
        #endregion

        #region ProjectInfoUpdates

        [HttpPost]
        public int addUpdate(int projID, string updateTxt)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                int? newUpdateId = -1;

                int ret = data.addProjectUpdate(projID, updateTxt, usr.UserId, usr.UserName, ref newUpdateId);
                int log = data.AddChangeLog(projID, DateTime.Now, usr.UserId, "Update added.", ref logId);


                return newUpdateId.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int changeUpdate(int projId, int updateID, string updateTxt, string updateOwnerId)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                int ret = data.updProjectUpdate(updateID, updateTxt, Convert.ToInt32(updateOwnerId), usr.UserName);
                int log = data.AddChangeLog(projId, DateTime.Now, usr.UserId, "Project update explanation changed.", ref logId);

                return ret;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int deleteUpdate(int projId, int updateID)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                int ret = data.delProjectUpdate(updateID, usr.UserName);
                int log = data.AddChangeLog(projId, DateTime.Now, usr.UserId, "Project update deleted.", ref logId);

                return ret;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ProjectInfoIssues

        [HttpPost]
        public int addIssue(int projID, string explanation, string status, string target, string resolution, string owner)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                int? newIssueId = -1;

                ProbeUser own = data.GetUser(owner);

                int ret = data.addProjectIssue(projID, explanation, Convert.ToDateTime(target), resolution, status, own.UserId, usr.UserName, ref newIssueId);
                int log = data.AddChangeLog(projID, DateTime.Now, usr.UserId, "Project issue added.", ref logId);


                return newIssueId.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int changeIssue(int issueID, int projectId, string explanation, string status, string target, string resolution, string owner)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                ProbeUser own = data.GetUser(owner);

                if (status == "")
                    status = "Unknown";

                int ret = data.updProjectIssue(issueID, projectId, explanation, Convert.ToDateTime(target), resolution, status, own.UserId, usr.UserName);
                int log = data.AddChangeLog(projectId, DateTime.Now, usr.UserId, "Project issue changed.", ref logId);


                return ret;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int deleteIssue(int projId, int issueID)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                int ret = data.delProjectIssue(issueID, usr.UserName);
                int log = data.AddChangeLog(projId, DateTime.Now, usr.UserId, "Project issue deleted.", ref logId);

                return ret;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ProjectInfoAttachments

        [HttpPost]
        public ActionResult addAttachment(int projectId, int attachmentGroupIds, IEnumerable<HttpPostedFileBase> uploadFile)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                if (uploadFile.Count() > 0)
                {
                    foreach (var ufile in uploadFile)
                    {
                        if (ufile != null)
                        {
                            ViewBag.AttachmentMessage = "";

                            byte[] bits = new byte[ufile.InputStream.Length];

                            ufile.InputStream.Read(bits, 0, bits.Length);

                            string filetype = Path.GetExtension(ufile.FileName);



                            string shortfilName = ufile.FileName.Substring(ufile.FileName.LastIndexOf("\\") + 1);

                            int ret = data.addProjectAttachment(projectId, shortfilName, bits, filetype, usr.UserName, attachmentGroupIds);
                            int log = data.AddChangeLog(projectId, DateTime.Now, usr.UserId, "Project attachment added.", ref logId);
                        }
                    }
                }

                return RedirectToAction("SelectedProject", new { id = projectId.ToString() });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult viewAttachment(int attachmentId)
        {
            try
            {
                ProjectAttachments myfile = data.GetSelectedProjectAttachment(attachmentId);
                return File(myfile.AttachmentBinary, "application/octet-stream", myfile.AttachmentTitle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public int deleteAttachment(int projId, int attachmentId)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
                try
                {
                    int ret = data.delProjectAttachment(attachmentId, usr.UserName);
                    return ret;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int log = data.AddChangeLog(projId, DateTime.Now, usr.UserId, "Project attachment deleted.", ref logId);



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ProjectInfoContributors
        [HttpPost]
        public int addContributor(int projectId, string uname)
        {
            usr = (ProbeUser)Session["CurrProbeUser"];
            ProbeUser x = data.GetUser(uname);
            int ret = data.AddProjectUserRole(projectId, x.UserId, "Contributor", usr.UserName);
            string msg = x.DisplayName + " added as contributor";
            int log = data.AddChangeLog(projectId, DateTime.Now, usr.UserId, msg, ref logId);

            return ret;
        }
        [HttpPost]
        public int deleteContributor(int projectId, int contributorId)
        {
            usr = (ProbeUser)Session["CurrProbeUser"];
            int ret = data.DelProjectUserRole(projectId, contributorId, "Contributor");
            ProbeUser x = data.GetUser(contributorId);
            string msg = "Contributor " + x.DisplayName + " was removed";
            int log = data.AddChangeLog(projectId, DateTime.Now, usr.UserId, msg, ref logId);
            return ret;
        }

        #endregion

        public NewProj PopulateProjectLists(NewProj proj)
        {
            //Users list
            IEnumerable<ProbeUser> usrs = data.usrs();
            var users = new List<SelectListItem>();
            users.Add(new SelectListItem { Value = null, Text = null, Selected = true });
            foreach (ProbeUser x in usrs)
            { users.Add(new SelectListItem { Value = x.UserId.ToString(), Text = x.DisplayName }); }
            proj.Users = users;

            //Required Users list
            IEnumerable<ProbeUser> reqUsrs = data.usrs();
            var reqUsers = new List<SelectListItem>();
            foreach (ProbeUser x in reqUsrs)
            { reqUsers.Add(new SelectListItem { Value = x.UserId.ToString(), Text = x.DisplayName }); }
            proj.RequiredUsers = reqUsers;

            //Groups list
            IEnumerable<Group> grps = data.grps();
            var groups = new List<SelectListItem>();
            groups.Add(new SelectListItem { Value = null, Text = null, Selected = true });
            foreach (var x in grps)
            { groups.Add(new SelectListItem { Value = x.GroupId.ToString(), Text = x.GroupName }); }
            proj.Groups = groups;

            //ProjectTypes list
            IEnumerable<string> typs = data.projTyps();
            var types = new List<SelectListItem>();
            types.Add(new SelectListItem { Value = null, Text = null, Selected = true });
            foreach (var x in typs)
            { types.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() }); }
            proj.ProjectTypes = types;

            //Customers list
            IEnumerable<string> cstmrs = data.custmrs();
            var customers = new List<SelectListItem>();
            customers.Add(new SelectListItem { Value = null, Text = null, Selected = true });
            foreach (var x in cstmrs)
            { customers.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() }); }
            proj.Customers = customers;

            //Agreements list
            IEnumerable<string> agreemts = data.GetServiceAgreementTypes();
            var ServiceAgreements = new List<SelectListItem>();
            ServiceAgreements.Add(new SelectListItem { Value = null, Text = null, Selected = true });
            foreach (var x in agreemts)
            { ServiceAgreements.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() }); }
            proj.Agreements = ServiceAgreements;

            return proj;
        }

        public ProjectInfo PopulateProjectInfoLists(ProjectInfo project)
        {
            try
            {
                //Managers List
                IEnumerable<ProbeUser> usrs = data.usrs();
                NewProj serviceAgree = data.getWarrantyInfo(project.ProjectId);
                NewProj VendorName= data.getVendorInfo(project.ProjectId);
                var items = new List<SelectListItem>();
                var agreements = new List<SelectListItem>();
                items.Add(new SelectListItem { Value = null, Text = null, Selected = true });
                //project.warrantyAgreements = serviceAgree[0].ServiceAgreementExpDt;

                project.warrantyAgreement = serviceAgree.ServiceAgreementType;
                project.warrantyAgreementDate = serviceAgree.ServiceAgreementExpDt;
                project.Manager = data.GetProjectManager(project.ProjectId);


                foreach (var x in usrs)
                {
                    if (x.UserName == project.Manager)
                    {
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName, Selected = true });
                    }
                    else
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName });
                }

                project.ManagerList = items;

                //Sponsors
                items = new List<SelectListItem>();
                items.Add(new SelectListItem { Value = null, Text = null, Selected = true });

                project.Sponsor = data.GetProjectSponsor(project.ProjectId);
                foreach (var x in usrs)
                {
                    if (x.UserName == project.Sponsor)
                    {
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName, Selected = true });
                    }
                    else
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName });
                }

                project.SponsorList = items;

                //Engineers
                items = new List<SelectListItem>();
                items.Add(new SelectListItem { Value = null, Text = null, Selected = true });

                project.Engineer = data.GetProjectEngineer(project.ProjectId);
                foreach (var x in usrs)
                {
                    if (x.UserName == project.Engineer)
                    {
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName, Selected = true });
                    }
                    else
                        items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName });
                }
                project.EngineerList = items;

                //Contributors
                items = new List<SelectListItem>();
                IEnumerable<ProbeUser> contributors = data.GetListofAvailableContributors(project.ProjectId);

                foreach (var x in contributors)
                {
                    items.Add(new SelectListItem { Value = x.UserName, Text = x.DisplayName });
                }

                project.ContributorList = items;

                IEnumerable<ProjectTypes> pTypes = data.GetProjectTypesData();

                //Budget Types List
                var budTypes = new List<SelectListItem>();
                project.BudgetTypeList = new List<SelectListItem>();
                budTypes.Add(new SelectListItem { Value = "Unknown", Text = "" });

                int ptylength = pTypes.Count();
                foreach(var f in pTypes)
                {
                    string l = f.ProjectType;
                   budTypes.Add(new SelectListItem { Value = f.ProjectType, Text = f.ProjectType });
                }

                foreach (var x in budTypes)
                {
                    if (x.Value == project.ProjectType)
                    {
                        x.Selected = true;
                        break;
                    }
                }
                project.BudgetTypeList = budTypes;

                //Customers List
                IEnumerable<string> cstmrs = data.custmrs();
                var customers = new List<SelectListItem>();
                customers.Add(new SelectListItem { Value = null, Text = null, Selected = true });
                foreach (var x in cstmrs)
                {
                    if (string.IsNullOrEmpty(x.ToString()))
                    { customers.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString(), Selected = true }); }
                    else if (x.ToString() == project.Customer)
                    { customers.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString(), Selected = true }); }
                    else
                    { customers.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() }); }
                }
                project.CustomerList = customers;

                //Activity Status
                IEnumerable<string> activityStatus = data.activityStatuses(project.ApprovalStatus);
                var actStatList = new List<SelectListItem>();
                foreach (var x in activityStatus)
                {
                    if (string.IsNullOrEmpty(x.ToString()))
                    { actStatList.Add(new SelectListItem { Value = null, Text = null, Selected = true }); }
                    else if (x.ToString() == project.ActivityStatus)
                    { actStatList.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString(), Selected = true }); }
                    else
                    { actStatList.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() }); }
                }

                project.ActivityStatusList = actStatList;
                var appList = new List<SelectListItem>();
                appList.Add(new SelectListItem { Value = "Approved", Text = "Approved" });
                appList.Add(new SelectListItem { Value = "Proposed", Text = "Proposed", Selected = true });
                project.ApprovalStatusList = appList;

                //Groups
                IEnumerable<Group> groups = data.grps();
                items = new List<SelectListItem>();
                items.Add(new SelectListItem { Value = null, Text = null, Selected = true });

                project.ProjectGroup = data.GetProjectGroup(project.ProjectId);
                if (project.ProjectGroup == null)
                {
                    project.ProjectGroup = new Group();
                    project.ProjectGroup.GroupId = 0;
                    project.ProjectGroup.GroupName = "";
                }

                foreach (var x in groups)
                {
                    if (x.GroupId == project.ProjectGroup.GroupId)
                    {
                        items.Add(new SelectListItem { Value = x.GroupId.ToString(), Text = x.GroupName, Selected = true });
                    }
                    else
                        items.Add(new SelectListItem { Value = x.GroupId.ToString(), Text = x.GroupName });
                }
                project.GroupList = items;

                return project;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReconcileProjectDates(ProjectInfo model, string changedField)
        {
            string doPopup = "false";
            ProjectInfo oldModel = data.GetProjectInfo(model.ProjectId);
            int x = 0;
            DateTime oldDt;
            DateTime newDt;

            switch (changedField)
            {
                case "ActualStart":
                    if (model.ActualStart == null)
                    {
                        if (oldModel.ActualStart != null)
                        {
                            doPopup = "true";
                        }
                    }
                    else if (oldModel.ActualStart != null)
                    {
                        oldDt = oldModel.ActualStart.Value;
                        newDt = model.ActualStart.Value;

                        x = oldDt.CompareTo(newDt);

                        if (x < 0)
                        {
                            if (oldDt.Year == newDt.Year)
                            {
                                if ((oldDt.Month - newDt.Month) > 0)
                                {
                                    doPopup = "true";
                                }
                            }
                            else
                            {
                                doPopup = "true";
                            }
                        }
                    }
                    else
                    {
                        if (oldModel.ActivityStatus == "Not Started")
                            doPopup += "|This will set the Activity Status to 'In Progress'.";
                    }
                    break;
                case "ActualEnd":
                    if (model.ActualEnd == null)
                    {
                        if (oldModel.ActualEnd != null)
                        {
                            doPopup = "true";
                        }
                    }
                    else if (oldModel.ActualEnd != null)
                    {
                        oldDt = oldModel.ActualEnd.Value;
                        newDt = model.ActualEnd.Value;

                        x = oldDt.CompareTo(newDt);

                        if (x > 0)
                        {
                            if (oldDt.Year == newDt.Year)
                            {
                                if ((oldDt.Month - newDt.Month) > 0)
                                {
                                    doPopup = "true";
                                }
                            }
                            else
                            {
                                doPopup = "true";
                            }
                        }
                    }

                    if (oldModel.ActivityStatus != "Completed" && model.ActualEnd != null)
                        doPopup += "|This will set the Activity Status to 'Completed' and can only be reversed by the Admin User.";
                    break;
                case "ProjectedStart":
                    //Projected dates should never be null

                    oldDt = oldModel.ProjectedStart.Value;
                    newDt = model.ProjectedStart.Value;

                    x = oldDt.CompareTo(newDt);

                    if (x < 0)
                    {
                        if (oldDt.Year == newDt.Year)
                        {
                            if ((oldDt.Month - newDt.Month) > 0)
                            {
                                doPopup = "true";
                            }
                        }
                        else
                        {
                            doPopup = "true";
                        }
                    }

                    break;
                case "ProjectedEnd":

                    oldDt = oldModel.ProjectedEnd.Value;
                    newDt = model.ProjectedEnd.Value;

                    x = oldDt.CompareTo(newDt);

                    if (x > 0)
                    {
                        if (oldDt.Year == newDt.Year)
                        {
                            if ((oldDt.Month - newDt.Month) > 0)
                            {
                                doPopup = "true";
                            }
                        }
                        else
                        {
                            doPopup = "true";
                        }
                    }

                    break;
            }
            return doPopup;
        }

        public int UpdateProjectInfo(ProjectInfo model, string changedField)
        {
            try
            {
                usr = (ProbeUser)Session["CurrProbeUser"];

                ProjectInfo oldModel = data.GetProjectInfo(model.ProjectId);
                oldModel = PopulateProjectInfoLists(oldModel);
                if (model.warrantyAgreement == "")
                {

                }

                if (changedField == "ApprovalStatus")
                {
                    model.ApprovalStatus = "Approved";
                }
               
                else if (changedField == "ActualStart")
                {
                    if (string.IsNullOrEmpty(model.ActualStart.ToString()))
                    {
                        if (model.ActivityStatus == "In Progress")
                        {
                            model.ActivityStatus = "Not Started";
                        }
                    }
                    else
                    {
                        if (model.ActivityStatus == "Not Started")
                        {
                            model.ActivityStatus = "In Progress";
                        }
                    }
                }
                else if (changedField == "ActualEnd")
                {
                    if (string.IsNullOrEmpty(model.ActualEnd.ToString()))
                    {
                        if (model.ActivityStatus == "Completed")
                        {
                            model.ActivityStatus = "In Progress";
                        }
                    }
                    else
                    {
                        if (model.ActivityStatus == "In Progress")
                        {
                            model.ActivityStatus = "Completed";
                        }
                    }

                }
                else if (changedField == "ActivityStatus")
                {
                    switch (model.ActivityStatus)
                    {
                        case "Not Started":
                            if (!string.IsNullOrEmpty(model.ActualStart.ToString()))
                                model.ActualStart = null;

                            break;
                        case "In Progress":
                            if (string.IsNullOrEmpty(model.ActualStart.ToString()) || model.ActualStart < Convert.ToDateTime("1/1/2000"))
                                model.ActualStart = DateTime.Now;//model.ProjectedStart;

                            break;
                        case "Completed":
                            if (string.IsNullOrEmpty(model.ActualEnd.ToString()) || model.ActualEnd < Convert.ToDateTime("1/1/2000"))
                            {
                                model.ActualEnd = DateTime.Now;//model.ProjectedEnd;
                            }
                            break;

                        case "Cancelled":

                            break;

                        case "On Hold":

                            break;
                    }
                }
                else if (changedField == "Justification")
                {
                    model.InitApproved = true;
                }else if (changedField == "TerminateAgreement")
                {


                }

                int ret = data.UpdateProject(oldModel, model, changedField, usr.UserId, usr.UserName);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult TPage1()
        {
            return View();
        }

        public ActionResult SelectedReport(string RptNm)
        {

            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                ViewBag.Title = RptNm;
                switch (RptNm)
                {
                    case "Project Information Report":
                        //PgMO Report
                        Session["ActiveReportPath"] = "/DSS/Probe/GeneralReports/ProjInfoByYear";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Budgets Report":
                        //PgMO Report
                        Session["ActiveReportPath"] = "/DSS/Probe/PgMOReports/ProjectBudget";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Benefits Report":
                        //PgMO Report
                        Session["ActiveReportPath"] = "/DSS/Probe/PgMOReports/ProjectBenefits";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Resources Report":
                        //PgMO Report
                        Session["ActiveReportPath"] = "/DSS/Probe/PgMOReports/ProjectResources";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Information By Year and Project Manager":
                        //General Report
                        Session["ActiveReportPath"] = "/DSS/Probe/GeneralReports/ProjInfoByYear";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Information By Year and Project Group":
                        //General Report
                        Session["ActiveReportPath"] = "/DSS/Probe/GeneralReports/ProjInfoByGroup";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Information By Year and Project Engineer":
                        //General Report
                        Session["ActiveReportPath"] = "/DSS/Probe/GeneralReports/ProjInfoByYear";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Resources By Year and Resource Group":
                        //Department Report
                        Session["ActiveReportPath"] = "/DSS/Probe/DepartmentReports/ManHoursByYearGroupResource";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Information By Year and Department":
                        //Department Report
                        Session["ActiveReportPath"] = "/DSS/Probe/DepartmentReports/ProjBudgetByYearDept";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Project Budgets By Project and Year":
                        //Executive Report
                        Session["ActiveReportPath"] = "/DSS/Probe/ExecutiveReports/BudByProjYear";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Budgets Totals By Year":
                        //Executive Report
                        Session["ActiveReportPath"] = "/DSS/Probe/ExecutiveReports/BudTotByYear";
                        Session["ActiveReportName"] = RptNm;

                        break;
                    case "Status Update Report":
                        //Admin Reports
                        Session["ActiveReportPath"] = "/DSS/Probe/AdminReports/AdminOverdueStatusUpdate";
                        Session["ActiveReportName"] = RptNm;

                        break;

                    case "Global Search Report":
                        Session["ActiveReportPath"] = "/DSS/Probe/GeneralReports/SearchProjData";
                        Session["ActiveReportName"] = RptNm;
                        var c = System.Web.HttpContext.Current;
                        string v = c.Request.QueryString["val"];
                        Session["Reporttytpe"] = v;
                        break;




                    default:
                        break;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }

        }





    }


}
