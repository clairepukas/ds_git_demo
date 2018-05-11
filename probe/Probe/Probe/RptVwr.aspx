<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptVwr.aspx.cs" Inherits="Probe.RptVwr" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report</title>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
    <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1' />
    <link href="~/Content/Probe.css" rel="stylesheet" type="text/css" />
    <link href="~/Content/Site.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-2.1.3.js"></script>
</head>
<body>
    <div style="text-align: left;">
        <asp:Label ID="ErrorMsg" runat="server" style="font-family:Arial;font-size:8pt;color:red"></asp:Label>
    </div>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="false" />        
        <rsweb:ReportViewer 
            ID="ReportViewer1" 
            runat="server" 
            ProcessingMode="Remote" 
            AsyncRendering="false"
            ShowZoomControl="false"
            PageCountMode="Actual" 
            ShowPromptAreaButton="false" 
            SizeToReportContent="true" 
            Width="100%" 
            Height="100%">
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
    <script>

        

        var $e = $("#ParametersRowReportViewer1").nextAll().eq(2).children().eq(2).children().eq(0).children().eq(0).html()

    </script>
</html>
