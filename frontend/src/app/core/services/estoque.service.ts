import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AtualizarProdutoDto, CriarProdutoDto, Produto } from '../models/produto.model';

@Injectable({ providedIn: 'root' })
export class EstoqueService {
  private readonly baseUrl = environment.estoqueApi;

  constructor(private readonly http: HttpClient) { }

  listarProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(`${this.baseUrl}/produtos`);
  }

  criarProduto(dto: CriarProdutoDto): Observable<Produto> {
    return this.http.post<Produto>(`${this.baseUrl}/produtos`, dto);
  }

  atualizarProduto(id: string, dto: AtualizarProdutoDto): Observable<Produto> {
    return this.http.put<Produto>(`${this.baseUrl}/produtos/${id}`, dto);
  }
}