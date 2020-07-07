using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientWebOPCUA
{
    public partial class Read : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);
        }

        private void Disconnect_MyButtonClicked(object sender, EventArgs e)
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

        protected void Read_Click(object sender, EventArgs e)
        {
            List<String> nodeIdStrings = new List<String>();
            List<String> values = new List<String>();
            nodeIdStrings.Add(ID.Text);
            try
            {
                values = Connect.myClientHelperAPI.ReadValues(nodeIdStrings);
                read.Text = values.ElementAt<String>(0);
                ClientScript.RegisterStartupScript(this.GetType(), "Success", "messtimer('success','Read Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.Equals(this, ex.ToString());
            }
        }

        protected void Write_Click(object sender, EventArgs e)
        {
            List<String> values = new List<string>();
            List<String> nodeIdStrings = new List<string>();
            if (write.Text != "")
            {
                values.Add(write.Text);
                nodeIdStrings.Add(ID.Text);
                try
                {
                    Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
                }
                catch (Exception ex)
                {
                    PageUtility.MessageBox(this, ex.ToString());
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('warning','Oops...','You miss value to write')", true);

            }

        }
    }
}