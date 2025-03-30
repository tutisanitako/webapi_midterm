using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace OnlineCinemaManagement
{
    public class JsonBehaviorAttribute : Attribute, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint,
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        {
            var formatter = new WebHttpBehavior();
            formatter.ApplyDispatchBehavior(endpoint, endpointDispatcher);
        }

        public void Validate(ServiceEndpoint endpoint) { }
    }
}