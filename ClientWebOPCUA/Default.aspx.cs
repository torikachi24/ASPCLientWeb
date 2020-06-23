using System;
using System.Web.Services;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    [WebMethod]
    public static string DoMyTask(int id, string name)
    {
        // Do stuff with the received ID and Name.
        return "Success";
    }
}