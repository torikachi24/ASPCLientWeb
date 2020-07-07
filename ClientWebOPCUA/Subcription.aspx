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
            <ajaxToolkit:TabPanel runat="server" HeaderText="TableView" ID="TabPanel1" Height="580" BackColor="#D2FFFFFF" >
                <ContentTemplate>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" Width="1130px">
                                <RowStyle Width="50%" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" HeaderText="GraphView" ID="TabPanel2" Height="580" ScrollBars="Horizontal">
                <ContentTemplate>
                    <asp:Label ID="Label1" runat="server" Text="Export Image :"></asp:Label>
                    <asp:Button ID="Button1" runat="server" Text="Click me!" />
                    <div class="chartWrapper">
                        <canvas id="myChart"></canvas>
                    </div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" HeaderText="Report" ID="TabPanel3" Height="580" ScrollBars="Horizontal">
                <ContentTemplate>
                    <asp:Button ID="Button3" runat="server" Text="Load" OnClick="Load_Report_Click" />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="1150px" ProcessingMode="Local"></rsweb:ReportViewer>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>

    <script type="text/javascript">

        var canvas = document.getElementById('myChart');
        var data = {
            labels: ["", "", "", "", "", "", "", "", "", ""],
            datasets: [
                {
                    label: "Data Monitored",
                    fill: true,
                    fillColor: "rgba(75,192,192,0.2)",
                    lineTension: 0.0,
                    backgroundColor: "rgba(75,192,192,0.4)",
                    borderColor: "rgba(75,192,192,1)",
                    borderCapStyle: 'butt',
                    borderDash: [],
                    borderDashOffset: 0.0,
                    borderJoinStyle: 'miter',
                    pointBorderColor: "rgba(75,192,192,1)",
                    pointBackgroundColor: "#fff",
                    pointBorderWidth: 1,
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: "rgba(75,192,192,1)",
                    pointHoverBorderColor: "rgba(220,220,220,1)",
                    pointHoverBorderWidth: 2,
                    pointRadius: 5,
                    pointHitRadius: 10,
                    data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    steppedLine: true,
                }
            ]
        };

        function adddata(num) {
            var now = new Date();
            myLineChart.data.labels.push(now.format("hh:mm mm/dd/yyyy"));
            var length = myLineChart.data.datasets.length;

            //myLineChart.data.labels.splice(0, 1);
            //myLineChart.data.datasets[0].data.splice(0, 1);
            //myLineChart.data.datasets[0].data.push(num);
            myLineChart.data.datasets.forEach((dataset) => {
                dataset.data.push(num);
            });

            myLineChart.update();
        };

        var option = {
            //responsive: true,
            //maintainAspectRatio: false,
            showLines: true,
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        beginAtZero: true,
                    }
                }],
            },
        };
        var myLineChart = Chart.Line(canvas, {
            data: data,
            options: option
        });
        new Chart(canvas).Line(data, {
            onAnimationComplete: function () {
                var sourceCanvas = this.chart.ctx.canvas;
                var copyWidth = this.scale.xScalePaddingLeft - 5;
                var copyHeight = this.scale.endPoint + 5;
                var targetCtx = document.getElementById("myChart").getContext("2d");
                targetCtx.canvas.width = copyWidth;
                targetCtx.drawImage(sourceCanvas, 0, 0, copyWidth, copyHeight, 0, 0, copyWidth, copyHeight);
            }
        });
    </script>
</asp:Content>