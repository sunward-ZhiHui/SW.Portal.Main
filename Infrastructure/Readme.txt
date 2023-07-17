Scaffold-DbContext "Server=MYJB140;Database=ACTInventoryDB;Encrypt=False;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Persistence -force


// NAV Soap have to below line to use windows authentication 

// private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
       // {
            if ((endpointConfiguration == EndpointConfiguration.CRTIMS_PostTransferEntry_Port))
           // {

 result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
 result.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
 result.Security.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Windows;