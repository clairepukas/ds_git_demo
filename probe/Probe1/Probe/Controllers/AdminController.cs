using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using System.Collections;
using System.Reflection;
using System.Configuration;
using System.Web.Script.Serialization;


namespace Probe.Controllers
{
    public class AdminController : Controller
    {
        ProbeData data = ProbeDataContext.GetDataContext();
        ProbeUser usr = null;
        int? logId = -1;


        #region Admin Entity Resources
        public ActionResult AdminEntityResources()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //List<DashboardProject> dbProjects = data.GetDashboardProjects();

                //foreach (var proj in dbProjects)
                //{
                //    if (proj.EndDate < DateTime.Today)
                //    {
                //        if (proj.Health != 2)
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 2, usr.UserName);
                //    }
                //    else if (proj.StartDate < DateTime.Today && proj.ActivityStatus == "Not Started")
                //    {
                //        if (proj.Health != 1)
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 1, usr.UserName);
                //    }
                //}

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasResourcesEdit = false;
                bool hasResourcesEdit = false;

                if (usr.IsAdmin)
                {
                    hasResourcesEdit = true;
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
                    ViewBag.hasResourcesEdit = true;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string AdminEntityResourcesHandler(string RsrcId = null, string EntId = null, string ActTyp = null, string comments = null, string yr = null, string mhrs = null, string rsrcTitle = null, string rsrcEmail = null, string rsrcPhone = null, string parentent = null)
        {

            // bool hasEdit = false;
            ProbeUser usr = new ProbeUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
            }

            ViewBag.hasResourcesEdit = false;
            bool hasResourcesEdit = false;

            if (usr.IsAdmin)
            {
                hasResourcesEdit = true;
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
                                r.EntityId                                
                                from Resources r 
                                where r.EntityId = " + EntId +
                                            @" and r.ResourceId=" + RsrcId;

                            var recinfo = db.Query(sqltxt, null);
                            retVal = "<h4>" + recinfo.FirstOrDefault().ResourceName + "</h4>";
                            retVal += "<form id='frmRsrcInfo' name='frmRsrcInfo' >";
                            retVal += "<table><tr>";
                            retVal += "<td><span>Title:</span><span></td><td><input id='ResourceTitle' name='ResourceTitle' type='text' value='" + recinfo.FirstOrDefault().ResourceTitle + "' /></td></tr>";
                            retVal += "<tr><td><span>Email:</span></td><td><input id='ResourceEmail' name='ResourceEmail' type='email' value='" + recinfo.FirstOrDefault().ResourceEmail + "' /></td></tr>";
                            retVal += "<tr><td><span>Phone:</span></td><td><input id='ResourcePhone' name='ResourcePhone' type='tel' value='" + recinfo.FirstOrDefault().ResourcePhone + "' /></td></tr>";
                            retVal += "<tr><td><span>Comments:</span></td><td><textarea id='rsrc_comments' name='rsrc_comments' rows='10' cols='4' >" + recinfo.FirstOrDefault().Comments + "</textarea></td></tr>";
                            retVal += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveRsrcInfo()' />";
                            retVal += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retVal += "<input id='rsrcId' name='rsrcId' type='hidden' value='" + RsrcId + "' />";
                            retVal += "</form>";

                            break;

                        case "rsrcnotes":
                            var rec = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            retVal = "<h4>" + rec.FirstOrDefault().ResourceName + "</h4>";
                            retVal += "<form id='frmComments' name='frmComments'>";
                            retVal += "<table><tr><td>Comments:</td>";
                            retVal += "<td><textarea id='ta_comments' name='ta_comments' rows='10' cols='4' >" + rec.FirstOrDefault().Comments;
                            retVal += "</textarea></td></tr>";
                            retVal += "<tr><td>&nbsp;</td><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveRsrcComments()' />";
                            retVal += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeNotesDialog()'/></td></tr></table>";
                            retVal += "<input id='rsrcId' name='rsrcId' type='hidden' value='" + RsrcId + "' />";
                            retVal += "</form>";

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
                            sqltxt = @"select
                                        r.ResourceName,  
								        rm.*                            
                                        from ResourceManHours rm  
                                        join Resources r on r.ResourceId = rm.ResourceId
                                        where  rm.ResourceId= " + RsrcId + " and rm.[Year] = " + yr;

                            var recs = db.Query(sqltxt, null);
                            if (recs.Count() > 0)
                            {
                                retVal += "<form id='frmRsrcEdit' name='frmRsrcEdit'>";
                                retVal += "<h5>Resource Man Hours for : " + recs.FirstOrDefault().ResourceName + "</h5>";
                                retVal += "<h5>Year: " + recs.FirstOrDefault().Year + "</h5>";
                                retVal += "<br/><span>Total Base:</span><input id='totBase' name='totBase'  type='text' readonly />&nbsp;&nbsp;<span>Total Core:</span><input type=text' id='totCore' name='totCore'  readonly />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                retVal += "<br/><span style='font-size:.75em'>(enter values here to populate all months. -->)</span>&nbsp;&nbsp;<span>All Base:</span><input id='allBase' name='allBase'  type='text' value='0'   onkeypress='return isNumberKey(event)' onkeyup='getKey();' />&nbsp;&nbsp;<span>All Core:</span><input type=text' id='allCore' name='allCore' value='0'   onkeypress='return isNumberKey(event)' onkeyup='getKey();' />";
                                retVal += "<br/> <br/><div class='mhrWrapper'>";
                                foreach (var rsrcrec in recs)
                                {
                                    retVal += "<div class='mnthWrapper'><div class='iBlock'>";
                                    retVal += "<div class='mnthLbl'>" + mnlist[rsrcrec.Month] + "</div>";
                                    retVal += "<div><input class='mhrLbl' value='Base:' readonly />";
                                    retVal += "<input type ='text' id='base_" + rsrcrec.Month + "' name='base_" + rsrcrec.Month + "' value='" + rsrcrec.BaseManHours + "'  onkeypress='return isNumberKey(event)' onkeyup='getKey();' /></div>";
                                    retVal += "<div><input class='mhrLbl' value='Core:' readonly />";
                                    retVal += "<input class='mhrLbl' type ='text' id='core_" + rsrcrec.Month + "' name='core_" + rsrcrec.Month + "' value='" + rsrcrec.CoreManHours + "' onkeypress='return isNumberKey(event)' onkeyup='getKey();' /></div>";
                                    retVal += "</div><div class='cmtBlock'><input type='text' readonly class='mhrLbl' value='Comments:' /><br/>";
                                    retVal += "<textarea id='cmts_" + rsrcrec.Month + "' name='cmts_" + rsrcrec.Month + "'  rows='5' cols='40'  >" + rsrcrec.Comments + "</textarea></div></div>";
                                }
                                retVal += "</div>";
                                if (hasResourcesEdit)
                                {
                                    retVal += "<br/><div style='float:left'><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveRsrcEdits()' /></div>";
                                    retVal += "<div style='float:left'><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeEditDialog()'/></div>";
                                }
                                else
                                {
                                    retVal += "<br/><div style='float:left'><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/></div>";
                                }
                                retVal += "<input id='userId' name='userId' type='hidden' value='" + usr.UserId + "' />";
                                retVal += "<input id='rsrcId' name='rsrcId' type='hidden' value='" + RsrcId + "' />";
                                retVal += "<input id='entId' name='entId' type='hidden' value='" + EntId + "' />";
                                retVal += "<input id='year' name='year' type='hidden' value='" + yr + "' />";
                                retVal += "</form>";
                            }
                            else
                            {
                                retVal = "<p> No Record found for this resource and year. Contact the Administrator if this error persists.</p><br/><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/>";
                            }
                            break;

                        case "updateRsrcInfo":
                            var rsrcUpdRInfo = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecRInfo = rsrcUpdRInfo.FirstOrDefault();
                            data.UpdResources(rsrcRecRInfo.ResourceId, rsrcRecRInfo.ResourceName, rsrcTitle, rsrcEmail, rsrcPhone, comments, rsrcRecRInfo.EntityId, usr.UserName);
                            //no changlog record needed
                            retVal = "success";
                            break;
                        case "updateRsrcNotes":
                            var rsrcUpdN = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecN = rsrcUpdN.FirstOrDefault();
                            data.UpdResources(rsrcRecN.ResourceId, rsrcRecN.ResourceName, rsrcRecN.ResourceTitle, rsrcRecN.ResourceEmail, rsrcRecN.ResourcePhone, comments, rsrcRecN.EntityId, usr.UserName);
                            //no changlog record needed
                            retVal = "success";
                            break;
                        case "updateManHrs":
                            ResourceManHrs rsrcObj = (ResourceManHrs)JsonConvert.DeserializeObject(mhrs, typeof(ResourceManHrs));
                            var rmhUpd = db.Query("select pr.*, r.ResourceName from ResourceManHours pr join Resources r on pr.ResourceId = r.ResourceId where pr.ResourceId=" + RsrcId + " and pr.[Year] = " + yr + " and pr.[Month] = " + rsrcObj.Month, null);
                            var rmhRec = rmhUpd.FirstOrDefault();
                            data.UpdResourceManHours(rmhRec.ResourceId, rmhRec.Year, rmhRec.Month, rsrcObj.BaseManHours, rsrcObj.CoreManHours, rsrcObj.Comments, usr.UserName);
                            retVal = "success";
                            break;
                        case "getOpenRsrcList":
                            sqltxt = @"select r.ResourceId,
		                                r.EntityId,
		                                r.ResourceName
		                                from Resources r
		                                left outer join Entities e
		                                on e.EntityId = r.EntityId
		                                where e.EntityId is null";
                            var Rrecs = db.Query(sqltxt, null);
                            retVal = "<form id='frmRsrcList' name='frmRsrcList' >";
                            if (Rrecs.Count() > 0)
                            {
                                retVal += "<select id='selRsrc' name='selRsrc'>";
                                foreach (var rrec in Rrecs)
                                {
                                    retVal += "<option value='" + rrec.ResourceId + "' >" + rrec.ResourceName + "</option>";
                                }
                                retVal += "</select><br/>";
                                retVal += "<input type='hidden' id='entId' name='entId' value='" + EntId + "' />";
                                retVal += "<input id='btnsave' name='btnsave' type='button' value='save' onclick='addRsrc();' />";
                            }
                            else
                            {
                                retVal += "<p>No Unassigned Resources are Found</p>";
                            }
                            retVal += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeRsrcDialog()'/>";
                            retVal += "</form>";
                            break;
                        case "getOpenEntityList":
                            var precs = db.Query("select ParentId from Entities where EntityId=" + EntId);

                            sqltxt = @"select  
                                        e.EntityId,
                                        e.EntityName,   
                                        e.ParentId
                                        from Entities e 
                                        where e.ParentId = 999999999";
                            var Erecs = db.Query(sqltxt, null);
                            retVal = "<form id='frmEntList' name='frmEntList' >";
                            if (Erecs.Count() > 0)
                            {
                                retVal += "<select id='selEnt' name='selEnt'>";
                                foreach (var erec in Erecs)
                                {
                                    retVal += "<option value='" + erec.EntityId + "' >" + erec.EntityName + "</option>";
                                }
                                retVal += "</select><br/>";
                                retVal += "<input type='hidden' id='entId' name='entId' value='" + EntId + "' />";
                                retVal += "<input id='btnsave' name='btnsave' type='button' value='save' onclick='addEntity();' />";
                            }
                            else
                            {
                                retVal += "<p>No Unassigned Resources are Found</p>";
                            }
                            retVal += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeEntDialog()'/>";
                            retVal += "</form>";
                            break;
                        case "addEntity":
                            var entUpdEnt = db.Query("select e.* from Entities e where e.EntityId=" + EntId, null);
                            var entRecEnt = entUpdEnt.FirstOrDefault();
                            data.UpdEntities(entRecEnt.EntityId, entRecEnt.EntityName, entRecEnt.EntityType, int.Parse(parentent), entRecEnt.Notes, entRecEnt.UserId, usr.UserName);
                            //no changlog record needed
                            retVal = "success";
                            break;
                        case "addRsrc":
                            var rsrcUpdEnt = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecEnt = rsrcUpdEnt.FirstOrDefault();
                            data.UpdResources(rsrcRecEnt.ResourceId, rsrcRecEnt.ResourceName, rsrcRecEnt.ResourceTitle, rsrcRecEnt.ResourceEmail, rsrcRecEnt.ResourcePhone, rsrcRecEnt.Comments, int.Parse(EntId), usr.UserName);
                            //no changlog record needed
                            retVal = "success";

                            break;
                        case "remRsrc":
                            var rsrcRemEnt = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecRemEnt = rsrcRemEnt.FirstOrDefault();
                            data.UpdResources(rsrcRecRemEnt.ResourceId, rsrcRecRemEnt.ResourceName, rsrcRecRemEnt.ResourceTitle, rsrcRecRemEnt.ResourceEmail, rsrcRecRemEnt.ResourcePhone, rsrcRecRemEnt.Comments, -1, usr.UserName);
                            //no changlog record needed
                            retVal = "success";
                            break;
                        case "remEntity":
                            var entRemEnt = db.Query("select e.* from Entities e where e.EntityId=" + EntId, null);
                            var entRecRemEnt = entRemEnt.FirstOrDefault();
                            data.UpdEntities(entRecRemEnt.EntityId, entRecRemEnt.EntityName, entRecRemEnt.EntityType, 999999999, entRecRemEnt.Notes, entRecRemEnt.UserId, usr.UserName);
                            //no changlog record needed
                            break;
                        case "all":
                            ResourcesData rsrcAlldata = new ResourcesData();
                            rsrcAlldata.AllResources = hasResourcesEdit;
                            retVal = rsrcAlldata.GetResourcesAdminTree(usr);
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

        #region Admin Entity
        public ActionResult AdminEntity()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasResourcesEdit = false;
                bool hasResourcesEdit = false;

                if (usr.IsAdmin)
                {
                    hasResourcesEdit = true;
                }

                if (hasResourcesEdit)
                {
                    ViewBag.hasResourcesEdit = true;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string AdminEntityHandler(string ActTyp = null, string EntId = null, string entName = null, string entType = null, string notes = null, string userId = null, string parentId = null, string sort = null)
        {
            // bool hasEdit = false;
            ProbeUser usr = new ProbeUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
            }

            ViewBag.hasResourcesEdit = false;
            bool hasResourcesEdit = false;

            if (usr.IsAdmin)
            {
                ViewBag.hasResourcesEdit = true;
                hasResourcesEdit = true;
            }

            string tbl_o = "<table class='tblEntRsrcLists'>";
            string tbl_c = "</table>";
            string th_o = "<th>";
            string th_c = "</th>";
            string tr_o = "<tr>";
            string tr_c = "</tr>";
            string td_o = "<td>";
            string td_c = "</td>";

            string retval = "";
            int? logid = -1;
            try
            {
                var db = Database.Open("probedb");

                if (ActTyp != null)
                {
                    switch (ActTyp)
                    {
                        case "list":

                            var sqltxt = @"SELECT 	
                                            e.EntityId
                                            ,e.EntityName
                                            ,e.EntityType
                                            ,e.Notes
                                            ,e.UserId
                                            ,(select p.EntityName from Entities p where p.EntityId= e.ParentId) as Parent
                                            ,u.DisplayName                                            
                                        FROM [dbo].[Entities] e
                                        left outer join Users u on u.UserId = e.UserId ";

                            string sorttxt = " order by e.EntityName";
                            if (!String.IsNullOrEmpty(sort))
                            {
                                sorttxt = " order by " + sort;
                            }

                            sqltxt += sorttxt;

                            var erecs = db.Query(sqltxt);
                            retval = "<form id='frmEntities' name='frmEntities' >";
                            retval += "<div><a class='btnEntityAdd' id='addEntity'>New Entity</a></div>";
                            retval += tbl_o + tr_o;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('EntityName');\">Entity</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('EntityType');\">Type</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('Parent');\">Parent</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('DisplayName');\">User</a>" + th_c;
                            retval += tr_c;

                            foreach (var erec in erecs)
                            {
                                retval += tr_o;
                                retval += td_o + "<img id='delent_" + erec.EntityId + "' src='../images/delete.png' alt='delete entity' title='Delete Entity' />" + td_c;
                                retval += td_o + "<img id='editent_" + erec.EntityId + "' src='../images/pencil.png' alt='edit entity' title='Edit Entity' />" + td_c;
                                retval += td_o + erec.EntityName + td_c;
                                retval += td_o + erec.EntityType + td_c;
                                retval += td_o + erec.Parent + td_c;
                                retval += td_o + erec.DisplayName + td_c;
                                retval += tr_c;
                            }

                            retval += tbl_c;
                            retval += "<input type='hidden' id='sort' name='sort' value='" + sort + "' />";
                            retval += "</form>";

                            break;
                        case "new":
                            retval = "<form id='frmEntityInfo' name='frmEntityInfo' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Entity Name:</span></td><td><input id='EntityName' name='EntityName' type='text' value='' /></td></tr>";
                            retval += "<tr><td><span>Type:</span></td><td><input id='EntityType' name='EntityType' type='text' value='' /></td></tr>";

                            retval += "<tr><td><span>User:</span></td><td><select id='UserId' name='UserId'><option selected value='-1'>-- select user --</option>";
                            var entRecs = db.Query("select UserId, DisplayName from Users order by DisplayName");
                            foreach (var erec in entRecs)
                            {
                                retval += "<option value=" + erec.UserId + ">" + erec.DisplayName + "</option>";
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td><span>Comments:</span></td><td><textarea id='ent_comments' name='ent_comments' rows='10' cols='4' ></textarea></td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='AddEntity()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "</form>";
                            break;
                        case "info":
                            sqltxt = @"SELECT 	
                                            e.EntityId
                                            ,e.EntityName
                                            ,e.EntityType
                                            ,e.ParentId
                                            ,e.Notes
                                            ,e.UserId
                                            ,u.DisplayName                                            
                                        FROM [dbo].[Entities] e
                                        left outer join Users u on u.UserId = e.UserId
                                        where e.EntityId = " + EntId;

                            var recinfo = db.Query(sqltxt, null);
                            retval = "<h4>Editing : " + recinfo.FirstOrDefault().EntityName + "</h4>";
                            retval += "<form id='frmEntityInfo' name='frmEntityInfo' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Name:</span></td><td><input id='EntityName' name='EntityName' type='text' value='" + recinfo.FirstOrDefault().EntityName + "' /></td></tr>";
                            retval += "<tr><td><span>Type:</span></td><td><input id='EntityType' name='EntityType' type='text' value='" + recinfo.FirstOrDefault().EntityType + "' /></td></tr>";
                            string prntName = "";
                            if (recinfo.FirstOrDefault().ParentId == 999999999)
                            {
                                prntName = "None Assigned";
                            }
                            if (recinfo.FirstOrDefault().ParentId == -1)
                            {
                                prntName = "Top Level";
                            }

                            var pRecs = db.Query("Select EntityName, EntityId from Entities where EntityId=" + recinfo.FirstOrDefault().ParentId, null);
                            foreach (var prec in pRecs)
                            {
                                prntName = prec.EntityName;
                            }

                            retval += "<tr><td><span>Parent:</span><input type='hidden' id='parentId' name='parentId' value='" + recinfo.FirstOrDefault().ParentId + "'</td><td class='parentname'>" + prntName;
                            if (prntName != "Top Level")
                            {
                                retval += "<a class='makeTop' href='javascript:setTopLvl();' >Make this a Top Level Entity</a></td><tr>";
                            }
                            else
                            {
                                retval += "<a class='makeTop' href='javascript:setUnassigned();' >Make this an Unassigned Entity</a></td><tr>";
                            }
                            retval += "<tr><td><span>User:</span></td><td><select id='UserId' name='UserId'>";
                            var uRecs = db.Query("select UserId, DisplayName from Users order by DisplayName");
                            foreach (var urec in uRecs)
                            {
                                if (urec.UserId == recinfo.FirstOrDefault().UserId)
                                {
                                    retval += "<option selected value=" + urec.UserId + ">" + urec.DisplayName + "</option>";
                                }
                                else
                                {
                                    retval += "<option value=" + urec.UserId + ">" + urec.DisplayName + "</option>";
                                }
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td><span>Comments:</span></td><td><textarea id='ent_comments' name='ent_comments' rows='10' cols='4' >" + recinfo.FirstOrDefault().Notes + "</textarea></td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveEntityInfo()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "<input id='EntityId' name='EntityId' type='hidden' value='" + EntId + "' />";
                            retval += "</form>";
                            break;
                        case "updateEntityInfo":
                            var entUpdEInfo = db.Query("select e.EntityId,e.ParentId from Entities e where e.EntityId=" + EntId, null);
                            var entRecEInfo = entUpdEInfo.FirstOrDefault();
                            data.UpdEntities(entRecEInfo.EntityId, entName, entType, int.Parse(parentId), notes, int.Parse(userId), usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "delEntity":
                            data.DelEntities(int.Parse(EntId), usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "addEntity":
                            int rsrcId = -1;
                            data.AddEntities(entName, entType, 999999999, notes, int.Parse(userId), usr.UserName, ref rsrcId);
                            //no changlog record needed
                            retval = "success";
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                retval = "Error: " + ex.Message;
            }

            return retval;
        }

        #endregion

        #region Admin Resources
        public ActionResult AdminResources()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //List<DashboardProject> dbProjects = data.GetDashboardProjects();

                //foreach (var proj in dbProjects)
                //{
                //    if (proj.EndDate < DateTime.Today)
                //    {
                //        if (proj.Health != 2)
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 2, usr.UserName);
                //    }
                //    else if (proj.StartDate < DateTime.Today && proj.ActivityStatus == "Not Started")
                //    {
                //        if (proj.Health != 1)
                //            ret = data.UpdateProjectHealth(proj.ProjectId, 1, usr.UserName);
                //    }
                //}

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasResourcesEdit = false;
                bool hasResourcesEdit = false;

                if (usr.IsAdmin)
                {
                    hasResourcesEdit = true;
                }
                if (hasResourcesEdit)
                {
                    ViewBag.hasResourcesEdit = true;

                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }


        [HttpPost]
        public string AdminResourcesHandler(string RsrcId = null, string EntId = null, string ActTyp = null, string comments = null, string yr = null, string mhrs = null, string rsrcName = null, string rsrcTitle = null, string rsrcEmail = null, string rsrcPhone = null, string sort = null)
        {
            // bool hasEdit = false;
            ProbeUser usr = new ProbeUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
            }

            ViewBag.hasResourcesEdit = false;
            bool hasResourcesEdit = false;

            if (usr.IsAdmin)
            {
                ViewBag.hasResourcesEdit = true;
                hasResourcesEdit = true;
            }

            string tbl_o = "<table class='tblEntRsrcLists'>";
            string tbl_c = "</table>";
            string th_o = "<th>";
            string th_c = "</th>";
            string tr_o = "<tr>";
            string tr_c = "</tr>";
            string td_o = "<td>";
            string td_c = "</td>";

            string retval = "";
            int? logid = -1;
            try
            {
                var db = Database.Open("probedb");

                if (ActTyp != null)
                {
                    switch (ActTyp)
                    {
                        case "list":

                            var sqltxt = @"SELECT 	
                                            e.EntityName
		                                    ,[ResourceName]
		                                    ,[ResourceId]
                                            ,[ResourceTitle]
                                            ,[ResourceEmail]
                                            ,[ResourcePhone]
                                            ,[Comments]
                                            ,r.[EntityId]
                                            ,[ResourceNameAbbrev]
                                            ,r.[createDt]
                                            ,r.[updateDt]
                                            ,r.[updateBy]
                                            ,[ImportMap]
                                        FROM [dbo].[Resources] r
                                        left outer join Entities e on e.EntityId = r.EntityId";


                            string sorttxt = " order by e.EntityName,r.ResourceName";
                            if (!String.IsNullOrEmpty(sort))
                            {
                                sorttxt = " order by " + sort;
                            }

                            sqltxt += sorttxt;

                            var erecs = db.Query(sqltxt);
                            retval = "<form id='frmResources' name='frmResources' >";
                            retval += "<div><a class='btnRsrcAdd' id='addRsrc'>New Resource</a></div>";
                            retval += tbl_o + tr_o;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('EntityName');\">Entity</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('ResourceName');\">Resource</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('ResourceTitle');\">Title</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('ResourceEmail');\">Email</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('ResourcePhone');\">Phone</a>" + th_c;
                            retval += tr_c;

                            foreach (var erec in erecs)
                            {
                                retval += tr_o;
                                retval += td_o + "<img id='delrsrc_" + erec.ResourceId + "_" + erec.EntityId + "' src='../images/delete.png' alt='delete resource' title='Delete Resource' />" + td_c;
                                retval += td_o + "<img id='editrsrc_" + erec.ResourceId + "_" + erec.EntityId + "' src='../images/pencil.png' alt='edit resource' title='Edit Resource' />" + td_c;
                                retval += td_o + erec.EntityName + td_c;
                                retval += td_o + erec.ResourceName + td_c;
                                retval += td_o + erec.ResourceTitle + td_c;
                                retval += td_o + erec.ResourceEmail + td_c;
                                retval += td_o + erec.ResourcePhone + td_c;
                                retval += tr_c;
                            }

                            retval += tbl_c;
                            retval += "<input type='hidden' id='sort' name='sort' value='" + sort + "' />";
                            retval += "</form>";

                            break;
                        case "new":
                            retval = "<form id='frmRsrcInfo' name='frmRsrcInfo' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Resource Name:</span></td><td><input id='ResourceName' name='ResourceName' type='text' value='' /></td></tr>";
                            retval += "<tr><td><span>Title:</span></td><td><input id='ResourceTitle' name='ResourceTitle' type='text' value='' /></td></tr>";

                            //retval += "<tr><td><span>Entity:</span></td><td><select id='EntityId' name='EntityId'>";
                            //var entRecs = db.Query("select EntityId, EntityName from Entities order by EntityName");
                            //foreach(var erec in entRecs)
                            //{
                            //    if(erec.EntityId == recinfo.FirstOrDefault().EntityId)
                            //    {
                            //        retval += "<option selected value=" + erec.EntityId + ">" + erec.EntityName + "</option>";
                            //    }else{
                            //        retval += "<option value=" + erec.EntityId + ">" + erec.EntityName + "</option>";
                            //    }
                            //}                            
                            //retval += "</select></td></tr>";
                            retval += "<tr><td><span>Email:</span></td><td><input id='ResourceEmail' name='ResourceEmail' type='email' value='' /></td></tr>";
                            retval += "<tr><td><span>Phone:</span></td><td><input id='ResourcePhone' name='ResourcePhone' type='tel' value='' /></td></tr>";
                            retval += "<tr><td><span>Comments:</span></td><td><textarea id='rsrc_comments' name='rsrc_comments' rows='10' cols='4' ></textarea></td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='AddResource()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "</form>";
                            break;
                        case "info":
                            sqltxt = @"select r.ResourceId,
                                r.Comments,
                                r.ResourceName,
                                r.ResourceTitle,
                                r.ResourceEmail,
                                r.ResourcePhone,
                                r.EntityId,                                
                                e.EntityName
                                from Resources r 
                                left outer join Entities e on e.EntityId = r.EntityId
                                where r.EntityId = " + EntId +
                                             @" and r.ResourceId=" + RsrcId;

                            var recinfo = db.Query(sqltxt, null);
                            retval = "<h4>Editing : " + recinfo.FirstOrDefault().ResourceName + "</h4>";
                            retval += "<form id='frmRsrcInfo' name='frmRsrcInfo' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Name:</span></td><td><input id='ResourceName' name='ResourceName' type='text' value='" + recinfo.FirstOrDefault().ResourceName + "' /></td></tr>";
                            retval += "<tr><td><span>Title:</span></td><td><input id='ResourceTitle' name='ResourceTitle' type='text' value='" + recinfo.FirstOrDefault().ResourceTitle + "' /></td></tr>";
                            retval += "<tr><td><span>Entity:</span></td><td><input id='EntityId' name='EntityId' type='hidden' value=" + recinfo.FirstOrDefault().EntityId + "/>" + recinfo.FirstOrDefault().EntityName + "</td></tr>";
                            //retval += "<tr><td><span>Entity:</span></td><td><select id='EntityId' name='EntityId'>";
                            //var entRecs = db.Query("select EntityId, EntityName from Entities order by EntityName");
                            //foreach(var erec in entRecs)
                            //{
                            //    if(erec.EntityId == recinfo.FirstOrDefault().EntityId)
                            //    {
                            //        retval += "<option selected value=" + erec.EntityId + ">" + erec.EntityName + "</option>";
                            //    }else{
                            //        retval += "<option value=" + erec.EntityId + ">" + erec.EntityName + "</option>";
                            //    }
                            //}                            
                            //retval += "</select></td></tr>";
                            retval += "<tr><td><span>Email:</span></td><td><input id='ResourceEmail' name='ResourceEmail' type='email' value='" + recinfo.FirstOrDefault().ResourceEmail + "' /></td></tr>";
                            retval += "<tr><td><span>Phone:</span></td><td><input id='ResourcePhone' name='ResourcePhone' type='tel' value='" + recinfo.FirstOrDefault().ResourcePhone + "' /></td></tr>";
                            retval += "<tr><td><span>Comments:</span></td><td><textarea id='rsrc_comments' name='rsrc_comments' rows='10' cols='4' >" + recinfo.FirstOrDefault().Comments + "</textarea></td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveRsrcInfo()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "<input id='rsrcId' name='rsrcId' type='hidden' value='" + RsrcId + "' />";
                            retval += "</form>";
                            break;
                        case "updateRsrcInfo":
                            var rsrcUpdRInfo = db.Query("select r.* from Resources r where r.ResourceId=" + RsrcId, null);
                            var rsrcRecRInfo = rsrcUpdRInfo.FirstOrDefault();
                            data.UpdResources(rsrcRecRInfo.ResourceId, rsrcName, rsrcTitle, rsrcEmail, rsrcPhone, comments, rsrcRecRInfo.EntityId, usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "delResource":
                            data.DelResources(int.Parse(RsrcId), usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "addResource":
                            int rsrcId = -1;
                            data.AddResources(rsrcName, rsrcTitle, rsrcEmail, rsrcPhone, comments, -1, usr.UserName, ref rsrcId);
                            //no changlog record needed
                            retval = "success";
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                retval = "Error: " + ex.Message;
            }

            return retval;
        }

        #endregion

        #region Admin Templates

        public ActionResult AdminTemplates()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasTemplatesEdit = false;
                bool hasTemplatesEdit = false;

                if (usr.IsAdmin)
                {
                    hasTemplatesEdit = true;
                }

                if (hasTemplatesEdit)
                {
                    ViewBag.hasTemplatesEdit = true;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string AdminTemplatesHandler(string ActTyp = null, string tpltName = null, string tpltType = null, string tpltDesc = null, string tpltFileTyp = null, string tpltDocTyp = null, string sort = null)
        {
            // bool hasEdit = false;
            ProbeUser usr = new ProbeUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
            }

            ViewBag.hasTemplatesEdit = false;
            bool hasTemplatesEdit = false;

            if (usr.IsAdmin)
            {
                ViewBag.hasTemplatesEdit = true;
                hasTemplatesEdit = true;
            }

            string tbl_o = "<table class='tblTemplates'>";
            string tbl_c = "</table>";
            string th_o = "<th>";
            string th_c = "</th>";
            string tr_o = "<tr>";
            string tr_c = "</tr>";
            string td_o = "<td>";
            string td_c = "</td>";

            string retval = "";
            int? logid = -1;
            try
            {
                var db = Database.Open("probedb");

                if (ActTyp != null)
                {
                    switch (ActTyp)
                    {
                        case "list":

                            var sqltxt = @"SELECT [TemplateFileName]
                                              ,[TemplateDesc]
                                              ,[FileType]
                                              ,[TemplateType]
                                              ,[DocumentType]
                                              ,[Template]
                                              ,[createdDt]
                                              ,[updateDt]
                                              ,[updateBy]
                                          FROM [probe].[dbo].[Templates] ";

                            string sorttxt = " order by TemplateType";
                            if (!String.IsNullOrEmpty(sort))
                            {
                                sorttxt = " order by " + sort;
                            }

                            sqltxt += sorttxt;

                            var tplts = db.Query(sqltxt);
                            //var erecs = db.Query(sqltxt);
                            retval = "<form id='frmTemplates' name='frmTemplates' >";
                            retval += "<div><a class='btnTpltAdd' id='addTplt'>New Template</a></div>";
                            retval += tbl_o + tr_o;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a>&nbsp;</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('TemplateType');\">Type</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('DocumentType');\">Doc Type</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('TemplateFileName');\">Name</a>" + th_c;
                            //retval += th_o + "<a href=\"javascript:applySort('TemplateDesc');\">Desc</a>" + th_c;                            
                            retval += th_o + "<a href=\"javascript:applySort('FileType');\">File Type</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('updateDt');\">Last Updt Dt</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('updateBy');\">Last Updt By</a>" + th_c;
                            retval += tr_c;

                            foreach (var erec in tplts)
                            {
                                bool hasFile = (erec.Template.Length > 0 ? true : false);
                                retval += tr_o;
                                retval += td_o + "<img id='deltplt_" + erec.TemplateFileName + "_" + erec.TemplateType + "_" + erec.DocumentType + "' src='../images/delete.png' alt='delete Template' title='Delete Template' />" + td_c;
                                retval += td_o + "<img id='edittplt_" + erec.TemplateFileName + "_" + erec.TemplateType + "_" + erec.DocumentType + "' src='../images/pencil.png' alt='edit Template' title='Edit Template' />" + td_c;
                                if (hasFile)
                                {
                                    retval += td_o + "<img id='dnldtplt_" + erec.TemplateFileName + "_" + erec.TemplateType + "_" + erec.DocumentType + "' src='../images/arrow_down.png' alt='download Template' title='Download Template' />" + td_c;
                                }
                                else
                                {
                                    retval += td_o + "<img style='visibility:hidden' id='dnldtplt_" + erec.TemplateFileName + "_" + erec.TemplateType + "_" + erec.DocumentType + "' src='../images/arrow_down.png' alt='download Template' title='Download Template' />" + td_c;
                                }
                                retval += td_o + "<img id='upldtplt_" + erec.TemplateFileName + "_" + erec.TemplateType + "_" + erec.DocumentType + "' src='../images/arrow_up.png' alt='upload Template' title='Upload Template' />" + td_c;
                                retval += td_o + erec.TemplateType + td_c;
                                retval += td_o + erec.DocumentType + td_c;
                                retval += td_o + erec.TemplateFileName + td_c;
                                //retval += td_o + erec.TemplateDesc + td_c;
                                retval += td_o + erec.FileType + td_c;
                                retval += td_o + erec.updateDt + td_c;
                                retval += td_o + erec.updateBy + td_c;
                                retval += tr_c;
                            }

                            retval += tbl_c;
                            retval += "<input type='hidden' id='sort' name='sort' value='" + sort + "' />";
                            retval += "</form>";

                            break;
                        case "new":
                            retval = "<form id='frmNewTplt' name='frmNewTplt' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Template Name:</span></td><td><input id='TemplateFileName' name='TemplateFileName' type='text' value='' /></td></tr>";
                            retval += "<tr><td><span>Template Desc:</span></td><td><textarea cols='3' rows='8' id='TemplateDesc' name='TemplateDesc' ></textarea></td></tr>";
                            retval += "<tr><td><span>File Type:</span></td><td><input id='FileType' name='FileType' type='text' value='' /></td></tr>";
                            retval += "<tr><td><span>Template Type:</span></td><td><select id='TemplateType' name='TemplateType'><option selected value='-1'>-- select template type --</option>";
                            var tpltTyps = db.Query("select TemplateType from TemplateTypes order by TemplateType");
                            foreach (var erec in tpltTyps)
                            {
                                retval += "<option value='" + erec.TemplateType + "'>" + erec.TemplateType + "</option>";
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><span>Document Type:</span></td><td><select id='DocumentType' name='DocumentType'><option selected value='-1'>-- select document type --</option>";
                            var tpltDocs = db.Query("select DocumentType from DocumentTypes order by DocumentType");
                            foreach (var erec in tpltDocs)
                            {
                                retval += "<option value='" + erec.DocumentType + "'>" + erec.DocumentType + "</option>";
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='AddTemplate()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "</form>";
                            break;
                        case "info":
                            sqltxt = @"SELECT [TemplateFileName]
                                              ,[TemplateDesc]
                                              ,[FileType]
                                              ,[TemplateType]
                                              ,[DocumentType]
                                              ,[Template]
                                              ,[createdDt]
                                              ,[updateDt]
                                              ,[updateBy]
                                          FROM [probe].[dbo].[Templates] 
                                        where TemplateFileName = '" + tpltName + "' and TemplateType='" + tpltType + "' and DocumentType='" + tpltDocTyp + "'";

                            var recinfo = db.Query(sqltxt, null);
                            var fndtplt = recinfo.FirstOrDefault();
                            retval = "<form id='frmTpltInfo' name='frmTpltInfo' >";
                            retval += "<table><tr>";
                            retval += "<td><span>Template Name:</span></td><td><input id='TemplateFileName' name='TemplateFileName' type='text' value='" + fndtplt.TemplateFileName + "' /></td></tr>";
                            retval += "<tr><td><span>Template Desc:</span></td><td><textarea id='TemplateDesc' name='TemplateDesc' cols='3' rows='6' >" + fndtplt.TemplateDesc + "</textarea></td></tr>";
                            retval += "<tr><td><span>File Type:</span></td><td><input id='FileType' name='FileType' type='text' value='" + fndtplt.FileType + "' /></td></tr>";
                            retval += "<tr><td><span>Template Type:</span></td><td><select id='TemplateType' name='TemplateType'><option selected value='-1'>-- select template type --</option>";
                            var tpltTypsInfo = db.Query("select TemplateType from TemplateTypes order by TemplateType");
                            foreach (var erec in tpltTypsInfo)
                            {
                                retval += "<option " + (erec.TemplateType == recinfo.FirstOrDefault().TemplateType ? " selected " : "") + " value='" + erec.TemplateType + "'>" + erec.TemplateType + "</option>";
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><span>Document Type:</span></td><td><select id='DocumentType' name='DocumentType'><option selected value='-1'>-- select document type --</option>";
                            var tpltDocsInfo = db.Query("select DocumentType from DocumentTypes order by DocumentType");
                            foreach (var erec in tpltDocsInfo)
                            {
                                retval += "<option  " + (erec.DocumentType == recinfo.FirstOrDefault().DocumentType ? " selected " : "") + " value='" + erec.DocumentType + "'>" + erec.DocumentType + "</option>";
                            }
                            retval += "</select></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><span style='font-weight:bold;'>Upload doc:</span></td><td><input style='width:250px' type='file' name='FileUpload1' id='fileUpload' /></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveTpltInfo()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "</form>";
                            break;
                        case "upload":
                            sqltxt = @"SELECT [TemplateFileName]
                                              ,[TemplateDesc]
                                              ,[FileType]
                                              ,[TemplateType]
                                              ,[DocumentType]
                                              ,[Template]
                                              ,[createdDt]
                                              ,[updateDt]
                                              ,[updateBy]
                                          FROM [probe].[dbo].[Templates] 
                                        where TemplateFileName = '" + tpltName + "' and TemplateType='" + tpltType + "' and DocumentType='" + tpltDocTyp + "'";

                            var upldinfo = db.Query(sqltxt, null);
                            var fndtpltupld = upldinfo.FirstOrDefault();
                            retval = "<form id='frmTpltUpld' name='frmTpltUpld' method='post' >";
                            retval += "<h4>Uploading file for Template:    " + fndtpltupld.TemplateFileName + "</h4>";
                            retval += "<table><tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<td><span>Choose a file to upload:</span></td><td><input style='width:350px;' id='upldFile' name='upldFile' type='file' /></td></tr>";
                            retval += "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>";
                            retval += "<tr><td><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveTpltUpld()' />";
                            retval += "</td><td><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/></td></tr></table>";
                            retval += "<input type='hidden' id='TemplateFileName' name='TemplateFileName' value='" + fndtpltupld.TemplateFileName + "' />";
                            retval += "<input type='hidden' id='TemplateType' name='TemplateType' value='" + fndtpltupld.TemplateType + "' />";
                            retval += "<input type='hidden' id='DocumentType' name='DocumentType' value='" + fndtpltupld.DocumentType + "' />";
                            retval += "<input type='hidden' id='ActTyp' name='ActTyp' value='putBinary' />";
                            retval += "</form>";
                            break;
                        case "updTemplate":
                            var tpltUpdInfo = db.Query("select * from Templates t where t.TemplateFileName='" + tpltName + "' and TemplateType='" + tpltType + "'", null);
                            var tpltUpdRec = tpltUpdInfo.FirstOrDefault();
                            data.UpdTemplates(tpltName, tpltDesc, tpltFileTyp, tpltType, tpltDocTyp, tpltUpdRec.Template, usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "delTemplate":
                            data.DelTemplates(tpltName, tpltType, usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "addTemplate":
                            string empty = "";
                            byte[] nobits = System.Text.Encoding.ASCII.GetBytes("");
                            data.AddTemplates(tpltName, tpltDesc, tpltFileTyp, tpltType, tpltDocTyp, nobits, usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "putBinary":
                            var upldfilcnt = Request.Files.Count;
                            if (upldfilcnt > 0)
                            {
                                HttpPostedFileBase fb = Request.Files[0];
                                Stream strm = fb.InputStream;
                                byte[] bits = new byte[] { };
                                //bits.Length =  fb.ContentLength;
                                strm.Read(bits, 0, fb.ContentLength);
                                var tpltUpldInfo = db.Query("select * from Templates t where t.TemplateFileName='" + tpltName + "' and TemplateType='" + tpltType + "'", null);
                                var tpltUpldRec = tpltUpldInfo.FirstOrDefault();
                                data.UpdTemplates(tpltUpldRec.TemplateFileName, tpltUpldRec.TemplateDesc, tpltUpldRec.FileType, tpltUpldRec.TemplateType, tpltUpldRec.DocumentType, bits, usr.UserName);
                                //no changlog record needed
                                retval = "success";
                            }
                            else
                            {
                                retval = "no file detected in form submission";
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                retval = "Error: " + ex.Message;
            }

            return retval;
        }


        public ActionResult upload()
        {
            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].FileName != "") 
               {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/ App_Data / uploads /";
                    string filename = Path.GetFileName(Request.Files[upload].FileName);
                    Request.Files[upload].SaveAs(Path.Combine(path, filename));
                }
            }
            return View("Upload");
        }



        [AcceptVerbs("GET", "POST")]
        public ActionResult GetTemplateFile(string tName = null, string tFilTyp = null, string tDocTyp = null)
        {
            var db = Database.Open("probedb");
            byte[] bits = null;
            string mimetyp = "text/plain";
            //HttpResponseMessage retMsg = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {

                var tpltBinary = db.Query("select Template,TemplateFileName, FileType from Templates  where TemplateFileName = '" + tName + "' and TemplateType='" + tFilTyp + "' and DocumentType='" + tDocTyp + "'");
                if (tpltBinary.Count() > 0)
                {
                    bits = tpltBinary.FirstOrDefault().Template;
                    string ftyp = tpltBinary.FirstOrDefault().FileType;

                    switch (ftyp)
                    {
                        case "docx":
                            mimetyp = "application/msword";
                            break;
                        case "xlsx":
                            mimetyp = "application/excel";
                            break;
                        case "pdf":
                            mimetyp = "application/pdf";
                            break;
                        case "png":
                            mimetyp = "image/png";
                            break;
                        case "jpg":
                            mimetyp = "image/jpg";
                            break;
                        case "tif":
                            mimetyp = "image/tif";
                            break;
                    }

                }

                return File(bits, mimetyp);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public string UpldTemplateFile(HttpPostedFileBase upldFile)
        {
            var db = Database.Open("probedb");
            try
            {
                ProbeUser usr = new ProbeUser();
                ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
                if (Session["CurrProbeUser"] != null)
                {
                    usr = (ProbeUser)Session["CurrProbeUser"];
                }

                byte[] bits = new byte[upldFile.ContentLength];
                string tName = Request.Form["TemplateFileName"];
                string tFilTyp = upldFile.FileName.Split('.')[1];
                string tDocTyp = Request.Form["DocumentType"];
                string tpltType = Request.Form["TemplateType"];
                Stream strm = upldFile.InputStream;
                strm.Read(bits, 0, upldFile.ContentLength);
                var tpltUpldInfo = db.Query("select * from Templates t where t.TemplateFileName='" + tName + "' and TemplateType='" + tpltType + "'", null);
                var tpltUpldRec = tpltUpldInfo.FirstOrDefault();
                data.UpdTemplates(tpltUpldRec.TemplateFileName, tpltUpldRec.TemplateDesc, tFilTyp, tpltUpldRec.TemplateType, tpltUpldRec.DocumentType, bits, usr.UserName);


            }
            catch (Exception ex)
            {
                //return null;
            }
            Response.Redirect("~/Admin/AdminTemplates");
            return "";
        }
        #endregion

        #region Admin Users
        public ActionResult AdminUsers()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasUsersEdit = false;
                bool hasUsersEdit = false;

                if (usr.IsAdmin)
                {
                    hasUsersEdit = true;
                }

                if (hasUsersEdit)
                {
                    ViewBag.hasUsersEdit = true;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }

        [HttpPost]
        public string AdminUsersHandler(string ActTyp = null, string userId = null, string userJson = null, string sort = null)
        {
            // bool hasEdit = false;
            ProbeUser usr = new ProbeUser();
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                usr = (ProbeUser)Session["CurrProbeUser"];
            }

            ViewBag.hasUsersEdit = false;
            bool hasUsersEdit = false;

            if (usr.IsAdmin)
            {
                ViewBag.hasUsersEdit = true;
                hasUsersEdit = true;
            }

            string tbl_o = "<table class='tblUsers'>";
            string tbl_c = "</table>";
            string th_o = "<th>";
            string th_c = "</th>";
            string tr_o = "<tr>";
            string tr_c = "</tr>";
            string td_o = "<td>";
            string td_c = "</td>";
            string spn_o = "<span>";
            string spn_c = "</span>";

            string retval = "";
            int? logid = -1;
            try
            {
                var db = Database.Open("probedb");

                if (ActTyp != null)
                {
                    switch (ActTyp)
                    {
                        case "list":

                            var sqltxt = @"SELECT  [UserId]
                                                  ,[Active]
                                                  ,[UserName]
                                                  ,[DisplayName]
                                                  ,[FirstName]
                                                  ,[LastName]
                                                  ,[MiddleInitial]
                                                  ,[Email]
                                                  ,[Phone]
                                                  ,[createDt]
                                                  ,[updateDt]
                                                  ,[updateBy]
                                              FROM [probe].[dbo].[Users]";

                            string sorttxt = " order by UserName";
                            if (!String.IsNullOrEmpty(sort))
                            {
                                sorttxt = " order by " + sort;
                            }

                            sqltxt += sorttxt;

                            var users = db.Query(sqltxt);
                            //var erecs = db.Query(sqltxt);
                            retval = "<form id='frmUsers' name='frmUsers' >";
                            retval += "<div><a class='btnUserAdd' id='addUser'>New User</a><span>" + users.Count() + " Users Found</span></div>";
                            retval += "<table class='tblUsers' >" + tr_o;
                            retval += th_o + "&nbsp;" + th_c;
                            retval += th_o + "&nbsp;" + th_c;
                            retval += th_o + "&nbsp;" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('Active');\">Active</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('UserName');\">User Name</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('DisplayName');\">Display Name</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('FirstName');\">First Name</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('LastName');\">Last Name</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('MiddleInitial');\">Middle Initial</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('Email');\">Email</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('Phone');\">Phone</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('createDt');\">Created</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('updateDt');\">Updated</a>" + th_c;
                            retval += th_o + "<a href=\"javascript:applySort('updateBy');\">Last Updt By</a>" + th_c;
                            retval += tr_c;

                            foreach (var erec in users)
                            {
                                retval += tr_o;
                                if (erec.Active == 0)
                                {
                                    retval += td_o + "<img id='actUser_" + erec.UserId + "' src='../images/add.png' alt='activate User' title='Activate User' />" + td_c;
                                }
                                else
                                {
                                    retval += td_o + "<img id='deactUser_" + erec.UserId + "' src='../images/delete.png' alt='deactivate User' title='Deactivate User' />" + td_c;
                                }

                                retval += td_o + "<img id='editUser_" + erec.UserId + "' src='../images/pencil.png' alt='edit User' title='Edit User' />" + td_c;
                                retval += td_o + "<img id='editUserRoles_" + erec.UserId + "' src='../images/cog.png' alt='edit User Roles' title='Edit User Roles' />" + td_c;
                                retval += td_o + (erec.Active == 1 ? "yes" : "no") + td_c;
                                retval += td_o + erec.UserName + td_c;
                                retval += td_o + erec.DisplayName + td_c;
                                retval += td_o + erec.FirstName + td_c;
                                retval += td_o + erec.LastName + td_c;
                                retval += td_o + erec.MiddleInitial + td_c;
                                retval += td_o + erec.Email + td_c;
                                retval += td_o + erec.Phone + td_c;
                                retval += td_o + erec.createDt + td_c;
                                retval += td_o + erec.updateDt + td_c;
                                retval += td_o + erec.updateBy + td_c;
                                retval += tr_c;
                            }

                            retval += tbl_c;
                            retval += "<input type='hidden' id='sort' name='sort' value='" + sort + "' />";
                            retval += "</form>";

                            break;
                        case "new":
                            retval += "<form id='frmNewUser' name='frmNewUser' >";
                            retval += "<table class='tblUsers' >" + tr_o;
                            retval += tr_o + td_o + spn_o + "User Name (Network Id) :" + spn_c + td_c + td_o + "<input id='UserName' name='UserName' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Display Name:" + spn_c + td_c + td_o + "<input id='DisplayName' name='DisplayName' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "First Name:" + spn_c + td_c + td_o + "<input id='FirstName' name='FirstName' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Last Name:" + spn_c + td_c + td_o + "<input id='LastName' name='LastName' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Middle Initial:" + spn_c + td_c + td_o + "<input id='MiddleInitial' name='MiddleInitial' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Email:" + spn_c + td_c + td_o + "<input id='Email' name='Email' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Phone:" + spn_c + td_c + td_o + "<input id='Phone' name='Phone' type='text' value='' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Resource:" + spn_c + td_c + td_o + "<select id='ResourceId' name='ResourceId'>";

                            string rsrcOptions = "";

                            var uRecs = db.Query("select ResourceId, ResourceName from Resources order by ResourceName");
                            foreach (var urec in uRecs)
                            {
                                rsrcOptions += "<option value=" + urec.ResourceId + ">" + urec.ResourceName + "</option>";
                            }

                            retval += "<option></option>" + rsrcOptions;

                            retval += "</select>" + td_c + tr_c;
                            retval += tr_o + "<td colspan='2'>&nbsp;</td>" + tr_c;
                            retval += tr_o + "<td colspan='2'><input id='btnsave' name='btnsave' type='button' value='save' onclick='AddUser()' />";
                            retval += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/>" + td_c + tr_c + tbl_c;
                            retval += "</form>";
                            break;
                        case "info":
                            sqltxt = @"SELECT  [UserId]
                                                  ,[Active]
                                                  ,[UserName]
                                                  ,[DisplayName]
                                                  ,[FirstName]
                                                  ,[LastName]
                                                  ,[MiddleInitial]
                                                  ,[Email]
                                                  ,[Phone]
                                                  ,[ResourceId]
                                              FROM [probe].[dbo].[Users] where UserId = " + userId;

                            var recinfo = db.Query(sqltxt, null);
                            //retval = "<h4>Editing : " + recinfo.FirstOrDefault().EntityName + "</h4>";
                            retval += "<form id='frmUserInfo' name='frmUserInfo' >";
                            retval += "<table class='tblUser'>" + tr_o;
                            retval += td_o + spn_o + "Active:" + spn_c + td_c + td_o + "<input id='Active' name='Active' type='checkbox' " + (recinfo.FirstOrDefault().Active == 1 ? "checked" : "") + " />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "User Name:" + spn_c + td_c + td_o + "<input id='UserName' name='UserName' type='text' value='" + recinfo.FirstOrDefault().UserName + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Display Name:" + spn_c + td_c + td_o + "<input id='DisplayName' name='DisplayName' type='text' value='" + recinfo.FirstOrDefault().DisplayName + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "First Name:" + spn_c + td_c + td_o + "<input id='FirstName' name='FirstName' type='text' value='" + recinfo.FirstOrDefault().FirstName + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Last Name:" + spn_c + td_c + td_o + "<input id='LastName' name='LastName' type='text' value='" + recinfo.FirstOrDefault().LastName + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Middle Initial:" + spn_c + td_c + td_o + "<input id='MiddleInitial' name='MiddleInitial' type='text' value='" + recinfo.FirstOrDefault().MiddleInitial + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Email:" + spn_c + td_c + td_o + "<input id='Email' name='Email' type='text' value='" + recinfo.FirstOrDefault().Email + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Phone:" + spn_c + td_c + td_o + "<input id='Phone' name='Phone' type='text' value='" + recinfo.FirstOrDefault().Phone + "' />" + td_c + tr_c;
                            retval += tr_o + td_o + spn_o + "Resource:" + spn_c + td_c + td_o + "<select id='ResourceId' name='ResourceId'>";

                            string rsrcOpts = "";

                            var uInfoRecs = db.Query("select ResourceId, ResourceName from Resources order by ResourceName");
                            foreach (var urec in uInfoRecs)
                            {
                                if (urec.ResourceId == recinfo.FirstOrDefault().ResourceId)
                                {
                                    rsrcOpts += "<option selected value=" + urec.ResourceId + ">" + urec.ResourceName + "</option>";
                                }
                                else
                                {
                                    rsrcOpts += "<option value=" + urec.ResourceId + ">" + urec.ResourceName + "</option>";
                                }
                            }

                            retval += "<option></option>" + rsrcOpts;

                            retval += "</select>" + td_c + tr_c;
                            retval += tr_o + "<td colspan='3'>&nbsp;</td>" + tr_c;
                            retval += tr_o + "<td colspan='2'><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveUserInfo()' />";
                            retval += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/>" + td_c + tr_c + tbl_c;
                            retval += "<input id='UserId' name='UserId' type='hidden' value='" + userId + "' />";
                            retval += "</form>";
                            break;
                        case "roles":
                            string currUserNm = "";
                            sqltxt = @"select DisplayName from Users where UserId=" + userId;
                            var usrnmRec = db.Query(sqltxt);
                            currUserNm = usrnmRec.FirstOrDefault().DisplayName;

                            sqltxt = @"select r.RoleId,
                                              r.RoleName,
                                              r.RoleDesc,
                                              ur.RoleId,
                                              ur.UserId
											  from Roles r
											  left join UserRoles ur
											  on r.RoleId = ur.RoleId and ur.UserId = " + userId;

                            var recroles = db.Query(sqltxt, null);
                            retval = "<h4>Editing Application Roles for : " + currUserNm + "</h4><br/>";
                            retval += "<form id='frmUserRoles' name='frmUserRoles' >";

                            retval += "<table class='tblUsers' >" + tr_o;
                            retval += th_o + "&nbsp;" + th_c;
                            retval += th_o + "Role" + th_c;
                            retval += th_o + "Description" + th_c;
                            retval += tr_c;

                            foreach (var erec in recroles)
                            {
                                string idtxt = "hasRole_" + userId + "_" + erec.RoleId;
                                retval += tr_o;
                                retval += td_o + "<input id='" + idtxt + "' name='" + idtxt + "' type='checkbox' " + (erec.UserId != null ? "checked" : "") + " />" + td_c;
                                retval += td_o + erec.RoleName + td_c;
                                retval += td_o + erec.RoleDesc + td_c;
                                retval += tr_c;
                            }
                            retval += tr_o + "<td colspan='3'>&nbsp;</td>" + tr_c;
                            retval += tr_o + "<td colspan='3' style='text-align:center;'><input id='btnsave' name='btnsave' type='button' value='save' onclick='saveUserRoles()' />";
                            retval += "<input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeInfoDialog()'/>" + td_c;
                            retval += tr_c + tbl_c;
                            retval += "<input id='UserId' name='UserId' type='hidden' value='" + userId + "' />";
                            retval += "</form>";
                            break;
                        case "updateUserInfo":
                            ProbeUser usrInfo = (ProbeUser)JsonConvert.DeserializeObject(userJson, typeof(ProbeUser));
                            int? usrrsrcid = null;
                            int tryint;

                            if (int.TryParse("" + usrInfo.ResourceId, out tryint))
                            {
                                usrrsrcid = tryint;
                            }
                            data.UpdUsers(int.Parse(userId), usrInfo.UserName, "", usrInfo.DisplayName, usrInfo.Email, usrInfo.Phone, usrInfo.FirstName, usrInfo.LastName, usrInfo.MiddleInitial, usrrsrcid, "", 1, usr.UserName);
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "updateUserRoles":
                            //remove existing user roles
                            var allUsrRols = db.Query("select RoleId from UserRoles where UserId = " + userId);
                            foreach (var allrol in allUsrRols)
                            {
                                data.DelUserRoles(int.Parse(userId), allrol.RoleId);
                            }

                            List<UserRoles> usrRols = (List<UserRoles>)JsonConvert.DeserializeObject(userJson, typeof(List<UserRoles>));
                            foreach (var urol in usrRols)
                            {
                                data.AddUserRoles(int.Parse(userId), urol.RoleId, usr.UserName);
                            }
                            //no changlog record needed
                            retval = "success";
                            break;
                        case "delUser":
                            var usrDelRecs = db.Query("select * from Users where UserId=" + userId);
                            var usrDelInfo = usrDelRecs.FirstOrDefault();
                            var actVal = (usrDelInfo.Active == 1 ? 0 : 1);
                            data.UpdUsers(int.Parse(userId), usrDelInfo.UserName, "", usrDelInfo.DisplayName, usrDelInfo.Email, usrDelInfo.Phone, usrDelInfo.FirstName, usrDelInfo.LastName, usrDelInfo.MiddleInitial, usrDelInfo.ResourceId, "", actVal, usr.UserName);

                            //no changlog record needed
                            retval = "success";
                            break;
                        case "addUser":
                            var getjson = userJson;
                            ProbeUser newusrInfo = (ProbeUser)JsonConvert.DeserializeObject(userJson, typeof(ProbeUser));
                            int newUid = -1;
                            data.AddUsers(newusrInfo.UserName, "", newusrInfo.DisplayName, newusrInfo.Email, newusrInfo.Phone, newusrInfo.FirstName, newusrInfo.LastName, newusrInfo.MiddleInitial, newusrInfo.ResourceId, "", 1, usr.UserName, ref newUid);
                            //no changlog record needed
                            retval = "success";
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                retval = "Error: " + ex.Message;
            }

            return retval;
        }


        #endregion


        #region Admin Table Maintenance
        public ActionResult AdminTables()
        {
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                ViewBag.UserName = usr.UserName;
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);
                Session["CurrProbeUser"] = usr;

                int ret = -1;

                //logic for showing the Resrouces Admin sub menu item under Administration.
                ViewBag.hasTemplatesEdit = false;
                bool hasTableMaintEdit = false;
                ViewBag.hasTableMaintEdit = false;
                if (usr.IsAdmin)
                {
                    hasTableMaintEdit = true;
                }

                if (hasTableMaintEdit)
                {
                    ViewBag.hasTableMaintEdit = true;
                }

                return View();
            }
            else
            {
                Response.Redirect("~/Home/GuestUser");
                return View("GuestUser");
            }
        }


        public ActionResult AdminChangeLog()
        {
            List<ChangeLog_set> cLog = new List<ChangeLog_set>();
            cLog = data.GetProjectsLogs(-30);
            ViewBag.cLog = JsonConvert.SerializeObject(cLog);
            return View();
        }

        [HttpPost]
        public string AdminAddGroup(Group mod)
        {
            int t = mod.GroupId;
            int? NewId = -1;
            string h = ViewBag.UserName;
            usr = (ProbeUser)Session["CurrProbeUser"];
            int y = data._AddGroups(mod.GroupName, usr.UserName, ref NewId);
            IEnumerable<Groups> tbData = data.GetGroupsData();

            
            List<Groups> newObj = tbData.ToList();
            int bIndx = newObj.Count();
            Groups[] Jsonret = new Groups[bIndx];
            int cntr = 0;
            foreach (Groups idx in newObj)
            {
                Jsonret[cntr] = idx;
                cntr = cntr + 1;
            }
            string output = JsonConvert.SerializeObject(Jsonret);
            return output;
        }



        [HttpPost]
        public string TableMaintenance(string tblName, string actTyp, string origData, string newData = null)
        {
            bool hasEdit = false;
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);

                if (usr.IsAdmin)
                {
                    hasEdit = true;
                }

            }
            else
            {
                return "Error: User could not be found";
            }

            string retval = "";
            int cntr = 0;
            try
            {
                IEnumerable tblData = null;
                switch (tblName)
                {
                    case "ApprovalActivityStatuses":
                        tblData = data.GetApprovalActivityStatusesData();
                        break;
                    case "BenefitGroups":
                        tblData = data.GetBenefitGroupsData();
                        break;
                    case "Benefits":
                        tblData = data.GetBenefitsData();
                        break;
                    case "BudgetStatuses":
                        tblData = data.GetBudgetStatusesData();
                        break;
                    case "BudgetTypes":
                        tblData = data.GetBudgetTypesData();
                        break;
                    case "CostSavingsTypes":
                        tblData = data.GetCostSavingsTypesData();
                        break;
                    case "DocumentTypes":
                        tblData = data.GetDocumentTypesData();
                        break;
                    case "EntityTypes":
                        tblData = data.GetEntityTypesData();
                        break;
                    case "Groups":
                        tblData = data.GetGroupsData();
                        break;
                    case "Impacts":
                        tblData = data.GetImpactsData();
                        break;
                    case "IssueStatuses":
                        tblData = data.GetIssueStatusesData();
                        break;
                    case "Phases":
                        tblData = data.GetPhasesData();
                        break;
                    case "ProjectType":
                        tblData = data.GetProjectTypesData();
                        break;
                    case "TemplateTypes":
                        tblData = data.GetTemplateTypesData();
                        break;

                    case "ServiceAgreementTypes":
                        tblData = data.GetServiceTypesData();
                        break;

                    case "AttachmentGroups":
                        tblData = data.GetProjectAttachmentsGroup_();
                        break;


                    case "ServiceAgreementVendors":
                        tblData = data.GetVendorsAdmin();
                        break;
                }

                switch (actTyp)
                {
                    case "get":
                        retval = "<table class=\"editTable altrows\">";
                        foreach (var currRec in tblData)
                        {
                            if (cntr == 0)
                            {
                                retval += "<tr><th>&nbsp;</th><th>&nbsp;</th><th>&nbsp;</th>";
                                foreach (PropertyInfo prop in currRec.GetType().GetProperties())
                                {
                                    retval += "<th class='tblHdr'>" + prop.Name + "</th>";
                                }
                                retval += "</tr>";
                            }
                            retval += "<tr>";
                            retval += "<td><a class='btnDel' onclick='javascript:goDel();'><img class='imgdel' src='../images/delete.png' alt='delete record' /></a><input id='originalData' type='hidden' value='" + JsonConvert.SerializeObject(currRec) + "' /></td>";
                            retval += "<td><a class='btnAdd' onclick='javascript:goAdd()'><img class='imgadd' src='../images/add.png' alt='add record' /></a></td>";

                            List<string> noEditList = new List<string>();
                            noEditList.Add("ApprovalActivityStatuses");
                            noEditList.Add("BudgetStatuses");
                            noEditList.Add("BudgetTypes");
                            noEditList.Add("DocumentTypes");
                            noEditList.Add("EntityTypes");
                            noEditList.Add("IssueStatuses");
                            noEditList.Add("ProjectType");
                            noEditList.Add("TemplateTypes");
                            noEditList.Add("ServiceAgreementTypes");
                            noEditList.Add("Groups");
                            noEditList.Add("AttachmentGroups");

                            //there is no updating the time_periods data
                            if (!noEditList.Contains(tblName))
                            {

                                retval += "<td><a class='btnEdit' onclick=javascript:editGroup(this,'" + tblName + "') value='" + JsonConvert.SerializeObject(currRec) + "' ><img class='imgedit' src='../images/pencil.png' alt='edit record' /></a><input id='originalData' type='hidden'  /></td>";
                            }
                            else
                            {
                                retval += "<td>&nbsp;</td>";
                            }
                            foreach (PropertyInfo prop in currRec.GetType().GetProperties())
                            {

                                try
                                {
                                    object pval = prop.GetValue(currRec);

                                    if (pval.GetType() == typeof(DateTime))
                                    {
                                        DateTime ptim = (DateTime)prop.GetValue(currRec);
                                        pval = ptim.ToShortDateString();
                                    }

                                    retval += "<td>" + pval + "</td>";



                                }
                                catch(Exception ex)
                                {
                                    throw ex;
                                }
                               
                            }
                            retval += "</tr>";
                            cntr++;
                        }
                        retval += "</table>";
                        break;
                    case "edit":
                        try
                        {
                            object recData = null;

                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    recData = new ApprovalActivityStatuses();
                                    if (origData != "undefined")
                                    {
                                        ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                        recData = data.GetApprovalActivityStatusesRecord(ApprovalActivityStatusesObj);
                                    }
                                    break;
                                case "BenefitGroups":
                                    recData = new BenefitGroups();
                                    if (origData != "undefined")
                                    {
                                        BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                        recData = data.GetBenefitGroupsRecord(BenefitGroupsObj);
                                    }
                                    break;
                                case "Benefits":
                                    recData = new Benefits();
                                    if (origData != "undefined")
                                    {
                                        Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                        recData = data.GetBenefitsRecord(BenefitsObj);
                                    }
                                    break;
                                case "BudgetStatuses":
                                    recData = new BudgetStatuses();
                                    if (origData != "undefined")
                                    {
                                        BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(origData, typeof(BudgetStatuses));
                                        recData = data.GetBudgetStatusesRecord(BudgetStatusesObj);
                                    }
                                    break;
                                case "BudgetTypes":
                                    recData = new BudgetTypes();
                                    if (origData != "undefined")
                                    {
                                        BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(origData, typeof(BudgetTypes));
                                        recData = data.GetBudgetTypesRecord(BudgetTypesObj);
                                    }
                                    break;
                                case "CostSavingsTypes":
                                    recData = new CostSavingsTypes();
                                    if (origData != "undefined")
                                    {
                                        CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                        recData = data.GetCostSavingsTypesRecord(CostSavingsTypesObj);
                                    }
                                    break;
                                case "DocumentTypes":
                                    recData = new DocumentTypes();
                                    if (origData != "undefined")
                                    {
                                        DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(origData, typeof(DocumentTypes));
                                        recData = data.GetDocumentTypesRecord(DocumentTypesObj);
                                    }
                                    break;
                                case "EntityTypes":
                                    recData = new EntityTypes();
                                    if (origData != "undefined")
                                    {
                                        EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(origData, typeof(EntityTypes));
                                        recData = data.GetEntityTypesRecord(EntityTypesObj);
                                    }
                                    break;
                                case "Groups":
                                    recData = new Groups();
                                    if (origData != "undefined")
                                    {
                                        Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                        recData = data.GetGroupsRecord(GroupsObj);
                                    }
                                    break;
                                case "Impacts":
                                    recData = new Impacts();
                                    if (origData != "undefined")
                                    {
                                        Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                        recData = data.GetImpactsRecord(ImpactsObj);
                                    }
                                    break;
                                case "IssueStatuses":
                                    recData = new IssueStatuses();
                                    if (origData != "undefined")
                                    {
                                        IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(origData, typeof(IssueStatuses));
                                        recData = data.GetIssueStatusesRecord(IssueStatusesObj);
                                    }
                                    break;
                                case "Phases":
                                    recData = new Phases();
                                    if (origData != "undefined")
                                    {
                                        Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                        recData = data.GetPhasesRecord(PhasesObj);
                                    }
                                    break;
                                case "ProjectType":
                                    recData = new ProjectTypes();
                                    if (origData != "undefined")
                                    {
                                        ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(origData, typeof(ProjectTypes));
                                        recData = data.GetProjectTypesRecord(ProjectTypesObj);
                                    }
                                    break;
                                case "TemplateTypes":
                                    recData = new TemplateTypes();
                                    if (origData != "undefined")
                                    {
                                        TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(origData, typeof(TemplateTypes));
                                        recData = data.GetTemplateTypesRecord(TemplateTypesObj);
                                    }
                                    break;

                                case "AttachmentGroups":
                                    recData = new AttachmentGroup();
                                    if (origData != "undefined")
                                    {
                                        AttachmentGroup AttachmentGroupObj = (AttachmentGroup)JsonConvert.DeserializeObject(origData, typeof(AttachmentGroup));
                                        recData = data.GetAttachmentGroupRecord(AttachmentGroupObj);
                                    }
                                    break;

                                case "ServiceAgreementTypes":
                                    recData = new serviceTypes();
                                    if (origData != "undefined")
                                    {
                                        serviceTypes ProjectTypesObj = (serviceTypes)JsonConvert.DeserializeObject(origData, typeof(serviceTypes));
                                        recData = data.ServiceAgreementType(ProjectTypesObj);
                                    }
                                    break;


                                case "ServiceAgreementVendors":
                                    ServiceAgreementVendors b = new ServiceAgreementVendors();
                                    recData = b;
                                    break; 



                            }

                            List<BenefitGroups> grpsList = new List<BenefitGroups>();
                            List<ApprovalActivityStatuses> approvalList = new List<ApprovalActivityStatuses>();
                            List<ApprovalActivityStatuses> activityList = new List<ApprovalActivityStatuses>();
                            retval = "<form id='frmTblEdit' name='frmTblEdit'><table class=\"editTable tblEdit\">";
                            if (tblName == "ProviderResource")
                            {

                                // rsrclist = data.GetRsrcList();

                                // mtrlist = data.GetMtrList();
                            }

                            foreach (PropertyInfo prop in recData.GetType().GetProperties())
                            {
                                bool isDate = false;
                                bool isList = false;

                                object pval = prop.GetValue(recData);
                                if (pval != null)
                                {
                                    if (pval.GetType() == typeof(DateTime))
                                    {
                                        DateTime ptim = (DateTime)prop.GetValue(recData);
                                        pval = ptim.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    pval = "";
                                }

                                retval += "<tr><td>" + prop.Name + ":</td>";

                                string inputtxt = "";
                                string selecttxt = "";
                                string disabledtxt = "";

                                //based on table and field name and if editing a record set the disabled property for the input
                                switch (tblName)
                                {
                                    //case "ApprovalActivityStatuses":
                                    //    if (prop.Name == "ApprovalStatus" && newData != "add") { disabledtxt = "disabled"; }
                                    //    break;
                                    case "BenefitGroups":
                                        if (prop.Name == "BenefitGroupId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Benefits":
                                        if (prop.Name == "BenefitId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "CostSavingsTypes":
                                        if (prop.Name == "CostSavingsTypeId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Groups":
                                        if (prop.Name == "GroupId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Impacts":
                                        if (prop.Name == "ImpactId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Phases":
                                        if (prop.Name == "PhaseId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;


                                    case "ServiceAgreementVendors":
                                        if (prop.Name == "ServiceAgreementVendorId" && newData != "add") { disabledtxt = "disabled"; }
                                        
                                        break;
                                }



                                inputtxt = "" + (hasEdit ? "<input id='" + prop.Name + "' type='text' value='" + pval.ToString() + "' " + disabledtxt + " />" : pval.ToString());

                                //inputtxt = "";
                                //check certain field names and create appropriate input types
                                if (tblName == "Benefits" && prop.Name == "BenefitGroupId" && newData == "add")
                                {

                                    grpsList = data.GetBenefitGroupsData().ToList<BenefitGroups>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in grpsList)
                                    {
                                        selecttxt += "<option value='" + grp.BenefitGroupId + "'>" + grp.BenefitGroupName + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }

                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ApprovalStatus" && newData == "add")
                                {

                                    approvalList = data.GetApprovalStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in approvalList)
                                    {
                                        selecttxt += "<option value='" + grp.ApprovalStatus + "'>" + grp.ApprovalStatus + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }
                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ActivityStatus" && newData == "add")
                                {

                                    activityList = data.GetActivityStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in activityList)
                                    {
                                        selecttxt += "<option value='" + grp.ActivityStatus + "'>" + grp.ActivityStatus + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }
                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ReportDefault" && newData == "add")
                                {

                                    activityList = data.GetActivityStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";

                                    selecttxt += "<option value='Y'>Y</option>";
                                    selecttxt += "<option value='N'>N</option>";

                                    selecttxt += "</select>";

                                }
                                //if selecttxt is populated then use the selecttxt variable else use the inputtxt
                                if (!String.IsNullOrEmpty(selecttxt))
                                {
                                    retval += "<td>" + selecttxt + "</td></tr>";
                                }
                                else
                                {
                                    retval += "<td>" + inputtxt + "</td></tr>";
                                }

                            }
                            retval += "</table>";
                            retval += "<input id='originalData' type='hidden' value='" + origData + "' />";
                            retval += "<input id='currTbl' type='hidden' value='" + tblName + "' />";
                            if (newData == "add")
                            {
                                retval += "<input id='doAdd' type='hidden' value='add' />";
                            }
                            retval += "</form>";
                            if (hasEdit)
                            {
                                retval += "<br/><div style='float:left'><input id='btnsave' name='btnsave' type='button' value='save' onclick=createGroup('" + tblName + "') /></div>";
                                retval += "<div style='float:left'><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeEditDialog()'/></div>";
                            }
                            else
                            {
                                retval += "<br/><div style='float:left'><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/></div>";
                            }
                        }
                        catch (Exception ex)
                        {
                            retval = "Error getting record data. : " + ex.Message;
                        }
                        break;
                    case "update":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObjOrig = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(newData, typeof(ApprovalActivityStatuses));
                                    recData = data.UpdApprovalActivityStatuses(ApprovalActivityStatusesObjOrig.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, ApprovalActivityStatusesObj.ReportDefault, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    BenefitGroups BenefitGroupsObjOrig = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                    BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(newData, typeof(BenefitGroups));
                                    recData = data.UpdBenefitGroups(BenefitGroupsObjOrig.BenefitGroupId, BenefitGroupsObj.BenefitGroupName, BenefitGroupsObj.BenefitGroupOrder, usr.UserName);
                                    break;
                                case "Benefits":
                                    Benefits BenefitsObjOrig = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                    Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(newData, typeof(Benefits));
                                    recData = data.UpdBenefits(BenefitsObjOrig.BenefitId, BenefitsObj.BenefitName, BenefitsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObjOrig = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(newData, typeof(CostSavingsTypes));
                                    recData = data.UpdCostSavingsTypes(CostSavingsTypesObjOrig.CostSavingsTypeId, CostSavingsTypesObj.CostSavingsType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObjOrig = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(newData, typeof(Groups));
                                    recData = data.UpdGroups(GroupsObjOrig.GroupId, GroupsObj.GroupName, usr.UserName);
                                    break;
                                case "Impacts":
                                    Impacts ImpactsObjOrig = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                    Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(newData, typeof(Impacts));
                                    recData = data.UpdImpacts(ImpactsObjOrig.ImpactId, ImpactsObj.ImpactName, ImpactsObj.ImpactDesc, usr.UserName);
                                    break;
                                case "Phases":
                                    Phases PhasesObjOrig = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                    Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(newData, typeof(Phases));
                                    recData = data.UpdPhases(PhasesObjOrig.PhaseId, PhasesObj.PhaseName, PhasesObj.PhaseDesc, PhasesObj.PhaseOrder, PhasesObj.color, PhasesObj.weeks_duration, usr.UserName);
                                    break;



                                case "ServiceAgreementTypes":
                                    serviceTypes serviceTypesObjOrig = (serviceTypes)JsonConvert.DeserializeObject(origData, typeof(serviceTypes));
                                    serviceTypes serviceTypesObj = (serviceTypes)JsonConvert.DeserializeObject(newData, typeof(serviceTypes));
                                    //recData = data.UpdPhases(serviceTypesObjOrig., PhasesObj.PhaseName, PhasesObj.PhaseDesc, PhasesObj.PhaseOrder, PhasesObj.color, PhasesObj.weeks_duration, usr.UserName);
                                    break;


                                case "ServiceAgreementVendors":
                                    ServiceAgreementVendors serviceVendorObjOrig = (ServiceAgreementVendors)JsonConvert.DeserializeObject(origData, typeof(ServiceAgreementVendors));
                                    ServiceAgreementVendors vendorTypesObj = (ServiceAgreementVendors)JsonConvert.DeserializeObject(newData, typeof(ServiceAgreementVendors));
                                    usr = (ProbeUser)Session["CurrProbeUser"];
                                    string updateBy = usr.UserName;
                                    recData = data.upDateVendors(vendorTypesObj.ServiceAgreementVendorId, vendorTypesObj.ServiceAgreementVendorName, updateBy);

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error updating record.: Msg: " + ex.Message;
                        }
                        break;
                    case "delete":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                    recData = data.DelApprovalActivityStatuses(ApprovalActivityStatusesObj.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                    recData = data.DelBenefitGroups(BenefitGroupsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "Benefits":
                                    Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                    recData = data.DelBenefits(BenefitsObj.BenefitId, usr.UserName);
                                    break;
                                case "BudgetStatuses":
                                    BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(origData, typeof(BudgetStatuses));
                                    recData = data.DelBudgetStatuses(BudgetStatusesObj.BudgetStatus, usr.UserName);
                                    break;
                                case "BudgetTypes":
                                    BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(origData, typeof(BudgetTypes));
                                    recData = data.DelBudgetTypes(BudgetTypesObj.BudgetType, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                    recData = data.DelCostSavingsTypes(CostSavingsTypesObj.CostSavingsTypeId, usr.UserName);
                                    break;
                                case "DocumentTypes":
                                    DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(origData, typeof(DocumentTypes));
                                    recData = data.DelDocumentTypes(DocumentTypesObj.DocumentType, usr.UserName);
                                    break;
                                case "EntityTypes":
                                    EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(origData, typeof(EntityTypes));
                                    recData = data.DelEntityTypes(EntityTypesObj.EntityType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                    recData = data.DelGroups(GroupsObj.GroupId, usr.UserName);
                                    break;
                                case "Impacts":
                                    Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                    recData = data.DelImpacts(ImpactsObj.ImpactId, usr.UserName);
                                    break;
                                case "IssueStatuses":
                                    IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(origData, typeof(IssueStatuses));
                                    recData = data.DelIssueStatuses(IssueStatusesObj.IssueStatus, usr.UserName);
                                    break;
                                case "Phases":
                                    Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                    recData = data.DelPhases(PhasesObj.PhaseId, usr.UserName);
                                    break;
                                case "ProjectType":
                                    ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(origData, typeof(ProjectTypes));
                                    recData = data.DelProjectTypes(ProjectTypesObj.ProjectType, usr.UserName);
                                    break;

                                case "ServiceAgreementTypes":
                                    tableMaintenanceTypes ServiceAgreementTypesObj = (tableMaintenanceTypes)JsonConvert.DeserializeObject(origData, typeof(tableMaintenanceTypes));
                                    recData = data.DelServiceAgreementTypes(ServiceAgreementTypesObj.ServiceAgreementType, usr.UserName);
                                    break;
                                case "TemplateTypes":
                                    TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(origData, typeof(TemplateTypes));
                                    recData = data.DelTemplateTypes(TemplateTypesObj.TemplateType, usr.UserName);
                                    break;

                                case "AttachmentGroups":
                                    AttachmentGroup AttachmentGroupsTypesObj = (AttachmentGroup)JsonConvert.DeserializeObject(origData, typeof(AttachmentGroup));
                                    recData = data.DelAttachmentGroup(AttachmentGroupsTypesObj.AttachmentGroupId, usr.UserName);

                                    break;

                                

                                case "ServiceAgreementVendors":
                                    ServiceAgreementVendors serviceVendorObjOrig = (ServiceAgreementVendors)JsonConvert.DeserializeObject(origData, typeof(ServiceAgreementVendors));
                                    //ServiceAgreementVendors vendorTypesObj = (ServiceAgreementVendors)JsonConvert.DeserializeObject(newData, typeof(ServiceAgreementVendors));
                                    usr = (ProbeUser)Session["CurrProbeUser"];
                                    string updateBy = usr.UserName;
                                    try
                                    {
                                        recData = data.DeleteVendors(serviceVendorObjOrig.ServiceAgreementVendorId, updateBy);
                                    }
                                    catch(Exception ex)
                                    {
                                        throw ex;
                                    }
                                    

                                 

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error deleting record.: Msg: " + ex.Message;
                        }
                        break;
                    case "insert":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(newData, typeof(ApprovalActivityStatuses));
                                    recData = data.AddApprovalActivityStatuses(ApprovalActivityStatusesObj.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, ApprovalActivityStatusesObj.ReportDefault, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(newData, typeof(BenefitGroups));
                                    //recData = data.AddBenefitGroups(BenefitGroupsObj.BenefitGroupName, BenefitGroupsObj.BenefitGroupOrder, usr.UserName);
                                    break;
                                case "Benefits":
                                    Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(newData, typeof(Benefits));
                                    //recData = data.AddBenefits(BenefitsObj.BenefitName, BenefitsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "BudgetStatuses":
                                    //BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(newData, typeof(BudgetStatuses));
                                    //recData = data.AddBudgetStatuses(BudgetStatusesObj.BudgetStatus, usr.UserName);
                                    break;
                                case "BudgetTypes":
                                    BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(newData, typeof(BudgetTypes));
                                    recData = data.AddBudgetTypes(BudgetTypesObj.BudgetType, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(newData, typeof(CostSavingsTypes));
                                    //recData = data.AddCostSavingsTypes(CostSavingsTypesObj.CostSavingsType, usr.UserName);
                                    break;
                                case "DocumentTypes":
                                    DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(newData, typeof(DocumentTypes));
                                    recData = data.AddDocumentTypes(DocumentTypesObj.DocumentType, usr.UserName);
                                    break;
                                case "EntityTypes":
                                    EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(newData, typeof(EntityTypes));
                                    recData = data.AddEntityTypes(EntityTypesObj.EntityType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(newData, typeof(Groups));
                                    recData = data.AddGroups(GroupsObj.GroupName, usr.UserName);
                                    break;
                                case "Impacts":
                                    Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(newData, typeof(Impacts));
                                    recData = data.AddImpacts(ImpactsObj.ImpactName, ImpactsObj.ImpactDesc, usr.UserName, ImpactsObj.ImpactId);
                                    break;
                                case "IssueStatuses":
                                    IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(newData, typeof(IssueStatuses));
                                    try
                                    {
                                        recData = data.AddIssueStatuses(IssueStatusesObj.IssueStatus, usr.UserName);

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    break;
                                case "Phases":
                                    Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(newData, typeof(Phases));
                                    recData = data.AddPhases(PhasesObj.PhaseName, PhasesObj.PhaseDesc, PhasesObj.PhaseOrder, PhasesObj.color, usr.UserName,PhasesObj.PhaseId);
                                    break;
                                case "ProjectType":
                                    ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(newData, typeof(ProjectTypes));
                                    recData = data.AddProjectTypes(ProjectTypesObj.ProjectType, usr.UserName);
                                    break;
                                case "TemplateTypes":
                                    TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(newData, typeof(TemplateTypes));
                                    recData = data.AddTemplateTypes(TemplateTypesObj.TemplateType, usr.UserName);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error inserting record.: Msg: " + ex.Message;
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
            return retval;

        }









        [HttpPost]
        public JsonResult getSelectOptions(tableMaintenanceTypes mod)
        {
            string qString = @"Select * from BenefitGroups";
            var udb = Database.Open("probedb");
            IEnumerable usrEnts = udb.Query(qString).ToList();


            return Json(new { success = true, usrEnts });

        }




        [HttpPost]
        public string TableMaintenance1(tableMaintenanceTypes mod)
        {
            bool hasEdit = false;
            string uname = "";

            string tblName = mod.tableName;
            string origData = "";
            string newData = "";
            ViewBag.HelpUrl = new Uri(ConfigurationManager.AppSettings["ProbeHelpUrl"]);
            if (Session["CurrProbeUser"] != null)
            {
                string username = User.Identity.Name;
                username = username.Substring(username.IndexOf("\\") + 1);
                uname = username;
                usr = data.GetUser(username);
                usr.IsAdmin = data.IsUserAdmin(usr.UserId);

                if (usr.IsAdmin)
                {
                    hasEdit = true;
                }

            }
            else
            {
                return "Error: User could not be found";
            }

            string retval = "";
            int cntr = 0;
            try
            {
                IEnumerable tblData = null;
                switch (mod.tableName)
                {
                    case "ApprovalActivityStatuses":

                        try
                        {
                            int chk = data.AddApprovalActivityStatuses(mod.ApprovalStatus, mod.ActivityStatus, mod.ReportDefault, uname);
                        }
                        catch (Exception ex)
                        {

                        }

                        //tblData = data.GetApprovalActivityStatusesData();
                        break;
                    case "BenefitGroups":

                        try
                        {

                            int AddBenefitchk = data.AddBenefitGroups(mod.BenefitGroupName, mod.BenefitGroupOrder, uname, mod.BenefitGroupId);

                            //tblData = data.GetBenefitGroupsData();
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                    case "Benefits":
                        try
                        {
                            int Benefitschk = data.AddBenefits(mod.BenefitName, mod.BenefitGroupId, uname, mod.BenefitId);
                            return Benefitschk.ToString();
                        }
                        catch (Exception e)
                        {

                        }

                        break;
                    case "BudgetStatuses":
                        try
                        {
                            int BudgetStatus = data.AddBudgetStatuses(mod.BudgetStatus, uname, mod.BudgetStatusDisplayed);
                            //int BudgetChk = data.AddBudgetStatuses(mod.BudgetStatus, uname);
                        }
                        catch (Exception e)
                        {

                        }

                        //tblData = data.GetBudgetStatusesData();
                        break;
                    case "BudgetTypes":

                        try
                        {
                            int BudgetTypesChk = data.AddBudgetTypes(mod.BudgetType, uname);
                        }
                        catch (Exception e)
                        {

                        }

                        break;
                    case "CostSavingsTypes":
                        try
                        {
                            int CostSavingsChk = data.AddCostSavingsTypes(mod.CostSavingsTypes, uname, mod.CostSavingsTypeId);
                        }
                        catch (Exception e)
                        {

                        }

                        //tblData = data.GetCostSavingsTypesData();
                        break;
                    case "DocumentTypes":
                        tblData = data.GetDocumentTypesData();
                        break;
                    case "EntityTypes":
                        tblData = data.GetEntityTypesData();
                        break;
                    case "Groups":
                        tblData = data.GetGroupsData();
                        break;
                    case "Impacts":
                        tblData = data.GetImpactsData();
                        break;
                    case "IssueStatuses":
                        tblData = data.GetIssueStatusesData();
                        break;
                    case "Phases":
                        tblData = data.GetPhasesData();
                        break;
                    case "ProjectType":
                        tblData = data.GetProjectTypesData();
                        break;
                    case "TemplateTypes":
                        //tblData = data.GetTemplateTypesData();
                        break;

                    case "ServiceAgreementTypes":
                        tblData = data.GetServiceTypesData();
                        break;


                    case "AttachmentGroups":
                        //tblData = data.AddAttachMentGroup(mod.AttachmentGroupName,"estes");
                        mod.action = "insert";
                        break;
                }
                string actTyp = mod.action;
                switch (actTyp)
                {
                    case "get":
                        retval = "<table class=\"editTable altrows\">";
                        foreach (var currRec in tblData)
                        {
                            if (cntr == 0)
                            {
                                retval += "<tr><th>&nbsp;</th><th>&nbsp;</th><th>&nbsp;</th>";
                                foreach (PropertyInfo prop in currRec.GetType().GetProperties())
                                {
                                    retval += "<th class='tblHdr'>" + prop.Name + "</th>";
                                }
                                retval += "</tr>";
                            }
                            retval += "<tr>";
                            retval += "<td><a class='btnDel' onclick='javascript:goDel();'><img class='imgdel' src='../images/delete.png' alt='delete record' /></a><input id='originalData' type='hidden' value='" + JsonConvert.SerializeObject(currRec) + "' /></td>";
                            retval += "<td><a class='btnAdd' onclick='javascript:goAdd()'><img class='imgadd' src='../images/add.png' alt='add record' /></a></td>";

                            List<string> noEditList = new List<string>();
                            noEditList.Add("ApprovalActivityStatuses");
                            noEditList.Add("BudgetStatuses");
                            noEditList.Add("BudgetTypes");
                            noEditList.Add("DocumentTypes");
                            noEditList.Add("EntityTypes");
                            noEditList.Add("IssueStatuses");
                            noEditList.Add("ProjectType");
                            noEditList.Add("TemplateTypes");
                            noEditList.Add("ServiceAgreementTypes");
                            noEditList.Add("Groups");
                            noEditList.Add("AttachmentGroups");
                            

                            //there is no updating the time_periods data
                            if (!noEditList.Contains(tblName))
                            {
                                retval += "<td><a class='btnEdit' onclick='javascript:editGroup(this,'" + tblName + "');'><img class='imgedit' src='../images/pencil.png' alt='edit record' /></a><input id='originalData' type='hidden' value='" + JsonConvert.SerializeObject(currRec) + "' /></td>";
                            }
                            else
                            {
                                retval += "<td>&nbsp;</td>";
                            }
                            foreach (PropertyInfo prop in currRec.GetType().GetProperties())
                            {
                                object pval = prop.GetValue(currRec);
                                if (pval.GetType() == typeof(DateTime))
                                {
                                    DateTime ptim = (DateTime)prop.GetValue(currRec);
                                    pval = ptim.ToShortDateString();
                                }
                                if (pval.GetType() == typeof(DateTime))
                                {
                                    DateTime ptim = (DateTime)prop.GetValue(currRec);
                                    pval = ptim.ToShortDateString();
                                }
                                retval += "<td>" + pval + "</td>";
                            }
                            retval += "</tr>";
                            cntr++;
                        }
                        retval += "</table>";
                        break;
                    case "edit":
                        try
                        {
                            object recData = null;

                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    recData = new ApprovalActivityStatuses();
                                    if (origData != "undefined")
                                    {
                                        ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                        recData = data.GetApprovalActivityStatusesRecord(ApprovalActivityStatusesObj);
                                    }
                                    break;
                                case "BenefitGroups":
                                    recData = new BenefitGroups();
                                    if (origData != "undefined")
                                    {
                                        BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                        recData = data.GetBenefitGroupsRecord(BenefitGroupsObj);
                                    }
                                    break;
                                case "Benefits":
                                    recData = new Benefits();
                                    if (origData != "undefined")
                                    {
                                        Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                        recData = data.GetBenefitsRecord(BenefitsObj);
                                    }
                                    break;
                                case "BudgetStatuses":
                                    recData = new BudgetStatuses();
                                    if (origData != "undefined")
                                    {
                                        BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(origData, typeof(BudgetStatuses));
                                        recData = data.GetBudgetStatusesRecord(BudgetStatusesObj);
                                    }
                                    break;
                                case "BudgetTypes":
                                    recData = new BudgetTypes();
                                    if (origData != "undefined")
                                    {
                                        BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(origData, typeof(BudgetTypes));
                                        recData = data.GetBudgetTypesRecord(BudgetTypesObj);
                                    }
                                    break;
                                case "CostSavingsTypes":
                                    recData = new CostSavingsTypes();
                                    if (origData != "undefined")
                                    {
                                        CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                        recData = data.GetCostSavingsTypesRecord(CostSavingsTypesObj);
                                    }
                                    break;
                                case "DocumentTypes":
                                    recData = new DocumentTypes();
                                    if (origData != "undefined")
                                    {
                                        DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(origData, typeof(DocumentTypes));
                                        recData = data.GetDocumentTypesRecord(DocumentTypesObj);
                                    }
                                    break;
                                case "EntityTypes":
                                    recData = new EntityTypes();
                                    if (origData != "undefined")
                                    {
                                        EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(origData, typeof(EntityTypes));
                                        recData = data.GetEntityTypesRecord(EntityTypesObj);
                                    }
                                    break;
                                case "Groups":
                                    recData = new Groups();
                                    if (origData != "undefined")
                                    {
                                        Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                        recData = data.GetGroupsRecord(GroupsObj);
                                    }
                                    break;
                                case "Impacts":
                                    recData = new Impacts();
                                    if (origData != "undefined")
                                    {
                                        Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                        recData = data.GetImpactsRecord(ImpactsObj);
                                    }
                                    break;
                                case "IssueStatuses":
                                    recData = new IssueStatuses();
                                    if (origData != "undefined")
                                    {
                                        IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(origData, typeof(IssueStatuses));
                                        recData = data.GetIssueStatusesRecord(IssueStatusesObj);
                                    }
                                    break;
                                case "Phases":
                                    recData = new Phases();
                                    if (origData != "undefined")
                                    {
                                        Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                        recData = data.GetPhasesRecord(PhasesObj);
                                    }
                                    break;
                                case "ProjectType":
                                    recData = new ProjectTypes();
                                    if (origData != "undefined")
                                    {
                                        ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(origData, typeof(ProjectTypes));
                                        recData = data.GetProjectTypesRecord(ProjectTypesObj);
                                    }
                                    break;
                                case "TemplateTypes":
                                    recData = new TemplateTypes();
                                    if (origData != "undefined")
                                    {
                                        TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(origData, typeof(TemplateTypes));
                                        recData = data.GetTemplateTypesRecord(TemplateTypesObj);
                                    }
                                    break;
                            }

                            List<BenefitGroups> grpsList = new List<BenefitGroups>();
                            List<ApprovalActivityStatuses> approvalList = new List<ApprovalActivityStatuses>();
                            List<ApprovalActivityStatuses> activityList = new List<ApprovalActivityStatuses>();
                            retval = "<form id='frmTblEdit' name='frmTblEdit'><table class=\"editTable tblEdit\">";
                            if (tblName == "ProviderResource")
                            {

                                // rsrclist = data.GetRsrcList();

                                // mtrlist = data.GetMtrList();
                            }

                            foreach (PropertyInfo prop in recData.GetType().GetProperties())
                            {
                                bool isDate = false;
                                bool isList = false;

                                object pval = prop.GetValue(recData);
                                if (pval != null)
                                {
                                    if (pval.GetType() == typeof(DateTime))
                                    {
                                        DateTime ptim = (DateTime)prop.GetValue(recData);
                                        pval = ptim.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    pval = "";
                                }

                                retval += "<tr><td>" + prop.Name + ":</td>";

                                string inputtxt = "";
                                string selecttxt = "";
                                string disabledtxt = "";

                                //based on table and field name and if editing a record set the disabled property for the input
                                switch (tblName)
                                {
                                    //case "ApprovalActivityStatuses":
                                    //    if (prop.Name == "ApprovalStatus" && newData != "add") { disabledtxt = "disabled"; }
                                    //    break;
                                    case "BenefitGroups":
                                        if (prop.Name == "BenefitGroupId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Benefits":
                                        if (prop.Name == "BenefitId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "CostSavingsTypes":
                                        if (prop.Name == "CostSavingsTypeId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Groups":
                                        if (prop.Name == "GroupId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Impacts":
                                        if (prop.Name == "ImpactId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                    case "Phases":
                                        if (prop.Name == "PhaseId" && newData != "add") { disabledtxt = "disabled"; }
                                        break;
                                }



                                inputtxt = "" + (hasEdit ? "<input id='" + prop.Name + "' type='text' value='" + pval.ToString() + "' " + disabledtxt + " />" : pval.ToString());


                                //check certain field names and create appropriate input types
                                if (tblName == "Benefits" && prop.Name == "BenefitGroupId" && newData == "add")
                                {

                                    grpsList = data.GetBenefitGroupsData().ToList<BenefitGroups>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in grpsList)
                                    {
                                        selecttxt += "<option value='" + grp.BenefitGroupId + "'>" + grp.BenefitGroupName + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }

                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ApprovalStatus" && newData == "add")
                                {

                                    approvalList = data.GetApprovalStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in approvalList)
                                    {
                                        selecttxt += "<option value='" + grp.ApprovalStatus + "'>" + grp.ApprovalStatus + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }
                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ActivityStatus" && newData == "add")
                                {

                                    activityList = data.GetActivityStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";
                                    foreach (var grp in activityList)
                                    {
                                        selecttxt += "<option value='" + grp.ActivityStatus + "'>" + grp.ActivityStatus + "</option>";
                                    }
                                    selecttxt += "</select>";

                                }
                                if (tblName == "ApprovalActivityStatuses" && prop.Name == "ReportDefault" && newData == "add")
                                {

                                    activityList = data.GetActivityStatusesList().ToList<ApprovalActivityStatuses>();
                                    selecttxt = "<select id='" + prop.Name + "'>";
                                    selecttxt += "<option value=''>-- select -- </option>";

                                    selecttxt += "<option value='Y'>Y</option>";
                                    selecttxt += "<option value='N'>N</option>";

                                    selecttxt += "</select>";

                                }
                                //if selecttxt is populated then use the selecttxt variable else use the inputtxt
                                if (!String.IsNullOrEmpty(selecttxt))
                                {
                                    retval += "<td>" + selecttxt + "</td></tr>";
                                }
                                else
                                {
                                    retval += "<td>" + inputtxt + "</td></tr>";
                                }

                            }
                            retval += "</table>";
                            retval += "<input id='originalData' type='hidden' value='" + origData + "' />";
                            retval += "<input id='currTbl' type='hidden' value='" + tblName + "' />";
                            if (newData == "add")
                            {
                                retval += "<input id='doAdd' type='hidden' value='add' />";
                            }
                            retval += "</form>";
                            if (hasEdit)
                            {
                                retval += "<br/><div style='float:left'><input id='btnsave' name='btnsave' type='button' value='save' onclick=createGroup('" + tblName + "') /></div>";
                                retval += "<div style='float:left'><input id='btncancel' name='btncancel' type='button' value='cancel' onclick='closeEditDialog()'/></div>";
                            }
                            else
                            {
                                retval += "<br/><div style='float:left'><input id='btncancel' name='btncancel' type='button' value='close' onclick='closeEditDialog()'/></div>";
                            }
                        }
                        catch (Exception ex)
                        {
                            retval = "Error getting record data. : " + ex.Message;
                        }
                        break;
                    case "update":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObjOrig = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(newData, typeof(ApprovalActivityStatuses));
                                    recData = data.UpdApprovalActivityStatuses(ApprovalActivityStatusesObjOrig.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, ApprovalActivityStatusesObj.ReportDefault, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    BenefitGroups BenefitGroupsObjOrig = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                    BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(newData, typeof(BenefitGroups));
                                    recData = data.UpdBenefitGroups(BenefitGroupsObjOrig.BenefitGroupId, BenefitGroupsObj.BenefitGroupName, BenefitGroupsObj.BenefitGroupOrder, usr.UserName);
                                    break;
                                case "Benefits":
                                    Benefits BenefitsObjOrig = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                    Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(newData, typeof(Benefits));
                                    recData = data.UpdBenefits(BenefitsObjOrig.BenefitId, BenefitsObj.BenefitName, BenefitsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObjOrig = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(newData, typeof(CostSavingsTypes));
                                    recData = data.UpdCostSavingsTypes(CostSavingsTypesObjOrig.CostSavingsTypeId, CostSavingsTypesObj.CostSavingsType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObjOrig = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(newData, typeof(Groups));
                                    recData = data.UpdGroups(GroupsObjOrig.GroupId, GroupsObj.GroupName, usr.UserName);
                                    break;
                                case "Impacts":
                                    Impacts ImpactsObjOrig = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                    Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(newData, typeof(Impacts));
                                    recData = data.UpdImpacts(ImpactsObjOrig.ImpactId, ImpactsObj.ImpactName, ImpactsObj.ImpactDesc, usr.UserName);
                                    break;
                                case "Phases":
                                    Phases PhasesObjOrig = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                    Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(newData, typeof(Phases));
                                    recData = data.UpdPhases(PhasesObjOrig.PhaseId, PhasesObj.PhaseName, PhasesObj.PhaseDesc, PhasesObj.PhaseOrder, PhasesObj.color, PhasesObj.weeks_duration, usr.UserName);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error updating record.: Msg: " + ex.Message;
                        }
                        break;
                    case "delete":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(origData, typeof(ApprovalActivityStatuses));
                                    recData = data.DelApprovalActivityStatuses(ApprovalActivityStatusesObj.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(origData, typeof(BenefitGroups));
                                    recData = data.DelBenefitGroups(BenefitGroupsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "Benefits":
                                    Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(origData, typeof(Benefits));
                                    recData = data.DelBenefits(BenefitsObj.BenefitId, usr.UserName);
                                    break;
                                case "BudgetStatuses":
                                    BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(origData, typeof(BudgetStatuses));
                                    recData = data.DelBudgetStatuses(BudgetStatusesObj.BudgetStatus, usr.UserName);
                                    break;
                                case "BudgetTypes":
                                    BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(origData, typeof(BudgetTypes));
                                    recData = data.DelBudgetTypes(BudgetTypesObj.BudgetType, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(origData, typeof(CostSavingsTypes));
                                    recData = data.DelCostSavingsTypes(CostSavingsTypesObj.CostSavingsTypeId, usr.UserName);
                                    break;
                                case "DocumentTypes":
                                    DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(origData, typeof(DocumentTypes));
                                    recData = data.DelDocumentTypes(DocumentTypesObj.DocumentType, usr.UserName);
                                    break;
                                case "EntityTypes":
                                    EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(origData, typeof(EntityTypes));
                                    recData = data.DelEntityTypes(EntityTypesObj.EntityType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(origData, typeof(Groups));
                                    recData = data.DelGroups(GroupsObj.GroupId, usr.UserName);
                                    break;
                                case "Impacts":
                                    Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(origData, typeof(Impacts));
                                    recData = data.DelImpacts(ImpactsObj.ImpactId, usr.UserName);
                                    break;
                                case "IssueStatuses":
                                    IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(origData, typeof(IssueStatuses));
                                    recData = data.DelIssueStatuses(IssueStatusesObj.IssueStatus, usr.UserName);
                                    break;
                                case "Phases":
                                    Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(origData, typeof(Phases));
                                    recData = data.DelPhases(PhasesObj.PhaseId, usr.UserName);
                                    break;
                                case "ProjectType":
                                    ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(origData, typeof(ProjectTypes));
                                    recData = data.DelProjectTypes(ProjectTypesObj.ProjectType, usr.UserName);
                                    break;
                                case "TemplateTypes":
                                    TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(origData, typeof(TemplateTypes));
                                    recData = data.DelTemplateTypes(TemplateTypesObj.TemplateType, usr.UserName);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error deleting record.: Msg: " + ex.Message;
                        }
                        break;
                    case "insert":
                        retval = "success";
                        try
                        {
                            object recData = null;
                            switch (tblName)
                            {
                                case "ApprovalActivityStatuses":
                                    ApprovalActivityStatuses ApprovalActivityStatusesObj = (ApprovalActivityStatuses)JsonConvert.DeserializeObject(newData, typeof(ApprovalActivityStatuses));
                                    recData = data.AddApprovalActivityStatuses(ApprovalActivityStatusesObj.ApprovalStatus, ApprovalActivityStatusesObj.ActivityStatus, ApprovalActivityStatusesObj.ReportDefault, usr.UserName);
                                    break;
                                case "BenefitGroups":
                                    //BenefitGroups BenefitGroupsObj = (BenefitGroups)JsonConvert.DeserializeObject(newData, typeof(BenefitGroups));
                                    //recData = data.AddBenefitGroups(BenefitGroupsObj.BenefitGroupName, BenefitGroupsObj.BenefitGroupOrder, usr.UserName);
                                    break;
                                case "Benefits":
                                    //Benefits BenefitsObj = (Benefits)JsonConvert.DeserializeObject(newData, typeof(Benefits));
                                    //recData = data.AddBenefits(BenefitsObj.BenefitName, BenefitsObj.BenefitGroupId, usr.UserName);
                                    break;
                                case "BudgetStatuses":
                                    //BudgetStatuses BudgetStatusesObj = (BudgetStatuses)JsonConvert.DeserializeObject(newData, typeof(BudgetStatuses));
                                    //recData = data.AddBudgetStatuses(BudgetStatusesObj.BudgetStatus, usr.UserName);
                                    break;
                                case "BudgetTypes":
                                    BudgetTypes BudgetTypesObj = (BudgetTypes)JsonConvert.DeserializeObject(newData, typeof(BudgetTypes));
                                    recData = data.AddBudgetTypes(BudgetTypesObj.BudgetType, usr.UserName);
                                    break;
                                case "CostSavingsTypes":
                                    CostSavingsTypes CostSavingsTypesObj = (CostSavingsTypes)JsonConvert.DeserializeObject(newData, typeof(CostSavingsTypes));
                                    recData = data.AddCostSavingsTypes(CostSavingsTypesObj.CostSavingsType, usr.UserName, CostSavingsTypesObj.CostSavingsTypeId);
                                    break;
                                case "DocumentTypes":

                                    DocumentTypes DocumentTypesObj = (DocumentTypes)JsonConvert.DeserializeObject(newData, typeof(DocumentTypes));
                                    try
                                    {
                                        recData = data.AddDocumentTypes(mod.DocumentType, usr.UserName);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    break;
                                case "EntityTypes":
                                    EntityTypes EntityTypesObj = (EntityTypes)JsonConvert.DeserializeObject(newData, typeof(EntityTypes));
                                    recData = data.AddEntityTypes(mod.EntityType, usr.UserName);
                                    break;
                                case "Groups":
                                    Groups GroupsObj = (Groups)JsonConvert.DeserializeObject(newData, typeof(Groups));
                                    recData = data.AddGroups(GroupsObj.GroupName, usr.UserName);
                                    break;
                                case "Impacts":
                                    //Impacts ImpactsObj = (Impacts)JsonConvert.DeserializeObject(newData, typeof(Impacts));
                                    try
                                    {
                                        recData = data.AddImpacts(mod.ImpactName, mod.ImpactDesc, usr.UserName, mod.ImpactId);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    break;
                                case "IssueStatuses":
                                    IssueStatuses IssueStatusesObj = (IssueStatuses)JsonConvert.DeserializeObject(newData, typeof(IssueStatuses));
                                    try
                                    {
                                        recData = data.AddIssueStatuses(mod.IssueStatus, usr.UserName);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    break;
                                case "Phases":
                                    //Phases PhasesObj = (Phases)JsonConvert.DeserializeObject(newData, typeof(Phases));
                                    recData = data.AddPhases(mod.PhaseName, mod.PhaseDesc, mod.PhaseOrder, mod.color, usr.UserName,7);
                                    break;
                                case "ProjectType":
                                    //ProjectTypes ProjectTypesObj = (ProjectTypes)JsonConvert.DeserializeObject(newData, typeof(ProjectTypes));
                                    recData = data.AddProjectTypes(mod.ProjectType, usr.UserName);
                                    break;

                                case "ServiceAgreementTypes":
                                    recData = data.AddServiceAgreementTypes(mod.ServiceAgreementType, usr.UserName);
                                    break;
                                case "TemplateTypes":
                                    //TemplateTypes TemplateTypesObj = (TemplateTypes)JsonConvert.DeserializeObject(newData, typeof(TemplateTypes));
                                    recData = data.AddTemplateTypes(mod.TemplateType, usr.UserName);
                                    break;

                                case "AttachmentGroups":
                                    recData = data.AddAttachMentGroup(mod.AttachmentGroupName, usr.UserName, 2);
                                    break;

                                case "ServiceAgreementVendors":

                                    recData = data.AddVendors(mod.ServiceAgreementVendorName, usr.UserName, 2);
                                    break;
                            }


                        }
                        catch (Exception ex)
                        {
                            //TODO log error
                            retval = "Error inserting record.: Msg: " + ex.Message;
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
            return retval;

        }


        #endregion
    }
}