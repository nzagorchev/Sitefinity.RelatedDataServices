using System.Collections.Generic;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Services.RelatedData.Responses;

namespace SitefinityWebApp.ServiceStackCustom
{
    public class RelatedDataItemResponseExtended : RelatedDataItemResponse, IItemResponse
    {
        public RelatedDataItemResponseExtended()
        {
            this.CustomProps = new Dictionary<string, object>();
            this.CustomTaxonomyProps = new Dictionary<string, object>();
        }

        public Dictionary<string, object> CustomProps { get; set; }
        public Dictionary<string, object> CustomTaxonomyProps { get; set; }

        internal static RelatedDataItemResponseExtended FromRelatedItemResponse
            (RelatedDataItemResponse item, IDynamicFieldsContainer mappedItem, List<string> fieldNames)
        {
            RelatedDataItemResponseExtended response = new RelatedDataItemResponseExtended()
            {
                Id = item.Id,
                Title = item.Title,
                ProviderName = item.ProviderName,
                Status = item.Status,
                LifecycleStatus = item.LifecycleStatus,
                DetailsViewUrl = item.DetailsViewUrl,
                IsRelated = item.IsRelated,
                IsEditable = item.IsEditable,
                ContentTypeName = item.ContentTypeName,
                Ordinal = item.Ordinal,
                SubTitle = item.SubTitle,
                AvailableLanguages = item.AvailableLanguages,
                LastModified = item.LastModified,
                Owner = item.Owner,
                PreviewUrl = item.PreviewUrl
            };

            ItemResponseExtendedHelper.PopulateFields(mappedItem, fieldNames, response);

            return response;
        }

        internal static RelatedDataItemResponseExtended FromRelatedItemResponse(RelatedDataItemResponse item,
            IDynamicFieldsContainer mappedItem, List<string> fieldNames, List<string> taxonomyFieldNames)
        {
            var response = RelatedDataItemResponseExtended.FromRelatedItemResponse(item, mappedItem, fieldNames);

            ItemResponseExtendedHelper.PopulateTaxonomyFields(mappedItem, taxonomyFieldNames, response);

            return response;
        }
    }
}