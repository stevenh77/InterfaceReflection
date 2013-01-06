using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace InterfaceReflection
{
    public class FacadeMetadata
    {
        public FacadeMetadata()
        {
            Endpoints = new List<Endpoint>();
        }
        public string Name { get; set; }
        public IList<Endpoint> Endpoints { get; set; } 

        public override string ToString()
        {
            return Name;
        }
    }

    public class Endpoint
    {
        public string Name { get; set; }
        public IList<Request> Requests { get; set; }
        public Type Response { get; set; }
        public HttpVerb HttpVerb { get; set; }
        public Format RequestFormat { get; set; }
        public Format ResponseFormat { get; set; }
        public string UriTemplate { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Request
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }

    public enum HttpVerb
    {
        NotSet = 0,
        DELETE,
        GET,
        POST,
        PUT,
        UPDATE
    }

    public enum Format
    {
        NotSet = 0,
        Json,
        Xml
    }

    class Program
    {
        static void Main(string[] args)
        {
            var facades = new MetadataHelper().GetFacades();
        }
    }

    class MetadataHelper
    {
        public IList<FacadeMetadata> GetFacades() 
        {
            var response = new List<FacadeMetadata>();
            var facadeInterfaces = Assembly.GetExecutingAssembly()
                                           .GetTypes()
                                           .Where(t => t.IsInterface && t.Name.EndsWith("Facade"));

            foreach (Type facadeInterface in facadeInterfaces)
            {
                var facadeMetadata = new FacadeMetadata() { Name = facadeInterface.Name };
                
                foreach (MethodInfo method in facadeInterface.GetMethods())
	            {
                    var webInvokeAttribute = GetWebInvokeAttribute(method);
                    if (webInvokeAttribute == null) continue;

                    var endpoint = CreateEndpoint(method, webInvokeAttribute);
                    facadeMetadata.Endpoints.Add(endpoint);                    
                }
                response.Add(facadeMetadata);
            }

            return response;
        }

        private Endpoint CreateEndpoint(MethodInfo method, WebInvokeAttribute webInvokeAttribute)
        {
            var request = CreateRequest(method);

            var endpoint = new Endpoint()
            {
                Name = method.Name,
                Requests = request,
                HttpVerb = (HttpVerb)Enum.Parse(typeof(HttpVerb), webInvokeAttribute.Method),
                Response = method.ReturnType,
                RequestFormat = (Format)Enum.Parse(typeof(Format), webInvokeAttribute.RequestFormat.ToString()),
                ResponseFormat = (Format)Enum.Parse(typeof(Format), webInvokeAttribute.ResponseFormat.ToString()),
                UriTemplate = webInvokeAttribute.UriTemplate
            };
            return endpoint;
        }

        private List<Request> CreateRequest(MethodInfo method)
        {
            var request = new List<Request>();
            foreach (var parameter in method.GetParameters())
            {
                request.Add(new Request() { Type = parameter.ParameterType, Name = parameter.Name });
            }
            return request;
        }

        public WebInvokeAttribute GetWebInvokeAttribute(MethodInfo method)
        {
            var attributes = method.GetCustomAttributes(false)
                                   .Where(a => a.GetType() == typeof(WebInvokeAttribute) || a.GetType() == typeof(OperationContractAttribute));
            
            return (attributes.Count() == 2)
                ? (WebInvokeAttribute) method.GetCustomAttributes(typeof(WebInvokeAttribute), false).First()
                : null;
        }
    }
}
