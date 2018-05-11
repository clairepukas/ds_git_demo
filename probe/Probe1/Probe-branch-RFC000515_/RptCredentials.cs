using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Net;
using System.Security.Principal;

namespace Probe
{
    [Serializable]
    public sealed class RptCredentials : IReportServerCredentials
    {
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use the default Windows user.  Credentials will be
                // provided by the NetworkCredentials property.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Read the user information from the Web.config file.  
                // By reading the information on demand instead of 
                // storing it, the credentials will not be stored in 
                // session, reducing the vulnerable surface area to the
                // Web.config file, which can be secured with an ACL.

                // User name
                string userName = ConfigurationManager.AppSettings["ReportUser"];

                if (string.IsNullOrEmpty(userName))
                    throw new Exception(
                        "Missing reprot user name from web.config file");

                // Password
                string password = ConfigurationManager.AppSettings["ReportPwd"];

                if (string.IsNullOrEmpty(password))
                    throw new Exception(
                        "Missing report password from web.config file");

                // Domain
                string domain = ConfigurationManager.AppSettings["ReportDomain"];

                if (string.IsNullOrEmpty(domain))
                    throw new Exception(
                        "Missing report domain from web.config file");

                return new NetworkCredential(userName, password, domain);
            }
        }
        public bool GetFormsCredentials(out Cookie authCookie,
                    out string userName, out string password,
                    out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            // Not using form credentials
            return false;
        }
    }
}