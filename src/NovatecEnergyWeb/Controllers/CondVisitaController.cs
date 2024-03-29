﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NovatecEnergyWeb.Core;

using System.Dynamic;
using Microsoft.AspNetCore.Http;
using NovatecEnergyWeb.Filters.ActionFilters;
using NovatecEnergyWeb.Models.Exportacao;
using NovatecEnergyWeb.Models.StoredProcedures;
using NovatecEnergyWeb.Domain.Interfaces.Repository;

namespace NovatecEnergyWeb.Controllers
{
    public class CondVisitaController : Controller
    {

        private BDNVTContext _context;

        private ICondVisitasRepository _condVisitasRepository;
        private IAreaRepository _areaRepository;
        private ILoteRepository _loteRepository;
        private ICondominioLoteAtivo _condominioRepository;
        private IMotivoRejeicao _motivoRejeicaoRepository;
        private IExcelExportVisitaEndereco _ExcelExportvisitaEndereco;


        public CondVisitaController(BDNVTContext context,
           ICondVisitasRepository condVisitasRepository, IMotivoRejeicao motivoRejeicaoRepository, IAreaRepository areaRepository,
           ICondominioLoteAtivo condominioRepository, ILoteRepository loteRepository, IExcelExportVisitaEndereco ExcelExportvisitaEndereco

           )

        {
            _context = context;
            _condVisitasRepository = condVisitasRepository;
            _motivoRejeicaoRepository = motivoRejeicaoRepository;
            _areaRepository = areaRepository;
            _loteRepository = loteRepository;
            _condominioRepository = condominioRepository;
            _ExcelExportvisitaEndereco = ExcelExportvisitaEndereco;

        }


        [HttpGet]
        [AutenticacaoFilter]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscaVisitas(int num, int zonas, int delegacao, int area, int lotes, int condominio, String visitado,
           String interesse, String pco, int tSocial, int tipoVisita, int rejeicao, int agecomercial, String diavisita1, String diavisita2,
            String condominioinput, String agcomercialinput, String localidadeinput, String bairro, String logradouroinput,
            int Numero1, int Numero2
         )
        {
            
            var d = HttpContext.Session.GetInt32("Delegação");
            var z = HttpContext.Session.GetInt32("Zona");
            var quantArea = HttpContext.Session.GetInt32("QuantidadeArea");

            if (quantArea != null && ((int)quantArea == 1))
            {
                area = (int) HttpContext.Session.GetInt32("Área");
            }
           

            // se for cliente, vai ter as variáveis de sessão preenchidas
            zonas = (z != null) ? (int)z : zonas;
            delegacao = (d != null) ? (int)d : delegacao;


            // definicoes da paginacao
            var pagina = 0;
            var PaginaClicada = num;
            var itensPagina = 20;

            if (PaginaClicada != 0)
            {
                pagina = (PaginaClicada - 1) * 20;
            }



            // retorna a consulta filtrada pelos parametros
            var visitas = _condVisitasRepository.GetVisitasFiltro(zonas, delegacao, area, condominio);



            // filtra as seguintes colunas
            if (lotes != 0)
            {
                visitas = visitas.Where(v => v.IdLote == lotes).ToList();
            }

            if (visitado != null)
            {

                visitas = visitas.Where(c => c.Visitado == visitado).ToList();

            }

            if (interesse != null)
            {

                visitas = visitas.Where(c => c.Interesse == interesse).ToList();

            }



            if (pco != null)
            {

                visitas = visitas.Where(c => c.Pco == pco).ToList();

            }

            if (tSocial != 0)
            {

                visitas = visitas.Where(c => c.TarifaSocial == tSocial).ToList();

            }

            if (tipoVisita != 0)
            {

                visitas = visitas.Where(c => c.TipoVisitaId == tipoVisita).ToList();

            }

            if (rejeicao != 0)
            {

                visitas = visitas.Where(c => c.NegativaId == rejeicao).ToList();

            }

            if (agecomercial != 0)
            {

                visitas = visitas.Where(c => c.AgId == agecomercial).ToList();

            }

           



            if (condominioinput != null)
            {
                visitas = visitas.Where(c => c.Condominio.Contains(condominioinput)).ToList();
            }

            if (agcomercialinput != null)
            {
                visitas = visitas.Where(c => c.AgVisita.Contains(agcomercialinput)).ToList();
            }

            if (localidadeinput != null)
            {

                visitas = visitas.Where(c => c.Localidade.Contains(localidadeinput)).ToList();

            }

            if (bairro != null)
            {

                visitas = visitas.Where(c => c.Bairro.Contains(bairro)).ToList();

            }

            if (logradouroinput != null)
            {

                visitas = visitas.Where(c => c.Logradouro.Contains(logradouroinput)).ToList();

            }

            if (Numero1 != 0 && Numero2 != 0)
            {

                visitas = visitas.Where(c => c.Num >= Numero1 && c.Num <= Numero2).OrderBy(c => c.Num).ToList();

            }



            if (diavisita1 != null && diavisita2 != null)
            {

                DateTime dt = Convert.ToDateTime(diavisita1);
                DateTime dt2 = Convert.ToDateTime(diavisita2);

                visitas = visitas.Where(w => w.DataHora >= dt && w.DataHora <= dt2

                ).ToList();
            }


            var retorno = new
            {
                // paginacao
                vis = visitas.Skip(pagina).Take(itensPagina),
                contagem = visitas.Count(),  // contagem
                totalPag = visitas.Count() / itensPagina + 1
            };

            return Json(retorno);
        }



        public IActionResult ExportaExcel(int num, int zonas, int delegacao, int area, int lotes, int condominio, int visitado,
           int interesse, int pco, int tSocial, int tipoVisita, int rejeicao, int agecomercial, String diavisita1, String diavisita2,
            String condominioinput, String agcomercialinput, String localidadeinput, String bairro, String logradouroinput,
            int Numero1, int Numero2
         )

        {
            // retorna a consulta filtrada pelos parametros
            var visitas = _condVisitasRepository.GetVisitasFiltro(zonas, delegacao, area, condominio);

            // filtra as seguintes colunas
            if (lotes != 0)
            {
                visitas = visitas.Where(v => v.IdLote == lotes).ToList();
            }

            if (visitado != 0)
            {

                visitas = visitas.Where(c => c.Visitado == visitado.ToString()).ToList();

            }

            if (interesse != 0)
            {

                visitas = visitas.Where(c => c.Interesse == interesse.ToString()).ToList();

            }



            if (pco != 0)
            {

                visitas = visitas.Where(c => c.Pco == pco.ToString()).ToList();

            }

            if (tSocial != 0)
            {

                visitas = visitas.Where(c => c.TarifaSocial == tSocial).ToList();

            }

            if (tipoVisita != 0)
            {

                visitas = visitas.Where(c => c.TipoVisitaId == tipoVisita).ToList();

            }

            if (rejeicao != 0)
            {

                visitas = visitas.Where(c => c.NegativaId == rejeicao).ToList();

            }

            if (agecomercial != 0)
            {

                visitas = visitas.Where(c => c.AgId == agecomercial).ToList();

            }





            if (condominioinput != null)
            {

                visitas = visitas.Where(c => c.Condominio.Contains(condominioinput)).ToList();

            }

            if (agcomercialinput != null)
            {

                visitas = visitas.Where(c => c.AgVisita.Contains(agcomercialinput)).ToList();

            }

            if (localidadeinput != null)
            {

                visitas = visitas.Where(c => c.Localidade.Contains(localidadeinput)).ToList();

            }

            if (bairro != null)
            {

                visitas = visitas.Where(c => c.Bairro.Contains(bairro)).ToList();

            }

            if (logradouroinput != null)
            {

                visitas = visitas.Where(c => c.Logradouro.Contains(logradouroinput)).ToList();

            }

            if (Numero1 != 0 && Numero2 != 0)
            {

                visitas = visitas.Where(c => c.Num >= Numero1 && c.Num <= Numero2).OrderBy(c => c.Num).ToList();

            }



            if (diavisita1 != null && diavisita2 != null)
            {

                DateTime dt = Convert.ToDateTime(diavisita1);
                DateTime dt2 = Convert.ToDateTime(diavisita2);

                visitas = visitas.Where(w => w.DataHora >= dt && w.DataHora <= dt2

                ).ToList();
            }


            // cast IEnumerable em List
            List<CondVisita> vs = visitas.ToList();


            byte[] fileBytes = _ExcelExportvisitaEndereco.ExportaPadraoNovatecCondVisita(vs);

            return File(fileBytes, "application/x-msdownload", _ExcelExportvisitaEndereco.FileName);
        }

    }
}
