using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestaoEstoque
{
    public class Produto
    {
        [JsonPropertyName("codigoProduto")]
        public int CodigoProduto { get; set; }

        [JsonPropertyName("descricaoProduto")]
        public string DescricaoProduto { get; set; } = string.Empty;

        [JsonPropertyName("estoque")]
        public int QuantidadeEstoque { get; set; }
    }

    public class DadosEstoque
    {
        [JsonPropertyName("estoque")]
        public List<Produto> Produtos { get; set; } = new();
    }

    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2
    }

    public class Movimentacao
    {
        public int Id { get; set; }
        public int CodigoProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int EstoqueFinal { get; set; }
        public DateTime DataHora { get; set; }
    }

    public class Deposito
    {
        private readonly Dictionary<int, Produto> _produtos;
        private int _proximoIdMovimentacao = 1;

        public Deposito(string jsonEstoque)
        {
            var dados = JsonSerializer.Deserialize<DadosEstoque>(jsonEstoque);
            _produtos = dados?.Produtos.ToDictionary(p => p.CodigoProduto)
                        ?? new Dictionary<int, Produto>();
        }

        public void ExibirProdutos()
        {
            Console.WriteLine("\n--- PRODUTOS EM ESTOQUE ---");
            Console.WriteLine("Cód. | Descrição                   | Qtd. Atual");
            Console.WriteLine("-----------------------------------------------");
            foreach (var p in _produtos.Values)
            {
                Console.WriteLine($"{p.CodigoProduto,-4} | {p.DescricaoProduto,-27} | {p.QuantidadeEstoque}");
            }
            Console.WriteLine("-----------------------------------------------\n");
        }

        public int LancarMovimentacao(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
        {
            if (!_produtos.TryGetValue(codigoProduto, out var produto))
            {
                throw new ArgumentException($"Produto com código {codigoProduto} não foi encontrado.");
            }

            if (quantidade <= 0)
            {
                throw new ArgumentException("A quantidade movimentada deve ser maior que zero.");
            }

            if (tipo == TipoMovimentacao.Entrada)
            {
                produto.QuantidadeEstoque += quantidade;
            }
            else if (tipo == TipoMovimentacao.Saida)
            {
                if (produto.QuantidadeEstoque < quantidade)
                {
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para {produto.DescricaoProduto}. " +
                        $"Disponível: {produto.QuantidadeEstoque}, Solicitado: {quantidade}");
                }
                produto.QuantidadeEstoque -= quantidade;
            }

            var movimento = new Movimentacao
            {
                Id = _proximoIdMovimentacao++,
                CodigoProduto = codigoProduto,
                NomeProduto = produto.DescricaoProduto,
                Tipo = tipo,
                Quantidade = quantidade,
                Descricao = descricao,
                EstoqueFinal = produto.QuantidadeEstoque,
                DataHora = DateTime.Now
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCESSO] Movimentação #{movimento.Id} realizada!");
            Console.WriteLine($"Produto: {movimento.NomeProduto} (Cód: {movimento.CodigoProduto})");
            Console.WriteLine($"Tipo: {movimento.Tipo} | Quantidade: {movimento.Quantidade}");
            Console.WriteLine($"Descrição: {movimento.Descricao}");
            Console.WriteLine($">>> Qtd. Final do Estoque: {movimento.EstoqueFinal} unidades <<<\n");
            Console.ResetColor();

            return produto.QuantidadeEstoque;
        }
    }

    public class Program
    {
        public static void Main()
        {
            string jsonEstoque = @"
            {
              ""estoque"": [
                { ""codigoProduto"": 101, ""descricaoProduto"": ""Caneta Azul"", ""estoque"": 150 },
                { ""codigoProduto"": 102, ""descricaoProduto"": ""Caderno Universitário"", ""estoque"": 75 },
                { ""codigoProduto"": 103, ""descricaoProduto"": ""Borracha Branca"", ""estoque"": 200 },
                { ""codigoProduto"": 104, ""descricaoProduto"": ""Lápis Preto HB"", ""estoque"": 320 },
                { ""codigoProduto"": 105, ""descricaoProduto"": ""Marcador de Texto Amarelo"", ""estoque"": 90 }
              ]
            }";

            Deposito deposito = new Deposito(jsonEstoque);
            bool executando = true;

            while (executando)
            {
                Console.WriteLine("=====================================");
                Console.WriteLine("  SISTEMA DE MOVIMENTAÇÃO DE ESTOQUE ");
                Console.WriteLine("=====================================");
                Console.WriteLine("1 - Lançar Movimentação");
                Console.WriteLine("2 - Visualizar Estoque");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine() ?? "";

                switch (opcao)
                {
                    case "1":
                        LancarMovimentacaoInterativa(deposito);
                        break;
                    case "2":
                        deposito.ExibirProdutos();
                        break;
                    case "0":
                        executando = false;
                        Console.WriteLine("\nSaindo do sistema...");
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida! Tente novamente.\n");
                        break;
                }
            }
        }

        private static void LancarMovimentacaoInterativa(Deposito deposito)
        {
            try
            {
                deposito.ExibirProdutos();

                Console.Write("Digite o código do produto: ");
                if (!int.TryParse(Console.ReadLine(), out int codigo))
                {
                    Console.WriteLine("Código inválido!");
                    return;
                }

                Console.WriteLine("Selecione o tipo de movimentação:");
                Console.WriteLine("1 - Entrada");
                Console.WriteLine("2 - Saída");
                Console.Write("Opção: ");
                if (!int.TryParse(Console.ReadLine(), out int tipoInt) || (tipoInt != 1 && tipoInt != 2))
                {
                    Console.WriteLine("Tipo de movimentação inválido!");
                    return;
                }
                TipoMovimentacao tipo = (TipoMovimentacao)tipoInt;

                Console.Write("Digite a quantidade: ");
                if (!int.TryParse(Console.ReadLine(), out int quantidade))
                {
                    Console.WriteLine("Quantidade inválida!");
                    return;
                }

                Console.Write("Digite a descrição/motivo da movimentação: ");
                string descricao = Console.ReadLine() ?? "";

                deposito.LancarMovimentacao(codigo, tipo, quantidade, descricao);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERRO] {ex.Message}\n");
                Console.ResetColor();
            }
        }
    }
}