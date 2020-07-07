using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;

namespace ClientWebOPCUA
{
    public partial class Browse : System.Web.UI.Page
    {
        public ReferenceDescription _ref { get; set; }
        private ReferenceDescription refDesc;

        protected void Page_Load(object sender, EventArgs e)
        {
            nodeTreeView.HoverNodeStyle.CssClass = "under";
            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);
            if (!IsPostBack)
            {
                BrowsePage_Enter();
            }
        }

        protected void Disconnect_MyButtonClicked(object sender, EventArgs e)
        {
            if (Connect.mySession != null && Connect.mySession.Connected == true)
            {
                Connect connect = new Connect();
                connect.Disconnect();
            }
            else
            {
                return;
            }
        }

        private void BrowsePage_Enter()
        {
            //if (Connect.myReferenceDescriptionCollection == null)
            //{
            try
            {
                Connect.myReferenceDescriptionCollection = Connect.myClientHelperAPI.BrowseRoot();
                foreach (ReferenceDescription refDesc in Connect.myReferenceDescriptionCollection)
                {
                    _ref = refDesc;
                    CustomTreeNode Root = new CustomTreeNode("Root");
                    Root.Text = refDesc.DisplayName.ToString();
                    Root.Tag = _ref;
                    nodeTreeView.Nodes.Add(Root);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
            }
            //}
        }

        protected void nodeTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            ReferenceDescriptionCollection referenceDescriptionCollection;
            CustomTreeNode treeNode = (CustomTreeNode)nodeTreeView.SelectedNode;
            if (nodeTreeView.SelectedNode.Parent != null)
            {
                foreach (CustomTreeNode node in treeNode.Parent.ChildNodes)
                {
                    if (node.Selected == true)
                    {
                        refDesc = (ReferenceDescription)node.Tag;

                        break;
                    }
                }
            }
            else
            {
                foreach (CustomTreeNode node in nodeTreeView.Nodes)
                {
                    if (node.Selected == true)
                    {
                        refDesc = (ReferenceDescription)node.Tag;
                        break;
                    }
                }
            }
            try
            {
                referenceDescriptionCollection = Connect.myClientHelperAPI.BrowseNode(refDesc);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
                return;
            }

            foreach (ReferenceDescription tempRefDesc in referenceDescriptionCollection)
            {
                if (tempRefDesc.ReferenceTypeId != ReferenceTypeIds.HasNotifier)
                {
                    CustomTreeNode tree = new CustomTreeNode();
                    tree.Text = tempRefDesc.DisplayName.ToString();
                    tree.Tag = tempRefDesc;
                    nodeTreeView.SelectedNode.ChildNodes.Add(tree);
                }
            }
            nodeTreeView.SelectedNode.Expand();

            DataGridViewUpdate();
        }

        private void DataGridViewUpdate()
        {
            try
            {
                Node node = Connect.myClientHelperAPI.ReadNode(refDesc.NodeId.ToString());
                VariableNode variableNode = new VariableNode();

                DataTable datatable = new DataTable();
                datatable.Columns.Add("Attribute", typeof(string));
                datatable.Columns.Add("Value", typeof(string));

                string[] row1 = new string[] { "Node Id", refDesc.NodeId.ToString() };
                string[] row2 = new string[] { "Namespace Index", refDesc.NodeId.NamespaceIndex.ToString() };
                string[] row3 = new string[] { "Identifier Type", refDesc.NodeId.IdType.ToString() };
                string[] row4 = new string[] { "Identifier", refDesc.NodeId.Identifier.ToString() };
                string[] row5 = new string[] { "Browse Name", refDesc.BrowseName.ToString() };
                string[] row6 = new string[] { "Display Name", refDesc.DisplayName.ToString() };
                string[] row7 = new string[] { "Node Class", refDesc.NodeClass.ToString() };
                string[] row8 = new string[] { "Description", "null" };
                try { row8 = new string[] { "Description", node.Description.ToString() }; }
                catch { row8 = new string[] { "Description", "null" }; }
                string[] row9 = new string[] { "Type Definition", refDesc.TypeDefinition.ToString() };
                string[] row10 = new string[] { "Write Mask", node.WriteMask.ToString() };
                string[] row11 = new string[] { "User Write Mask", node.UserWriteMask.ToString() };
                if (node.NodeClass == NodeClass.Variable)
                {
                    variableNode = (VariableNode)node.DataLock;
                    List<NodeId> nodeIds = new List<NodeId>();
                    List<string> displayNames = new List<string>();
                    List<ServiceResult> errors = new List<ServiceResult>();
                    NodeId nodeId = new NodeId(variableNode.DataType);
                    nodeIds.Add(nodeId);
                    Connect.mySession.ReadDisplayName(nodeIds, out displayNames, out errors);
                    var value = Connect.myClientHelperAPI.VariableRead(refDesc.NodeId.ToString());
                    string[] row12 = new string[] { "Value", value.ToString() };
                    string[] row13 = new string[] { "Data Type", displayNames[0] };
                    string[] row14 = new string[] { "Value Rank", variableNode.ValueRank.ToString() };
                    string[] row15 = new string[] { "Array Dimensions", variableNode.ArrayDimensions.Capacity.ToString() };
                    string[] row16 = new string[] { "Access Level", variableNode.AccessLevel.ToString() };
                    string[] row17 = new string[] { "Minimum Sampling Interval", variableNode.MinimumSamplingInterval.ToString() };
                    string[] row18 = new string[] { "Historizing", variableNode.Historizing.ToString() };

                    object[] rows = new object[] { row1, row2, row3, row4, row5, row6, row7, row8, row9, row10, row11, row12, row13, row14, row15, row16, row17 };
                    object[] rows0 = new object[] { row1[0], row2[0], row3[0], row4[0], row5[0], row6[0], row7[0], row8[0], row9[0], row10[0], row11[0], row12[0], row13[0], row14[0], row15[0], row16[0], row17[0], row18[0] };
                    object[] rows1 = new object[] { row1[1], row2[1], row3[1], row4[1], row5[1], row6[1], row7[1], row8[1], row9[1], row10[1], row11[1], row12[1], row13[1], row14[1], row15[1], row16[1], row17[1], row18[1] };
                    foreach (string[] rowArray in rows)
                    {
                        DataRow dr = datatable.NewRow();
                        datatable.Rows.Add(rowArray);
                    }
                }
                else
                {
                    object[] rows = new object[] { row1, row2, row3, row4, row5, row6, row7, row8, row9, row10, row11 };
                    object[] rows0 = new object[] { row1[0], row2[0], row3[0], row4[0], row5[0], row6[0], row7[0], row8[0], row9[0], row10[0], row11[0] };
                    object[] rows1 = new object[] { row1[1], row2[1], row3[1], row4[1], row5[1], row6[1], row7[1], row8[1], row9[1], row10[1], row11[1] };

                    foreach (string[] rowArray in rows)
                    {
                        DataRow dr = datatable.NewRow();
                        datatable.Rows.Add(rowArray);
                    }
                }

                ViewState["CurrentTable"] = datatable;
                descriptionGridView.DataSource = datatable;
                descriptionGridView.DataBind();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
            }
        }
        
        [WebMethod]
        public static string Name()
        {
            string Name = "Hello Rohatash Kumar";
            return Name;
        }

       
    }
}