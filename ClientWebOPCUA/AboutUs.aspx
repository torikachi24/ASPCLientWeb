<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="ClientWebOPCUA.AboutUs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
        <link rel="stylesheet" href="/Content/styleAbout.css" type="text/css" />

    <div id="frame">
        <h2>About Us</h2>
        <asp:Label ID="Label1" CssClass="LV" runat="server" Text="Luận văn tốt nghiệp"></asp:Label>
        <br />
         <asp:Label ID="Label2" runat="server" Text="NGHIÊN CỨU VÀ PHÁT TRIỂN GIẢI PHÁP TÍCH HỢP HỆ THỐNG ĐIỀU KHIỂN VỚI ĐIỆN TOÁN ĐÁM MÂY SỬ DỤNG OPC UA"></asp:Label>
        <br />
        <asp:Label ID="Label3" runat="server" Text="RESEARCH AND DEVELOPMENT OF SOLUTION ABOUT INTEGRATION CONTROL SYSTEM TO CLOUD USING OPC UA"></asp:Label>

     <br />
        </div>
</asp:Content>