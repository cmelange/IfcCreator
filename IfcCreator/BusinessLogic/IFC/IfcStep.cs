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
            FixAsterisk(outputStream);
        }

        private static void FixAsterisk(Stream stream)
        {
            if (stream.CanWrite)
            {
                stream.Seek( 0, SeekOrigin.Begin );
                long endPosition = Math.Min(10000, stream.Length);
                long currentPosition = stream.Position;
                string stringToMatch = "IFCSIUNIT(";
                int matchPosition = 0;
                while (currentPosition < endPosition)
                {
                    char currentChar = (char) stream.ReadByte();
                    if (currentChar == stringToMatch[matchPosition])
                    {
                        if (matchPosition == stringToMatch.Length-1)
                        {   // found a match
                            stream.WriteByte(Convert.ToByte('*'));
                            matchPosition = 0;
                        }
                        else
                        {
                            matchPosition += 1;
                        }
                    }
                    else
                    {   // current sequence does not match, start again with searching
                        matchPosition = 0;
                    }

                    currentPosition = stream.Position;
                }
                stream.Seek( 0, SeekOrigin.Begin );
            }
        }
    }
}