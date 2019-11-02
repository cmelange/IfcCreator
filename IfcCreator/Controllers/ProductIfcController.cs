using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

using IfcCreator.Interface;
using IfcCreator.Interface.DTO;

namespace IfcCreator.Controllers
{
    [Route("api/create-ifc/product")]
    [ApiController]
    public class ProductIfcController : ControllerBase
    {
        private readonly IProductIfcCreator _ifcCreator;

        public ProductIfcController(IProductIfcCreator ifcCreator)
        {
            this._ifcCreator = ifcCreator;
        }

        // POST api/create-ifc/product/{productName}
        [HttpPost("{name}")]
        public async Task<IActionResult> Post(string name, [FromBody] ProductIfcRequest request)
        {
            MemoryStream memStream = new MemoryStream();
            this._ifcCreator.CreateProductIfc(request, memStream);
            return File(memStream, "application/octet-stream", String.Format("{0}.ifc", name));
        }

        [HttpGet]
        public string Get()
        {
            return "success";
        }

    }
}
