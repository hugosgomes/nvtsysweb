﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using NovatecEnergyWeb.Models;

using Microsoft.EntityFrameworkCore;
using NovatecEnergyWeb.Models.MetasViewModels;
using System.Dynamic;
using NovatecEnergyWeb.Filters.ActionFilters;
using Microsoft.AspNetCore.Http;
using NovatecEnergyWeb.Core;

namespace NovatecEnergyWeb.Controllers
{
    public class MetasController : Controller
    {

        private BDNVTContext _context;

        public MetasController(BDNVTContext context)
        {
            _context = context;
        }

       // [FuncionarioFilter]
        [AutenticacaoFilter]
        public IActionResult Index()
        {
            return BuscaMetasFiltradas(DateTime.Now.Year,true);
        }
     //   [FuncionarioFilter]
        [AutenticacaoFilter]
        public IActionResult BuscaMetasFiltradas(int anoSelecionado, bool index)
        {
            var metasCargas = _context._10_MetasCargas.FromSql("EXECUTE [dbo].[10_MetasCargas]").Where(c => c.Ano == anoSelecionado).ToList();
            var cargasMetasD2 = _context._10_CargasMetas.FromSql("EXECUTE [dbo].[10_CargasMetas]").Where(c => c.AnoCarga == anoSelecionado).ToList();
            
            dynamic mymodel = new ExpandoObject();

            mymodel.Resultados = GetResultados(metasCargas, "resultados");
            mymodel.Metas = GetResultados(metasCargas, "metas");
            mymodel.Cargas = GetNumerosD2(cargasMetasD2, "cargas"); // cargas passou a usar a SP 10_CargasMetas
            mymodel.MetasD2 = GetResultados(metasCargas, "metasD2");

            mymodel.CargasD2 = GetNumerosD2(cargasMetasD2, "numeroD2");
            mymodel.ResultadoD2 = GetNumerosD2(cargasMetasD2, "resultadosD2");
            mymodel.PorcentagemD2 = GetNumerosD2(cargasMetasD2, "porcentagemD2");

            mymodel.Resumos = GetResumos(metasCargas, cargasMetasD2);

            if (index)
                return View(mymodel);
            else
                return Json(mymodel);
        }

        public List<ResultadosViewModel> GetResultados(List<_10_MetasCargas> metasCargas, string tipoMeta)
        {
            var fluminense = new ResultadosViewModel();
            var metropolitana = new ResultadosViewModel();

            metropolitana.Meses = new List<string>() { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            fluminense.Meses = new List<string>() { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };

            metropolitana.Id = 1;
            fluminense.Id = 2;

            List<ResultadosViewModel> resultados = new List<ResultadosViewModel>();

            
            foreach (var item in metasCargas)
            {
                if (item.ZonaId == 1)
                {

                    if (tipoMeta == "resultados")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                metropolitana.Meses[i - 1] = Convert.ToInt32(item.Res).ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "metas")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                metropolitana.Meses[i - 1] = Convert.ToInt32(item.Meta).ToString();
                            }

                        }
                    }
                    else if (tipoMeta == "metasD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                metropolitana.Meses[i - 1] = Convert.ToInt32(item.MetaD2).ToString();
                            }
                        }
                    }
                     


                    if (item.Mes == 1 || item.Mes == 2 || item.Mes == 3)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 4 || item.Mes == 5 || item.Mes == 6)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 7 || item.Mes == 8 || item.Mes == 9)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 10 || item.Mes == 11 || item.Mes == 12)
                    {

                        if (tipoMeta == "resultados")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.MetaD2);
                    }

                 /*   if (tipoMeta == "resultados")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Res).ToString()); // valor correspondente a cada mês
                    else if (tipoMeta == "metas")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Meta).ToString());
                    else if (tipoMeta == "metasD2")
                        metropolitana.Meses.Add(Convert.ToInt32(item.MetaD2).ToString()); */
                }
                else
                {

                    if (tipoMeta == "resultados")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                fluminense.Meses[i - 1] = Convert.ToInt32(item.Res).ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "metas")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                fluminense.Meses[i - 1] = Convert.ToInt32(item.Meta).ToString();
                            }

                        }
                    }
                    else if (tipoMeta == "metasD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.Mes == i)
                            {
                                fluminense.Meses[i - 1] = Convert.ToInt32(item.MetaD2).ToString();
                            }
                        }
                    }


                    if (item.Mes == 1 || item.Mes == 2 || item.Mes == 3)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 4 || item.Mes == 5 || item.Mes == 6)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 7 || item.Mes == 8 || item.Mes == 9)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 10 || item.Mes == 11 || item.Mes == 12)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.MetaD2);
                    }

                  /*  if (tipoMeta == "resultados")
                        fluminense.Meses.Add(Convert.ToInt32(item.Res).ToString()); // valor correspondente a cada mês
                    else if (tipoMeta == "metas")
                        fluminense.Meses.Add(Convert.ToInt32(item.Meta).ToString());
                    else if (tipoMeta == "metasD2")
                        fluminense.Meses.Add(Convert.ToInt32(item.MetaD2).ToString()); */
                }
            }

            metropolitana.AcrescentaMesesQueFaltam();
            fluminense.AcrescentaMesesQueFaltam();

            metropolitana.Zona = "Metropolitana";
            metropolitana.Anual = metropolitana.Trim1 + metropolitana.Trim2 +
            metropolitana.Trim3 + metropolitana.Trim4;

            fluminense.Zona = "Fluminense";
            fluminense.Anual = fluminense.Trim1 + fluminense.Trim2 +
            fluminense.Trim3 + fluminense.Trim4;


            //Retirando os zeros
            for (int i = 0; i < 12; i++)
            {
                if (metropolitana.Meses[i] == "0")
                    metropolitana.Meses[i] = "";

                if (fluminense.Meses[i] == "0")
                    fluminense.Meses[i] = "";
            }

            resultados.Add(fluminense);
            resultados.Add(metropolitana);

            return resultados;
        }

        public List<ResultadosViewModel> GetNumerosD2(List<_10_CargasMetas> listaD2, string tipoMeta)
        {

            var fluminense = new ResultadosViewModel();
            var metropolitana = new ResultadosViewModel();

            metropolitana.Meses = new List<string>() { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            fluminense.Meses = new List<string>() { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };

            metropolitana.Id = 1;
            fluminense.Id = 2;

            List<ResultadosViewModel> resultados = new List<ResultadosViewModel>();

            //Contadores de meses NÂO VAZIOS
            int contaMesesMediaMetro1 = 0;
            int contaMesesMediaMetro2 = 0;
            int contaMesesMediaMetro3 = 0;
            int contaMesesMediaMetro4 = 0;

            int contaMesesMediaFlu1 = 0;
            int contaMesesMediaFlu2 = 0;
            int contaMesesMediaFlu3 = 0;
            int contaMesesMediaFlu4 = 0;

            foreach (var item in listaD2)
            {
                if (item.ZonaId == 1)
                {
                    if (tipoMeta == "cargas")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                metropolitana.Meses[i - 1] = Convert.ToInt32(item.Cargas).ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "numeroD2")
                    {
                        for (int i = 1; i <= 12; i++) // é necessário fazer essa lógica pois estava inserindo os valores errados
                        {//                              ex: Valores de Dezembro estavam sendo inseridos no "proximo" mês do array
                            if (item.MesCarga == i)        // ao invês de inserir Janeiro, e Dezembro, estava "Janeiro - Fevereiro
                            {
                                metropolitana.Meses[i - 1] = item.D2.ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "resultadosD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                metropolitana.Meses[i - 1] = Convert.ToInt32(item.Rd2).ToString();
                            }

                        }
                    }
                    else if (tipoMeta == "porcentagemD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                metropolitana.Meses[i - 1] = (Convert.ToDouble(item.Pd2) * 100).ToString();
                            }
                        }                      
                    }

                    if (item.MesCarga == 1 || item.MesCarga == 2 || item.MesCarga == 3)
                    {
                        if (tipoMeta == "cargas")
                        {
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Cargas);
                        }
                        else if (tipoMeta == "numeroD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);
                            if (metropolitana.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaMetro1 = contaMesesMediaMetro1 + 1;
                        }
                    }
                    else if (item.MesCarga == 4 || item.MesCarga == 5 || item.MesCarga == 6)
                    {
                        if (tipoMeta == "cargas")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (metropolitana.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaMetro2 = contaMesesMediaMetro2 + 1;
                        }
                    }
                    else if (item.MesCarga == 7 || item.MesCarga == 8 || item.MesCarga == 9)
                    {
                        if (tipoMeta == "cargas")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (metropolitana.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaMetro3 = contaMesesMediaMetro3 + 1;
                        }
                    }
                    else if (item.MesCarga == 10 || item.MesCarga == 11 || item.MesCarga == 12)
                    {
                        if (tipoMeta == "cargas")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);
                            if (metropolitana.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaMetro4 = contaMesesMediaMetro4 + 1;
                        }
                    }
                }
                else //fluminense
                {
                    //---------------------------
                    if (tipoMeta == "cargas")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                fluminense.Meses[i - 1] = Convert.ToInt32(item.Cargas).ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "numeroD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                fluminense.Meses[i - 1] = item.D2.ToString();
                            }
                        }
                    }
                    else if (tipoMeta == "resultadosD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                fluminense.Meses[i - 1] = Convert.ToInt32(item.Rd2).ToString();
                            }

                        }
                    }
                    else if (tipoMeta == "porcentagemD2")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            if (item.MesCarga == i)
                            {
                                fluminense.Meses[i - 1] = (Convert.ToDouble(item.Pd2) * 100).ToString();
                            }
                        }
                    }

                    // -----------------
                    if (item.MesCarga == 1 || item.MesCarga == 2 || item.MesCarga == 3)
                    {
                        if (tipoMeta == "cargas")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (fluminense.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaFlu1 = contaMesesMediaFlu1 + 1;
                        }
                    }
                    else if (item.MesCarga == 4 || item.MesCarga == 5 || item.MesCarga == 6)
                    {
                        if (tipoMeta == "cargas")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (fluminense.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaFlu2 = contaMesesMediaFlu2 + 1;
                        }
                    }
                    else if (item.MesCarga == 7 || item.MesCarga == 8 || item.MesCarga == 9)
                    {
                        if (tipoMeta == "cargas")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Rd2);
                        else if (tipoMeta == "porcentagemD2")
                        {
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (fluminense.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaFlu3 = contaMesesMediaFlu3 + 1;
                        }
                    }
                    else if (item.MesCarga == 10 || item.MesCarga == 11 || item.MesCarga == 12)
                    {
                        if (tipoMeta == "cargas")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "numeroD2")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2") { 
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Rd2);}
                        else if (tipoMeta == "porcentagemD2")
                        {
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(Convert.ToDouble(item.Pd2) * 100);

                            if (fluminense.Meses[item.MesCarga - 1] != "0")
                                contaMesesMediaFlu4 = contaMesesMediaFlu4 + 1;
                        }
                    }

                }
            }
            //caso não tenha as colunas todas de meses preenchidos
            metropolitana.AcrescentaMesesQueFaltam();
            fluminense.AcrescentaMesesQueFaltam();

            metropolitana.Zona = "Metropolitana";
            fluminense.Zona = "Fluminense";

            if (tipoMeta != "porcentagemD2")
            {
                metropolitana.Anual = metropolitana.Trim1 + metropolitana.Trim2 +
                metropolitana.Trim3 + metropolitana.Trim4;

                fluminense.Anual = fluminense.Trim1 + fluminense.Trim2 +
                fluminense.Trim3 + fluminense.Trim4;
            }else
            {
                //Cálculo da média   
                fluminense.Trim1 = (contaMesesMediaFlu1 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(fluminense.Trim1), Convert.ToDecimal(contaMesesMediaFlu1))):0;
                fluminense.Trim2 = (contaMesesMediaFlu2 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(fluminense.Trim2), Convert.ToDecimal(contaMesesMediaFlu2))):0;
                fluminense.Trim3 = (contaMesesMediaFlu3 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(fluminense.Trim3), Convert.ToDecimal(contaMesesMediaFlu3))):0;
                fluminense.Trim4 = (contaMesesMediaFlu4 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(fluminense.Trim4), Convert.ToDecimal(contaMesesMediaFlu4))):0;

                metropolitana.Trim1 = (contaMesesMediaMetro1 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(metropolitana.Trim1), Convert.ToDecimal(contaMesesMediaMetro1))):0;
                metropolitana.Trim2 = (contaMesesMediaMetro2 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(metropolitana.Trim2), Convert.ToDecimal(contaMesesMediaMetro2))):0;
                metropolitana.Trim3 = (contaMesesMediaMetro3 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(metropolitana.Trim3), Convert.ToDecimal(contaMesesMediaMetro3))):0;
                metropolitana.Trim4 = (contaMesesMediaMetro4 != 0)? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(metropolitana.Trim4), Convert.ToDecimal(contaMesesMediaMetro4))):0;


                // caso todos fiquem vazios
                bool fazMediaF = true;
                bool fazMediaM = true;
                foreach (var mes in fluminense.Meses)
                {
                    if (mes == ""){
                        fazMediaF = false;
                    }
                }

                foreach (var mes in metropolitana.Meses)
                {
                    if (mes == ""){
                        fazMediaM = false;
                    }
                }
                if (fazMediaF)
                    fluminense.Anual = Convert.ToInt32(fluminense.Meses.Where(c => c != "").Select(int.Parse).ToList().Where(f => f != 0).Average());
                else
                    fluminense.Anual = 0;

                if (fazMediaM)
                    metropolitana.Anual = Convert.ToInt32(metropolitana.Meses.Where(c => c != "").Select(int.Parse).ToList().Where(f => f != 0).Average());
                else
                    metropolitana.Anual = 0;

                //Concatena %
                for (int i = 0; i < 12; i++)
                {
                    if ((fluminense.Meses[i] != "0") && (fluminense.Meses[i] != ""))
                    {
                        fluminense.Meses[i] = fluminense.Meses[i] + "%";
                    }
                    if ((metropolitana.Meses[i] != "0") && (metropolitana.Meses[i] != ""))
                    {
                        metropolitana.Meses[i] = metropolitana.Meses[i] + "%";
                    }
                }
            }

            //Retirando os zeros
            for (int i = 0; i < 12; i++)
            {
                if (metropolitana.Meses[i] == "0")
                    metropolitana.Meses[i] = "";

                if (fluminense.Meses[i] == "0")
                    fluminense.Meses[i] = "";
            }

            resultados.Add(fluminense);
            resultados.Add(metropolitana);


            return resultados;

        }

        public List<ResultadosViewModel> GetResumos(List<_10_MetasCargas> metasCargas, List<_10_CargasMetas> cargasMetasD2)
        {
            var resultadoViewModel = new ResultadosViewModel();
            var metasViewModel = new ResultadosViewModel();
            var cargasViewModel = new ResultadosViewModel();

            resultadoViewModel.Meses = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            metasViewModel.Meses = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
            cargasViewModel.Meses = new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };

            resultadoViewModel.Id = 1;
            metasViewModel.Id = 2;
            cargasViewModel.Id = 3;

            List <ResultadosViewModel> resumos = new List<ResultadosViewModel>();

            //Meses
            foreach (var item in metasCargas)
            {
                resultadoViewModel.Meses[item.Mes - 1] = (Convert.ToInt32(resultadoViewModel.Meses[item.Mes - 1]) + Convert.ToInt32(item.Res.ToString())).ToString();
                metasViewModel.Meses[item.Mes - 1] = (Convert.ToInt32(metasViewModel.Meses[item.Mes - 1]) + Convert.ToInt32(item.Meta.ToString())).ToString();
            }
            foreach (var item in cargasMetasD2)
            {
                cargasViewModel.Meses[item.MesCarga - 1] = (Convert.ToInt32(cargasViewModel.Meses[item.MesCarga - 1]) + Convert.ToInt32(item.Cargas.ToString())).ToString();
            }

            //Trimestres
            resultadoViewModel.Trim1 = resultadoViewModel.Meses.Select(int.Parse).Take(3).Sum();
            resultadoViewModel.Trim2 = resultadoViewModel.Meses.Select(int.Parse).Skip(3).Take(3).Sum(); // iniciasse no index 3 (skip) e pega 3 elementos(Take)
            resultadoViewModel.Trim3 = resultadoViewModel.Meses.Select(int.Parse).Skip(6).Take(3).Sum();
            resultadoViewModel.Trim4 = resultadoViewModel.Meses.Select(int.Parse).Skip(9).Take(3).Sum();

            metasViewModel.Trim1 = metasViewModel.Meses.Select(int.Parse).Take(3).Sum();
            metasViewModel.Trim2 = metasViewModel.Meses.Select(int.Parse).Skip(3).Take(3).Sum();
            metasViewModel.Trim3 = metasViewModel.Meses.Select(int.Parse).Skip(6).Take(3).Sum();
            metasViewModel.Trim4 = metasViewModel.Meses.Select(int.Parse).Skip(9).Take(3).Sum();

            cargasViewModel.Trim1 = cargasViewModel.Meses.Select(int.Parse).Take(3).Sum();
            cargasViewModel.Trim2 = cargasViewModel.Meses.Select(int.Parse).Skip(3).Take(3).Sum();
            cargasViewModel.Trim3 = cargasViewModel.Meses.Select(int.Parse).Skip(6).Take(3).Sum();
            cargasViewModel.Trim4 = cargasViewModel.Meses.Select(int.Parse).Skip(9).Take(3).Sum();

            //Anual
            resultadoViewModel.Anual = resultadoViewModel.Trim1 + resultadoViewModel.Trim2
                + resultadoViewModel.Trim3 + resultadoViewModel.Trim4;

            metasViewModel.Anual = metasViewModel.Trim1 + metasViewModel.Trim2 + metasViewModel.Trim3 +
                metasViewModel.Trim4;

            cargasViewModel.Anual = cargasViewModel.Trim1 + cargasViewModel.Trim2 + cargasViewModel.Trim3 +
                cargasViewModel.Trim4;

            resultadoViewModel.Zona = "Resultado"; // na verdade não é zona(ex.:fluminense, metropolitana). É somente a primeira coluna string;
            metasViewModel.Zona = "Metas";
            cargasViewModel.Zona = "Cargas";

            //Retirando os zeros
            for (int i = 0; i < 12; i++)
            {
                if (resultadoViewModel.Meses[i] == "0")
                    resultadoViewModel.Meses[i] = "";

                if (metasViewModel.Meses[i] == "0")
                    metasViewModel.Meses[i] = "";

                if (cargasViewModel.Meses[i] == "0")
                    cargasViewModel.Meses[i] = "";
            }
            
            //Adicionando na listagem de ResultadosViewModel
            resumos.Add(resultadoViewModel);
            resumos.Add(metasViewModel);
            resumos.Add(cargasViewModel);

            return resumos;
        }
    }
}
