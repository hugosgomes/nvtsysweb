﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NovatecEnergyWeb.Models;
using NovatecEnergyWeb.Filters.ActionFilters;
using Microsoft.EntityFrameworkCore;
using NovatecEnergyWeb.Models.MetasViewModels;
using System.Dynamic;

namespace NovatecEnergyWeb.Controllers
{
    public class MetasController : Controller
    {

        private BDNVTContext _context;

        public MetasController(BDNVTContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            var metasCargas = _context._10_MetasCargas.FromSql("EXECUTE [dbo].[10_MetasCargas]").Where(c => c.Ano == 2017).ToList();
            var cargasMetasD2 = _context._10_CargasMetas.FromSql("EXECUTE [dbo].[10_CargasMetas]").Where(c => c.AnoCarga == 2017).ToList();

            dynamic mymodel = new ExpandoObject();

            mymodel.Resultados = GetResultados(metasCargas,"resultados");
            mymodel.Metas = GetResultados(metasCargas, "metas");
            mymodel.Cargas = GetResultados(metasCargas, "cargas");
            mymodel.MetasD2 = GetResultados(metasCargas, "metasD2");

            mymodel.CargasD2 = GetNumerosD2(cargasMetasD2,"numeroD2");
            mymodel.ResultadoD2 = GetNumerosD2(cargasMetasD2,"resultadosD2");
            mymodel.PorcentagemD2 = GetNumerosD2(cargasMetasD2, "porcentagemD2");

            return View(mymodel);
        }

        public List<ResultadosViewModel> GetResultados(List<_10_MetasCargas> metasCargas, string tipoMeta)
        {
            var fluminense = new ResultadosViewModel();
            var metropolitana = new ResultadosViewModel();

            metropolitana.Meses = new List<string>();
            fluminense.Meses = new List<string>();

            List<ResultadosViewModel> resultados = new List<ResultadosViewModel>();

            
            foreach (var item in metasCargas)
            {
                if (item.ZonaId == 1)
                {

                    if (item.Mes == 1 || item.Mes == 2 || item.Mes == 3)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 4 || item.Mes == 5 || item.Mes == 6)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 7 || item.Mes == 8 || item.Mes == 9)
                    {
                        if (tipoMeta == "resultados")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 10 || item.Mes == 11 || item.Mes == 12)
                    {

                        if (tipoMeta == "resultados")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.MetaD2);
                    }

                    if (tipoMeta == "resultados")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Res).ToString()); // valor correspondente a cada mês
                    else if (tipoMeta == "metas")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Meta).ToString());
                    else if (tipoMeta == "cargas")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Cargas).ToString());
                    else if (tipoMeta == "metasD2")
                        metropolitana.Meses.Add(Convert.ToInt32(item.MetaD2).ToString());
                }
                else
                {
                    if (item.Mes == 1 || item.Mes == 2 || item.Mes == 3)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 4 || item.Mes == 5 || item.Mes == 6)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 7 || item.Mes == 8 || item.Mes == 9)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.MetaD2);
                    }
                    if (item.Mes == 10 || item.Mes == 11 || item.Mes == 12)
                    {
                        if (tipoMeta == "resultados")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Res);
                        else if (tipoMeta == "metas")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Meta);
                        else if (tipoMeta == "cargas")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Cargas);
                        else if (tipoMeta == "metasD2")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.MetaD2);
                    }

                    if (tipoMeta == "resultados")
                        fluminense.Meses.Add(Convert.ToInt32(item.Res).ToString()); // valor correspondente a cada mês
                    else if (tipoMeta == "metas")
                        fluminense.Meses.Add(Convert.ToInt32(item.Meta).ToString());
                    else if (tipoMeta == "cargas")
                        fluminense.Meses.Add(Convert.ToInt32(item.Cargas).ToString());
                    else if (tipoMeta == "metasD2")
                        fluminense.Meses.Add(Convert.ToInt32(item.MetaD2).ToString());
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

            resultados.Add(fluminense);
            resultados.Add(metropolitana);

            return resultados;
        }

        public List<ResultadosViewModel>GetNumerosD2(List<_10_CargasMetas> listaD2, string tipoMeta)
        {
            var fluminense = new ResultadosViewModel();
            var metropolitana = new ResultadosViewModel();

            metropolitana.Meses = new List<string>();
            fluminense.Meses = new List<string>();

            metropolitana.MesesPorcentagem = new List<string>();
            fluminense.MesesPorcentagem = new List<string>();

            List<ResultadosViewModel> resultados = new List<ResultadosViewModel>();

            foreach (var item in listaD2)
            {
                if (item.ZonaId == 1)
                {

                    if (item.MesCarga == 1 || item.MesCarga == 2 || item.MesCarga == 3)
                    {
                        if (tipoMeta == "numeroD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim1 = metropolitana.Trim1 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 4 || item.MesCarga == 5 || item.MesCarga == 6)
                    {
                        if (tipoMeta == "numeroD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim2 = metropolitana.Trim2 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 7 || item.MesCarga == 8 || item.MesCarga == 9)
                    {
                        if (tipoMeta == "numeroD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim3 = metropolitana.Trim3 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 10 || item.MesCarga == 11 || item.MesCarga == 12)
                    {
                        if (tipoMeta == "numeroD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            metropolitana.Trim4 = metropolitana.Trim4 + Convert.ToInt32(item.Rd2);
                    }

                    if (tipoMeta == "numeroD2")
                        metropolitana.Meses.Add(item.D2.ToString());
                    else if (tipoMeta == "resultadosD2")
                        metropolitana.Meses.Add(Convert.ToInt32(item.Rd2).ToString());
                    else if (tipoMeta == "porcentagemD2")
                        metropolitana.MesesPorcentagem.Add((Convert.ToDouble(item.Pd2)*100).ToString() + "%");
                }
                else
                {
                    if (item.MesCarga == 1 || item.MesCarga == 2 || item.MesCarga == 3)
                    {
                        if (tipoMeta == "numeroD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim1 = fluminense.Trim1 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 4 || item.MesCarga == 5 || item.MesCarga == 6)
                    {
                        if (tipoMeta == "numeroD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim2 = fluminense.Trim2 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 7 || item.MesCarga == 8 || item.MesCarga == 9)
                    {
                        if (tipoMeta == "numeroD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim3 = fluminense.Trim3 + Convert.ToInt32(item.Rd2);
                    }
                    else if (item.MesCarga == 10 || item.MesCarga == 11 || item.MesCarga == 12)
                    {
                        if (tipoMeta == "numeroD2")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.D2);
                        else if (tipoMeta == "resultadosD2")
                            fluminense.Trim4 = fluminense.Trim4 + Convert.ToInt32(item.Rd2);
                    }
                    if (tipoMeta == "numeroD2")
                        fluminense.Meses.Add(Convert.ToInt32(item.D2).ToString());
                    else if (tipoMeta == "resultadosD2")
                        fluminense.Meses.Add(Convert.ToInt32(item.Rd2).ToString());
                    else if (tipoMeta == "porcentagemD2")
                        fluminense.MesesPorcentagem.Add((Convert.ToDouble(item.Pd2) * 100).ToString()+"%");
                }
            }

            //caso não tenha as colunas todas de meses preenchidos
            metropolitana.AcrescentaMesesQueFaltam();
            fluminense.AcrescentaMesesQueFaltam();

            metropolitana.Zona = "Metropolitana";
            metropolitana.Anual = metropolitana.Trim1 + metropolitana.Trim2 +
            metropolitana.Trim3 + metropolitana.Trim4;

            fluminense.Zona = "Fluminense";
            fluminense.Anual = fluminense.Trim1 + fluminense.Trim2 +
            fluminense.Trim3 + fluminense.Trim4;

            resultados.Add(fluminense);
            resultados.Add(metropolitana);


            return resultados;

        }
    }
}
