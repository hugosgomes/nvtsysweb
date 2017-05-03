﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NovatecEnergyWeb.Models.Repository
{
    public class MotivoRejeicaoRepository : IMotivoRejeicao
    {
        private BDNVTContext _context;

        public MotivoRejeicaoRepository(BDNVTContext context)
        {
            _context = context;
        }

        public List<_11MotivosRej> GetMotivosProdutoSv()
        {
            var motivosRejeicao = _context._11MotivosRej.Where(c => c.Produto == 2 && c.Grupo != 4).OrderBy(c =>c.Id).ToList();
            return motivosRejeicao;
        }
    }
}