# Boas-vindas ao repositório do projeto `Up Bank API`!

🌱 Neste projeto temos a base para qualquer _banco_ do tipo digital. Dentro da aplicação temos os módulos/micro serviços de cliente, funcionário, agência e conta, cada um com suas funções, banco de dados e CRUDs diferentes.

# 🚀​ Módulos e seus respectivos desenvolvedores:

## Clientes: @DCicuto - Daiane Cicuto 👩🏻‍💻​

<details>
<summary>💼​ <strong>Funções</strong></summary><br />
<li>Gerenciar os clientes através de um CRUD básico, além de funções extras como solicitar abertura de conta.</li>
<li>Ao criar um cliente, implicitamente ele solicita uma abertura de conta.</li>
    
</details>

<details>
<summary>🧰 <strong>Ferramentas e linguagens</strong></summary><br />
<li> <i>.Net Framework</i>;</li>
<li> <i>Entity Framework</i>;</li>
<li> <i>C#</i>;</li>
<li> <i>SQLServer</i>;</li>
    
</details>

<details>
<summary>🚀 <strong>Como rodar a API</strong></summary>
<br>

1. **Abra o terminal na pasta raiz do projeto 📁​UpBank.ClientAPI e use o comando:**

```bash
dotnet watch run
```

2. **Ou então selecione o projeto na inicialização do VisualStudio**

Neste projeto foi retirado a utilização do _Swagger_. Para testar os endpoints, foi utilizado o _Postman_.
</details>

<details>
<summary>🔗 <strong>Endpoints</strong></summary>
<br>
<li><strong>GetAll</strong> - <i>https://localhost:7142/api/Clients/</i></li>
<br>
<li><strong>GetByCPF</strong> - <i>https://localhost:7142/api/Clients/<strong>CPF</strong></i></li>
<br>
<li><strong>Post</strong> - <i>https://localhost:7142/api/Clients/</i></li>
<strong>Corpo / Body</strong>
  
```json
{
  "Name": "Cliente 4",
  "CPF": "09223989094",
  "BirthDt": "1985-04-23T00:00:00",
  "Sex": "M",
  "Address": {
    "ZipCode": "69009270",
    "Complement": "Casa",
    "Number": 430
  },
  "Salary": 1000.00,
  "Phone": "(11) 98765-4321",
  "Email": "cliente4@example.com"
}

```
<br>
<li><strong>Put</strong> - <i>https://localhost:7142/api/Clients/</i></li>
<strong>Corpo / Body</strong>

```json
{
  "CPF": "123.456.789-00",
  "Salary": 5000.50,
  "Phone": "+55 11 91234-5678",
  "Email": "example@example.com",
  "Restriction": true
}
```

<br>
<li><strong>Delete</strong> - <i>https://localhost:7142/api/Clients/<strong>CPF</strong></i></li>
<strong>Obs:</strong> o Delete apenas move para outra tabela no banco de dados e altera o status do cliente para <i>Restrito</i>.
    
</details>


## Funcionário: @masfazan - Maiara Soarde Fazan 👩🏻‍💻​

<details>
<summary>💼​ <strong>Funções</strong></summary><br />
<li>Gerenciar os funcionários através de um CRUD básico, além de funções extras como definir o perfil da conta e aprovar contas.</li>
</details>

<details>
<summary>🧰 <strong>Ferramentas e linguagens</strong></summary><br />
<li> <i>.Net Framework</i>;</li>
<li> <i>Dapper</i>;</li>
<li> <i>C#</i>;</li>
<li> <i>SQLServer</i>;</li>
</details>

<details>
<summary>🚀 <strong>Como rodar a API</strong></summary>
<br>

1. **Abra o terminal na pasta raiz do projeto 📁​UpBank.EmployeeAPI e use o comando:**

```bash
dotnet watch run
```

2. **Ou então selecione o projeto na inicialização do VisualStudio**

Neste projeto foi retirado a utilização do _Swagger_. Para testar os endpoints, foi utilizado o _Postman_.
</details>

<details>
<summary>🔗 <strong>Endpoints</strong></summary>
<br>
<li><strong>GetAll</strong> - <i>https://localhost:7042/api/Employees/</i></li>
<br>
<li><strong>GetByRegister</strong> - <i>https://localhost:7042/api/Employees/<strong>REGISTRO</strong></i></li>
<br>
<li><strong>Post - CreateEmployee</strong> - <i>https://localhost:7042/api/Employees/</i></li>
<strong>Corpo / Body</strong>
  
```json
{
  "Name": "Funcionario",
  "CPF": "31143524055",
  "BirthDt": "2010-05-15T00:00:00",
  "Sex": "M",
  "Address": {
    "ZipCode": "69900439",
    "Complement": "Apt 101",
    "Number": 123
  },
  "Salary": 1,
  "Phone": "(11) 98765-4321",
  "Email": "funcionario@example.com",
  "Manager": true,
  "Registry": 3
}
```
<br>
<li><strong>Post - CreateAccount</strong> - <i>https://localhost:7042/api/Employees/CreateAccount</i></li>
<strong>Corpo / Body</strong>

```json
{
    "EmployeeRegister": 1,
    "AccountNumber": "3",
    "AgencyNumber": "4415",
    "ClientCPF": [
        "29175166070"
    ]
}
```
<br>

<li><strong>Patch - ApproveAccount</strong> - <i>https://localhost:7042/api/Employees/ApproveAccount/{registry}/{number}</i></li>
<br>

<li><strong>Patch - UpdateEmployee</strong> - <i>https://localhost:7042/api/Employees/</i></li>
<br>
<strong>Corpo / Body</strong>

```json
{
  "Registry": 3,
  "Name": "Funcionario",
  "Sex": "M",
  "Salary": 2,
  "Phone": "+55 16 98765-4321",
  "Email": "funcionario@example.com",
  "Manager": false
}
```
<br>

<li><strong>Delete</strong> - <i>https://localhost:7042/api/Employees/<strong>REGISTRO</strong></i></li>
<strong>Obs:</strong> o Delete apenas move para outra tabela no banco de dados e altera o status do funcionário para <i>Restrito</i>.
</details>
<br>

## Agência: @gutvono - Gustavo Vono 👨🏻‍💻​

<details>
<summary>💼​ <strong>Funções</strong></summary><br />
<li>Gerenciar as agências através de um CRUD básico, além de funções extras como listar contras restritas, listar contas por perfil e listar contas com empréstimo ativo.</li>
</details>

<details>
<summary>🧰 <strong>Ferramentas e linguagens</strong></summary><br />
<li> <i>.Net Framework</i>;</li>
<li> <i>Entity Framework</i>;</li>
<li> <i>C#</i>;</li>
<li> <i>SQLServer</i>;</li>
</details>

<details>
<summary>🚀 <strong>Como rodar a API</strong></summary>
<br>

1. **Abra o terminal na pasta raiz do projeto 📁​UpBank.AgencyAPI e use o comando:**

```bash
dotnet watch run
```

2. **Ou então selecione o projeto na inicialização do VisualStudio**

Neste projeto foi retirado a utilização do _Swagger_. Para testar os endpoints, foi utilizado o _Postman_.
</details>

<details>
<summary>🔗 <strong>Endpoints</strong></summary>
<br>
<li><strong>GetAll</strong> - <i>https://localhost:7217/api/Agencies</i></li>
<br>
<li><strong>GetByNumber</strong> - <i>https://localhost:7217/api/Agencies/<strong>NUMERO DA AGÊNCIA</strong></i></li>
<br>
<li><strong>GetRestrictedAccountsByAgency</strong> - <i>https://localhost:7217/api/Agencies/restrict/<strong>NUMERO DA AGÊNCIA</strong></i></li>
<br>
<li><strong>GetAccountsByProfile</strong> - <i>https://localhost:7217/api/Agencies/profile/<strong>PERFIL DA CONTA</strong>/<strong>NUMERO DA AGÊNCIA</strong></i></li>
<br>
<li><strong>GetLendingAccounts</strong> - <i>https://localhost:7217/api/Agencies/lending</i></li>
<br>
<li><strong>Post</strong> - <i>https://localhost:7217/api/Agencies</i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "AddressId": "667c5cbc7903bc4770cfefb3",
    "CNPJ": "70.643.101/0001-66",
    "Employees": [numero de registro]
}
```
<br>
<li><strong>Put</strong> - <i>https://localhost:7217/api/Agencies/<strong>NUMERO DA GENCIA</strong></i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "AddressId": "667c5cbc7903bc4770cfefb3",
    "CNPJ": "70.643.101/0001-66",
    "Employees": [numero de registro]
}
```
<br>
<li><strong>Delete</strong> - <i>https://localhost:7217/api/Agencies/<strong>NUMERO DA GENCIA</strong></i></li>
<strong>Obs:</strong> o Delete apenas move para outra tabela no banco de dados e altera o status do funcionário e da agência para <i>Restrito</i>.
</details>
<br>

## Conta: @edenilsonjunior - Edenilson Garcia e @gabsoares - Gabriel Visicatto Soares 👨🏻‍💻​

<details>
<summary>💼​ <strong>Funções</strong></summary><br />
<li>Gerenciar as contas através de um CRUD básico, além de funções extras como efetuar transação, gerar extrato, consultar saldo e consultar transação por tipo.</li>
</details>

<details>
<summary>🧰 <strong>Ferramentas e linguagens</strong></summary><br />
<li> <i>.Net Framework</i>;</li>
<li> <i>Dapper</i>;</li>
<li> <i>C#</i>;</li>
<li> <i>SQLServer</i>;</li>
</details>

<details>
<summary>🚀 <strong>Como rodar a API</strong></summary>
<br>

1. **Abra o terminal na pasta raiz do projeto 📁​UpBank.AgencyAPI e use o comando:**

```bash
dotnet watch run
```

2. **Ou então selecione o projeto na inicialização do VisualStudio**

Neste projeto foi retirado a utilização do _Swagger_. Para testar os endpoints, foi utilizado o _Postman_.
</details>

<details>
<summary>🔗 <strong>Endpoints</strong></summary>
<br>
<li><strong>GetAll</strong> - <i>https://localhost:7011/api/Accounts/</i></li>
<br>
<li><strong>GetAccountByNumber</strong> - <i>https://localhost:7011/api/Accounts/1<strong>NUMERO DA CONTA</strong></i></li>
<br>
<li><strong>GetTransactionByType</strong> - <i>https://localhost:7011/api/Accounts/TransactionType/<strong>TIPO</strong></i></li>
<br>
<li><strong>GetBankStatement</strong> - <i>https://localhost:7011/api/Accounts/GetBankStatement/<strong>NUMERO DA CONTA</strong></i></li>
<br>
<li><strong>GetBalance</strong> - <i>https://localhost:7011/api/Accounts/GetBalance/<strong>NUMERO DA CONTA</strong></i></li>
<br>
<li><strong>GetAllTransactions</strong> - <i>https://localhost:7011/api/Transactions/</i></li>
<br>
<li><strong>GetTransactionById</strong> - <i>https://localhost:7011/api/Transactions/Id/<strong>ID DA TRANSAÇÃO</strong></i></li>
<br>
<li><strong>GetTransactionById</strong> - <i>https://localhost:7011/api/Transactions/Id/<strong>ID DA TRANSAÇÃO</strong></i></li>
<br>
<li><strong>GetTransactionByType</strong> - <i>https://localhost:7011/api/Transactions/Type/<strong>TIPO DA TRANSAÇÃO</strong></i></li>
<br>
<li><strong>Post - CreateAccount</strong> - <i>https://localhost:7011/api/Accounts/</i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "AccountNumber": "2",
    "AgencyNumber": "002",
    "AccountProfile": "Normal",
    "ClientCPF": [
        "22233344455"
    ],
    "Overdraft": 500,
    "CreditCardLimit": 5000.00,
    "CreditCardHolder": "John Doe"
}
```
<br>
<li><strong>Post - MakeTransaction</strong> - <i>https://localhost:7011/api/Accounts/MakeTransaction/</i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "Id": 1,
    "AccountNumber": "1",
    "TransactionDt": "2024-06-25T14:30:00",
    "TransactionType": "Lending",
    "ReceiverAccount": null,
    "TransactionValue": 17000
}
```
<br>
<li><strong>Post - MakeTransaction</strong> - <i>https://localhost:7011/api/Transactions</i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "Id": 1,
    "AccountNumber": "1",
    "TransactionDt": "2024-06-25T14:30:00",
    "TransactionType": "Payment",
    "ReceiverAccount": "2",
    "TransactionValue": 0.12
}
```
<br>
<li><strong>Patch - UpdateAccount</strong> - <i>https://localhost:7011/api/Transactions</i></li>
<strong>Corpo / Body</strong>
  
```json
{
    "Id": 1,
    "AccountNumber": "1",
    "TransactionDt": "2024-06-25T14:30:00",
    "TransactionType": "Payment",
    "ReceiverAccount": "2",
    "TransactionValue": 0.12
}
```
<br>
<li><strong>Delete</strong> - <i>https://localhost:7011/api/Accounts/</i></li>
<strong>Obs:</strong> o Delete apenas move para outra tabela no banco de dados e altera o status da conta para <i>Restrito</i>.
</details>
<br>

## Clientes: @edenilsonjunior - Edenilson Garcia 👨🏻‍💻​

<details>
<summary>💼​ <strong>Funções</strong></summary><br />
<li>Gerenciar os endereços através de um Put e um GetByID.</li>
    
</details>

<details>
<summary>🧰 <strong>Ferramentas e linguagens</strong></summary><br />
<li> <i>.Net Framework</i>;</li>
<li> <i>C#</i>;</li>
<li> <i>MongoDB</i>;</li>

</details>

<details>
<summary>🚀 <strong>Como rodar a API</strong></summary>
<br>

1. **Abra o terminal na pasta raiz do projeto 📁​UpBank.AddressAPI e use o comando:**

```bash
dotnet watch run
```

2. **Ou então selecione o projeto na inicialização do VisualStudio**

Neste projeto foi retirado a utilização do _Swagger_. Para testar os endpoints, foi utilizado o _Postman_.
</details>

<details>
<summary>🔗 <strong>Endpoints</strong></summary>
<br>
<li><strong>GetById</strong> - <i>https://localhost:7084/api/Addresses/<strong>ID</strong></i></li>
<br>
<li><strong>Post - CreateAddress</strong> - <i>https://localhost:7084/api/Addresses</i></li>
<strong>Corpo / Body</strong>
  
```json
{
  "ZipCode": "69900439",
  "Complement": "Apt 101",
  "Number": 123
}
```
</details>

# Observações
Como sempre, um projeto nunca termina de fato. Sempre haverão **melhorias** para serem aplicadas.

<details>
<summary><strong>Futuro potencial/melhorias</strong></summary>
<li>Melhorias estruturais seguindo alguns design de software como SOLID, POO, DDD, etc;</li>
<li>Testes unitários;</li>
<li>Completar este README.md com as validações de dados encontradas em cada API que garantem a persistência dos dados;</li>
<li>Completar este README.md com os testes unitários assim que estiverem prontos.</li>
</details>
