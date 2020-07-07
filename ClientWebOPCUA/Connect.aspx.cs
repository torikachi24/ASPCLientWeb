using Opc.Ua;
using Opc.Ua.Client;
using Siemens.UAClientHelper;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;

namespace ClientWebOPCUA
{
    public partial class Connect : System.Web.UI.Page
    {
        #region Fields

        public static MonitoredItemNotification MonNotificate;
        public static MonitoredItem MonItem;
        public static string Oldvalue;
        public static Session mySession;
        public static Subscription mySubscription;
        public static UAClientHelperAPI myClientHelperAPI;
        private EndpointDescription mySelectedEndpoint;
        public static MonitoredItem myMonitoredItem;
        private List<String> myRegisteredNodeIdStrings;
        public static ReferenceDescriptionCollection myReferenceDescriptionCollection;

        //private List<string[]> myStructList;
        public static Int16 itemCount;

        private int cnt = -1;
        public static string datetime { get; set; }
        public int a;

        #endregion Fields

        protected void Page_Load(object sender, EventArgs e)
        {
            (this.Master as Masterpage).MyButtonClicked += new EventHandler(Disconnect_MyButtonClicked);
            if (!IsPostBack)
            {
                if (mySession != null && mySession.Connected == true)
                {
                    if (Session["endpoint"] != null && Session["endpoint"].ToString() != "")
                    {
                        var enp = Session["endpoint"] as EndpointDescription;
                        var enps = Session["endpointlist"] as List<string>;
                        SerName.Text = enp.Server.ApplicationName.ToString();
                        SerUri.Text = enp.Server.ApplicationUri.ToString();
                        Serc.Text = enp.SecurityPolicyUri.ToString();
                        Status.Text = "Connected";
                        Sinced.Text = Session["sinced"].ToString();
                        ConnectBtn.Text = "Disconnect from server";
                        foreach (string item in enps)
                        {
                            endpointListView.Items.Add(item);
                        }
                        discoveryTextBox.Text = Session["Urltext"].ToString();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    a = new int();
                    ControlDisable();
                    myClientHelperAPI = new UAClientHelperAPI();
                    myRegisteredNodeIdStrings = new List<String>();
                    itemCount = 0;
                }
            }
        }

        protected void Disconnect_MyButtonClicked(object sender, EventArgs e)
        {
            if (mySession != null && mySession.Connected == true)
            {
                Disconnect();
            }
            else
            {
                return;
            }
        }

        private void ControlDisable()
        {
            Control browse = this.Page.Master.FindControl("list").FindControl("Browse");
            Control rdwr = this.Page.Master.FindControl("list").FindControl("Rdwr");
            Control subcribe = this.Page.Master.FindControl("list").FindControl("Subcribe");
            Control disconnect = this.Page.Master.FindControl("list").FindControl("Disconnect");
            Control scada = this.Page.Master.FindControl("list").FindControl("Scada");
            browse.Visible = false;
            rdwr.Visible = false;
            subcribe.Visible = false;
            disconnect.Visible = false;
            scada.Visible = false;
        }

        protected void GetEnpoints_Click(object sender, EventArgs e)
        {
            if (discoveryTextBox.Text != "")
            {
                bool foundEndpoints = false;
                endpointListView.Items.Clear();
                Session["Urltext"] = discoveryTextBox.Text;
                //The local discovery URL for the discovery server
                string discoveryUrl = discoveryTextBox.Text;
                try
                {
                    ApplicationDescriptionCollection servers = myClientHelperAPI.FindServers(discoveryUrl);
                    foreach (ApplicationDescription ad in servers)
                    {
                        foreach (string url in ad.DiscoveryUrls)
                        {
                            try
                            {
                                EndpointDescriptionCollection endpoints = myClientHelperAPI.GetEndpoints(url);
                                foundEndpoints = foundEndpoints || endpoints.Count > 0;
                                List<string> enps = new List<string>();
                                foreach (EndpointDescription ep in endpoints)
                                {
                                    string securityPolicy = ep.SecurityPolicyUri.Remove(0, 42);
                                    string key = "[" + ad.ApplicationName + "] " + " [" + ep.SecurityMode + "] " + " [" + securityPolicy + "] " + " [" + ep.EndpointUrl + "]";
                                    enps.Add(key);
                                    endpointListView.Items.Add(key);
                                }
                                Session["endpointlist"] = enps;
                            }
                            catch (ServiceResultException sre)
                            {
                                //If an url in ad.DiscoveryUrls can not be reached, myClientHelperAPI will throw an Exception
                                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Oops...'," + sre.ToString() + ")", true);
                            }
                        }
                        if (!foundEndpoints)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('warning','Oops...','Could not get any Endpoints!')", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Oops...'," + ex.ToString() + ")", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('warning','Warning...','You forget write endpoint Url!')", true);
            }
        }

        protected void Connect_Click(object sender, EventArgs e)
        {
            if (ConnectBtn.Text == "Connect")
            {
                if (discoveryTextBox.Text != "")
                {
                    bool foundEndpoints = false;
                    for (int a = 0; a < endpointListView.Items.Count; a++)
                    {
                        if (endpointListView.Items[a].Selected == true)
                        {
                            cnt = a;
                            break;
                        }
                    }
                    string discoveryUrl = discoveryTextBox.Text;
                    ApplicationDescriptionCollection servers = myClientHelperAPI.FindServers(discoveryUrl);
                    foreach (ApplicationDescription ad in servers)
                    {
                        foreach (string url in ad.DiscoveryUrls)
                        {
                            EndpointDescriptionCollection endpoints = myClientHelperAPI.GetEndpoints(url);
                            foundEndpoints = foundEndpoints || endpoints.Count > 0;
                            if (cnt != -1)
                            {
                                mySelectedEndpoint = endpoints[cnt];
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('warning','Warning...','Please select an endpoint before connecting')", true);
                                return;
                            }
                        }
                    }
                    //Check if sessions exists; If yes > delete subscriptions and disconnect

                    try
                    {
                        //Register mandatory events (cert and keep alive)
                        myClientHelperAPI.KeepAliveNotification += new KeepAliveEventHandler(Notification_KeepAlive);
                        myClientHelperAPI.CertificateValidationNotification += new CertificateValidationEventHandler(Notification_ServerCertificate);
                        //Check for a selected endpoint
                        //Call connect
                        myClientHelperAPI.Connect(mySelectedEndpoint, RadioButtonList1.Items.FindByValue("User/Password").Selected, userTextBox.Text, pwTextBox.Text).Wait();
                        //Extract the session object for further direct session interactions
                        mySession = myClientHelperAPI.Session;

                        SerName.Text = mySelectedEndpoint.Server.ApplicationName.ToString();
                        SerUri.Text = mySelectedEndpoint.Server.ApplicationUri.ToString();
                        Serc.Text = mySelectedEndpoint.SecurityLevel.ToString();
                        Status.Text = "Connected";
                        Sinced.Text = DateTime.Now.ToString();

                        //Data save Page Reload
                        Session["endpoint"] = mySelectedEndpoint;
                        Session["sinced"] = DateTime.Now.ToString();

                        //UI settings
                        ConnectBtn.Text = "Disconnect from server";
                        ControlEnable();
                        ClientScript.RegisterStartupScript(this.GetType(), "Success", "messtimer('success','Connect Successful')", true);

                        //myCertForm = null;
                    }
                    catch (Exception ex)
                    {
                        //myCertForm = null;
                        //ResetUI();

                        ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('error','Opps...'," + ex.ToString() + ")", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "mess('warning','Warning...','You forget write endpoint Url!')", true);
                }
            }
            else
            {
                Disconnect();
                //mySubscription.Delete(true);
            }
        }

        public void Disconnect()
        {
            myClientHelperAPI.Disconnect();
            mySession = myClientHelperAPI.Session;
            var page = (Page)HttpContext.Current.CurrentHandler;
            string url = page.AppRelativeVirtualPath;
            if (url == "~/Connect.aspx")
            {
                ControlDisable();
                ResetUI();
            }
            else
            {
                Server.Transfer("Connect.aspx");
            }
            ClientScript.RegisterStartupScript(this.GetType(), "Success", "messtimer('success','Disconnect Successfully')", true);
        }

        private void ControlEnable()
        {
            Control browse = this.Page.Master.FindControl("list").FindControl("Browse");
            Control rdwr = this.Page.Master.FindControl("list").FindControl("Rdwr");
            Control subcribe = this.Page.Master.FindControl("list").FindControl("Subcribe");
            Control disconnect = this.Page.Master.FindControl("list").FindControl("Disconnect");
            Control scada = this.Page.Master.FindControl("list").FindControl("Scada");

            browse.Visible = true;
            rdwr.Visible = true;
            subcribe.Visible = true;
            disconnect.Visible = true;
            scada.Visible = true;
        }

        private void ResetUI()
        {
            ConnectBtn.Text = "Connect";
            SerName.Text = null;
            SerUri.Text = null;
            Serc.Text = null;
            Status.Text = null;
            Sinced.Text = null;
            discoveryTextBox.Text = null;
            userTextBox.Text = null;
            pwTextBox.Text = null;
            endpointListView.Items.Clear();
        }

        #region OpcEventHandlers

        private void Notification_ServerCertificate(CertificateValidator cert, CertificateValidationEventArgs e)
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new CertificateValidationEventHandler(Notification_ServerCertificate), cert, e);
            //    return;
            //}

            try
            {
                //Search for the server's certificate in store; if found -> accept
                X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509CertificateCollection certCol = store.Certificates.Find(X509FindType.FindByThumbprint, e.Certificate.Thumbprint, true);
                store.Close();
                if (certCol.Capacity > 0)
                {
                    e.Accept = true;
                }

                //Show cert dialog if cert hasn't been accepted yet
                else
                {
                    //if (!e.Accept & myCertForm == null)
                    //{
                    //    myCertForm = new UAClientCertForm(e);
                    //    myCertForm.ShowDialog();
                    //}
                }
            }
            catch
            {
                ;
            }
        }

        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
        }

        private void Notification_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new KeepAliveEventHandler(Notification_KeepAlive), sender, e);
            //    return;
            //}

            try
            {
                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(sender, mySession))
                {
                    return;
                }

                // check for disconnected session.
                if (!ServiceResult.IsGood(e.Status))
                {
                    // try reconnecting using the existing session state
                    mySession.Reconnect();
                }
            }
            catch (Exception ex)
            {
                PageUtility.MessageBox(this, ex.ToString());
                //ResetUI();
            }
        }

        #endregion OpcEventHandlers
    }
}