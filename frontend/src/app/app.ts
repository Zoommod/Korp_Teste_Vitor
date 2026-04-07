import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, delay } from 'rxjs';

import { EstoqueService } from './core/services/estoque.service';
import { FaturamentoService } from './core/services/faturamento.service';
import {
  AtualizarProdutoDto,
  CriarProdutoDto,
  Produto,
} from './core/models/produto.model';
import { ItemNotaFiscal, NotaFiscal } from './core/models/nota-fiscal.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly estoqueService = inject(EstoqueService);
  private readonly faturamentoService = inject(FaturamentoService);

  produtos: Produto[] = [];
  itensNotaTemporaria: ItemNotaFiscal[] = [];

  isImprimindo = false;

  toastMensagem = '';
  toastErro = false;
  private toastTimeoutId?: ReturnType<typeof setTimeout>;

  codigoNovo = '';
  descricaoNova = '';
  saldoInicialNovo = 0;

  produtoSaldoId = '';
  quantidadeSaldo = 0;

  codigoItem = '';
  quantidadeItem = 1;

  idNotaParaImprimir = '';

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(exibirMensagem: boolean = false): void {
    this.estoqueService.listarProdutos().subscribe({
      next: (produtos) => {
        this.produtos = produtos;
        if (!this.produtoSaldoId && produtos.length > 0) {
          this.produtoSaldoId = produtos[0].id;
        }
        if (!this.codigoItem && produtos.length > 0) {
          this.codigoItem = produtos[0].codigo;
        }
        
        if (exibirMensagem) {
          this.mostrarToast('Produtos carregados.');
        }
      },
      error: () => {
        this.mostrarToast('Erro ao carregar produtos.', true);
      },
    });
  }

  criarProduto(): void {
    const codigo = this.codigoNovo.trim();
    const descricao = this.descricaoNova.trim();
    const saldoInicial = Number(this.saldoInicialNovo);

    if (!codigo) {
      this.mostrarToast('Codigo obrigatorio para criar produto.', true);
      return;
    }

    if (!descricao) {
      this.mostrarToast('Descricao obrigatoria para criar produto.', true);
      return;
    }

    if (!Number.isFinite(saldoInicial) || saldoInicial < 0) {
      this.mostrarToast('Saldo inicial invalido.', true);
      return;
    }

    const dto: CriarProdutoDto = { codigo, descricao, saldoInicial };

    this.estoqueService.criarProduto(dto).subscribe({
      next: (produto) => {
        this.mostrarToast(`Produto criado: ${produto.codigo}.`);
        this.codigoNovo = '';
        this.descricaoNova = '';
        this.saldoInicialNovo = 0;
        this.carregarProdutos();
      },
      error: (err) => {
        let mensagemErro = 'Erro ao criar produto.';

        if (err.error) {
          if (Array.isArray(err.error) && err.error.length > 0) {
            // Extrai a mensagem se vier como Array
            mensagemErro = err.error[0].mensagem || mensagemErro;
          } else {
            // Extrai a mensagem se vier como Objeto em outras rotas
            mensagemErro = err.error.mensagem || err.error.Mensagem || mensagemErro;
          }
        }

        this.mostrarToast(mensagemErro, true);
      }
    });
  }

  adicionarSaldo(): void {
    const quantidadeNormalizada = Number(this.quantidadeSaldo);

    if (!this.produtoSaldoId) {
      this.mostrarToast('Selecione um produto para adicionar saldo.', true);
      return;
    }

    if (!Number.isFinite(quantidadeNormalizada) || quantidadeNormalizada <= 0) {
      this.mostrarToast('Quantidade invalida para adicionar saldo.', true);
      return;
    }

    const produto = this.produtos.find((p) => p.id === this.produtoSaldoId);

    if (!produto) {
      this.mostrarToast('Produto selecionado nao encontrado.', true);
      return;
    }

    const novoSaldo = produto.saldo + quantidadeNormalizada;

    const dto: AtualizarProdutoDto = {
      codigo: produto.codigo,
      descricao: produto.descricao,
      saldo: novoSaldo,
    };

    this.estoqueService.atualizarProduto(produto.id, dto).subscribe({
      next: () => {
        this.mostrarToast(`Saldo atualizado para ${produto.codigo}.`);
        this.carregarProdutos();
        this.quantidadeSaldo = 0;
      },
      error: (err) => {
        let mensagemErro = 'Erro ao adicionar saldo.';

        if (err.error) {
          if (Array.isArray(err.error) && err.error.length > 0) {
            // Extrai a mensagem se vier como Array
            mensagemErro = err.error[0].mensagem || mensagemErro;
          } else {
            // Extrai a mensagem se vier como Objeto em outras rotas
            mensagemErro = err.error.mensagem || err.error.Mensagem || mensagemErro;
          }
        }
        this.mostrarToast(mensagemErro, true);
      }
    });
  }

  adicionarItemNota(codigo: string, quantidade: number): void {
    const codigoNormalizado = codigo.trim();
    const quantidadeNormalizada = Number(quantidade);

    if (!codigoNormalizado) {
      this.mostrarToast('Codigo obrigatorio para item da nota.', true);
      return;
    }

    if (!Number.isFinite(quantidadeNormalizada) || quantidadeNormalizada <= 0) {
      this.mostrarToast('Quantidade invalida para item da nota.', true);
      return;
    }

    this.itensNotaTemporaria = [
      ...this.itensNotaTemporaria,
      { codigoProduto: codigoNormalizado, quantidade: quantidadeNormalizada },
    ];

    this.mostrarToast(`Item adicionado: ${codigoNormalizado} x${quantidadeNormalizada}.`);
    this.quantidadeItem = 1;
  }

  removerItemNota(indice: number): void {
    this.itensNotaTemporaria = this.itensNotaTemporaria.filter((_, i) => i !== indice);
  }

  criarNota(): void {
    if (this.itensNotaTemporaria.length === 0) {
      this.mostrarToast('Adicione itens antes de gerar a nota.', true);
      return;
    }

    this.faturamentoService.criarNota({ itens: this.itensNotaTemporaria }).subscribe({
      next: (idNota: string) => {
        this.itensNotaTemporaria = [];
        this.idNotaParaImprimir = idNota;
        this.mostrarToast(`Nota fiscal gerada com sucesso.`);
      },
      error: (err) => {
        let mensagemErro = 'Erro ao criar nota fiscal.';

        if (err.error) {
          if (Array.isArray(err.error) && err.error.length > 0) {
            mensagemErro = err.error[0].mensagem || mensagemErro;
          } else {
            mensagemErro = err.error.mensagem || err.error.Mensagem || mensagemErro;
          }
        }

        this.mostrarToast(mensagemErro, true);
      }
    });
  }

  imprimirNota(id: string): void {
    const idNormalizado = id.trim();
    if (!idNormalizado) {
      this.mostrarToast('Nenhuma nota criada para imprimir.', true);
      return;
    }

    this.isImprimindo = true;

    this.faturamentoService
      .imprimirNota(idNormalizado)
      .pipe(
        delay(800),
        finalize(() => (this.isImprimindo = false))
      )
      .subscribe({
        next: (nota: NotaFiscal) => {
          this.mostrarToast(`Nota fiscal impressa com sucesso.`);
          this.carregarProdutos();
        },
        error: (err) => {
          let mensagemErro = 'Erro ao imprimir nota.';
          if (err.error) {
            if (Array.isArray(err.error) && err.error.length > 0) {
              mensagemErro = err.error[0].mensagem || mensagemErro;
            } else {
              mensagemErro = err.error.mensagem || err.error.Mensagem || mensagemErro;
            }
          }
          this.mostrarToast(mensagemErro, true);
        },
      });
  }

  mostrarToast(mensagem: string, erro = false): void {
    if (this.toastTimeoutId) {
      clearTimeout(this.toastTimeoutId);
    }

    this.toastMensagem = mensagem;
    this.toastErro = erro;

    this.toastTimeoutId = setTimeout(() => {
      this.toastMensagem = '';
    }, 3500);
  }
}