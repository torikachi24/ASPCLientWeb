<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="Read_Write.aspx.cs" Inherits="ClientWebOPCUA.Read" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <a id="div1" class="lbrun">I'm Client Read/Write Page</a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="/Content/styleRead_Write.css" type="text/css" />
    <div class="config">
        <h1>Read/Write</h1>
        <label>
            <span class="name">Node ID</span>
            <span class="mark">:</span>
            <asp:TextBox ID="ID" CssClass="inread" runat="server"></asp:TextBox>
        </label>
        <label>
            <span class="name">Data Read</span>
            <span class="mark">:</span>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                                <asp:TextBox ID="read" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>

                </ContentTemplate>
            </asp:UpdatePanel>
        </label>
         <label>
            <span class="name">Data Write</span>
            <span class="mark">:</span>
            <asp:TextBox ID="write" CssClass="output" runat="server" ReadOnly="False"></asp:TextBox>
        </label>
        <asp:Button ID="Button1" class="btn" runat="server" Text="Read" OnClick="Read_Click" />
        <asp:Button ID="Button2" class="btn" runat="server" Text="Write" OnClick="Write_Click" />
    </div>
</asp:Content>