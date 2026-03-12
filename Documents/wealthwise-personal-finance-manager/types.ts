
export interface Transaction {
  id: string;
  name: string;
  description: string;
  category: string;
  date: string;
  status: 'Completed' | 'Processing' | 'Failed';
  amount: number;
  type: 'debit' | 'credit';
}

export interface ExpenseCategory {
  name: string;
  amount: number;
  color: string;
  icon: string;
  percentage: number;
}
