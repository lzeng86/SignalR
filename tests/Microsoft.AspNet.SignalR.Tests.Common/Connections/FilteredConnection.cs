using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.FunctionalTests
{
    public class FilteredConnection : PersistentConnection
    {
        public static string NegotiateRequestType { get; private set; }
        public static string PingRequestType { get; private set; }

        public override Task ProcessRequest(Hosting.HostContext context)
        {
            var ct = context.Request.Headers.GetValues("Content-Type");
            if (IsNegotiationRequest(context.Request))
            {
                NegotiateRequestType = context.Request.Headers.GetValues("Content-Type")[0];
            }

            else if (IsPingRequest(context.Request))
            {
                PingRequestType = context.Request.Headers.GetValues("Content-Type")[0];
            }

            return base.ProcessRequest(context);
        }

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, NegotiateRequestType);
            // return base.OnConnected(request, connectionId);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return Connection.Send(connectionId, data);
        }

        private static bool IsPingRequest(IRequest request)
        {
            return request.Url.LocalPath.EndsWith("/ping", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNegotiationRequest(IRequest request)
        {
            return request.Url.LocalPath.EndsWith("/negotiate", StringComparison.OrdinalIgnoreCase);
        }
    }
}