# Desafio - Autorizador
A partir das regras elucidadas no documento do desafio, foi criado uma aplicação de linha de comando que receberá um conjunto de dados via `stdin` e retornar informações via `stdout`. Esta aplicação foi criada utilizando o **Microsoft .Net** como framework e a linguagem **C#**.


## Arquitetura
Baseando-se em alguns princípios do *Domain Driven Design*, foi criado uma arquitetura em que teremos um biblioteca que conterá todas as regras e lógicas relatadas no desafio, o que nos permite uma maior simplicidade em melhorias ou implementações, o *DDD* conduz em seus princípios uma facilidade de demonstrar as regras existentes e a forma em que elas podem ser executadas, o que acaba acontecendo neste projeto, todas as regras são facilmente entendidas e extensíveis. Com essa separação de regras em uma biblioteca, temos uma flexibilidade muito maior para que estas sejam utilizadas em outras aplicações ou integradas por terceiros. Para que as regras fossem de fácil entendimento e fácil extensão utilizamos o padrão de projetos `Specification`, esta nos permitiu definir todas as regras e através de testes unitários validamos se as situações foram satisfeitas conforme especificação. Para administração da informação, utilizamos o padrão de projetos `Repository`. Pois no permite tratar a informação de forma única e não necessita que o dominio saiba a infraestrutura aplicada, mas saiba as funcionalidades do repositório de uma entidade. Para interagir com as regras especificadas (`Specification Pattern`) e administrar as informações a partir de um repositório (`Repository Pattern`) utilizamos os serviços de domínio.

A solução então contém um projeto executável, que consumirá todas as regras a partir de uma classe que faz um papel orquestração,  onde esta receberá, de forma não acoplada, todos os pontos de comunicação,  sejam serviços de regras de negócios até interpretadores e escritores de informações (`stdin` e `stdout`). Neste mesmo projeto, temos a implementação dos repositórios de informações, pois não necessitamos de uma grande separação de infraestrutura, até pela dimensão que o desafio se propõe no documento. A idéia foi deixar o mais extensível possível a partir das regras e busca de informações, mas, ao mesmo tempo, enxuto o suficiente para um entendimento rápido e simples que a solução transpareça. 
Evidenciamos, também, que a forma de ler os dados e escrever informações podem ser extensíveis, visto que há uma separação de formato da informação na qual está se trabalhando.

## Decisões técnicas
A utilização da linguagem **C#** se dá pela facilidade de uso e aplicabilidade para soluções de tamanhos variados. Também pela afinidade e tempo de experiência com a linguagem que tenho. 
Para que seja possível utilizar a linguagem, é necessário a instalação do framework **Microsoft .Net** e com este temos funcionalidades que permitem: compilar para múltiplas plataformas, ferramentas de desenvolvimento e com uma grande gama de bibliotecas e apoio da comunidade de desenvolvimento para dar suporte para qualquer tipo de solução. 
Já para validar as regras descritas e códigos de forma unitária, foi utilizado o framework **xUnit**, pois ele é compacto e amplamente utilizado dentro da comunidade, por sua facilidade e rapidez de execução. Uma biblioteca que vai de encontro com testes unitários é **Moq**, esta facilita muito os testes onde há necessidade de dados 'mocados' e por isso uso desta nos projetos de testes.

Como mencionado, a solução foi dividida em duas partes pela série de fatores relatados, são eles:

- *authorize*: Aplicação que contém o consumo das regras de dominio.
- *authorizer.domain*: Define todas as regras que devem ser seguidas para solução.

E em meio ao desenvolvimento houveram algumas decisões  baseadas em performance e outras na semântica do negócio.

Uma decisão tomada pela semântica é a de que uma violação de transação dupla pode ocorrer junto a uma violação de alta frequência em pequeno intervalo, pois validei que ambas ocorrem com base no tempo da transação atual e a conta foi validada. Sendo assim, a conta está elegível para realizar a transação corrente, porém a transação corrente pode não estar! Pois a transação corrente pode ser réplica de alguma ocorrida há dois minutos e ela está sendo uma quarta tentativa em um intervalo de dois minutos.

As classes de repositórios, que estão no projeto `authorizer` na pasta `infrastructure\repositories\memory`, utilizam as estruturas de lista duplamente encadeada, lista simples e uma instância única.
A classe `TransactionMemoryRepository` utiliza uma instância do tipo `LinkedList<Transaction>`, pois ela demonstra uma performance muito alta para adição de dados, porém o contrário ocorre para busca de dados, com base no problema disposto na documentação, validei que não precisaria buscar uma informação em repositório com dados históricos com mais de dois minutos a partir da transação que se está se inserindo. E, também, é sabido que todas as transações estão sendo inseridas em ordem cronólogica, o que facilitou o uso da solução de uma segunda lista, sendo está simples `List<Transaction>`, assim, esta conterá somente transações dos últimos dois minutos.


```csharp
///  <inheritdoc  cref="ITransactionRepository" />
public  void  Add(Transaction  transaction)
{
	// This store transactions as history of
	// All transactions of the current account
	Transactions.AddLast(transaction);

	// It's store in second data structure but only store last two minutes
	// transaction, this only is valid if the transactions come in chronological
	// order. For this, all transactions is storing but first thing to do is remove
	// older transaction of two minutes from the transaction time
	LastTwoMinutesTransactions.RemoveAll(t  =>  t.Time  <  transaction.Time.Subtract(TwoMinutesInterval));

	LastTwoMinutesTransactions.Add(transaction);
}
```
Em um teste de carga com 5.000.000 de registros, a utilização desta segunda lista permitiu que o tempo de reposta fosse de até um minuto, enquanto, na forma em que se utilizava a lista de todas as transações, o tempo de resposta aumentava para além de 5 minutos.

E para armazenar uma conta foi utilizada uma propriedade simples, com o último estado da conta e todos os métodos do repositório pertencente a conta, executando algum tipo de processo nesta propriedade.

```csharp
///  <summary>
/// Storing the Account state at memory
///  </summary>
///  <value></value>
public  Account  CurrentAccount  {  get;  private  set; }
  
///  <inheritdoc  cref="IAccountRepository" />
public  void  Add(Account  account)
{
	CurrentAccount  =  account;
}
```

Um ponto importante que identifiquei foi o uso de `struct` para definir o tipo de uma `Transaction` no dominio.

```csharp
public  struct  Transaction
...
	public  Transaction(string  merchant,  uint  amount,  DateTimeOffset  time) =>  (Merchant,  Amount,  Time)  = (merchant,  amount,  time);
```

Isso se deve pela semântica de que para alterar qualquer tipo de informação em uma transação seria necessário criar uma nova, trazendo isso para código, faz muito sentido ser uma *Value Type* e não *Reference Type* como é o caso da classe `Account` (Onde, informações podem ser modificadas mas que devemos manter o estado das demais informações existentes).

O que se diferencia dos modelos existentes no projeto do executável(`authorize`), as classes-modelos por serem uma representação da informação externa, necessitam ser uma referência para uma rápida interpretação e geração da instância. Ao ver a classe `Transaction` vemos que temos atributos e estes são para deserialização e serialização em `Json`. 
```csharp
///  <summary>
/// Transaction json model
///  </summary>
public  class  Transaction
{
	///  <summary>
	/// Property store merchant information
	///  </summary>
	///  <value></value>
	[JsonPropertyName("merchant")]
	public  string  Merchant  {  get;  set;  }
...
}
```

Esta separação do modelo em uma pasta específica, chamada `json`, foi proposital, para casos de uma nova implementação em algum formato desejável. Isso em conjunto do modelo *input/output* de informações, que se encontra na pasta `io/json`, desta forma, a interpretação da informação, em um formato desejável, pode ser feita desde o processo de serialização até a escrita final dos resultados. Este formato só é especificado no momento do uso do processador `AuthorizationProcessor`, esta classe é a que orquestra todo o processo de autorização e devolução da informação para usuário.

## Como usar Autorizador?
Aqui estarão instruções para compilação do projeto para gerar um artefato binário de acordo com o sistema operacional e a utilização deste artefato.

### Requisitos mínimos
Se preferir manualmente executar a compilação e execução da aplicação, utilize:
- Microsoft .Net 5.0 SDK - Este pode ser encontrado neste [link](https://dotnet.microsoft.com/download/dotnet/5.0) (O projeto foi desenvolvido com base na versão 5.0.2).

Caso queira utilizar o docker, utilize:
- Docker - Este pode ser encontrado neste [link](https://www.docker.com/get-started).

### Vamos começar
Considerando que os requisitos mínimos foram instalados, o projeto foi descompactado e você esteja na pasta a raiz do projeto (nesta você terá pastas como `src`, `tests`, `docs` e o arquivo da solução de extensão `.sln`) as instruções abaixo serão válidas.

#### Compilação

A compilação do projeto para uma validação inicial seria feito da seguinte forma:

```bash
dotnet build 
```

Para que seja compilado um binário para execução, deve ser executado o seguinte comando:

 - Caso o sistema seja OSX
```bash
dotnet publish src/authorize/Authorize.csproj -c Release -o ./output-osx -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```
ou (talvez necessite dar permissão de execução `chmod`)
```bash
./publish-osx.sh
```

O comando `./publish-osx.sh` é um atalho para o comando de publicação e pode ser alterado, caso necessite.

- Caso o sistema seja Unix-Based
```bash
dotnet publish src/authorize/Authorize.csproj -c Release -o ./output-linux -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```
ou (talvez necessite dar permissão de execução `chmod`)
```bash
./publish-linux.sh
```

O comando `./publish-linux.sh` é um atalho para o comando de publicação e pode ser alterado, caso necessite.

Caso queira compilar uma imagem docker, utilize o comando abaixo:
```bash
docker build --no-cache -f src/authorize/Dockerfile . -t authorize
```
ou (talvez necessite dar permissão de execução `chmod`)
```bash
./docker-build.sh
```

O comando `./docker-build.sh` é um atalho para o comando de publicação e pode ser alterado, caso necessite.

#### Execução
Após utilizar uma forma de compilação, serão gerados artefatos de acordo com a forma utilizada.

Quando compilado diretamente de uma maquina OSX-Based, teremos um diretório `output-osx`. Este diretório conterá um executável chamado `authorize` e com bibliotecas que são dependências para sua execução.
Já em casos compilados para Unix-Based, teremos uma pasta `output-linux` contendo apenas o executável.

A partir de uma das pastas, pode ser executado um comando para avaliar a aplicação. Coloque o arquivo `operations` dentro desta pasta e execute:
```bash
./authorize < operations
```

Caso esteja querendo executar a partir de uma imagem docker, execute o comando:
```bash
docker run --rm -i authorize < operation
```
O comando acima, irá criar um contêiner temporário para executar a aplicação e esta execução será no modo interativo. Deste forma, podemos visualizar todo o output da execução do contêiner temporário.