using Domain.Entidades;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Data.Repositories.XmlTexto.Context
{
    public class MobilusXmlContext
    {
        private readonly XmlSerializer _serializer;
        private readonly string _path;

        public MobilusXmlContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _path = Path.GetDirectoryName(Directory.GetCurrentDirectory() + configuration.GetSection("XmlPath:DataPath").Value);
            _serializer = new XmlSerializer(typeof(List<Produto>));

            if (!File.Exists(_path))
            {
                using var stream = File.Create(_path);
            }
        }

        public string DiretorioXml()
        {
            return _path;
        }

        public XmlSerializer Serializador()
        {
            return _serializer;
        }

        public XmlWriterSettings ConfigsEscrever()
        {
            XmlWriterSettings configs = new()
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            return configs;
        }
    }
}
