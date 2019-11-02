using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

using IfcCreator.Interface;
using IfcCreator.Interface.DTO;
using IfcCreator.HTTP;

namespace IfcCreator.Controllers
{
    [Route("api/create-ifc/product")]
    [ApiController]
    public class ProductIfcController : ControllerBase
    {
        private readonly IProductIfcCreator _ifcCreator;
        private readonly IResponseHelper _responseHelper;

        public ProductIfcController(IProductIfcCreator ifcCreator,
                                    IResponseHelper responseHelper)
        {
            this._ifcCreator = ifcCreator;
            this._responseHelper = responseHelper;
        }

        // POST api/create-ifc/product/{productName}
        [HttpPost("{name}")]
        public async Task<IActionResult> Post(string name, [FromBody] ProductIfcRequest request)
        {
            MemoryStream memStream = new MemoryStream();
            await this._responseHelper.PostCommand((ProductIfcRequest request) => 
                                                   {
                                                       return Task.Run(() => 
                                                       {
                                                           this._ifcCreator.CreateProductIfc(request, memStream);
                                                       });
                                                   },
                                                   request,
                                                   HttpContext);
            return File(memStream, "application/octet-stream", String.Format("{0}.ifc", name));
        }

        [HttpGet]
        public string Get()
        {
            return "success";
        }

    }
}
