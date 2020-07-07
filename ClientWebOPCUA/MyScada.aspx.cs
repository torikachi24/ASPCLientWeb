using Microsoft.Reporting.WebForms;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ClientWebOPCUA
{
    public partial class MyScada : System.Web.UI.Page
    {
        private List<String> values;
        private List<String> nodeIdStrings;
        private Subscription mySubscription;
        private MonitoredItem myMonitoredItem;

        private string item1 = "ns=3;s=\"block\".\"Pump1\".\"Run\"",
                       item2 = "ns=3;s=\"block\".\"Tank1\".\"Level\"",
                       item3 = "ns=3;s=\"block\".\"Tank1\".\"Status\"",
                       item4 = "ns=3;s=\"block\".\"Tank1\".\"Run\"",
                       item5 = "ns=3;s=\"block\".\"Gate1\".\"Status\"",
                       item6 = "ns=3;s=\"block\".\"Pump2\".\"Run\"",
                       item7 = "ns=3;s=\"block\".\"Tank2\".\"Level\"",
                       item8 = "ns=3;s=\"block\".\"Tank2\".\"Status\"",
                       item9 = "ns=3;s=\"block\".\"Tank2\".\"Run\"",
                       item10 = "ns=3;s=\"block\".\"Gate2\".\"Status\"",
            item11= "ns=3;s=\"block\".\"StatusLed\"",
            item12="ns=3;s=\"block\".\"FaultLed\"";

        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;TransparentNetworkIPResolution=False");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
           
            if (!IsPostBack)
            {
                SqlConnection.ClearAllPools();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM scada ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DBCC CHECKIDENT (scada, RESEED, 0)";
                cmd.ExecuteNonQuery();
                try
                {
                    //use different item names for correct assignment at the notificatino event
                    Timer1.Enabled = true;
                    if (mySubscription == null)
                    {
                        mySubscription = Connect.myClientHelperAPI.Subscribe(1000);
                    }
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item1, item1, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item2, item2, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item3, item3, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item4, item4, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item5, item5, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item6, item6, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item7, item7, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item8, item8, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item9, item9, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item10, item10, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item11, item11, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item12, item12, 1);

                    Connect.myClientHelperAPI.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
                    
                }
                catch (Exception ex)
                {
                    PageUtility.MessageBox(this, ex.ToString());
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
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select nodeid from scada";
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
                cmd1.CommandText = "update scada set value='" + notification.Value.WrappedValue.ToString() + "',Time='" + notification.Value.ServerTimestamp.ToLocalTime() + "' where nodeid='" + monitoredItem.DisplayName.ToString() + "'";
                cmd1.ExecuteNonQuery();
            }
            else
            {
                SqlCommand cmd2 = con.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "insert into scada values('" + monitoredItem.DisplayName.ToString() + "','" + notification.Value.WrappedValue.ToString() + "','" + notification.Value.ServerTimestamp.ToLocalTime() + "')";
                cmd2.ExecuteNonQuery();
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
           
            Pump1();
            Pump2();
            Tank1_Read();
            Tank2_Read();
            Gate1();
            Gage2();
            Home();
        }

        private void Home()
        {
            List<string> data = ReadData();
            if (data[10] == "True")
            {
                Image41.ImageUrl = "/Img/light_green_on.png";
            }
            else
            {
                Image41.ImageUrl = "/Img/light_green_off.png";
            }
            if (data[11] == "True")
            {
                Image42.ImageUrl = "/Img/light_red_on.png";
            }
            else
            {
                Image42.ImageUrl = "/Img/light_red_off.png";
            }
        
    }

        private List<string> ReadData()
        {
            SqlDataAdapter da = new SqlDataAdapter("select value from scada", con);
            DataSet ds = new DataSet();
            da.Fill(ds, "authors");

            List<string> scada_data = new List<string>();
            foreach (DataRow row in ds.Tables["authors"].Rows)
            {
                scada_data.Add(row["value"].ToString());
            }
            return scada_data;
        }

        private void Gage2()
        {

            List<string> data = ReadData();
            if (data != null)
            {
                if (data[9] == "True")
                {
                    Image36.ImageUrl = "/Img/light_green_on.png";
                }
                else
                {
                    Image36.ImageUrl = "/Img/light_green_off.png";
                }
            }

              
        }

        private void Gate1()
        {
            List<string> data = ReadData();
            if (data != null)
            {
                if (data[4] == "True")
                {
                    Image35.ImageUrl = "/Img/light_green_on.png";
                }
                else
                {
                    Image35.ImageUrl = "/Img/light_green_off.png";
                }
            }
           
        }

        private void Tank1_Read()
        {
            List<string> data = ReadData();
            if (data != null)
            {
                lv_tank1.Text = "Level : " + data[1];

                if (data[2] == "0")
                {
                    sta_tank1.Text = "Status : Emty";
                }
                else if (data[2] == "1")
                {
                    sta_tank1.Text = "Status : Normal";
                }
                else
                {
                    sta_tank1.Text = "Status : Full";
                }

                if (data[3] == "True")
                {
                    Image23.ImageUrl = "/Img/Handvalve_On.png";
                }
                else
                {
                    Image23.ImageUrl = "/Img/Handvalve.png";
                }
            }
           
        }

        private void Tank2_Read()
        {
            List<string> data = ReadData();
            if (data != null)
            {
                lv_tank2.Text = "Level : " + data[6];

                if (data[7] == "0")
                {
                    sta_tank2.Text = "Status : Emty";
                }
                else if (data[7] == "1")
                {
                    sta_tank2.Text = "Status : Normal";
                }
                else
                {
                    sta_tank2.Text = "Status : Full";
                }

                if (data[8] == "True")
                {
                    Image24.ImageUrl = "/Img/Handvalve_On.png";
                }
                else
                {
                    Image24.ImageUrl = "/Img/Handvalve.png";
                }
            }
               
        }

        private void Pump1()
        {
            List<string> data = ReadData();
            if (data != null)
            {
                if (data[0] == "True")
                {
                    Image11.ImageUrl = "/Img/Pump_On.png";
                    Image38.ImageUrl = "/Img/light_green_on.png";
                }
                else
                {
                    Image11.ImageUrl = "/Img/Pump.png";
                    Image38.ImageUrl = "/Img/light_green_off.png";
                }
            }
            
        }

        private void Pump2()
        {
            List<string> data = ReadData();
            if (data != null)
            {
                if (data[5] == "True")
                {
                    Image12.ImageUrl = "/Img/Pump_On.png";
                    Image37.ImageUrl = "/Img/light_green_on.png";
                }
                else
                {
                    Image12.ImageUrl = "/Img/Pump.png";
                    Image37.ImageUrl = "/Img/light_green_off.png";
                }
            }
          
        }

        protected void Load_Report_Click(object sender, EventArgs e)
        {
            Timer1.Enabled = true;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from scada";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            Timer1.Enabled = false;
            ReportDataSource rds = new ReportDataSource("DataSet1", dt);
            ReportViewer1.LocalReport.ReportPath = @"E:\MyThesis\ClientWebOPCUA\ClientWebOPCUA\ReportFile\ReportScada.rdlc";
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

        protected void sta_p1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            nodeIdStrings.Add("ns=3;s=\"block\".\"Pump1\".\"Start\"");
            values.Add("True");
            try
            {
                Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
            }
            values = new List<string>();
            values.Add("False");
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

        protected void ope_val1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            nodeIdStrings.Add("ns=3;s=\"block\".\"Tank1\".\"Start\"");
            values.Add("True");
            try
            {
                Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
            }
            values = new List<string>();
            values.Add("False");
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

        protected void clo_val1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            nodeIdStrings.Add("ns=3;s=\"block\".\"Tank1\".\"Stop\"");
            values.Add("True");
            try
            {
                Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
            }
            values = new List<string>();
            values.Add("False");
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

        protected void ope_val2_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            nodeIdStrings.Add("ns=3;s=\"block\".\"Tank2\".\"Start\"");
            values.Add("True");
            try
            {
                Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
            }
            values = new List<string>();
            values.Add("False");
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

        protected void clo_val2_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            nodeIdStrings.Add("ns=3;s=\"block\".\"Tank2\".\"Stop\"");
            values.Add("True");
            try
            {
                Connect.myClientHelperAPI.WriteValues(values, nodeIdStrings);
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "messtimer('success','Write Successful')", true);
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
            }
            values = new List<string>();
            values.Add("False");
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

        protected void open_gate1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate1\".\"Open\"");
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

        protected void stop_gate1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate1\".\"Stop\"");
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

        protected void close_gate1_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate1\".\"Close\"");
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

        protected void open_gate2_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate2\".\"Open\"");
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

        protected void stop_gate2_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate2\".\"Stop\"");
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

        protected void close_gate2_Click(object sender, EventArgs e)
        {
            values = new List<string>();
            nodeIdStrings = new List<string>();
            values.Add("True");
            nodeIdStrings.Add("ns=3;s=\"block\".\"Gate2\".\"Close\"");
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