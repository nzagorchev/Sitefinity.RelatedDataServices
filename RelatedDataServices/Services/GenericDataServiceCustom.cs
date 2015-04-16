using SitefinityWebApp.RelatedDataServices.Helpers;
using System.Collections;
using Telerik.Sitefinity.Services.GenericData;
using Telerik.Sitefinity.Services.GenericData.Messages;
using Telerik.Sitefinity.Services.GenericData.Responses;

namespace SitefinityWebApp.RelatedDataServices.Services
{
    public class GenericDataServiceCustom : GenericDataService
    {
        /// <summary>
        /// Gets list of available items to be related
        /// </summary>
        /// GET: "/sitefinity/generic-data/data-items"
        new public object Get(DataItemMessage message)
        {           
            var result = base.Get(message);
            var data = (DataItemsResponse)result;
            var items = data.Items;
            
            this.ShowTaxonomiesFields = true;
            ArrayList list = ItemResponseExtendedHelper
                .BuildDataItemsResponseExtended<DataItemResponse>(message.ItemType, items, this.ShowTaxonomiesFields);

            DataItemsResponse resultNew = new DataItemsResponse();
            resultNew.Items = list;
            resultNew.TotalCount = data.TotalCount;

            return resultNew;
        }

        public bool ShowTaxonomiesFields { get; set; }
    }
}