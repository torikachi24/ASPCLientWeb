<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="Subcription.aspx.cs" Inherits="ClientWebOPCUA.Subcription" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <a id="div1" class="lbrun">I'm Client Subcribe Page</a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="/Content/styleMonitor.css" type="text/css" />
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.js"></script>
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/themes/fusioncharts.theme.fusion.js"></script>

    <script src="Scripts/jquery-3.5.1.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@2.9.3"></script>
    <%--<script src="https://cdn.jsdelivr.net/npm/hammerjs@2.0.8"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-zoom@0.7.7"></script>--%>
    <div class="frame" onscroll="true ">
        <asp:TextBox ID="SubcriptionID" CssClass="inread" runat="server"></asp:TextBox>
        <asp:Button ID="SubcribeBtn" CssClass="button" runat="server" Text="Subcribe" OnClick="Subcribe_Click" />
        <asp:Button ID="UnSubcribeBtn" CssClass="button" runat="server" Text="UnSubcribe" OnClick="UnSubcribeBtn_Click" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="100" OnTick="Timer1_Tick" Enabled="false"></asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>

        <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="tabcontain" ScrollBars="Auto" OnActiveTabChanged="TabContainer1_ActiveTabChanged1" AutoPostBack="true">
            <ajaxToolkit:TabPanel runat="server" HeaderText="TableView" ID="TabPanel1" Height="560" BackColor="#D2FFFFFF">
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" Width="1150px">
                                <RowStyle Width="50%" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" HeaderText="GraphView" ID="TabPanel2" Height="580" ScrollBars="Horizontal">
                <ContentTemplate>
                    <div class="chartWrapper">
                        <div class="chart-wrapper">
                            <div id="chart-container"></div>
                            <div class="buttonwrapper pt-2">
                                <div class="form-elements">
                                    <div class="input-wrapper">
                                        <select id="format">
                                            <option value="png">png</option>
                                            <option value="jpg">jpg</option>
                                            <option value="pdf">pdf</option>
                                            <option value="csv">csv</option>
                                        </select>&nbsp;&nbsp;
      <span class="label-shift-label">Select a format</span>&nbsp;
                                    </div>
                                    <button id="export" class="btn-1">Export Chart</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" HeaderText="Report" ID="TabPanel3" Height="560" ScrollBars="Horizontal">
                <ContentTemplate>
                    <asp:Button ID="Button3" runat="server" Text="Load" OnClick="Load_Report_Click" />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1100px" ProcessingMode="Local"></rsweb:ReportViewer>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>

    <script type="text/javascript">

        var data;
        function update(a) {
            data = a;
        }
        FusionCharts.ready(function () {
            var revenueChart = new FusionCharts({
                id: "stockRealTimeChart",
                type: 'realtimeline',
                renderAt: 'chart-container',
                width: '1000',
                height: '500',
                dataFormat: 'json',
                dataSource: {
                    "chart": {
                        "caption": "Graph Monitor Node ID",
                        "xAxisName": "Time",
                        "yAxisName": "Value",
                        "refreshinterval": "2",
                        "yaxisminvalue": "0",
                        "yaxismaxvalue": "100",
                        "numdisplaysets": "20",
                        "labeldisplay": "rotate",
                        "showRealTimeValue": "1",
                        "theme": "fusion",
                        "exportenabled": "1",
                        "exportshowmenuitem": "0"
                    },
                    "categories": [{
                        "category": [{
                            "label": "Day Start"
                        }]
                    }],
                    "dataset": [{
                        "data": [{
                            "value": "35.27"
                        }]
                    }]
                },
                "events": {
                    "initialized": function (e) {
                        function addLeadingZero(num) {
                            return (num <= 9) ? ("0" + num) : num;
                        }

                        function updateData() {
                            // Get reference to the chart using its ID
                            var chartRef = FusionCharts("stockRealTimeChart"),
                                // We need to create a querystring format incremental update, containing
                                // label in hh:mm:ss format
                                // and a value (random).
                                currDate = new Date(),
                                label = addLeadingZero(currDate.getHours()) + ":" +
                                    addLeadingZero(currDate.getMinutes()) + ":" +
                                    addLeadingZero(currDate.getSeconds()),
                                // Get random number between 35.25 & 35.75 - rounded to 2 decimal places
                                randomValue = data,
                                // Build Data String in format &label=...&value=...
                                strData = "&label=" + label +
                                    "&value=" +
                                    randomValue;
                            // Feed it to chart.
                            chartRef.feedData(strData);
                        }

                        var myVar = setInterval(function () {
                            updateData();
                            console.log(data);
                        }, 2000);

                    }
                }
            })
                .render();
            function export_chart() {
                var format = document.getElementById("format").value;
                revenueChart.exportChart({
                    exportFormat: format
                });
            }
            document.getElementById("export").addEventListener("click", export_chart);
        });
    </script>
</asp:Content>