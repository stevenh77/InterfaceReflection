using System.ServiceModel;
using System.ServiceModel.Web;

namespace InterfaceReflection
{
    [ServiceContract]
    public interface ILoginFacade
    {
        string A(string request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "GET", UriTemplate = "/Login")]
        GetLoginResponse GetLogin(GetLoginRequest request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "PUT", UriTemplate = "/Login")]
        UpdateLoginResponse UpdateLogin();

        string B(string request);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "GET", UriTemplate = "/Login")]
        void WriteToLog();

        string C(string request);
    }

    public class GetLoginRequest { public string Payload { get; set; } }
    public class GetLoginResponse { public string Payload { get; set; } }
    public class UpdateLoginRequest { public string Payload { get; set; } }
    public class UpdateLoginResponse { public string Payload { get; set; } }
}