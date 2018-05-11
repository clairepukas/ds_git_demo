using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Probe.Models;

using System.Configuration;
using System.Web.Configuration;
using Microsoft.Reporting.WebForms;
using System.Reflection;

namespace Probe
{
    public partial class RptVwr : System.Web.UI.Page
    {
        ProbeData data = ProbeDataContext.GetDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                //Required for report events to be handled properly.
                ReportViewer1.SubmittingParameterValues += rptvwr_SubmittingParameterValues;
                ReportViewer1.PreRender += ReportViewer_PreRender;
            }
            catch (Exception)
            {
                //Do something
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    ReportViewer rptvwr1 = ReportViewer1;
                    RptCredentials crd = new RptCredentials();

                    rptvwr1.ServerReport.ReportServerCredentials = crd;
                    rptvwr1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServerUrl"]);

                    if (!string.IsNullOrEmpty(Session["ActiveReportPath"].ToString()))
                    {
                        rptvwr1.ServerReport.ReportPath = Session["ActiveReportPath"].ToString();
                        List<ReportParameter> parms = SetParms(Session["ActiveReportName"].ToString());
                        rptvwr1.ServerReport.SetParameters(parms);
                    }
                }
            }
            catch (Exception)
            {
                //Do something
            }
        }

        public List<ReportParameter> SetParms(string ReportName)
        {
            List<ReportParameter> parms = new List<ReportParameter>();
            switch (ReportName)
            {
                case "Project Information Report":
                    parms.Add(new ReportParameter("ProjectRole", "All Managers"));
                    break;
                case "Project Budgets Report":

                    break;
                case "Project Benefits Report":

                    break;
                case "Project Resources Report":

                    break;
                case "Project Information By Year and Project Manager":
                    parms.Add(new ReportParameter("ProjectRole", "Manager"));
                    break;
                case "Project Information By Year and Project Engineer":
                    parms.Add(new ReportParameter("ProjectRole", "Engineer"));
                    break;
                case "Project Resources By Year and Resource Group":

                    break;
                case "Project Information By Year and Department":

                    break;
                case "Project Budgets By Project and Year":

                    break;
                case "Budgets Totals By Year":

                    break;
                case "Overdue Status Update Report":

                    break;

                default:
                    break;
            }
            return parms;
        }

        void rptvwr_SubmittingParameterValues(object sender, ReportParametersEventArgs e)
        {
            try
            {
                string bubba = "bubba";
            }
            catch
            {
                throw;
            }
        }

        protected void ReportViewer_PreRender(object sender, EventArgs e)
        {
            //DisableUnwantedExportFormat((ReportViewer)sender, "CSV");
            //DisableUnwantedExportFormat((ReportViewer)sender, "PDF");
        }

        public void DisableUnwantedExportFormat(ReportViewer ReportViewerID, string strFormatName)
        {
            FieldInfo info;
            foreach (RenderingExtension extension in ReportViewerID.ServerReport.ListRenderingExtensions())
            {
                if (extension.Name == strFormatName)
                {
                    info = extension.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                    info.SetValue(extension, false);
                }

            }
        }


    }
}