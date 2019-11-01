using System.IO;

using IfcCreator.Interface.DTO;

namespace IfcCreator.Interface
{
    public interface IProductIfcCreator
    {
        public void CreateProductIfc(ProductIfcRequest request,
                                     Stream outputStream);
    }
}