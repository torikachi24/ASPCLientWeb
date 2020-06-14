using Opc.Ua;

namespace ClientWebOPCUA
{
    public class EndpointDesc
    {
        public string appname { get; set; }
        public EndpointDescription ep { get; set; }

        public string Endpoint(string name, EndpointDescription e)
        {
            //this.name = appname;
            this.ep = ep;
            return e.EndpointUrl + e.SecurityMode;
        }
    }
}