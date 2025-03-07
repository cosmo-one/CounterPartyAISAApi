using CosmoOne.AADEPortal.API.Dtos.Functionality;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AADEPortalApi
{
    public class CommonBaseController : ControllerBase
    {
        public override OkObjectResult Ok([ActionResultObjectValue] object? value)
        {
            var res = new ApiResponse();

            res.IsSuccess = true;
            res.ErrorMessage = null;
            res.Data = value;


            return base.Ok(res);
        }
    }
}
