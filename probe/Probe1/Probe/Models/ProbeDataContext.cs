using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Runtime.Serialization;

namespace Probe.Models
{
    public class ProbeDataContext
    {
        public static ProbeData GetDataContext()
        {
            return new ProbeData(WebConfigurationManager.ConnectionStrings["probedb"].ConnectionString);
        }
    }

    public partial class ProbeData : DataContext
    {
        public ProbeData(string constr) : base(constr) { }

    }
    [DataContract]
    public class NameValuePair
    {
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string Value { get; set; } 
        public NameValuePair() { }
    }
}