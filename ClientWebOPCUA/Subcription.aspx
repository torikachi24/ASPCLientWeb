<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" EnableViewState="true" AutoEventWireup="true" CodeBehind="Subcription.aspx.cs" Inherits="ClientWebOPCUA.Subcription" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <a id="div1" class="lbrun">I'm Client Subcribe Page</a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="/Content/styleSubcript.css" />
    <div class="grid">
        <div class="frame" id="table">
            <%--  <asp:ScriptManager runat="server"></asp:ScriptManager>--%>
            <asp:Timer runat="server" Interval="1000" OnTick="UpdateTimer_Tick" ID="timer1" Enabled="false"></asp:Timer>

            <asp:TextBox ID="SubcriptionID" CssClass="SubID" runat="server"></asp:TextBox>
            <asp:Button ID="SubcribeBtn" CssClass="btn" runat="server" Text="Subcribe" OnClick="Subcribe_Click" />
            <asp:Button ID="UnSubcribeBtn" CssClass="btn" runat="server" Text="UnSubcribe" OnClick="UnSubcribeBtn_Click" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="timer1" EventName="Tick" />
                </Triggers>
                <ContentTemplate>
                    <asp:TextBox ID="subscriptionTextBox" EnableViewState="true" TextMode="MultiLine" CssClass="subtxt" runat="server" ReadOnly="True"></asp:TextBox>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:ListView ID="ListView1" runat="server"></asp:ListView>
        </div>

        <div class="frame" id="config">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:Chart ID="Chart1" runat="server" Height="250px" Width="400px" BackColor="Teal" BorderlineColor="Black">
                        <Series>
                            <asp:Series Name="Series1"></asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                                <AxisX Title="Time"></AxisX>
                                <AxisY Title="Value"></AxisY>
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="frame" id="gauge"></div>
    </div>
</asp:Content>