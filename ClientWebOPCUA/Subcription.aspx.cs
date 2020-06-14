using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Web.UI.DataVisualization.Charting;

namespace ClientWebOPCUA
{
    public partial class Subcription : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

                Connect.myClientHelperAPI.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
                timer1.Enabled = true;
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

        private void getcharttypes(string time, string value)
        {
            Series series = Chart1.Series["Series1"];
            series.Points.AddXY(time, value);
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Connect.MonItem != null && Connect.MonNotificate != null)
            {
                getcharttypes(Connect.MonNotificate.Value.SourceTimestamp.ToString(), Connect.MonNotificate.Value.WrappedValue.ToString());
                subscriptionTextBox.Text = "Item name: " + Connect.MonItem.DisplayName
                    + Environment.NewLine + "Value: " + Utils.Format("{0}", Connect.MonNotificate.Value.WrappedValue.ToString())
                    + Environment.NewLine + "Source timestamp: " + Connect.MonNotificate.Value.SourceTimestamp.ToString()
                    + Environment.NewLine + "Server timestamp: " + Connect.MonNotificate.Value.ServerTimestamp.ToString();
            }
        }
    }
}