# Desafio Técnico Korp - Sistema de Faturamento e Estoque

Este projeto é uma solução em arquitetura de microsserviços desenvolvida para gerenciar o faturamento de notas fiscais e o controle de estoque de produtos.


---

## Stack

### Backend
* **Linguagem:** C# (.NET Core)
* **Arquitetura:** Clean Architecture / Microsserviços
* **Banco de Dados:** SQL Server
* **ORM:** Entity Framework Core
* **Resiliência:** Polly (Retry Pattern)
* **Padrões de Projeto:** Result Pattern (tratamento de erros sem Exceptions de fluxo), Repository Pattern.
* **Controle de Concorrência:** Optimistic Concurrency (RowVersion).

### Frontend
* **Framework:** Angular 21 (Standalone Components)
* **Estilização:** SCSS (CSS Grid e Flexbox)
* **Reatividade:** RxJS (Observables, HttpClient, finalize)

### DevOps & Infraestrutura
* **Containers:** Docker & Docker Compose
* **Web Server (Frontend):** Nginx (na imagem Docker do Angular)

---

##  Como executar o projeto

### Pré-requisitos
* [Docker](https://www.docker.com/get-started) instalado e rodando.
* [Docker Compose](https://docs.docker.com/compose/install/) instalado.

### Passo a Passo

1. **Clone o repositório e acesse a pasta raiz do projeto:**
   ```bash
   git clone <https://github.com/Zoommod/Korp_Teste_Vitor.git>
   cd Korp_Teste_Vitor
2. **Inicie os containers com Docker Compose:**
    * Na raiz do projeto (onde está o arquivo docker-compose.yml), execute o comando abaixo. O Docker fará o build do Frontend (Angular), dos Backends (.NET) e do Banco de Dados (SQL Server).
    ```bash
    docker-compose up -d --build
3. **Acesse a aplicação no navegador:**
    * Frontend (Painel de Controle): http://localhost:4200