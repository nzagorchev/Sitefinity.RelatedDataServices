using SitefinityWebApp.RelatedDataServices.Helpers;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Metadata.Model;
using Telerik.Sitefinity.Services.RelatedData;
using Telerik.Sitefinity.Services.RelatedData.Messages;
using Telerik.Sitefinity.Services.RelatedData.Responses;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp.RelatedDataServices.Services
{
    public class RelatedDataServiceCustom : RelatedDataService
    {
        new public object Get(ParentItemMessage message)
        {
            var result = base.Get(message);

            string childItemTypeName = GetChildItemTypeName(message.ParentItemType, message.FieldName);

            if (childItemTypeName == typeof(Image).FullName)
            {
                return result;
            }

            var data = (RelatedItemsResponse)result;
            var items = data.Items;

            this.ShowTaxonomiesFields = true;  

            ArrayList list = ItemResponseExtendedHelper
                .BuildRelatedItemsResponseExtended<RelatedDataItemResponse>(childItemTypeName, items, this.ShowTaxonomiesFields);

            RelatedItemsResponse resultNew = new RelatedItemsResponse();
            resultNew.Items = list;
            resultNew.TotalCount = data.TotalCount;

            return resultNew;
        }

        private static string GetChildItemTypeName(string parentItemType, string fieldName)
        {
            var type = TypeResolutionService.ResolveType(parentItemType);
            var field = TypeDescriptor.GetProperties(type).OfType<RelatedDataPropertyDescriptor>()
                .Where(f => f.Name == fieldName)
                .Single();

            string childItemTypeName = string.Empty;
            var attributesCollection = field.Attributes[typeof(MetaFieldAttributeAttribute)] as MetaFieldAttributeAttribute;
            if (attributesCollection != null)
            {

                attributesCollection.Attributes.TryGetValue("RelatedType", out childItemTypeName);
            }

            return childItemTypeName;
        }

        public bool ShowTaxonomiesFields { get; set; }
    }
}