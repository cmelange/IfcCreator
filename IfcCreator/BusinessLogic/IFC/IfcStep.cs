using System;
using System.IO;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.Serialization;
using BuildingSmart.Serialization.Step;

namespace IfcCreator.Ifc
{
    public enum IFC_SCHEMA {
        IFC2X3,
        IFC4,
        IFC4X1
    }
#nullable enable
    public static class IfcProjectSerializerExtension
    {
        public static void SerializeToStep(this IfcProject project,
                                           Stream outputStream,
                                           IFC_SCHEMA schema,
                                           String? application)
        {
            Serializer serializer = new StepSerializer(typeof(IfcProject),
                                                       null,
                                                       schema.ToString(),
                                                       null,
                                                       application ?? "ECL IfcCreator");
            serializer.WriteObject(outputStream, project);
        }
    }
}