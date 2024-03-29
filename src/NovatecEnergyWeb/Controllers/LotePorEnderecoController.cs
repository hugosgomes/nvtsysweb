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
using NovatecEnergyWeb.Models.Exportacao;
using Microsoft.AspNetCore.Hosting;
using NovatecEnergyWeb.Filters.ActionFilters;
using NovatecEnergyWeb.Core;
using NovatecEnergyWeb.Domain.Interfaces.Repository;

namespace NovatecEnergyWeb.Controllers
{
    public class LotePorEnderecoController : Controller
    {
        private BDNVTContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ILoteRepository _loteRepository;

        public LotePorEnderecoController(BDNVTContext context, IHostingEnvironment he, 
            ILoteRepository loteRepository)
        {
            _context = context;
            _hostingEnvironment = he;
            _loteRepository = loteRepository;
        }

        [AutenticacaoFilter]
        public IActionResult Index()
        {
            BindSelects();
            return View("Index", new List<LotePorEndereco>());

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
            
            dynamic retorno = new ExpandoObject();
            retorno.Delegacao = delegacao;
            retorno.Areas = areasL;
            retorno.Lotes = _loteRepository.GetLotes(listAreaInt,0);

            return Json(retorno);
        }
 
        public IActionResult DelegacaoCascade(int delegacao)
        {
            //area
            var AreasL = _context._00Areas.Where(c => c.Delegacao == delegacao).ToList();
            var listAreaInt = new List<int>();
            foreach (var item in AreasL)
            {
                listAreaInt.Add(item.Id);
            }

            dynamic retorno = new ExpandoObject();
            retorno.Areas = AreasL;
            retorno.Lotes = _loteRepository.GetLotes(listAreaInt,0);

            return Json(retorno);
        }

        public IActionResult AreaCascade(int lote)
        {
            var listInt = new List<int>();
            dynamic retorno = new ExpandoObject();
            retorno.Lotes = _loteRepository.GetLotes(listInt, lote);

            return Json(retorno);
        }

        public IActionResult AreasDoCliente()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            dynamic retorno = new ExpandoObject();
            if (id != null)
            {
                var areas = (from ca in _context.ClientesAreas
                             where ca.IdCliente == Convert.ToInt32(id)
                             join a in _context._00Areas on ca.IdArea equals a.Id
                             select new _00Areas
                             {
                                 Id = a.Id,
                                 Area = a.Area,
                                 Delegacao = a.Delegacao,
                                 Ge = a.Ge
                             }).ToList();

                
                retorno.Areas = areas;

                var listAreaInt = new List<int>();
                foreach (var item in areas)
                {
                    listAreaInt.Add(item.Id);
                }

                retorno.Areas = areas;
                retorno.Lotes = _loteRepository.GetLotes(listAreaInt, 0);
                return Json(retorno);
            }else
            {
                retorno.Areas = null;
                retorno.Lotes = null;
                return Json(retorno);
            }
        }

        public void BindSelects()
        {
            ViewBag.Lotes = new List<List<dynamic>>();
            ViewBag.Lotes = _loteRepository.GetLotesJoinItems();
            
            var zonas = _context._00Zona.Where(c => c.Id < 3).ToList();
            ViewBag.Zonas = new List<_00Zona>();
            foreach (var item in zonas)
            {
                var z = new _00Zona();
                z.Id = item.Id;
                z.Zona = item.Zona;
                ViewBag.Zonas.Add(z);
            }

            var delegacoes = _context._00Delegacao.ToList();
            ViewBag.Delegacao = new List<_00Delegação>();
            foreach (var item in delegacoes)
            {
                var d = new _00Delegação();
                d.Id = item.Id;
                d.Delegacao = item.Delegacao;
                d.Zona = item.Zona;
                ViewBag.Delegacao.Add(d);
            }

            var areas = _context._00Areas.ToList();
            ViewBag.Areas = new List<_00Areas>();
            foreach (var item in areas)
            {
                var a = new _00Areas();
                a.Id = item.Id;
                a.Area = item.Area;
                ViewBag.Areas.Add(a);
            }
        }

       

        private FormFiltersVisitaEnderecosViewModels GetFiltrosSessao()
        {
            var ffvm = new FormFiltersVisitaEnderecosViewModels();
            ffvm.IdLote = (HttpContext.Session.GetString("IdLote") == "") ? null : HttpContext.Session.GetString("IdLote");
            ffvm.ZId = (HttpContext.Session.GetString("ZId") == "") ? null : HttpContext.Session.GetString("ZId");
            ffvm.DId = (HttpContext.Session.GetString("DId") == "") ? null : HttpContext.Session.GetString("DId");
            ffvm.AId = (HttpContext.Session.GetString("AId") == "") ? null : HttpContext.Session.GetString("AId");
            ffvm.Endereco = (HttpContext.Session.GetString("Endereco") == "") ? null : HttpContext.Session.GetString("Endereco");

            return ffvm;
        }

        private void SetFiltrosSessao(FormFiltersVisitaEnderecosViewModels data)
        {
            if (data != null)
            {
                HttpContext.Session.SetString("IdLote", (data.IdLote == null) ? "" : data.IdLote);
                HttpContext.Session.SetString("ZId", (data.ZId == null) ? "" : data.ZId);
                HttpContext.Session.SetString("DId", (data.DId == null) ? "" : data.DId);
                HttpContext.Session.SetString("AId", (data.AId == null) ? "" : data.AId);
                HttpContext.Session.SetString("Endereco", (data.Endereco == null) ? "" : data.Endereco);
            }
        }

        public List<LotePorEndereco>GetEnderecosAtivos([FromForm]FormFiltersVisitaEnderecosViewModels filtros)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            int? delegacao = HttpContext.Session.GetInt32("Delegação");
            int? zona = HttpContext.Session.GetInt32("Zona");
            string tipo = HttpContext.Session.GetString("UserTipo");
            int? area = HttpContext.Session.GetInt32("Área");

            IQueryable<LotePorEndereco> e;

            if( filtros == null)
            {
                e = _context._11_LoteAtivoEndereco.FromSql("exec [dbo].[LotePorEndereco_Ativos] {0},{1},{2},{3},{4},{5}", null,zona, delegacao,null,null,
                    (tipo == "cli" && (area != null)) ? id : null);
            }
            else
            {
                e = _context._11_LoteAtivoEndereco.FromSql("exec [dbo].[LotePorEndereco_Ativos] {0},{1},{2},{3},{4},{5}", filtros.IdLote,
                    ((zona != null) ? zona.ToString() : (filtros.ZId != null) ? filtros.ZId.ToString() : null),
                    ((delegacao != null) ? delegacao.ToString() : (filtros.DId != null) ? filtros.DId.ToString() : null),
                     filtros.AId, filtros.Endereco, (tipo == "cli" && (area != null)) ? id : null);
            }
            return e.ToList();
        }

        public IActionResult GetListLoteAtivoView([FromForm]FormFiltersVisitaEnderecosViewModels filtros, bool eIndex, int PaginaClicada)
        {
            SetFiltrosSessao(filtros);

            //lista vinda da SP
            var evList = GetEnderecosAtivos(filtros);

            ViewBag.Potencial = evList.Sum(c => c.Potencial);

            //lógica do tratado  ----------------------------------------------
            ViewBag.Tratados = evList.Sum(e => e.Tratados);
            ViewBag.TratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Tratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.NaoTratados = ViewBag.Potencial - ViewBag.Tratados;
            ViewBag.NaoTratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.NaoTratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;

            ViewBag.c0Visita = evList.Sum(e => e.Visitas0);
            ViewBag.c0VisitaPercent = (ViewBag.NaoTratados != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.c0Visita), Convert.ToDecimal(ViewBag.NaoTratados)) * 100) : 0;
            ViewBag.c1Visita = evList.Sum(e => e.Visitas1);
            ViewBag.c1VisitaPercent = (ViewBag.NaoTratados != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.c1Visita), Convert.ToDecimal(ViewBag.NaoTratados)) * 100) : 0;
            ViewBag.c2Visita = evList.Sum(e => e.Visitas2);
            ViewBag.c2VisitaPercent = (ViewBag.NaoTratados != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.c2Visita), Convert.ToDecimal(ViewBag.NaoTratados)) * 100) : 0;
            
            //fim da lógica do tratado ----------------------------------------------


            ViewBag.Visitados = evList.Sum(c => c.Visitados);
            ViewBag.VisitadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Visitados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.NaoVisitados = ViewBag.Potencial - ViewBag.Visitados;
            ViewBag.NaoVisitadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.NaoVisitados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            //ok

            ViewBag.Contratados = evList.Sum(c => c.Contratados);
            ViewBag.ContratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Contratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.NaoContratados = ViewBag.Potencial - ViewBag.Contratados;
            ViewBag.NaoContratadosPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.NaoContratados), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            //ok

            ViewBag.D2 = evList.Sum(e => e.D2);
            ViewBag.D2Percent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.D2), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.Svg = evList.Sum(e => e.Svg);
            ViewBag.SvgPercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Svg), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            ViewBag.Sve = evList.Sum(e => e.Sve);
            ViewBag.SvePercent = (ViewBag.Potencial != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Sve), Convert.ToDecimal(ViewBag.Potencial)) * 100) : 0;
            //ok 

            ViewBag.Visitas = evList.Sum(c => c.Visitas);
            ViewBag.Entrevistas = evList.Sum(c => c.Entrevistas);
            ViewBag.EntrevistasPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Entrevistas), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;
            ViewBag.VisitasImpr = evList.Sum(c => c.VisitasImpr);
            ViewBag.VisitasImprPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.VisitasImpr), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;
            ViewBag.Ausentes = evList.Sum(c => c.Ausencias);
            ViewBag.AusentesPercent = (ViewBag.Visitas != 0) ? Convert.ToInt32(decimal.Divide(Convert.ToDecimal(ViewBag.Ausentes), Convert.ToDecimal(ViewBag.Visitas)) * 100) : 0;
            ViewBag.VisitaAgendada = evList.Sum(c => c.VisitasAgendadas);
            //ok

            if (eIndex)
                return View("VisitasEnderecos", evList);
            else
            {
                dynamic jsonModel = new ExpandoObject();
                jsonModel.Numeracoes = new List<string>();
                jsonModel.Porcentagens = new List<string>();

                jsonModel.Numeracoes.Add(ViewBag.Potencial.ToString());//e0                                                                                      
                jsonModel.Numeracoes.Add(ViewBag.Visitados.ToString()); //e1
                jsonModel.Numeracoes.Add(ViewBag.NaoVisitados.ToString());//e2
                jsonModel.Numeracoes.Add(evList.Count().ToString()); //e3

                jsonModel.Numeracoes.Add(ViewBag.Contratados.ToString());//e4
                jsonModel.Numeracoes.Add(ViewBag.NaoContratados.ToString());//e5

                jsonModel.Numeracoes.Add(ViewBag.D2.ToString());//e6
                jsonModel.Numeracoes.Add(ViewBag.Svg.ToString());//e7
                jsonModel.Numeracoes.Add(ViewBag.Sve.ToString());//e8
                jsonModel.Numeracoes.Add(ViewBag.Visitas.ToString());//e9
                jsonModel.Numeracoes.Add(ViewBag.Entrevistas.ToString());//e10
                jsonModel.Numeracoes.Add(ViewBag.VisitasImpr.ToString());//e11
                jsonModel.Numeracoes.Add(ViewBag.Ausentes.ToString());//e12
                jsonModel.Numeracoes.Add(ViewBag.VisitaAgendada.ToString());//e13

                //tratados
                jsonModel.Numeracoes.Add(ViewBag.Tratados.ToString());   //e14
                jsonModel.Numeracoes.Add(ViewBag.NaoTratados.ToString());//e15               
                jsonModel.Numeracoes.Add(ViewBag.c0Visita.ToString());   //e16         
                jsonModel.Numeracoes.Add(ViewBag.c1Visita.ToString());   //e17 
                jsonModel.Numeracoes.Add(ViewBag.c2Visita.ToString());   //e18


                jsonModel.Porcentagens.Add(ViewBag.VisitadosPercent.ToString()); //ep0
                jsonModel.Porcentagens.Add(ViewBag.NaoVisitadosPercent.ToString()); //ep1
                jsonModel.Porcentagens.Add(ViewBag.ContratadosPercent.ToString());//ep2
                jsonModel.Porcentagens.Add(ViewBag.NaoContratadosPercent.ToString());//ep3
                jsonModel.Porcentagens.Add(ViewBag.D2Percent.ToString());//ep4
                jsonModel.Porcentagens.Add(ViewBag.SvgPercent.ToString());//ep5
                jsonModel.Porcentagens.Add(ViewBag.SvePercent.ToString());//ep6
                jsonModel.Porcentagens.Add(ViewBag.EntrevistasPercent.ToString());//ep7
                jsonModel.Porcentagens.Add(ViewBag.VisitasImprPercent.ToString());//ep8
                jsonModel.Porcentagens.Add(ViewBag.AusentesPercent.ToString()); //ep9

                //tratados
                jsonModel.Porcentagens.Add(ViewBag.TratadosPercent.ToString()); //ep10
                jsonModel.Porcentagens.Add(ViewBag.NaoTratadosPercent.ToString()); //ep11
                jsonModel.Porcentagens.Add(ViewBag.c0VisitaPercent.ToString()); //ep12
                jsonModel.Porcentagens.Add(ViewBag.c1VisitaPercent.ToString()); //ep13
                jsonModel.Porcentagens.Add(ViewBag.c2VisitaPercent.ToString()); //ep14
                // continuar dia 30/06/2017

                var pagina = 0;
                if (PaginaClicada != 0)
                {
                    pagina = (PaginaClicada - 1) * 20;
                }
                jsonModel.EV = evList.Skip(pagina).Take(20);
                jsonModel.QuantasPaginasExistem = (evList.Count() != 0) ? Math.Ceiling(decimal.Divide(Convert.ToDecimal(evList.Count()), 20)) : 1;

                return Json(jsonModel);
 
            }
        }

        public IActionResult LimpaFiltros()
        {
            return GetListLoteAtivoView(null, false,  0);
        }

        public IActionResult LimpaSelects()
        {
            BindSelects();
            var i = ViewBag.Lotes; // parece que se eu não fizer isso a ViewBag não atualiza

            dynamic jsonModel = new ExpandoObject();
            jsonModel.Zonas = ViewBag.Zonas;
            jsonModel.Delegacao = ViewBag.Delegacao;
            jsonModel.Areas = ViewBag.Areas;
            jsonModel.Lotes = ViewBag.Lotes; 
           
            return Json(jsonModel);
        }

        public IActionResult SetFiltrosTelaExportacao([FromForm]FormFiltersAgendaVisitaEnderecosViewModel filtrosTelaExportacao)
        {
            if (filtrosTelaExportacao != null)
            {
                HttpContext.Session.SetString("Ano", (filtrosTelaExportacao.Ano == null) ? "" : filtrosTelaExportacao.Ano);
                HttpContext.Session.SetString("Mes", (filtrosTelaExportacao.Mes == null) ? "" : filtrosTelaExportacao.Mes);
            }
            return Json("OK");
        }

        private FormFiltersAgendaVisitaEnderecosViewModel GetFiltrosAgenda()
        {
            var ffavem = new FormFiltersAgendaVisitaEnderecosViewModel();
            ffavem.Ano = (HttpContext.Session.GetString("Ano") == "") ? null : HttpContext.Session.GetString("Ano");
            ffavem.Mes = (HttpContext.Session.GetString("Mes") == "") ? null : HttpContext.Session.GetString("Mes");

            return ffavem;
        }

        public IActionResult ExportaAgendaAdesao()
        {
            var filtrosTelaAnterior = GetFiltrosSessao();
            var idLoteFiltro = HttpContext.Session.GetString("IdLote"); 

            // dados vindos das SPs 11_LoteAtivoEnderecos/ 11_LoteRodosEnderecos
            var dataEnderecosAtivos = GetEnderecosAtivos(filtrosTelaAnterior);

            var filtrosTelaExportacao = GetFiltrosAgenda();

            var dataExporta = _context._11_LoteAtivoEnderecosExportacao
                                .FromSql("exec [dbo].[LotePorEndereco_ExportaAgendaAdesao] {0},{1},{2}", idLoteFiltro, filtrosTelaExportacao.Ano,
                                filtrosTelaExportacao.Mes).ToList();

            var lote = (from l in _context._11Lotes
                        where l.Id == Convert.ToInt32(idLoteFiltro)
                        join ti in _context._00TabelasItems on l.Status equals ti.Id
                        join a in _context._00Areas on l.Area equals a.Id
                        select new
                        {
                            Id = l.Id,
                            LoteNum = l.LoteNum,
                            Area = a.Area,
                            Ge = l.Ge,
                            DataLote = l.DataLote
                        });

            EnderecoVisitasDataExporter exp = new EnderecoVisitasDataExporter(_hostingEnvironment);
            byte[] fileBytes = exp.ExportaAgendaAdesao(dataEnderecosAtivos, dataExporta, lote, filtrosTelaExportacao);

            return File(fileBytes, "application/x-msdownload", exp.FileName);
        }
    }
}
