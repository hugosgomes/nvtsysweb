﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace NovatecEnergyWeb.Filters.ActionFilters
{
    public class TipoFuncionarioFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string tipo = context.HttpContext.Session.GetString("UserTipo");
            int grupo = Convert.ToInt32(context.HttpContext.Session.GetInt32("Grupo"));
            if ((tipo == "func") &&(grupo == 1))
            {
                base.OnActionExecuting(context);
            }else
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index"
                }));
            }
            
        }
    }
}
