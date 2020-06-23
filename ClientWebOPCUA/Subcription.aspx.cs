using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Web.UI;

namespace ClientWebOPCUA
{
    public partial class Subcription : System.Web.UI.Page
    {
        // public event EventHandler ThresholdReached;
        protected void Page_Load(object sender, EventArgs e)
        {
            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);

            if (!IsPostBack)
            {
                //if (ThresholdReached != null)
                //{
                //    ThresholdReached(this, e);
                //}
                //if (Connect.MonItem != null && Connect.MonNotificate != null)
                //{
                //    this.ThresholdReached += new EventHandler(UpdateTimer_Tick);
                //}
               
            }
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

        protected void Subcribe_Click(object sender, EventArgs e)
        {
            if (Connect.myMonitoredItem != null && Connect.mySubscription != null)
            {
                try
                {
                    Connect.myMonitoredItem = Connect.myClientHelperAPI.RemoveMonitoredItem(Connect.mySubscription, Connect.myMonitoredItem);
                }
                catch
                {
                    //ignore
                    ;
                }
            }

            try
            {
                Connect.itemCount++;
                string monitoredItemName = "myItem" + Connect.itemCount.ToString();
                if (Connect.mySubscription == null)
                {
                    Connect.mySubscription = Connect.myClientHelperAPI.Subscribe(1000);
                }
                Connect.myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(Connect.mySubscription, SubcriptionID.Text, monitoredItemName, 1);
                Timer1.Enabled = true;
                Connect.myClientHelperAPI.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
            }
        }

        public  void Subcription_Browse(string ID)
        {
            System.Diagnostics.Debug.WriteLine("I am you");
            if (Connect.myMonitoredItem != null && Connect.mySubscription != null)
            {
                try
                {
                    Connect.myMonitoredItem = Connect.myClientHelperAPI.RemoveMonitoredItem(Connect.mySubscription, Connect.myMonitoredItem);
                }
                catch
                {
                    //ignore
                    ;
                }
            }

            try
            {
                Connect.itemCount++;
                string monitoredItemName = "myItem" + Connect.itemCount.ToString();
                if (Connect.mySubscription == null)
                {
                    Connect.mySubscription = Connect.myClientHelperAPI.Subscribe(1000);
                }
                Connect.myMonitoredItem = Connect.myClientHelperAPI.AddMonitoredItem(Connect.mySubscription, ID, monitoredItemName, 1);
               
                Connect.myClientHelperAPI.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
                Timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
            }
        }

        protected void UnSubcribeBtn_Click(object sender, EventArgs e)
        {
            Connect.myClientHelperAPI.RemoveSubscription(Connect.mySubscription);
            Connect.mySubscription = null;
            Connect.itemCount = 0;
            subscriptionTextBox.Text = "";
            Timer1.Enabled = false;
            Connect.Oldvalue = null;
            Connect.MonNotificate = null;
            Connect.MonItem = null;
        }

        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            Connect.MonNotificate = notification;
            Connect.MonItem = monitoredItem;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("I am you");
            if (Connect.MonItem != null && Connect.MonNotificate != null)
            {
                if (Connect.Oldvalue == null)
                {
                    subscriptionTextBox.Text = "Item name: " + Connect.MonItem.DisplayName
                   + Environment.NewLine + "Value: " + Utils.Format("{0}", Connect.MonNotificate.Value.WrappedValue.ToString())
                   + Environment.NewLine + "Source timestamp: " + Connect.MonNotificate.Value.SourceTimestamp.ToString()
                   + Environment.NewLine + "Server timestamp: " + Connect.MonNotificate.Value.ServerTimestamp.ToString();
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + Connect.MonNotificate.Value.WrappedValue.ToString() + ")", true);
                    Connect.Oldvalue = Connect.MonNotificate.Value.WrappedValue.ToString();
                }
                else
                {
                    if (Connect.Oldvalue != Connect.MonNotificate.Value.WrappedValue.ToString())
                    {
                        subscriptionTextBox.Text = "Item name: " + Connect.MonItem.DisplayName
                            + Environment.NewLine + "Value: " + Utils.Format("{0}", Connect.MonNotificate.Value.WrappedValue.ToString())
                            + Environment.NewLine + "Source timestamp: " + Connect.MonNotificate.Value.SourceTimestamp.ToString()
                            + Environment.NewLine + "Server timestamp: " + Connect.MonNotificate.Value.ServerTimestamp.ToString();
                        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "adddata(" + Connect.MonNotificate.Value.WrappedValue.ToString() + ")", true);
                        Connect.Oldvalue = Connect.MonNotificate.Value.WrappedValue.ToString();
                    }
                    else
                    {
                        return;
                    }
                }
            }

        }
    }
}