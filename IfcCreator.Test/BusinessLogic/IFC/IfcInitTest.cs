using System.IO;
using Xunit;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.IFC.IfcProductExtension;
using BuildingSmart.IFC.IfcActorResource;
using BuildingSmart.IFC.IfcUtilityResource;



namespace IfcCreator.Ifc
{
    public class IfcInitTest
    {
        [Fact]
        public void CreateProjectInitTest()
        {
            //==== DEFAULT PROJECT ====
            IfcProject project_default = IfcInit.CreateProject(null, null, null);
            IfcSite site_default = IfcInit.CreateSite(null, null, null);
            project_default.Aggregate(site_default, null);
            IfcBuilding building_default = IfcInit.CreateBuilding(null, null, null, null);
            site_default.Aggregate(building_default, null);
            IfcBuildingStorey storey_default = IfcInit.CreateBuildingStorey(null, null, null, null);
            building_default.Aggregate(storey_default, null);
            using (FileStream fs = File.Create("./default_project.ifc"))
            {
               project_default.SerializeToStep(fs, "IFC2X3", null);
            }

            //==== MANUAL CONTENT PROJECT ====
            IfcPerson person = IfcInit.CreatePerson("Melange", "Cedric");
            IfcOrganization organization = IfcInit.CreateOrganization("test organization", "a dummy organization for testing", "MyOrg");
            IfcApplication application = IfcInit.CreateApplication(organization, "1.0", "test app", "TestApp");
            IfcOwnerHistory ownerHistory = IfcInit.CreateOwnerHistory(person,
                                                                      organization,
                                                                      application);
            IfcProject project_manual = IfcInit.CreateProject("manual",
                                                              "My manual test project",
                                                              ownerHistory);
            IfcSite site_manual = IfcInit.CreateSite("test site", "a dummy site for testing", ownerHistory);
            project_manual.Aggregate(site_manual, ownerHistory);
            IfcBuilding building_manual = IfcInit.CreateBuilding("test building", "a dummy building for testing", ownerHistory, site_manual.ObjectPlacement);
            site_manual.Aggregate(building_manual, ownerHistory);
            IfcBuildingStorey storey_manual = IfcInit.CreateBuildingStorey("first storey", "first storey for testing", ownerHistory, building_manual.ObjectPlacement);
            building_manual.Aggregate(storey_manual, null);
            using (FileStream fs = File.Create("./manual_project.ifc"))
            {
               project_manual.SerializeToStep(fs, "IFC2X3", "my company");
            }
        }
    }
}
