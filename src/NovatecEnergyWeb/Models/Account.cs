﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovatecEnergyWeb.Validations;
using NovatecEnergyWeb.Validations.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NovatecEnergyWeb.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome do usuário")]
        [AccountLogin]
        [StatusCliente]
        public string Login { get; set; }

        [Required(ErrorMessage = "Digite a senha do usuário")]
        [SenhaValida]
        public string Senha { get; set; }


        public string Tipo { get; set; }
        
      
    }
}
