using System;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using NLog;

namespace IRIS.Law.WebServices
{
    /// <summary>
    /// Attribute to log all exceptions from a WCF service. 
    /// </summary>
    /// <remarks>
    /// See <see cref="http://codeidol.com/csharp/wcf/Faults/Error-Handling-Extensions/"/> for more.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorLoggerBehaviourAttribute : Attribute, IServiceBehavior, IErrorHandler, IDispatchMessageInspector
    {
        private Logger _logger;

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            _logger = LogManager.GetLogger("AppLogger");
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(this);
                foreach (var endpointDispatcher in dispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }

        public bool HandleError(Exception error)
        {
            _logger.ErrorException("An error occurred in a WCF service", error);
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var operationName = Path.GetFileName(request.Headers.Action);
            return _logger.TraceMethod(operationName);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var methodLogger = correlationState as MethodLogger;
            if (methodLogger != null)
            {
                methodLogger.Exited();
            }
        }
    }
}