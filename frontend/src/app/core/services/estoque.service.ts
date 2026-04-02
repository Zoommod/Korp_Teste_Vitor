import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Produto } from '../models/produto.model';

@Injectable({ providedIn: 'root' })
export class EstoqueService {
  private readonly baseUrl = environment.estoqueApi;

  constructor(private readonly http: HttpClient) {}

  listarProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(`${this.baseUrl}/produtos`);
  }

  adicionarSaldo(codigo: string, quantidade: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/produtos/${codigo}/saldo`, { quantidade });
  }
}