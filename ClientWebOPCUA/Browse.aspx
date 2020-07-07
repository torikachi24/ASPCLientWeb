<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage.Master" AutoEventWireup="true" CodeBehind="Browse.aspx.cs" Inherits="ClientWebOPCUA.Browse" %>

<%@ Register Assembly="ClientWebOPCUA" Namespace="ClientWebOPCUA" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <a id="div1" class="lbrun">I'm Client Browse Page</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="Scripts/jquery-1.3.2.min.js" type="text / javascript"></script>
    <script src="Scripts/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <style>
        .under {
            text-decoration: underline;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script>
        function input() {
            Swal.fire({
                title: 'Write New Value',
                input: 'text',
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                confirmButtonText: 'Write',
                showLoaderOnConfirm: true,
            }).then((result) => {
                if (result.value) {

                    PageMethods.Name(Success, Failure);
                    console.log("asdasdasd");

                    function Success(result) {
                        alert(result);
                    }

                    function Failure(error) {
                        alert(error);
                    }
                }
            })

        }
    </script>
    <link rel="stylesheet" href="/Content/styleBrowse.css" />
    <div class="config">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" CssClass="panel">
            <ContentTemplate>
                <div id="MyTreeDiv" style="overflow: auto; height: 76vh;">
                    <cc1:CustomTreeView ID="nodeTreeView" Style="overflow: auto;" runat="server" OnSelectedNodeChanged="nodeTreeView_SelectedNodeChanged" ImageSet="Simple" ShowLines="True" LineImagesFolder="~/TreeLineImages">
                        <HoverNodeStyle Font-Underline="True" ForeColor="#FFFFFF" />
                        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="#06c6bc" HorizontalPadding="0px" NodeSpacing="0px" VerticalPadding="0px" />
                        <ParentNodeStyle Font-Bold="True" />
                    </cc1:CustomTreeView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <%--<ul id="myMenu" class="contextMenu">
            <li class="copy"><a href="#add" runat="server" OnServerClick="Unnamed_ServerClick">Add</a></li>
            <li class="edit"><a href="#edit">Edit</a></li>
            <li class="delete"><a href="#delete">Delete</a></li>
            <li class="quit separator"><a href="#cancel">Cancel</a></li>
        </ul>--%>

        <%--<script type="text/javascript">

            $(document).ready(function () {
                $("#MyTreeDiv").contextMenu({
                    menu: 'myMenu'
                });
            });
            //function getGUID(mystr) {
            //    var reGUID = /\w{8}[-]\w{4}[-]\w{4}[-]\w{4}[-]\w{12}/g //regular expression defining GUID
            //    var retArr = [];
            //    var retval = '';
            //    retArr = mystr.match(reGUID);
            //    if (retArr != null) {
            //        retval = retArr[retArr.length - 1];
            //    }
            //    return retval;
            //}
        </script>--%>
    </div>
    <div class="config">
        <h1>Node</h1>
        <div class="grid_scroll">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="descriptionGridView" runat="server" CssClass="gridview" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical">
                        <AlternatingRowStyle BackColor="White" />
                        <FooterStyle BackColor="#CCCC99" />
                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                        <RowStyle BackColor="#F7F7DE" />
                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                        <SortedAscendingHeaderStyle BackColor="#848384" />
                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                        <SortedDescendingHeaderStyle BackColor="#575357" />
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>