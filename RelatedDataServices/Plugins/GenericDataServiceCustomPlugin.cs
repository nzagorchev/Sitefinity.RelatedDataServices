using ServiceStack;
using SitefinityWebApp.RelatedDataServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Services.GenericData.Messages;

namespace SitefinityWebApp.RelatedDataServices.Plugins
{
    public class GenericDataServiceCustomPlugin : IPlugin
    {
        public void Register(IAppHost appHost)
        {
            appHost.RegisterService(typeof(GenericDataServiceCustom));
            appHost.Routes
                .Add<DataItemMessage>(String.Concat(GenericDataServiceRoute, "/", "data-items"), "GET")
                .Add<DataItemMessage>(String.Concat(GenericDataServiceRoute, "/", "temp-items"), "DELETE");
        }

        private const string GenericDataServiceRoute = "/sitefinity/generic-data";
    }
}
