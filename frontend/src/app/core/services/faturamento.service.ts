import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CriarNotaFiscalDto, NotaFiscal } from '../models/nota-fiscal.model';

@Injectable({ providedIn: 'root' })
export class FaturamentoService {
  private readonly baseUrl = environment.faturamentoApi;

  constructor(private readonly http: HttpClient) { }

  criarNota(dto: CriarNotaFiscalDto): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/notas-fiscais`, dto);
  }

  imprimirNota(id: string): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.baseUrl}/notas-fiscais/${id}/imprimir`, {});
  }
}