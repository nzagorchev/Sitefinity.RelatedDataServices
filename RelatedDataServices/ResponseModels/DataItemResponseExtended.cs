using SitefinityWebApp.RelatedDataServices.Helpers;
using System.Collections.Generic;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Services.GenericData.Responses;

namespace SitefinityWebApp.RelatedDataServices.ResponseModels
{
    public class DataItemResponseExtended : DataItemResponse, IItemResponse
    {
        public DataItemResponseExtended()
        {
            this.CustomProps = new Dictionary<string, object>();
            this.CustomTaxonomyProps = new Dictionary<string, object>();
        }

        public Dictionary<string, object> CustomProps { get; set; }
        public Dictionary<string, object> CustomTaxonomyProps { get; set; }

        internal static DataItemResponseExtended FromDataItemResponse(DataItemResponse item, IDynamicFieldsContainer mappedItem, List<string> fieldNames)
        {
            DataItemResponseExtended response = new DataItemResponseExtended()
            {
                Id = item.Id,
                Title = item.Title,
                SubTitle = item.SubTitle,
                ProviderName = item.ProviderName,
                Status = item.Status,
                LifecycleStatus = item.LifecycleStatus,
                LastModified = item.LastModified,
                Owner = item.Owner,
                DetailsViewUrl = item.DetailsViewUrl,
                PreviewUrl = item.PreviewUrl,
                IsRelated = item.IsRelated,
                IsEditable = item.IsEditable,
                AvailableLanguages = item.AvailableLanguages,
            };

            ItemResponseExtendedHelper.PopulateFields(mappedItem, fieldNames, response);

            return response;
        }

        internal static DataItemResponseExtended FromDataItemResponse(DataItemResponse item,
            IDynamicFieldsContainer mappedItem, List<string> fieldNames, List<string> taxonomyFieldNames)
        {
            var response = DataItemResponseExtended.FromDataItemResponse(item, mappedItem, fieldNames);

            ItemResponseExtendedHelper.PopulateTaxonomyFields(mappedItem, taxonomyFieldNames, response);

            return response;
        }
    }
}