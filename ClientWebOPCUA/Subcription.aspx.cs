using Microsoft.Reporting.WebForms;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ClientWebOPCUA
{
    public partial class Subcription : System.Web.UI.Page
    {
        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;TransparentNetworkIPResolution=False");

        private Session m_session;
        protected string MyProperty { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabContainer1.Width = 1100;
            m_session = Connect.mySession;

            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlConnection.ClearAllPools();

            if (!IsPostBack)
            {
                if (Connect.mySubscription == null && Connect.myMonitoredItem == null)
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM subcription ";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DBCC CHECKIDENT (subcription, RESEED, 0)";
                    cmd.ExecuteNonQuery();
                }
            }
            disp_data();
        }

        private void Disconnect_MyButtonClicked(object sender, EventArgs e)
        {
            if (m_session != null && m_session.Connected == true)
            {
                Connect connect = new Connect();
                connect.Disconnect();
            }
            else
            {
                return;
            }
        }

        protected void Subcribe_Click(object sender, EventArgs e)
        {
            bool flag1 = false;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select NodeID from subcription";
            cmd.Connection = con;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                if (rd[0].ToString() == SubcriptionID.Text)
                {
                    flag1 = true;
                }
            }
            rd.Close();
            if (flag1 == true)
            {
                PageUtility.MessageBox(this, "You have Monitor this Node");
            }
            else
            {
                try
                {
                    //Connect.itemCount++;
                    //string monitoredItemName = "myItem" + Connect.itemCount.ToString();
                    if (Connect.mySubscription == null)
                    {
                        Connect.mySubscription = Connect.myClientHelperAPI.Subscribe(1000);
                    }
                    Connect.myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(Connect.mySubscription, SubcriptionID.Text, SubcriptionID.Text, 1);
                    Connect.myClientHelperAPI.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
                    Timer1.Enabled = true;
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
                }
            }
        }

        private bool flag = false;

        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            Connect.MonNotificate = notification;
            Connect.MonItem = monitoredItem;
            ///////////////////////////////
            Connect.Oldvalue = notification.Value.WrappedValue.ToString();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select NodeID from subcription";
            cmd.Connection = con;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                if (rd[0].ToString() == monitoredItem.DisplayName.ToString())
                {
                    flag = true;
                }
            }
            rd.Close();
            if (flag == true)
            {
                SqlCommand cmd1 = con.CreateCommand();
                cmd1.CommandType = CommandType.Text;
                cmd1.CommandText = "update subcription set Value='" + notification.Value.WrappedValue.ToString() + "',Time='" + DateTime.Now.ToString()  + "' where nodeid='" + monitoredItem.DisplayName.ToString() + "'";
                cmd1.ExecuteNonQuery();
                // ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + notification.Value.WrappedValue.ToString() + ")", true);
            }
            else
            {
                SqlCommand cmd2 = con.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "insert into subcription values('" + monitoredItem.DisplayName.ToString() + "','" + notification.Value.WrappedValue.ToString() + "','" + DateTime.Now.ToString() + "')";
                cmd2.ExecuteNonQuery();
                //ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + notification.Value.WrappedValue.ToString() + ")", true);
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Connect.mySubscription == null)
            {
                return;
            }
            else
            {
                disp_data();
            }

            if (Connect.MonItem != null && Connect.MonNotificate != null)
            {
                if (Connect.Oldvalue == null)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + Connect.MonNotificate.Value.WrappedValue.ToString() + ")", true);
                    Connect.Oldvalue = Connect.MonNotificate.Value.WrappedValue.ToString();
                }
                else
                {
                    if (Connect.Oldvalue != Connect.MonNotificate.Value.WrappedValue.ToString())
                    {
                        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + Connect.MonNotificate.Value.WrappedValue.ToString() + ")", true);
                        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "fitChart()", true);
                        Connect.Oldvalue = Connect.MonNotificate.Value.WrappedValue.ToString();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void disp_data()
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from subcription";
            cmd.ExecuteNonQuery();
            //////////
            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select max(value) from subcription";
            cmd2.Connection = con;
            SqlDataReader rd = cmd2.ExecuteReader();
            while (rd.Read())
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "update(" + rd[0].ToString() + ")", true);
            }
            rd.Close();
            //////////////
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void UnSubcribeBtn_Click(object sender, EventArgs e)
        {
            Connect.myMonitoredItem = Connect.myClientHelperAPI.RemoveMonitoredItem(Connect.mySubscription, Connect.myMonitoredItem);
            Connect.myClientHelperAPI.RemoveSubscription(Connect.mySubscription);
            Connect.mySubscription = null;
            //SubcriptionID.Text = "";
            //SqlCommand cmd = con.CreateCommand();
            //cmd.CommandType = CommandType.Text;
            //cmd.CommandText = "DELETE FROM subcription ";
            //cmd.ExecuteNonQuery();
            //cmd.CommandText = "DBCC CHECKIDENT (subcription, RESEED, 0)";
            //cmd.ExecuteNonQuery();
        }

        protected void Load_Report_Click(object sender, EventArgs e)
        {
            Timer1.Enabled = true;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from subcription";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            Timer1.Enabled = false;
            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            ReportViewer1.LocalReport.ReportPath = @"E:\MyThesis\ClientWebOPCUA\ClientWebOPCUA\ReportFile\ReportMonitor.rdlc";
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();
        }

        protected void TabContainer1_ActiveTabChanged1(object sender, EventArgs e)
        {
            if (TabContainer1.ActiveTabIndex == 0 || TabContainer1.ActiveTabIndex == 1)
            {
                Timer1.Enabled = true;
            }
            else
            {
                Timer1.Enabled = false;
            }
        }
    }
}