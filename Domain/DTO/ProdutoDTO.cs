﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Domain.DTO
{
    public class ProdutoDTO
    {
        [Key]
        [Display(Name = "ID")]
        [JsonIgnore]
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        [StringLength(255, ErrorMessage = "O nome enviado no campo {0} está em um tamanho inválido.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        public string Nome { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O valor enviado no campo {0} é inválido.")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        public decimal? Preco { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "O valor enviado no campo {0} é inválido.")]
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        public int? Quantidade { get; set; }

        [Display(Name = "Data de Criação")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [DataType(DataType.DateTime)]
        public DateTime? DataCriacao { get; set; }

        [Display(Name = "ID Compartilhado")]
        [JsonIgnore]
        public Guid IdCompartilhado { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "O valor enviado no campo {0} é inválido.")]
        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal ValorTotal { get; set; }
    }
}
