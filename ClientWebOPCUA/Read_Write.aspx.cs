using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientWebOPCUA
{
    public partial class Read : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Read_Click(object sender, EventArgs e)
        {

            List<String> nodeIdStrings = new List<String>();
            List<String> values = new List<String>();
            nodeIdStrings.Add(TextBox1.Text);
            try
            {
                values = Connect.myClientHelperAPI.ReadValues(nodeIdStrings);
                System.Diagnostics.Debug.WriteLine(values.ToString());
                TextBox2.Text = values.ElementAt<String>(0);
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
            values.Add(TextBox3.Text);
            nodeIdStrings.Add(TextBox1.Text);
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
        }
}