<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ClientWebOPCUA.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="chart-container-3">FusionCharts will render here</div>
        </div>
    </form>
</body>
</html>
