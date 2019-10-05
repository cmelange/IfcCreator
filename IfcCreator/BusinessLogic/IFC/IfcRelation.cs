using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcUtilityResource;
using BuildingSmart.IFC.IfcProductExtension;

namespace IfcCreator.Ifc
{
    public static class IfcRelationClass
    {
#nullable enable
        public static void Aggregate(this IfcObjectDefinition relatingObject,
                                     IfcObjectDefinition relatedObject,
                                     IfcOwnerHistory? ownerHistory)
        {
            relatingObject.Aggregate(new IfcObjectDefinition[] {relatedObject}, ownerHistory);
        }

        public static void Aggregate(this IfcObjectDefinition relatingObject,
                                     IfcObjectDefinition[] relatedObjects,
                                     IfcOwnerHistory? ownerHistory)
        {
            IfcOwnerHistory history = ownerHistory ?? IfcInit.CreateOwnerHistory(null, null);
            IfcRelAggregates relation = new IfcRelAggregates(IfcInit.CreateGloballyUniqueId(),
                                                             history,
                                                             null,  //__Name
                                                             null,  //__Description
                                                             relatingObject,
                                                             relatedObjects);
            foreach(IfcObjectDefinition relatedObject in relatedObjects)
            {
                relatingObject.IsDecomposedBy.Add(relation);
                relatedObject.Decomposes.Add(relation);
            }
        }

        public static void Contains(this IfcSpatialStructureElement relatingObject,
                                    IfcProduct relatedObject,
                                    IfcOwnerHistory? ownerHistory)
        {
            relatingObject.Contains(new IfcProduct[] {relatedObject}, ownerHistory);
        }

        public static void Contains(this IfcSpatialStructureElement relatingObject,
                                    IfcProduct[] relatedObjects,
                                    IfcOwnerHistory? ownerHistory)
        {
            IfcOwnerHistory history = ownerHistory ?? IfcInit.CreateOwnerHistory(null, null);
            IfcRelContainedInSpatialStructure relation = 
                    new IfcRelContainedInSpatialStructure(IfcInit.CreateGloballyUniqueId(),
                                                          history,
                                                          null,  //__Name
                                                          null,  //__Description
                                                          relatedObjects,
                                                          relatingObject);
            relatingObject.ContainsElements.Add(relation);
        }
    }
}