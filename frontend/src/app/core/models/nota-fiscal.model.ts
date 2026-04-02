export interface ItemNotaFiscal {
  codigoProduto: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: string;
  numero: number;
  status: string;
  itens: ItemNotaFiscal[];
}

export interface CriarNotaFiscalDto {
  itens: ItemNotaFiscal[];
}