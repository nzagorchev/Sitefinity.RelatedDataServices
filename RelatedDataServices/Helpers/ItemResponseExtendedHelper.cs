using SitefinityWebApp.RelatedDataServices.ResponseModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.OpenAccess;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Services.GenericData.Responses;
using Telerik.Sitefinity.Services.RelatedData.Responses;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp.RelatedDataServices.Helpers
{
    public class ItemResponseExtendedHelper
    {
        internal static readonly string lstringPropertyDescriptorName 
            = "Telerik.Sitefinity.Descriptors.DynamicLstringPropertyDescriptor";

        public static void PopulateFields(IDynamicFieldsContainer mappedItem, List<string> fieldNames, IItemResponse response)
        {
            foreach (string fieldName in fieldNames)
            {
                string valueAsString = string.Empty;
                var value = mappedItem.GetValue(fieldName);
                if (value != null)
                {
                    valueAsString = value.ToString();
                }

                response.CustomProps.Add(fieldName, valueAsString);
            }
        }

        public static void PopulateTaxonomyFields(IDynamicFieldsContainer mappedItem, List<string> taxonomyFieldNames, IItemResponse response)
        {
            foreach (string taxonomyFieldName in taxonomyFieldNames)
            {
                var manager = TaxonomyManager.GetManager();
                var taxaIds = mappedItem.GetValue<TrackedList<Guid>>(taxonomyFieldName);
                List<string> taxaTitles = new List<string>();
                if (taxaIds != null)
                {
                    taxaTitles = manager.GetTaxa<Taxon>().Where(t => taxaIds.Contains(t.Id))
                        .Select(t => t.Title).OfType<string>()
                        .ToList();
                }

                response.CustomTaxonomyProps.Add(taxonomyFieldName, taxaTitles);
            }
        }
    
        public static ArrayList BuildDataItemsResponseExtended<T>
            (string itemTypeName, IEnumerable items, bool showTaxonomiesFields = false) where T : DataItemResponse
        {
            Type itemType;
            List<string> fieldNames = ItemResponseExtendedHelper.GetFieldsForType(itemTypeName, out itemType);
            List<string> taxonomyfieldNames = new List<string>();
            if (showTaxonomiesFields)
            {
                taxonomyfieldNames = ItemResponseExtendedHelper.GetTaxonomyFieldsForType(itemType);
            }

            List<Guid> ids = items.OfType<T>().Select(i => i.Id).ToList();
            var realItems = ItemResponseExtendedHelper.GetMappedItems(itemType, ids);

            ArrayList list = new ArrayList();
            foreach (var item in items)
            {
                var dt = item as T;
                var mappedItem = realItems.Single(i => i.GetValue<Guid>("Id") == dt.Id);
                DataItemResponseExtended dtExtended = null;
                if (showTaxonomiesFields)
                {
                    dtExtended = DataItemResponseExtended.FromDataItemResponse(dt, mappedItem, fieldNames, taxonomyfieldNames);
                }
                else
                {
                    dtExtended = DataItemResponseExtended.FromDataItemResponse(dt, mappedItem, fieldNames);
                }

                list.Add(dtExtended);
            }
            return list;
        }

        public static ArrayList BuildRelatedItemsResponseExtended<T>
            (string itemTypeName, IEnumerable items, bool showTaxonomiesFields = false) where T : RelatedDataItemResponse
        {
            Type itemType;
            List<string> fieldNames = ItemResponseExtendedHelper.GetFieldsForType(itemTypeName, out itemType);
            List<string> taxonomyfieldNames = new List<string>();
            if (showTaxonomiesFields)
            {
                taxonomyfieldNames = ItemResponseExtendedHelper.GetTaxonomyFieldsForType(itemType);
            }

            List<Guid> ids = items.OfType<T>().Select(i => i.Id).ToList();
            var realItems = ItemResponseExtendedHelper.GetMappedItems(itemType, ids);

            ArrayList list = new ArrayList();
            foreach (var item in items)
            {
                var dt = item as T;
                var mappedItem = realItems.Single(i => i.GetValue<Guid>("Id") == dt.Id);
                RelatedDataItemResponseExtended dtExtended = null;
                if (showTaxonomiesFields)
                {
                    dtExtended = RelatedDataItemResponseExtended.FromRelatedItemResponse(dt, mappedItem, fieldNames, taxonomyfieldNames);
                }
                else
                {
                    dtExtended = RelatedDataItemResponseExtended.FromRelatedItemResponse(dt, mappedItem, fieldNames);
                }

                list.Add(dtExtended);
            }
            return list;
        }     

        public static List<IDynamicFieldsContainer> GetMappedItems(Type itemType, List<Guid> ids)
        {
            IManager manager = ManagerBase.GetMappedManager(itemType.FullName);
            string filterExpression = string.Empty;
            if (ids.Count > 0)
            {
                filterExpression += "Id = ";
                for (int i = 0; i < ids.Count; i++)
                {
                    filterExpression += ids[i].ToString();
                    if (i != ids.Count - 1)
                    {
                        filterExpression += " OR Id =";
                    }
                }
            }
            var mappedItems = manager.GetItems(itemType, filterExpression, null, 0, 0).Cast<IDynamicFieldsContainer>().ToList();

            if (itemType.FullName.Contains("sf_ec"))
            {
                mappedItems = mappedItems.Where(t => t.GetValue<string>("ClrType") == itemType.FullName).ToList();
            }

            return mappedItems;
        }

        public static List<string> GetTaxonomyFieldsForType(Type itemType)
        {
            var fields = TypeDescriptor.GetProperties(itemType).OfType<TaxonomyPropertyDescriptor>()
                .Select(f => f.Name)
                .ToList();

            return fields;
        }

        public static List<string> GetFieldsForType(string itemTypeName, out Type itemType)
        {
            itemType = TypeResolutionService.ResolveType(itemTypeName);

            var fields = TypeDescriptor.GetProperties(itemType).OfType<MetafieldPropertyDescriptor>()
                .Select(f => f.Name)
                .ToList();

            var lStringFields = TypeDescriptor.GetProperties(itemType).OfType<DataPropertyDescriptor>()
                .Where(f => f.PropertyType.FullName == typeof(Lstring).FullName
                    && f.GetType().FullName == ItemResponseExtendedHelper.lstringPropertyDescriptorName)
                    .Select(f => f.Name)
                    .ToList();

            if (itemType.BaseType == typeof(DynamicContent))
            {
                var it = itemType;
                var moduleType = ModuleBuilderManager.GetManager()
                    .Provider.GetDynamicModuleTypes()
                    .Where(t => t.TypeNamespace.Equals(it.Namespace))
                    .First();

                string fieldName = moduleType.MainShortTextFieldName;

                lStringFields.Remove(fieldName);
            }

            List<string> properties = new List<string>();

            properties.AddRange(fields);
            properties.AddRange(lStringFields);

            return properties;
        }
    }
}