using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace StingrayLoadBalancer
{
    public class AllowSelfSignedCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }
}
