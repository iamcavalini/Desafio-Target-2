Sistema de Gestão de Estoque

Este projeto consiste numa aplicação de consola interativa desenvolvida em C# (.NET 8.0) que lê o inventário inicial a partir de um ficheiro JSON, permitindo realizar movimentações de entrada e saída de mercadorias no depósito com controlo de identificadores únicos e validação de estoque em tempo real.

---

Regras de Negócio e Funcionalidades

- Identificador Único: Cada movimentação de estoque gera automaticamente um ID sequencial exclusivo.
- Tipos de Movimentação: Registro de operações de Entrada (adição ao estoque) e Saída (baixa de mercadoria).
- Validação de Estoque: O sistema impede saídas caso a quantidade solicitada seja superior ao estoque disponível no depósito.
- Atualização Dinâmica: Retorna e exibe a quantidade final atualizada do produto imediatamente após cada lançamento bem-sucedido.

---

Tecnologias Utilizadas

- Linguagem: C# (.NET 8.0)
- Biblioteca de Serialização: `System.Text.Json`
- Ambiente de Desenvolvimento: Visual Studio 2022

---

Como Executar o Projeto

Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado na máquina.
- IDE (Visual Studio).

Passos
1. Clonar o repositório
2. Acessar a pasta do projeto
3. Executar a aplicação via terminal
