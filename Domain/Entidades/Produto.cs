using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidades
{
    [Table("PRODUTO")]
    public class Produto
    {
        [Key]
        [BsonId]
        [Column("ID")]
        public int Id { get; set; }

        [Column("NOME")]
        public string Nome { get; set; }

        [Column("PRECO")]
        public decimal Preco { get; set; }

        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }
    }
}
