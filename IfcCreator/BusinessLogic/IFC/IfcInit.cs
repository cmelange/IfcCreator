using System;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcUtilityResource;
using BuildingSmart.IFC.IfcMeasureResource;
using BuildingSmart.Utilities.Conversion;
using BuildingSmart.IFC.IfcActorResource;
using BuildingSmart.IFC.IfcDateTimeResource;
using BuildingSmart.IFC.IfcRepresentationResource;
using BuildingSmart.IFC.IfcGeometryResource;
using BuildingSmart.IFC.IfcGeometricConstraintResource;

namespace IfcCreator.Ifc
{
    public static class IfcInit
    {
#nullable enable

        public static IfcAxis2Placement3D CreateIfcAxis2Placement3D()
        {
            return new IfcAxis2Placement3D(new IfcCartesianPoint(0,0,0), null, null);
        }
        public static IfcOwnerHistory CreateOwnerHistory(IfcOrganization? organization,
                                                         IfcApplication? application)
        {
            IfcPerson user = new IfcPerson(null,                //__Identification
                                           null,                //__FamilyName
                                           null,                //__GivenName
                                           new IfcLabel[0],     //__MidleNames
                                           new IfcLabel[0],     //__PrefixTitles
                                           new IfcLabel[0],     //__SuffixTitles
                                           new IfcActorRole[0], //__Roles
                                           new IfcAddress[0]);  //__Addresses
            IfcOrganization owningOrganization = organization ?? CreateOrganization("", "");
            IfcPersonAndOrganization owningPersonAndOrganization = 
                new IfcPersonAndOrganization(user, owningOrganization, new IfcActorRole[0]);
            IfcApplication owningApplication = application ??
                CreateApplication(CreateOrganization("ECL", ""),
                                  "",
                                  "IfcCreator library by ECL",
                                  "IfcCreator");
            double timestamp = 
                (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc)).TotalSeconds;
            return new IfcOwnerHistory(owningPersonAndOrganization,
                                       owningApplication,
                                       null,  //__State
                                       IfcChangeActionEnum.NOCHANGE,
                                       new IfcTimeStamp(),
                                       owningPersonAndOrganization,
                                       owningApplication,
                                       new IfcTimeStamp());
        }

        public static IfcOrganization CreateOrganization(String? name,
                                                         String? description)
        {
            return new IfcOrganization(null,                    //__Identification  
                                       new IfcLabel(name),
                                       new IfcText(description), //__Description
                                       new IfcActorRole[0],      //__Roles
                                       new IfcAddress[0]);       //__Addresses
        }

        public static IfcApplication CreateApplication(IfcOrganization organization, String? version, String? name, String? identifier)
        {
            return new IfcApplication(organization,
                                      new IfcLabel(version),
                                      new IfcLabel(name),
                                      new IfcIdentifier(identifier));
        }

        public static IfcGloballyUniqueId CreateGloballyUniqueId()
        {
            return GlobalId.Format(Guid.NewGuid());
        }

        public static IfcProject CreateProject(String? name,
                                               String? description,
                                               IfcOwnerHistory? ownerHistory)
        {
            IfcGloballyUniqueId gid = CreateGloballyUniqueId();
            IfcGeometricRepresentationContext representationContext =
                new IfcGeometricRepresentationContext(null,  //__ContextIdentifier
                                                      null,  //__ContectType  
                                                      new IfcDimensionCount(3),
                                                      new IfcReal(1e-5),
                                                      CreateIfcAxis2Placement3D(),
                                                      null); //__TrueNorth;
            IfcSIUnit angleSIUnit = new IfcSIUnit(null,
                                                  IfcUnitEnum.PLANEANGLEUNIT,
                                                  null,
                                                  IfcSIUnitName.RADIAN);
            IfcConversionBasedUnit angleConversionUnit = 
                new IfcConversionBasedUnit(new IfcDimensionalExponents(0,0,0,0,0,0,0),
                                           IfcUnitEnum.PLANEANGLEUNIT,
                                           new IfcLabel("degree"),
                                           new IfcMeasureWithUnit(new IfcRatioMeasure(0.0174532925199433),
                                                                  angleSIUnit));
            IfcUnitAssignment unitAssignment =
                new IfcUnitAssignment(new IfcUnit[] {new IfcSIUnit(null, 
                                                                   IfcUnitEnum.LENGTHUNIT,
                                                                   null,
                                                                   IfcSIUnitName.METRE),
                                                     new IfcSIUnit(null,
                                                                   IfcUnitEnum.AREAUNIT,
                                                                   null,
                                                                   IfcSIUnitName.SQUARE_METRE),
                                                     new IfcSIUnit(null,
                                                                   IfcUnitEnum.VOLUMEUNIT,
                                                                   null,
                                                                   IfcSIUnitName.CUBIC_METRE),
                                                     angleSIUnit,
                                                     angleConversionUnit} );
            return new IfcProject(gid,
                                  ownerHistory ?? CreateOwnerHistory(null, null),
                                  new IfcLabel(name),
                                  new IfcText(description),
                                  null,    //__ObjectType
                                  null,    //__LongName
                                  null,    //__Phase
                                  new IfcRepresentationContext[] {representationContext},
                                  unitAssignment);
        }

        public static IfcSite CreateSite(String? name,
                                         String? description,
                                         IfcOwnerHistory? ownerHistory)
        {
            return new IfcSite(CreateGloballyUniqueId(),
                               ownerHistory ?? CreateOwnerHistory(null, null),
                               new IfcLabel(name),
                               new IfcText(description),
                               null,    //__ObjectType
                               new IfcLocalPlacement(null,
                                                     CreateIfcAxis2Placement3D()),
                               null,    //__Representation
                               null,    //__LongName
                               IfcElementCompositionEnum.ELEMENT,
                               null,    //__RefLatitude
                               null,    //__RefLongitude
                               null,    //__RefElevation
                               null,    //__LandTitleNumber
                               null     //__SiteAddress
            );
        }

        public static IfcBuilding CreateBuilding(String? name,
                                                 String? description,
                                                 IfcOwnerHistory? ownerHistory,
                                                 IfcObjectPlacement? relObjectPlacement)
        {
            return new IfcBuilding(CreateGloballyUniqueId(),
                                   ownerHistory ?? CreateOwnerHistory(null, null),
                                   new IfcLabel(name),
                                   new IfcText(description),
                                   null,    //__ObjectType,
                                   new IfcLocalPlacement(relObjectPlacement,
                                                         CreateIfcAxis2Placement3D()),
                                   null,    //__Representation
                                   null,    //__LongName
                                   IfcElementCompositionEnum.ELEMENT,
                                   null,    //__ElevationOfRefHeight
                                   null,    //__ElevationOfTerrain
                                   null     //__BuildingAddress
            );
        }

        public static IfcBuildingStorey CreateBuildingStorey(String? name,
                                                             String? description,
                                                             IfcOwnerHistory? ownerHistory,
                                                             IfcObjectPlacement? relObjectPlacement)
        {
            return new IfcBuildingStorey(CreateGloballyUniqueId(),
                                         ownerHistory ?? CreateOwnerHistory(null, null),
                                         new IfcLabel(name),
                                         new IfcText(description),
                                         null,    //__ObjectType
                                         new IfcLocalPlacement(relObjectPlacement,
                                                               CreateIfcAxis2Placement3D()),
                                         null,    //__Representation
                                         null,    //__LongName
                                         IfcElementCompositionEnum.ELEMENT,
                                         null     //__Elevation
            );

        }

        public static IfcProxy CreateProxy(String? name,
                                           String? description,
                                           IfcOwnerHistory? ownerHistory,
                                           IfcObjectPlacement? relObjectPlacement,
                                           IfcAxis2Placement? relativePlacement)
        {
            IfcLocalPlacement localPlacement = 
                new IfcLocalPlacement(relObjectPlacement,
                                      relativePlacement ?? CreateIfcAxis2Placement3D());
            return new IfcProxy(CreateGloballyUniqueId(),
                                ownerHistory ?? CreateOwnerHistory(null, null),
                                new IfcLabel(name),
                                new IfcText(description),
                                null,    //__ObjectType
                                localPlacement,
                                null,    //__Representation
                                IfcObjectTypeEnum.PRODUCT,
                                null);   //__Tag
        }
    }
}