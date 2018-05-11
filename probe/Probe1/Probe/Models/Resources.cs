using Probe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;

public class ResourcesData
{
    string li_o = "<li>";
    string li_c = "</li>";
    string ul_o = "<ul>";
    string ul_c = "</ul>";
    string a_o = "<a class='rsrcLnk' >";
    string a_c = "</a>";
    string d_o = "<div class='yearset'>";
    string d_c = "</div>";
    string i_o = "<input type='text' ";
    string i_c = " />";
    string i_val0 = " value='0' ";
    string rdonly = " readonly ";
    string ick_o = "<input type='checkbox' ";
    string ick_c = " />";
    string li_wid = "<li id=";



    public bool AllResources { get; set; }
    public bool IsReadonly { get; set; }

    #region Project Entity Resources
    public string GetResourcesTree(int projId)
    {
        var db = Database.Open("probedb");
        string retval = "";

        //get project years
        List<int> years = new List<int>();
        DateTime startDt = getProjectStart(projId);
        DateTime endDt = getProjectEnd(projId);

        //create year title elements
       // retval = ul_o + li_o + a_o + "&nbsp;" + a_c;
        //retval += "<div class='actyrtitles'> Projected/Actual" + d_c + "<div class='propyrtitles'> Proposed" + d_c + li_c + ul_c;
        
        //retval = ul_o + li_o + a_o + "<h2>Resource Man Hours</h2>" + a_c + "<a id='expand' class='rsrcLnk' >expand all" + a_c + "<a id='collapse' class='rsrcLnk' >collapse all" + a_c;

        //retval += li_c + ul_c;

        retval = ul_o + li_o + a_o + "&nbsp;" + a_c;

        //add titles above year row

        string propYrTitles = "<div><div class='propyrtitles'>Proposed" + d_c + d_o ;
        string actYrTitles = "<div><div class='actyrtitles'>Projected/Actual" + d_c + d_o;

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

        string activitystatus = db.QueryValue("select ApprovalStatus from Projects where ProjectId = " + projId);
        bool doActuals = (activitystatus == "Approved"?true:false);

        for (int addyr = startDt.Year; endDt.Year >= addyr; addyr++)
        {
            years.Add(addyr);
            propYrTitles += i_o + " value='" + addyr + "' /><img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
            actYrTitles += i_o + " value='" + addyr + "' /><img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
        }
        //the order of these is important
        retval += (doActuals?actYrTitles:"") + d_c + d_c + propYrTitles + d_c + d_c + li_c + ul_c;

            try
            {

                var toplvlrecs = db.Query(@"select * from Entities where ParentId <2");


                foreach (var top in toplvlrecs)
                {
                    string toplvlText = "";

                    bool hasChildren = false;

                    //each of these starts a new UL element with first LI element being the Entity name inside an anchor tag
                    toplvlText += ul_o + li_wid + "\"entityNode_" + projId + "_" + top.EntityId + "\">" + a_o + top.EntityName + a_c;

                    // ****************  add year sets for each year in the year list ***********************************
                    string proposedYrs = d_o;

                    string actualYrs = d_o;
                    string propYrInputs = "";
                    string actYrInputs = "";

                    foreach (int yr in years)
                    {
                        propYrInputs += i_o + " id='proptot_" + yr + "_" + projId + "_" + top.EntityId + "' " + i_val0 + " readonly " + i_c;
                        propYrInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                        actYrInputs += i_o + " id='acttot_" + yr + "_" + projId + "_" + top.EntityId + "' " + i_val0 + " readonly " + i_c;
                        actYrInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                    }

                    proposedYrs += propYrInputs + d_c;
                    actualYrs += actYrInputs + d_c;

                    //this order is important to have it render correctly in the browser.
                    toplvlText += (doActuals ? actualYrs : "") + proposedYrs;

                    //*************************** end year inputs ********************************************************



                    //get any child entities
                    string childEnts = getChildEntities(projId, top.EntityId, years,doActuals);
                    string childRsrcs = getChildResources(projId, top.EntityId, years,doActuals);


                    if (AllResources)
                    {
                        toplvlText += childEnts + childRsrcs;
                        toplvlText += li_c + ul_c;
                        retval += toplvlText;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(childEnts))
                        {
                            toplvlText += childEnts;
                            hasChildren = true;
                        }
                        if (!String.IsNullOrEmpty(childRsrcs))
                        {
                            toplvlText += childRsrcs;
                            hasChildren = true;
                        }
                        if (hasChildren)
                        {
                            toplvlText += li_c + ul_c;
                            retval += toplvlText;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        return retval;
    }
    public string getChildEntities(int projId, int entityId, List<int> years, bool doActuals)
    {
        string retval = "";
        
        var db = Database.Open("probedb");

        var chrecs = db.Query(@"select * from Entities where ParentId = " + entityId);

        foreach (var ch in chrecs)
        {
            string childEntTxt = "";
            bool hasChildren = false;
            //each of these starts a new UL element with first LI element being the Entity name inside an anchor tag
            childEntTxt += ul_o + li_wid + "\"entityNode_" + projId + "_" + ch.EntityId + "\">" + a_o + ch.EntityName + a_c;

            // add year sets for each year in the year list
            string proposedYrs = d_o;
            string actualYrs = d_o;
            string propInputs = "";
            string actInputs = "";
            foreach (int yr in years)
            {
                propInputs += i_o + " id='proptot_" + yr + "_" + projId + "_" + ch.EntityId + "' " + i_val0 + " readonly " + i_c;
                propInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                actInputs += i_o + " id='acttot_" + yr + "_" + projId + "_" + ch.EntityId + "' " + i_val0 + " readonly" + i_c;
                actInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
            }
            proposedYrs += propInputs + d_c;
            actualYrs += actInputs + d_c;

            //this order is important to have it render correctly in the browser.
            childEntTxt += (doActuals?actualYrs:"") + proposedYrs;

            //get any child entities and Resources
            string childEnts = getChildEntities(projId, ch.EntityId, years,doActuals);
            string childRsrcs = getChildResources(projId, ch.EntityId, years,doActuals);

            if (AllResources)
            {
                childEntTxt += childEnts + childRsrcs;
                childEntTxt += li_c + ul_c;
                retval += childEntTxt;
            }
            else
            {
                if (!String.IsNullOrEmpty(childEnts))
                {
                    childEntTxt += childEnts;
                    hasChildren = true;
                }
                if (!String.IsNullOrEmpty(childRsrcs))
                {
                    childEntTxt += childRsrcs;
                    hasChildren = true;
                }
                if (hasChildren)
                {
                    childEntTxt += li_c + ul_c;
                    retval += childEntTxt;
                }
            }

        }
        return retval;
    }

    public string getChildResources(int projId, int entityId, List<int> years,bool doActuals)
    {
        string retval = "";
        
        var db = Database.Open("probedb");

        string sqltxt = @"select r.ResourceId,
                                r.Comments,
                                r.ResourceName,
                                r.EntityId                                
                                from Resources r 
                                where r.EntityId = " + entityId + " order by r.ResourceName";

        var chrecs = db.Query(sqltxt);
        foreach (var rec in chrecs)
        {
            string childRsrcTxt = "";

            // add year sets for each year in the year list
            string proposedYrs = d_o;
            string actualYrs = d_o;
            string propInputs = "";
            string actInputs = "";
            bool InProject = false;
          

            foreach (int yr in years)
            {
                string propMhrs = "0";
                string actMhrs = "0";
                //for (int mn = 1; mn < 13; mn++)
                //{
                    //get any matching ProjectResource record by ProjectId, ResourceId and Year
                string prsql =@"select pr.ProjectId, pr.ResourceId,
                                        pr.Year,
                                    sum(isnull(pr.ProposedManHrs,0)) as ProposedManHrs,
                                    sum(isnull(pr.ActualManHrs,0)) as ActualManHrs                                                          
                                    from ProjectResources pr                                     
                                where pr.ProjectId = " + projId + " and pr.ResourceId =" + rec.ResourceId + " and [Year] = " + yr + " group by pr.ProjectId, pr.ResourceId,pr.Year";


                    string noteimg = "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                    var pr_recs = db.Query(prsql, null);
                    if (pr_recs.Count() == 1)
                    {
                        InProject = true;
                        propMhrs = "" + pr_recs.FirstOrDefault().ProposedManHrs;
                        actMhrs = "" + pr_recs.FirstOrDefault().ActualManHrs;
                    }
                    noteimg = "<img id='edit_" + yr + "_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/pencil.png' alt='edit image'  title='Edit this Resource for this Year'  />";

                    //propInputs += i_o + " class='propval' id='propval_" + yr +  "_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  value='" + propMhrs + "' onkeypress='return isNumberKey(event)' onkeyup='getKey();' " + (IsReadonly ? " readonly " : " ") + i_c;
                    propInputs += i_o + " class='propval' id='propval_" + yr +  "_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  value='" + propMhrs + "' readonly " + i_c;
                    propInputs += noteimg;
                    //actInputs += i_o + " class='actval'  id='actval_" + yr + "_" + projId + "_" + entityId + "_" + rec.ResourceId + "' value='" + actMhrs + "' onkeypress='return isNumberKey(event);' onkeyup='getKey();' " + (IsReadonly ? " readonly " : " ") + i_c;
                    actInputs += i_o + " class='actval'  id='actval_" + yr + "_" + projId + "_" + entityId + "_" + rec.ResourceId + "' value='" + actMhrs + "' readonly " + i_c;
                    //actInputs += "<img id='notes_" + yr + "_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";
                    actInputs += noteimg;
                //}
            }
            proposedYrs += propInputs + d_c;
            actualYrs += actInputs + d_c;

            if (AllResources)
            {
                childRsrcTxt += ul_o + li_o;
                childRsrcTxt += ick_o + " class='inProject' id='ckval_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  " + (InProject ? " checked " : " ") + (IsReadonly ? " disabled " : " ") + " "  + ick_c;
                //childRsrcTxt += "<img id='notes_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";
                childRsrcTxt += "<img id='info_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/information.png' alt='info image'  title='View this Resource Details' />";
                childRsrcTxt += a_o + rec.ResourceName + a_c;

                //this order is important to have it render correctly in the browser.
                childRsrcTxt += (doActuals?actualYrs:"") + proposedYrs;

                childRsrcTxt += li_c + ul_c;
                retval += childRsrcTxt;
            }
            else
            {
                if (InProject)
                {
                    childRsrcTxt += ul_o + li_o;
                    childRsrcTxt += ick_o + " class='inProject' id='ckval_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  " + (InProject ? " checked " : " ") + (IsReadonly ? " disabled " : " ") + " " + ick_c;
                    //childRsrcTxt += "<img id='notes_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";
                    childRsrcTxt += "<img id='info_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/information.png' alt='info image'  title='View this Resource Details'  />";
                    childRsrcTxt += a_o + rec.ResourceName + a_c;

                    //this order is important to have it render correctly in the browser.
                    childRsrcTxt += (doActuals?actualYrs:"") + proposedYrs;

                    childRsrcTxt += li_c + ul_c;
                    retval += childRsrcTxt;
                }

            }

        }
                
        return retval;
    }
    #endregion

    #region Entity Resources Admin
    public string GetResourcesAdminTree(ProbeUser usr)
    {
        var db = Database.Open("probedb");
        string retval = "";

        //get ResourceManHours years
        var rmhYrs = db.Query("Select distinct [Year] from ResourceManHours where [Year] >= datepart(yy,getdate()) order by [Year]");
        List<int> years = new List<int>();

        retval = ul_o + li_o + a_o + "&nbsp;" + a_c;

        //add titles above year row

        string baseYrTitles = "<div><div class='baseyrtitles'>Base Hours" + d_c + d_o;
        string coreYrTitles = "<div><div class='coreyrtitles'>Core Hours" + d_c + d_o;

        foreach (var addyr in rmhYrs)
        {
            years.Add(addyr.Year);
            baseYrTitles += i_o + " value='" + addyr.Year + "' />";
            baseYrTitles += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
            coreYrTitles += i_o + " value='" + addyr.Year + "' />";
            coreYrTitles += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
        }
        //the order of these is important
        retval += coreYrTitles + d_c + d_c + baseYrTitles + d_c + d_c + li_c + ul_c;

        try
        {
            string sqltxt = @"select * from Entities where ParentId < 2";
            if (!usr.IsAdmin)
            {
                sqltxt = "select * from Entities where UserId=" + usr.UserId;
            }

            var toplvlrecs = db.Query(sqltxt);


            foreach (var top in toplvlrecs)
            {
                string toplvlText = "";

                bool hasChildren = false;

                //each of these starts a new UL element with first LI element being the Entity name inside an anchor tag
                toplvlText += ul_o + li_wid + "\"entityNode_" + top.EntityId + "\">" + a_o + top.EntityName + a_c;
                if (AllResources) //assume this is admin
                {
                    toplvlText += "<img id='addEntity_" + top.EntityId + "' src='../images/chart_organisation.png' alt='add entity' title='Add Entity'/>";
                    toplvlText += "<img id='addRsrc_" + top.EntityId + "' src='../images/user.png' alt='add resource' title='Add Resource'/>";
                    //toplvlText += "<img id='delEntity_'" + top.EntityId + "' src='../images/delete.png' alt='remove entity' title='Remove this Entity' />";
                }

                // add year sets for each year in the year list
                string baseYrs = d_o;
                string coreYrs = d_o;                string baseInputs = "";
                string coreInputs = "";
                foreach (int yr in years)
                {
                    baseInputs += i_o + " id='basetot_" + yr + "_" + usr.UserId + "_" + top.EntityId + "' " + i_val0 + " readonly " + i_c;
                    baseInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                    coreInputs += i_o + " id='coretot_" + yr + "_" + usr.UserId + "_" + top.EntityId + "' " + i_val0 + " readonly " + i_c;
                    coreInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                }
                baseYrs += baseInputs + d_c;
                coreYrs += coreInputs + d_c;

                //this order is important to have it render correctly in the browser.
                toplvlText += coreYrs + baseYrs;

                //get any child entities
                string childEnts = getRsrcAdminChildEntities(usr.UserId, top.EntityId, years);
                string childRsrcs = getRsrcAdminChildResources(usr.UserId, top.EntityId, years);

                if (AllResources)
                {
                    toplvlText += childEnts + childRsrcs;
                    toplvlText += li_c + ul_c;
                    retval += toplvlText;
                }
                else
                {
                    if (!String.IsNullOrEmpty(childEnts))
                    {
                        toplvlText += childEnts;
                        hasChildren = true;
                    }
                    if (!String.IsNullOrEmpty(childRsrcs))
                    {
                        toplvlText += childRsrcs;
                        hasChildren = true;
                    }
                    if (hasChildren)
                    {
                        toplvlText += li_c + ul_c;
                        retval += toplvlText;
                    }
                }

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return retval;
    }
    public string getRsrcAdminChildEntities(int UserId, int entityId, List<int> years)
    {
        string retval = "";

        var db = Database.Open("probedb");

        var chrecs = db.Query(@"select * from Entities where ParentId = " + entityId);

        foreach (var ch in chrecs)
        {
            string childEntTxt = "";
            bool hasChildren = false;
            //each of these starts a new UL element with first LI element being the Entity name inside an anchor tag
            childEntTxt += ul_o + li_wid + "\"entityNode_" + ch.EntityId + "\">" + a_o + ch.EntityName + a_c;
            if (AllResources)//assume this is admin
            {
                childEntTxt += "<img id='addEntity_" + ch.EntityId + "' src='../images/chart_organisation.png' alt='add entity' title='Add Entity'/>";
                childEntTxt += "<img id='addRsrc_" + ch.EntityId + "' src='../images/user.png' alt='add resource' title='Add Resource'/>";
                childEntTxt += "<img id='delEntity_" + ch.EntityId + "' src='../images/delete.png' alt='remove entity' title='Remove this Entity'/>";
            }
            // add year sets for each year in the year list
            string baseYrs = d_o;
            string coreYrs = d_o;
            string baseInputs = "";
            string coreInputs = "";
            foreach (int yr in years)
            {
                baseInputs += i_o + " id='basetot_" + yr + "_" + UserId + "_" + ch.EntityId + "' " + i_val0 + " readonly " + i_c;
                baseInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                coreInputs += i_o + " id='coretot_" + yr + "_" + UserId + "_" + ch.EntityId + "' " + i_val0 + " readonly" + i_c;
                coreInputs += "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";

            }
            baseYrs += baseInputs + d_c;
            coreYrs += coreInputs + d_c;

            //this order is important to have it render correctly in the browser.
            childEntTxt += coreYrs + baseYrs;

            //get any child entities and Resources
            string childEnts = getRsrcAdminChildEntities(UserId, ch.EntityId, years);
            string childRsrcs = getRsrcAdminChildResources(UserId, ch.EntityId, years);
            if (AllResources)
            {
                childEntTxt += childEnts + childRsrcs;
                childEntTxt += li_c + ul_c;
                retval += childEntTxt;
            }
            else
            {
                if (!String.IsNullOrEmpty(childEnts))
                {
                    childEntTxt += childEnts;
                    hasChildren = true;
                }
                if (!String.IsNullOrEmpty(childRsrcs))
                {
                    childEntTxt += childRsrcs;
                    hasChildren = true;
                }
                if (hasChildren)
                {
                    childEntTxt += li_c + ul_c;
                    retval += childEntTxt;
                }
            }

        }
        return retval;
    }

    public string getRsrcAdminChildResources(int UserId, int entityId, List<int> years)
    {
        string retval = "";

        var db = Database.Open("probedb");

        string sqltxt = @"select r.ResourceId,
                                r.Comments,
                                r.ResourceName,
                                r.EntityId                                
                                from Resources r 
                                where r.EntityId = " + entityId + " order by r.ResourceName";

        var chrecs = db.Query(sqltxt);
        foreach (var rec in chrecs)
        {
            string childRsrcTxt = "";

            // add year sets for each year in the year list
            string baseYrs = d_o;
            string coreYrs = d_o;
            string baseInputs = "";
            string coreInputs = "";
            bool InProject = false;

            foreach (int yr in years)
            {
                string baseMhrs = "0";
                string coreMhrs = "0";

                //get any matching ProjectResource record by ProjectId, ResourceId and Year
                string prsql = @"select sum(isnull(rmh.BaseManHours,0)) as BaseManHours,
                                    sum(isnull(rmh.CoreManHours,0)) as CoreManHours,
                                    rmh.Year                                                          
                                    from ResourceManHours rmh  
                                where rmh.ResourceId =" + rec.ResourceId + " and [Year] = " + yr + " group by rmh.ResourceId,rmh.Year";

                string noteimg = "";// "<img id='blank_icon' src='../images/blank_icon.png' alt='blank icon image' />";
                var pr_recs = db.Query(prsql, null);
                if (pr_recs.Count() == 1)
                {
                    InProject = true;
                    baseMhrs = "" + pr_recs.FirstOrDefault().BaseManHours;
                    coreMhrs = "" + pr_recs.FirstOrDefault().CoreManHours;
                }
                noteimg = "<img id='edit_" + yr + "_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/pencil.png' alt='note image' title='Edit this Resource for this Year'  />";

                baseInputs += i_o + " class='propval' id='baseval_" + yr + "_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  value='" + baseMhrs + "' onkeypress='return isNumberKey(event)' onkeyup='getKey();' " + (IsReadonly ? " readonly " : " ") + i_c;
                baseInputs += noteimg;
                coreInputs += i_o + " class='propval'  id='coreval_" + yr + "_" + UserId + "_" + entityId + "_" + rec.ResourceId + "' value='" + coreMhrs + "' onkeypress='return isNumberKey(event);' onkeyup='getKey();' " + (IsReadonly ? " readonly " : " ") + i_c;
                coreInputs += noteimg;
                //actInputs += "<img id='notes_" + yr + "_" + projId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";

            }
            baseYrs += baseInputs + d_c;
            coreYrs += coreInputs + d_c;

            if (AllResources)
            {
                childRsrcTxt += ul_o + li_o;
                //childRsrcTxt += ick_o + " class='inProject' id='ckval_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  " + (InProject ? " checked " : " ") + (IsReadonly ? " disabled " : " ") + " " + ick_c;
                //childRsrcTxt += "<img id='rsrcnotes_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";
                childRsrcTxt += "<img id='delRsrc_" + UserId + "_" + entityId + "_" + rec.ResourceId + "' src='../images/delete.png' alt='add resource' title='Remove this Resource' />";
                childRsrcTxt += "<img id='info_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/pencil.png' alt='info image' title='Edit this Resource' />";
                childRsrcTxt += a_o + rec.ResourceName + a_c;

                //this order is important to have it render correctly in the browser.
                childRsrcTxt += coreYrs + baseYrs;

                childRsrcTxt += li_c + ul_c;
                retval += childRsrcTxt;
            }
            else
            {
                if (InProject)
                {
                    childRsrcTxt += ul_o + li_o;
                    //childRsrcTxt += ick_o + " class='inProject' id='ckval_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  " + (InProject ? " checked " : " ") + (IsReadonly ? " disabled " : " ") + " " + ick_c;
                    //childRsrcTxt += "<img id='rsrcnotes_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/note.png' alt='note image' />";
                    childRsrcTxt += "<img id='info_" + UserId + "_" + entityId + "_" + rec.ResourceId + "'  src='../images/pencil.png' alt='info image' title='Edit this Resource'  />";
                    childRsrcTxt += a_o + rec.ResourceName + a_c;

                    //this order is important to have it render correctly in the browser.
                    childRsrcTxt += coreYrs + baseYrs;

                    childRsrcTxt += li_c + ul_c;
                    retval += childRsrcTxt;
                }

            }

        }

        return retval;
    }

    public DateTime getProjectStart(int projId)
    {
        var db = Database.Open("probedb");
        var projectData = db.Query(@"select
                                        p.ProjectId,
	                                         p.ProjectName,
		                                        p.ProposedStart,
		                                        p.ProjectedStart,
		                                        p.ActualStart
		                                        from Projects p
		                                        where p.ProjectId=" + projId);

        var actStart = projectData.FirstOrDefault().ActualStart;
        var projStart = projectData.FirstOrDefault().ProjectedStart;
        var schedStart = projectData.FirstOrDefault().ProposedStart;

        if (actStart != null) { actStart = (actStart.Year < 2000 ? null : actStart); }
        if (projStart != null) { projStart = (projStart.Year < 2000 ? null : projStart); }
        if (schedStart != null) { schedStart = (schedStart.Year < 2000 ? null : schedStart); }

        if (projStart != null)
        {
            if (schedStart == null)
            {
                schedStart = projStart;
            }
            else
            {
                if (projStart.Year > 1910)
                {
                    if (projStart < schedStart)
                    {

                        schedStart = projStart;
                    }
                }
            }
        }

        if (actStart != null)
        {
            if (schedStart == null)
            {
                schedStart = actStart;
            }
            else
            {
                if (actStart.Year > 1910)
                {
                    if (actStart < schedStart)
                    {
                        schedStart = actStart;
                    }
                }
            }
        }

        return schedStart;
    }
    public DateTime getProjectEnd(int projId)
    {
        var db = Database.Open("probedb");
        var projectData = db.Query(@"select
                                        p.ProjectId,
	                                         p.ProjectName,
		                                        p.ProposedEnd,
		                                        p.ProjectedEnd,
		                                        p.ActualEnd
		                                        from Projects p
		                                        where p.ProjectId=" + projId);

        var actEnd = projectData.FirstOrDefault().ActualEnd;
        var projEnd = projectData.FirstOrDefault().ProjectedEnd;
        var schedEnd = projectData.FirstOrDefault().ProposedEnd;

        if (actEnd != null){actEnd = (actEnd.Year < 2000?null:actEnd); }
        if (projEnd != null) { projEnd = (projEnd.Year < 2000 ? null : projEnd); }
        if (schedEnd != null) { schedEnd = (schedEnd.Year < 2000 ? null : schedEnd); }


        if (projEnd != null)
        {
            if (schedEnd == null)
            {
                schedEnd = projEnd;
            }
            else
            {
                if (projEnd.Year > 1910)
                {
                    if (projEnd > schedEnd)
                    {

                        schedEnd = projEnd;
                    }
                }
            }
        }

        if (actEnd != null)
        {
            if (schedEnd == null)
            {
                schedEnd = actEnd;
            }
            else
            {
                if (actEnd.Year > 1910)
                {
                    if (actEnd > schedEnd)
                    {
                        schedEnd = actEnd;
                    }
                }
            }
        }

        return schedEnd;
    }
    #endregion

}

