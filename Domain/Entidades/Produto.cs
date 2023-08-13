using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Domain.Entidades
{
    [Table("PRODUTO")]
    public class Produto
    {
        [Key]
        [BsonId]
        [Column("ID")]
        [BsonElement("ID")]
        public int Id { get; set; }

        [Column("NOME")]
        [BsonElement("NOME")]
        public string Nome { get; set; }

        [Column("PRECO")]
        [BsonElement("PRECO")]
        public decimal Preco { get; set; }

        [Column("QUANTIDADE")]
        [BsonElement("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Column("DATA_CRIACAO")]
        [BsonElement("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        [Column("ID_COMPARTILHADO")]
        [BsonElement("ID_COMPARTILHADO")]
        public int IdCompartilhado { get; set; }
    }
}
