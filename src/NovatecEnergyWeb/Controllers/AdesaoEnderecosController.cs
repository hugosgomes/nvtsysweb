﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NovatecEnergyWeb.Models;
using Microsoft.EntityFrameworkCore;
using NovatecEnergyWeb.Models.StoredProcedures;
using NovatecEnergyWeb.Models.AdesaoViewModels;
using Microsoft.AspNetCore.Http;
using System.Dynamic;

namespace NovatecEnergyWeb.Controllers
{
    public class AdesaoEnderecosController : Controller
    {
        private BDNVTContext _context;

        public AdesaoEnderecosController(BDNVTContext context)
        {
            _context = context;
        }

        public IActionResult ZonaCascade(int zona)
        {
            //delegacao
            var delegacao = _context._00Delegacao.Where(c => c.Zona == zona)
                .Select(c => new _00Delegação { Id = c.Id, Delegacao = c.Delegacao, Zona = c.Zona })
            .ToList();

            var listint = new List<int>();
            foreach (var item in delegacao)
            {
                listint.Add(item.Id);
            }

            //area
            var areasL = _context._00Areas.Where(x => listint.Contains(Convert.ToInt32(x.Delegacao))).ToList();

            //lotes 
            var listAreaInt = new List<int>();
            foreach (var item in areasL)
            {
                listAreaInt.Add(item.Id);
            }

            var lotes = (from l in _context._11Lotes
                         where listAreaInt.Contains(l.Area)
                         join ti in _context._00TabelasItems on l.Status equals ti.Id
                         join ti2 in _context._00TabelasItems on l.Procedencia equals ti2.Id
                         select new
                         {
                             Id = l.Id,
                             LoteNum = l.LoteNum,
                             Ge = l.Ge,
                             DataLote = l.DataLote,
                             Tipo = ti2.Item,
                             Status = ti.Item
                         }).ToList();

            var Lotes = new List<List<dynamic>>();
            foreach (var item in lotes)
            {
                var d = new List<dynamic>();
                d.Add(item.Id);
                d.Add(item.LoteNum);
                d.Add(item.Ge);
                d.Add(item.DataLote.GetValueOrDefault().ToString("dd/MM/yyyy"));
                d.Add(item.Tipo);
                d.Add(item.Status);
                Lotes.Add(d);
            }

            dynamic retorno = new ExpandoObject();
            retorno.Delegacao = delegacao;
            retorno.Area = areasL;
            retorno.Lote = Lotes;

            return Json(retorno);
        }

       

        public void BindSelects()
        {
            var lotes = (from l in _context._11Lotes
                        join ti in _context._00TabelasItems on l.Status equals ti.Id
                        join ti2 in _context._00TabelasItems on l.Procedencia equals ti2.Id
                        select new
                        {
                            Id = l.Id,
                            LoteNum = l.LoteNum,
                            Ge = l.Ge,
                            DataLote = l.DataLote,
                            Tipo = ti2.Item,
                            Status = ti.Item
                        }).ToList();
            ViewBag.Lotes = new List<List<dynamic>>();
            foreach (var item in lotes)
            {
                var l = new List<dynamic>();
                l.Add(item.Id);
                l.Add(item.LoteNum);
                l.Add(item.Ge);
                l.Add(item.DataLote);
                l.Add(item.Tipo);
                l.Add(item.Status);
                ViewBag.Lotes.Add(l);
            }

            var zonas = _context._00Zona.Where(c => c.Id < 3).ToList();
            ViewBag.Zonas = new List<_00Zona>();
            ViewBag.Zonas = zonas;

            var delegacoes = _context._00Delegacao.ToList();
            ViewBag.Delegacao = new List<_00Delegação>();
            ViewBag.Delegacao = delegacoes;

            var areas = _context._00Areas.ToList();
            ViewBag.Areas = new List<_00Areas>();
            ViewBag.Areas = areas;
        }

        public IActionResult VisitasEnderecos()
        {
            BindSelects();
            return View("VisitasEnderecos", new List<_11_LoteAtivoEnderecos>());

        }

        private void setFiltrosSessao(FormFiltersVisitaEnderecosViewModels data, string Botao)
        {
            if (data != null)
            {
                HttpContext.Session.SetString("IdLote", (data.IdLote == null) ? "" : data.IdLote);
                HttpContext.Session.SetString("ZId", (data.ZId == null) ? "" : data.ZId);
                HttpContext.Session.SetString("DId", (data.DId == null) ? "" : data.DId);
                HttpContext.Session.SetString("AId", (data.AId == null) ? "" : data.AId);
                HttpContext.Session.SetString("Endereco", (data.Endereco == null) ? "" : data.Endereco);
            }

            string valorSP = "";

            switch (Botao)
            {
                case "ativos":
                    valorSP = "[dbo].[11_LoteAtivoEnderecos]";
                    break;
                case "todos":
                    valorSP = " "; //implementar depois
                    break;
            }

            HttpContext.Session.SetString("SP_Enderecos", valorSP);
        }

        public List<_11_LoteAtivoEnderecos>GetEnderecosAtivos([FromForm]FormFiltersVisitaEnderecosViewModels filtros)
        {
            string storedProcedure = HttpContext.Session.GetString("SP_Enderecos");
            int? id = HttpContext.Session.GetInt32("UserId");
            int? delegacao = HttpContext.Session.GetInt32("Delegação");
            int? zona = HttpContext.Session.GetInt32("Zona");

            IQueryable<_11_LoteAtivoEnderecos> e;

            if( filtros == null)
            {
                e = _context._11_LoteAtivoEndereco.FromSql("exec "+storedProcedure+" {0},{1},{2},{3},{4}",null,zona, delegacao,null,null);
            }
            else
            {
                e = _context._11_LoteAtivoEndereco.FromSql("exec " + storedProcedure + " {0},{1},{2},{3},{4}", filtros.IdLote,
                    ((zona != null) ? zona.ToString() : (filtros.ZId != null) ? filtros.ZId.ToString() : null),
                    ((delegacao != null) ? delegacao.ToString() : (filtros.DId != null) ? filtros.DId.ToString() : null),
                     filtros.AId, filtros.Endereco);
            }
            return e.ToList();
        }

        public IActionResult GetListLoteAtivoView([FromForm]FormFiltersVisitaEnderecosViewModels filtros, bool eIndex, string Botao, int PaginaClicada)
        {
            setFiltrosSessao(filtros, Botao);

            //lista vinda da SP
            var evList = GetEnderecosAtivos(filtros);

            ViewBag.Potencial = evList.Sum(c => c.Potencial);
            ViewBag.Visitados = evList.Sum(c => c.Visitados);
            ViewBag.VisitadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Visitados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.NaoVisitados = ViewBag.Potencial - ViewBag.Visitados;
            ViewBag.NaoVisitadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.NaoVisitados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;

            ViewBag.Contratados = evList.Sum(c => c.Contratados);
            ViewBag.ContratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Contratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.NaoContratados = ViewBag.Potencial - ViewBag.Contratados;
            ViewBag.NaoContratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.NaoContratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;

            ViewBag.D2 = evList.Sum(e => e.D2);
            ViewBag.D2Percent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.D2), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.Svg = evList.Sum(e => e.Svg);
            ViewBag.SvgPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Svg), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.Sve = evList.Sum(e => e.Sve);
            ViewBag.SvePercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Sve), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;

            ViewBag.NovaVisitaAgendada = evList.Sum(e => e.VisitasAgendadas);

            ViewBag.VisitaAgendada = evList.Sum(c => c.UnidadesAgendadas);
            ViewBag.VisitaAgendadaPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.VisitaAgendada), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.Visitas = evList.Sum(c => c.Visitas);
            ViewBag.Ausentes = evList.Sum(c => c.Ausencias);
            ViewBag.VisitasComResposta = ViewBag.Visitas - ViewBag.Ausentes;
            ViewBag.VisitasComRespostaPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.VisitasComResposta), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;
            ViewBag.AusentesPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Ausentes), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;

            ViewBag.Endinex = evList.Sum(c => c.Endinex);
            ViewBag.EndinexPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Endinex), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;

            if (eIndex)
                return View("VisitasEnderecos", evList);
            else
            {
                dynamic jsonModel = new ExpandoObject();
                jsonModel.Numeracoes = new List<string>();
                jsonModel.Porcentagens = new List<string>();

                jsonModel.Numeracoes.Add(evList.Count().ToString()); //e0
                jsonModel.Numeracoes.Add(ViewBag.Potencial.ToString()); //e1
                jsonModel.Numeracoes.Add(ViewBag.Visitados.ToString()); // e2
                jsonModel.Numeracoes.Add(ViewBag.NaoVisitados.ToString());//e3
                jsonModel.Numeracoes.Add(ViewBag.VisitaAgendada.ToString());//e4
                jsonModel.Numeracoes.Add(ViewBag.Contratados.ToString());//e5
                jsonModel.Numeracoes.Add(ViewBag.NaoContratados.ToString());//e6
                jsonModel.Numeracoes.Add(ViewBag.D2.ToString());//e7
                jsonModel.Numeracoes.Add(ViewBag.Svg.ToString());//e8
                jsonModel.Numeracoes.Add(ViewBag.Sve.ToString());//e9
                jsonModel.Numeracoes.Add(ViewBag.NovaVisitaAgendada.ToString());//e10
                jsonModel.Numeracoes.Add(ViewBag.Visitas.ToString());//e11
                jsonModel.Numeracoes.Add(ViewBag.VisitasComResposta.ToString());//e12
                jsonModel.Numeracoes.Add(ViewBag.Ausentes.ToString());//e13
                jsonModel.Numeracoes.Add(ViewBag.Endinex.ToString()); //e14

                jsonModel.Porcentagens.Add(ViewBag.VisitadosPercent.ToString()); //ep0
                jsonModel.Porcentagens.Add(ViewBag.NaoVisitadosPercent.ToString()); //ep1
                jsonModel.Porcentagens.Add(ViewBag.VisitaAgendadaPercent.ToString());//ep2
                jsonModel.Porcentagens.Add(ViewBag.ContratadosPercent.ToString());//ep3
                jsonModel.Porcentagens.Add(ViewBag.NaoContratadosPercent.ToString());//ep4
                jsonModel.Porcentagens.Add(ViewBag.D2Percent.ToString());//ep5
                jsonModel.Porcentagens.Add(ViewBag.SvgPercent.ToString());//ep6
                jsonModel.Porcentagens.Add(ViewBag.SvePercent.ToString());//ep7
                jsonModel.Porcentagens.Add(ViewBag.VisitasComRespostaPercent.ToString());//ep8
                jsonModel.Porcentagens.Add(ViewBag.AusentesPercent.ToString()); //ep9

                jsonModel.EV = evList;

                return Json(jsonModel);
 
            }
        }


    }
}