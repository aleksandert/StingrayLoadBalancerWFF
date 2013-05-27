using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace StingrayLoadBalancer
{
    public class AllowSelfSignedCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
        {
            return true;
        }
    }
}
