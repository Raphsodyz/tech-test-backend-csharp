# Soluções adotadas

O projeto foi criado na forma de uma WebApi para consumo dos 3 repositórios de bancos de dados conforme solicitado. Utilizei o pattern de repository e de injeção de dependência para estruturação do código, divisão das responsabilidades, facilidade de leitura e manutenção. Foram incluídas 3 camadas bases no projeto:

- Data: Responsável por realizar o acesso ao banco de dados, contendo os 3 context respectivos de cada banco com as operações básicas necessárias para o CRUD.
- Domain: Responsável pelas classes utilizadas como: entidades, enumerators, constantes e DTOs.
- Business: Camada de negócio responsável pelo gerenciamento e consumo dedos e estruturas da Data e Domain. Nela contem as regras de negócio solicitadas para integridade, consistência e persistência dos dados.
- Services: Camada de exposição dos dados, é acessada via protocolos HTTP na API rest, expondo e consumindo os dados em formato JSON. As URLs dos endpoints foram feitas conforme solicitado.

Existem alguns métodos auxiliares também como o de sincronização que também disponibilizei na API para persistência dos dados, caso alguma database tenha um dados que a outra não tem.
Utilizei em formato de texto local um arquivo em .xml para armazenamento dos dados. Foi escolhido por conta de ser fácil legibilidade e também de fácil serialização para a máquina.

Para a validação dos dados, foram incluídas Data Annotations nas DTOs para verificar a regra de preços e também de campos obrigatórios, tamanho de strings e range dos inteiros. O campo calculado foi incluído da DTO produtosDetalheDTO, a qual quando é feito o mapeamento da entidade Produto para a produtosDetalheDTO é incluído este campo.

# Execução do código

## Requisitos:

- .net core 7.0 (SDK e Runtime);
- MySQL 8.0;
- Visual Studio(Ou visual code, tutorial baseado no visual studio);
- MongoDB 6.0.8 (Necessário instalação do CLI para execução via terminal mongod && mongosh);
- Tutorial escrito para Windows. Pode ser feito também em outros dos principais SO's tendo os devidos requisitos acima cumpridos.

### Configurando os bancos de dados:

Com o código acima em mãos, entre em seu Mysql, crie um banco de dados com o nome que desejar para que a aplicação possa consumir.
Com o código em mãos, abra o arquivo CadastroProAuto.sln pelo visual studio na opção 'Abrir um projeto ou solução'. Dentro projeto, pela direita no 'Gerenciador de Soluções' abra a pasta 'Services' e clique com botão direito em cima do arquivo 'Services' e em 'adicionar/Novo item' crie um arquivo chamado `appsettings.json`; esse arquivo será utilizado para as interações da aplicação com o seu banco de dados/etc. Dentro do appsettings.json(apague se vier algo escrito), crie uma seção chamada: 

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;"
  }
}
```

Os campos dentro da opção 'default' a serem preenchidos são:

- Server: O servidor do seu banco de dados ex: localhost, 197.168.0.1 e etc.
- Database: O nome do banco de dados a ser utilizado.
- User id: Seu usuário do Mysql.
- Password: A senha do seu usuário Mysql.

Para o MongoDB utilizei diretamente sua CLI via prompt de comando para cricação dos dados. O tutorial irá seguir dessa forma.
Com o MongoDB installado e seu CLI funcionando, abra o prompt de comando do windows e digite `mongosh`(Talvez seja necessário digitar antes `mongod` para iniciar o servidor). Precisamos da coleção de produtos para a aplicação consumir, para isso digite no CLI:

```script
db.createCollection("Produto")
```

Para confirmar se a coleção Produto foi criada digite:

```script
show collections
```

Dentro do arquivo `appsettings.json` criado anteriormente, adicione uma sessão dentro de 'ConnectionStrings' chamada `mongodb` ficando assim:

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;",
   "mongodb":""
  }
}
```

Na seção do mongodb criada informe o seu servidor para acesso. Geralmente o servidor de acesso é na porta 27017, ficando assim:

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;",
  "mongodb":"mongodb://localhost:27017"
  }
}
```

Adicione mais uma sessão chamada `Database` que vai conter dentro a referência dos Produtos da coleção `Produto` que criamos acima:

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;",
  "mongodb":"mongodb://localhost:27017"
  },
 "Database": {
 "Produtos":"Produto"
 }
}
```

Crie agora uma nova sessão no arquivo `appsettings.json` chamada `XmlPath` e dentro dessa sessão mais duas, chamadas de `RootPath` e `DataPath` ficando assim:

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;",
  "mongodb":"mongodb://localhost:27017"
  },
"Database": {
 "Produtos":"Produto"
 },
 "XmlPath": {
   "RootPath":"",
   "DataPath":""
 }
}
```

Dentro do `RootPath` vai ficar o nome da pasta onde está a raiz do projeto, em meu caso ficou o nome da pasta 'tech-test-backend-csharp-main'. E dentro do `DataPath`, vamos adicionar este caminho: 'Data/Repositories/XmlTexto/Data/produtos.xml'; Ficando assim:

```json
{
"ConnectionStrings":{
  "default":"server= ; database= ; user id= ; password= ;",
  "mongodb":"mongodb://localhost:27017"
  },
"Database": {
 "Produtos":"Produto"
 },
 "XmlPath": {
   "RootPath":"tech-test-backend-csharp-main",
   "DataPath":"Data/Repositories/XmlTexto/Data/produtos.xml"
 }
}
```

Agora voltando ao Visual Studio, na parte superior clique na barra de pesquisa e digite 'Console do Gerenciador de Pacotes'. Dentro do terminal do gerenciador de pacotes aberto em baixo, clique na opção 'Projeto padrão' e selecione a opção 'Data\Data'. No shell de execução do gerenciador de pacotes, faça um update do seu banco, digitando o comando'Update-Database' para adição das tabelas no banco de dados do Mysql.

A aplicação então está pronta para iniciar. Lembre-se de configurar no visual studio para rodar a aplicação pelo IISExpress. A aplicação também conta com uma interface para teste da OpenApi(Swagger) para execução dos métodos do CRUD. Dentro do projeto Test contem os arquivos de teste unitário.

Obs.: O projeto se inicia pela Services, sendo as demais camada de suporte para o sistema, como a repository, businesse e etc. Caso não esteja configurado, clique com o botão direito na solução 'FutureSpace' e selecione 'Propriedades'. Clique na opção 'Vários projetos de inicialização' e com a setinha da janela, mova o projeto 'Services' para a última posição, após clique na opção 'Nenhum' e selecione a opção 'Iniciar'. Clique em 'Aplicar e 'Ok' após essas alterações.
