using System;

namespace ClientWebOPCUA
{
    public partial class Masterpage : System.Web.UI.MasterPage
    {
        public event EventHandler MyButtonClicked;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void DisConnect(object sender, EventArgs e)
        {
            MyButtonClicked(sender, e);
        }
    }
}