<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="Connect.aspx.cs" Inherits="ClientWebOPCUA.Connect" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <a id="div1" class="lbrun">I'm Client Connect Page</a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="Stylesheet" runat="server" type="text/css" href="Content/styleConnect.css" />
    <div class="config">
        <div class="intbox">

            <asp:Label ID="Label1" CssClass="label" runat="server" Text="Get all available Endpoints of a server's or discovery server's URL:"></asp:Label>
            <asp:TextBox ID="discoveryTextBox" CssClass="textboxurl" runat="server" Font-Size="Medium"></asp:TextBox>
            <asp:Button ID="GetEnpoints" CssClass="buttonfind" runat="server" Text="Get Endpoints" OnClick="GetEnpoints_Click" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:ListBox ID="endpointListView" CssClass="listbox" runat="server" Font-Size="Medium" Font-Underline="False"></asp:ListBox>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Label ID="Label2" CssClass="label" runat="server" Text="User Authentication"></asp:Label>

            <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Vertical" RepeatLayout="Table" TextAlign="Right" CssClass="inline-rb">
                <asp:ListItem Selected="True">Anonymous</asp:ListItem>
                <asp:ListItem>User/Password</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RadioButtonList ID="RadioButtonList2" CssClass="Radiobtn" runat="server"></asp:RadioButtonList>
            <asp:TextBox ID="userTextBox" placeholder="User's Name" CssClass="text" runat="server"></asp:TextBox>
            <asp:TextBox ID="pwTextBox" type="Password" placeholder="Your Password" CssClass="text" runat="server"></asp:TextBox>
            <asp:Button ID="ConnectBtn" CssClass="connect" runat="server" Text="Connect" OnClick="Connect_Click" />
        </div>
    </div>

    <div class="config">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <h1 class="h1cnt">Status</h1>
                <label>
                    <span class="name">Server Name</span>
                    <span class="mark">:</span>
                    <asp:TextBox ID="SerName" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>
                </label>
                <label>
                    <span class="name">Server Uri</span>
                    <span class="mark">:</span>
                    <asp:TextBox ID="SerUri" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>
                </label>
                <label>
                    <span class="name">Security</span>
                    <span class="mark">:</span>
                    <asp:TextBox ID="Serc" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>
                </label>
                <label>
                    <span class="name">Connection Status</span>
                    <span class="mark">:</span>
                    <asp:TextBox ID="Status" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>
                </label>
                <label>
                    <span class="name">Connected Sinced</span>
                    <span class="mark">:</span>
                    <asp:TextBox ID="Sinced" CssClass="output" runat="server" ReadOnly="True"></asp:TextBox>
                </label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>