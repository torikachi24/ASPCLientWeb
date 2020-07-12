using Microsoft.Reporting.WebForms;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ClientWebOPCUA
{
    public partial class MyScada : System.Web.UI.Page
    {
        private List<String> values;
        private List<String> nodeIdStrings;
        private Subscription mySubscription;
        private MonitoredItem myMonitoredItem;

        private string item1 = "ns=2;s=Pump_1.Start_Button",
            item2 = "ns=2;s=Pump_1.Stop_Button",
            item3 = "ns=2;s=Pump_1.Status",
            item6 = "ns=2;s=Tank_1.Status",
            item7 = "ns=2;s=Tank_1.Sensor_Level",
            item8 = "ns=2;s=Tank_1.Full_Level",
            item9 = "ns=2;s=Tank_1.Empty_Level",
            item10 = "ns=2;s=Door_1.Open_Button",
            item11 = "ns=2;s=Door_1.Close_Button",
            item12 = "ns=2;s=Door_1.Stop_Button",
            item13 = "ns=2;s=Door_1.Status",
            item14 = "ns=2;s=Pump_2.Start_Button",
            item15 = "ns=2;s=Pump_2.Stop_Button",
            item16 = "ns=2;s=Pump_2.Status",
            item19 = "ns=2;s=Tank_2.Status",
            item20 = "ns=2;s=Tank_2.Sensor_Level",
            item21 = "ns=2;s=Tank_2.Full_Level",
            item22 = "ns=2;s=Tank_2.Empty_Level",
            item23 = "ns=2;s=Door_2.Open_Button",
            item24 = "ns=2;s=Door_2.Close_Button",
            item25 = "ns=2;s=Door_2.Stop_Button",
            item26 = "ns=2;s=Door_2.Status",
            item27 = "ns=2;s=System_Emergency",
            item28 = "ns=2;s=System_Reset",
            item29 = "ns=2;s=System_ManualAuto",
            item30 = "ns=2;s=System_Status",
            item31 = "ns=2;s=System_Fault",
            item32 = "ns=2;s=Tank_1.Open_Valve",
            item33 = "ns=2;s=Tank_1.Close_Valve",
            item34 = "ns=2;s=Tank_1.Status_Valve",
            item35 = "ns=2;s=Tank_2.Open_Valve",
            item36 = "ns=2;s=Tank_2.Close_Valve",
            item37 = "ns=2;s=Tank_2.Status_Valve";

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

        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;TransparentNetworkIPResolution=False");

        protected void Page_Load(object sender, EventArgs e)
        {
            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            TabContainer1.Width = 1270;
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
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item3, item3, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item6, item6, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item7, item7, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item13, item13, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item16, item16, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item19, item19, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item20, item20, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item26, item26, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item30, item30, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item31, item31, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item34, item34, 1);
                    myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(mySubscription, item37, item37, 1);

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
            Gauge();
        }

        private void Gauge()
        {
            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select value from scada where nodeid ='" + item7.ToString() + "'";
            cmd2.Connection = con;
            SqlDataReader rd = cmd2.ExecuteReader();
            while (rd.Read())
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "update1(" + rd[0].ToString() + ")", true);
             }
            rd.Close();
            cmd2.CommandText = "select value from scada where nodeid ='" + item20.ToString() + "'";
            cmd2.Connection = con;
            SqlDataReader rd1 = cmd2.ExecuteReader();
            while (rd1.Read())
            {
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS2", "update2(" + rd1[0].ToString() + ")", true);
               
            }
            rd1.Close();
        }

        private void Home()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data[8] == "True")
                {
                    Image41.ImageUrl = "/Img/light_green_on.png";
                }
                else
                {
                    Image41.ImageUrl = "/Img/light_green_off.png";
                }
                if (data[9] == "True")
                {
                    Image42.ImageUrl = "/Img/light_red_on.png";
                }
                else
                {
                    Image42.ImageUrl = "/Img/light_red_off.png";
                }
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
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data != null)
                {
                    if (data[7] == "True")
                    {
                        Image36.ImageUrl = "/Img/light_green_on.png";
                    }
                    else
                    {
                        Image36.ImageUrl = "/Img/light_green_off.png";
                    }
                }
            }
        }

        private void Gate1()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data != null)
                {
                    if (data[3] == "True")
                    {
                        Image35.ImageUrl = "/Img/light_green_on.png";
                    }
                    else
                    {
                        Image35.ImageUrl = "/Img/light_green_off.png";
                    }
                }
            }
        }

        private void Tank1_Read()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data != null)
                {
                    lv_tank1.Text = "Level : " + data[1];

                    if (data[1] == "0")
                    {
                        sta_tank1.Text = "Status : Emty";
                    }
                    else if (data[1] == "1")
                    {
                        sta_tank1.Text = "Status : Normal";
                    }
                    else
                    {
                        sta_tank1.Text = "Status : Full";
                    }

                    if (data[10] == "True")
                    {
                        Image23.ImageUrl = "/Img/Handvalve_On.png";
                    }
                    else
                    {
                        Image23.ImageUrl = "/Img/Handvalve.png";
                    }
                }
            }
        }

        private void Tank2_Read()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data != null)
                {
                    lv_tank2.Text = "Level : " + data[6];

                    if (data[5] == "0")
                    {
                        sta_tank2.Text = "Status : Emty";
                    }
                    else if (data[5] == "1")
                    {
                        sta_tank2.Text = "Status : Normal";
                    }
                    else
                    {
                        sta_tank2.Text = "Status : Full";
                    }

                    if (data[11] == "True")
                    {
                        Image24.ImageUrl = "/Img/Handvalve_On.png";
                    }
                    else
                    {
                        Image24.ImageUrl = "/Img/Handvalve.png";
                    }
                }
            }
        }

        private void Pump1()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
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
        }

        private void Pump2()
        {
            List<string> data = ReadData();
            int index = 13;
            if (index >= 0 && index < data.Count)
            {
                if (data != null)
                {
                    if (data[4] == "True")
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
            nodeIdStrings.Add(item1);
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
            nodeIdStrings.Add(item32);
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
            nodeIdStrings.Add(item33);
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
            nodeIdStrings.Add(item35);
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
            nodeIdStrings.Add(item36);
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
            nodeIdStrings.Add(item10);
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
            nodeIdStrings.Add(item12);
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
            nodeIdStrings.Add(item11);
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
            nodeIdStrings.Add(item23);
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
            nodeIdStrings.Add(item25);
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
            nodeIdStrings.Add(item24);
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