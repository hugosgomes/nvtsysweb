﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NovatecEnergyWeb.Core
{
    public interface IFiltroService
    {
        IActionResult ZonaCascade(int zona);
        IActionResult DelegacaoCascade(int delegacao);
        IActionResult AreaCascade(int lote);
    }
}