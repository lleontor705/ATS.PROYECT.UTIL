using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ATS.PROYECT.UTIL.CallApiClient
{
    public static class CallApiClientExternal
    {
        public static async Task<RestResponse> CallApiRestExtenalService(string url, string jsonrequest, RestSharp.Method method, List<Parameter> lstheaderparameters = null, List<Parameter> lstdefaultparameters = null, List<Parameter> lstqueryparameters = null, List<Parameter> lsturlsegmentsparameters = null, CookieContainer cookies = null, int switchhttpsnotsafe = 0)
        {
            const string contentType = "application/json";

            var client = new RestClient(url);
            var request = new RestRequest { Method = method };
            if (lstdefaultparameters != null && lstdefaultparameters.Exists(p => p.Name.ToUpper().Equals("ACCEPT")))
                client.DefaultParameters.RemoveParameter(client.DefaultParameters.GetEnumerator().Current.Name, client.DefaultParameters.GetEnumerator().Current.Type);

            if (cookies != null)
            {
                Uri uri = new Uri(url);
                foreach (Cookie item in cookies.GetCookies(uri))
                {
                    request.AddCookie(item.Name, item.Value, item.Path, item.Domain);
                }
            }

            if (lstdefaultparameters != null)
            {
                lstdefaultparameters.ForEach(x =>
                {
                    client.DefaultParameters.AddParameter(x);
                });
            }



            foreach (var item in request.Parameters)
            {
                request.Parameters.RemoveParameter(item);
            }

            if (lstheaderparameters != null)
            {
                lstheaderparameters.ForEach(x =>
                {
                    request.AddHeader(x.Name, x.Value.ToString());
                });
            }

            request.AddHeader("Accept", contentType);

            if (!string.IsNullOrEmpty(jsonrequest))
                request.AddParameter(contentType, jsonrequest, ParameterType.RequestBody);


            if (lstqueryparameters != null)
            {
                lstqueryparameters.ForEach(x =>
                {
                    request.AddParameter(x.Name, x.Value.ToString(), ParameterType.QueryString);
                });
            }

            if (lsturlsegmentsparameters != null)
            {
                lsturlsegmentsparameters.ForEach(x =>
                {
                    request.AddParameter(x.Name, x.Value.ToString(), ParameterType.UrlSegment);
                });
            }

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                if (switchhttpsnotsafe == 1)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                var handle = await client.ExecuteAsync(request);

                RestResponse response = (RestResponse)(handle);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static bool ValidateServerCertificate(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
