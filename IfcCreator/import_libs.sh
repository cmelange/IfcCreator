dotnet nuget locals all --clear
git clone --single-branch --branch NET_Core https://github.com/cmelange/IfcDoc.git ifc-kit
cd ./ifc-kit/IfcKit
./build.sh
cp ./nupkgs/* ../../lib
cd ../..
dotnet add package 
rm -r -f ./ifc-kit
dotnet add package BuildingSmart.IFC
dotnet add package BuildingSmart.Serialization.Step
cd ../IfcCreator.Test
./import_libs.sh