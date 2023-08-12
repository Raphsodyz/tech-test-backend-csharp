using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Domain.Entidades
{
    [Table("PRODUTO")]
    [XmlRoot(ElementName = "Produto")]
    public class Produto
    {
        [Key]
        [BsonId]
        [Column("ID")]
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Nome")]
        [Column("NOME")]
        public string Nome { get; set; }

        [XmlAttribute("Preco")]
        [Column("PRECO")]
        public decimal Preco { get; set; }

        [XmlAttribute("Quantidade")]
        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [XmlAttribute("DataCriacao")]
        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        [XmlAttribute("IdCompartilhado")]
        [Column("ID_COMPARTILHADO")]
        public int IdCompartilhado { get; set; }
    }
}
