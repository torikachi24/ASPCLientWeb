<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="ClientWebOPCUA.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
      <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.js"></script>
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/themes/fusioncharts.theme.fusion.js"></script>
    <script type="text/javascript">
         FusionCharts.ready(function () {
            var chartObj = new FusionCharts({
                type: 'cylinder',
                dataFormat: 'json',
                id:'tank1',
                renderAt: 'chart-container-3',
                width: '200',
                height: '200',
                dataSource: {
                    chart: {
                        caption :'Tank1',
                        animation:'true',
                        cylFillHoverColor: "#0099fd",
                        cylFillHoverAlpha: "85",
                        theme: "fusion",
                        lowerLimit: "0",
                        upperLimit: "120",
                        lowerLimitDisplay: "Empty",
                        upperLimitDisplay: "Full",
                        numberSuffix: " ltrs",
                        showValue: "1",
                        chartBottomMargin: "20",
                    },
                    value: 110,
                    annotations: {
                        origw: "400",
                        origh: "190",
                        autoscale: "1",
                    }
                }}).render();
                            var myVar = setInterval(function () {
                FusionCharts.items["tank1"].feedData("&value=" + Math.random()*100);
            }, 1000);
        });
 </script>
                <div id="chart-container-3">FusionCharts will render here</div>

</asp:Content>
