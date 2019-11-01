using System;
using System.IO;

using BuildingSmart.IFC.IfcKernel;
using BuildingSmart.Serialization;
using BuildingSmart.Serialization.Step;

namespace IfcCreator.Ifc
{

#nullable enable
    public static class IfcProjectSerializerExtension
    {
        public static void SerializeToStep(this IfcProject project,
                                           Stream outputStream,
                                           String schema,
                                           String? application)
        {
            Serializer serializer = new StepSerializer(typeof(IfcProject),
                                                       null,
                                                       schema,
                                                       null,
                                                       application ?? "ECL IfcCreator");
            serializer.WriteObject(outputStream, project);
        }
    }
}