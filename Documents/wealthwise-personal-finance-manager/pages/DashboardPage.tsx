
import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Transaction, ExpenseCategory } from '../types';
import AIInsights from '../components/AIInsights';

const DashboardPage: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [showAddModal, setShowAddModal] = useState(false);

  const chartData = [
    { name: 'Week 1', income: 4000, expenses: 2400 },
    { name: 'Week 2', income: 3000, expenses: 1398 },
    { name: 'Week 3', income: 2000, expenses: 9800 },
    { name: 'Week 4', income: 2780, expenses: 3908 },
    { name: 'Week 5', income: 1890, expenses: 4800 },
    { name: 'Week 6', income: 2390, expenses: 3800 },
    { name: 'Week 7', income: 3490, expenses: 4300 },
  ];

  const transactions: Transaction[] = [
    { id: '1', name: 'Starbucks Coffee', description: 'Daily Brew - Downtown', category: 'Food & Dining', date: 'Oct 24, 2023', status: 'Completed', amount: 6.50, type: 'debit' },
    { id: '2', name: 'Monthly Salary', description: 'TechCorp Solutions Inc.', category: 'Income', date: 'Oct 22, 2023', status: 'Completed', amount: 4200.00, type: 'credit' },
    { id: '3', name: 'Amazon Prime', description: 'Household Supplies', category: 'Shopping', date: 'Oct 21, 2023', status: 'Processing', amount: 124.99, type: 'debit' },
    { id: '4', name: 'Apartment Rent', description: 'November Payment', category: 'Housing', date: 'Oct 20, 2023', status: 'Completed', amount: 1500.00, type: 'debit' },
    { id: '5', name: 'FitLife Gym', description: 'Monthly Membership', category: 'Health', date: 'Oct 18, 2023', status: 'Completed', amount: 45.00, type: 'debit' },
  ];

  const categories: ExpenseCategory[] = [
    { name: 'Food & Drinks', amount: 840, color: 'bg-orange-400', icon: 'restaurant', percentage: 45 },
    { name: 'Shopping', amount: 1200, color: 'bg-blue-400', icon: 'shopping_bag', percentage: 65 },
    { name: 'Entertainment', amount: 320, color: 'bg-purple-400', icon: 'movie', percentage: 20 },
    { name: 'Housing', amount: 1500, color: 'bg-emerald-400', icon: 'home', percentage: 80 },
  ];

  const filteredTransactions = transactions.filter(t => 
    t.name.toLowerCase().includes(searchQuery.toLowerCase()) || 
    t.category.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="bg-primary-surface min-h-screen flex flex-col font-sans">
      <header className="flex items-center justify-between whitespace-nowrap border-b border-primary/10 bg-white px-6 md:px-10 py-3 sticky top-0 z-50 shadow-sm">
        <div className="flex items-center gap-8">
          <Link to="/" className="flex items-center gap-3 text-primary group">
            <div className="size-8 bg-primary rounded-lg flex items-center justify-center text-white group-hover:scale-110 transition-transform">
              <span className="material-symbols-outlined">account_balance_wallet</span>
            </div>
            <h2 className="text-xl font-bold tracking-tight">WealthWise</h2>
          </Link>
          <div className="hidden md:flex items-stretch bg-primary/5 rounded-lg h-10 w-64 overflow-hidden border border-primary/5 focus-within:border-primary/20 transition-all">
            <div className="flex items-center justify-center pl-4 text-primary/60">
              <span className="material-symbols-outlined text-[20px]">search</span>
            </div>
            <input 
              className="w-full bg-transparent outline-none text-primary px-3 text-sm placeholder:text-primary/40" 
              placeholder="Search transactions..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>
        </div>
        
        <div className="flex items-center gap-4">
          <div className="flex gap-2">
            <HeaderButton icon="notifications" />
            <HeaderButton icon="settings" />
          </div>
          <div className="h-8 w-px bg-primary/10 mx-2"></div>
          <div className="flex items-center gap-3">
            <div className="text-right hidden sm:block">
              <p className="text-xs font-bold text-primary">Alex Sterling</p>
              <p className="text-[10px] text-primary/60 uppercase tracking-wider font-semibold">Premium Member</p>
            </div>
            <div className="size-10 rounded-full border-2 border-primary/10 bg-cover bg-center ring-2 ring-transparent hover:ring-primary/20 transition-all cursor-pointer" style={{ backgroundImage: 'url(https://picsum.photos/100/100)' }}></div>
          </div>
        </div>
      </header>

      <main className="flex-1 flex flex-col px-4 md:px-10 py-8 max-w-[1440px] mx-auto w-full gap-8">
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h1 className="text-2xl font-black text-primary tracking-tight">Financial Dashboard</h1>
            <p className="text-primary/60 text-sm">Welcome back! Here's what's happening with your money.</p>
          </div>
          <button 
            onClick={() => setShowAddModal(true)}
            className="flex items-center gap-2 bg-primary text-white px-6 py-3 rounded-xl font-bold text-sm hover:opacity-90 hover:-translate-y-0.5 transition-all shadow-xl shadow-primary/20"
          >
            <span className="material-symbols-outlined text-[20px]">add</span>
            New Transaction
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <SummaryCard 
            title="Total Balance" 
            amount="$12,450.00" 
            trend="+2.4%" 
            trendColor="bg-emerald-100 text-emerald-700" 
            progress={75} 
            icon="account_balance"
          />
          <SummaryCard 
            title="Monthly Income" 
            amount="$5,200.00" 
            trend="On Track" 
            trendColor="bg-emerald-100 text-emerald-700" 
            subtext="Next expected: Oct 28" 
            icon="trending_up"
            iconColor="bg-emerald-100 text-emerald-700"
          />
          <SummaryCard 
            title="Monthly Expenses" 
            amount="$3,150.00" 
            trend="-5.1%" 
            trendColor="bg-rose-100 text-rose-700" 
            subtext="Budget remaining: $2,050.00" 
            icon="trending_down"
            iconColor="bg-rose-100 text-rose-700"
          />
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <div className="lg:col-span-8 flex flex-col gap-8">
            <div className="rounded-xl bg-white border border-primary/5 p-6 shadow-sm">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-bold text-primary">Spending vs Income Trends</h3>
                <div className="flex gap-2">
                  <span className="flex items-center gap-1.5 text-[10px] font-bold text-primary/60 bg-primary/5 px-2 py-1 rounded">
                    <span className="size-2 rounded-full bg-primary"></span> Income
                  </span>
                  <select className="bg-primary/5 border-none rounded-lg px-3 py-1.5 text-xs font-bold text-primary focus:ring-primary outline-none cursor-pointer">
                    <option>Last 30 Days</option>
                    <option>Last 3 Months</option>
                    <option>This Year</option>
                  </select>
                </div>
              </div>
              <div className="h-[300px] w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={chartData}>
                    <defs>
                      <linearGradient id="colorIncome" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#1a4231" stopOpacity={0.15}/>
                        <stop offset="95%" stopColor="#1a4231" stopOpacity={0}/>
                      </linearGradient>
                    </defs>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                    <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fontSize: 11, fill: '#64748b' }} dy={10} />
                    <YAxis hide />
                    <Tooltip 
                      contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 10px 15px -3px rgb(0 0 0 / 0.1)' }}
                    />
                    <Area type="monotone" dataKey="income" stroke="#1a4231" strokeWidth={3} fillOpacity={1} fill="url(#colorIncome)" />
                  </AreaChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="rounded-xl bg-white border border-primary/5 shadow-sm overflow-hidden">
              <div className="p-6 border-b border-primary/5 flex items-center justify-between">
                <h3 className="text-lg font-bold text-primary">Recent Transactions</h3>
                <div className="flex gap-2">
                  <HeaderButton icon="filter_list" size="sm" />
                  <HeaderButton icon="download" size="sm" />
                </div>
              </div>
              <div className="overflow-x-auto">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-primary/5 text-[11px] uppercase tracking-wider font-bold text-primary/60">
                      <th className="px-6 py-4">Transaction</th>
                      <th className="px-6 py-4">Category</th>
                      <th className="px-6 py-4">Date</th>
                      <th className="px-6 py-4">Status</th>
                      <th className="px-6 py-4 text-right">Amount</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-primary/5">
                    {filteredTransactions.map((t) => (
                      <tr key={t.id} className="hover:bg-primary/5 transition-colors group cursor-pointer">
                        <td className="px-6 py-4">
                          <div className="flex items-center gap-3">
                            <div className={`size-9 rounded-xl flex items-center justify-center ${t.type === 'credit' ? 'bg-emerald-100 text-emerald-600' : 'bg-primary/10 text-primary'} group-hover:scale-105 transition-transform`}>
                              <span className="material-symbols-outlined text-[18px]">
                                {t.category === 'Food & Dining' ? 'local_cafe' : t.category === 'Income' ? 'work' : t.category === 'Shopping' ? 'shopping_cart' : t.category === 'Housing' ? 'home' : 'fitness_center'}
                              </span>
                            </div>
                            <div>
                              <p className="text-sm font-bold text-primary">{t.name}</p>
                              <p className="text-xs text-primary/40">{t.description}</p>
                            </div>
                          </div>
                        </td>
                        <td className="px-6 py-4">
                          <span className="text-[10px] font-bold bg-primary/5 text-primary px-2.5 py-1 rounded-full border border-primary/10 uppercase tracking-wider">{t.category}</span>
                        </td>
                        <td className="px-6 py-4 text-sm text-primary/60 font-medium">{t.date}</td>
                        <td className="px-6 py-4">
                          <span className={`flex items-center gap-1.5 text-[10px] font-black uppercase tracking-widest ${t.status === 'Completed' ? 'text-emerald-600' : 'text-amber-500'}`}>
                            <span className={`size-1.5 rounded-full ${t.status === 'Completed' ? 'bg-emerald-600' : 'bg-amber-500'} animate-pulse`}></span>
                            {t.status}
                          </span>
                        </td>
                        <td className={`px-6 py-4 text-right font-black ${t.type === 'credit' ? 'text-emerald-600' : 'text-rose-500'}`}>
                          {t.type === 'credit' ? '+' : '-'}${t.amount.toLocaleString()}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <div className="lg:col-span-4 flex flex-col gap-8">
            <AIInsights transactions={transactions} />

            <div className="rounded-xl bg-white border border-primary/5 p-6 shadow-sm">
              <h3 className="text-lg font-bold text-primary mb-6">Expense Breakdown</h3>
              <div className="flex flex-col gap-6">
                {categories.map((cat, i) => (
                  <div key={i} className="flex items-center gap-4">
                    <div className={`size-10 rounded-lg flex items-center justify-center ${cat.color} bg-opacity-10 text-opacity-100 font-bold`}>
                      <span className="material-symbols-outlined text-[20px]">{cat.icon}</span>
                    </div>
                    <div className="flex-1">
                      <div className="flex justify-between items-center mb-1">
                        <span className="text-sm font-semibold text-primary">{cat.name}</span>
                        <span className="text-xs font-bold text-primary/60">${cat.amount}</span>
                      </div>
                      <div className="h-1.5 w-full bg-primary/5 rounded-full overflow-hidden">
                        <div className={`h-full ${cat.color} rounded-full`} style={{ width: `${cat.percentage}%` }}></div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
              <button className="w-full mt-8 py-3 bg-primary/5 rounded-xl text-xs font-bold text-primary hover:bg-primary/10 transition-colors border border-primary/10">
                Generate Full Wealth Report
              </button>
            </div>

            <div className="rounded-xl bg-gradient-to-br from-primary to-primary-dark p-6 text-white shadow-xl">
              <span className="material-symbols-outlined text-4xl mb-4 text-emerald-400">workspace_premium</span>
              <h4 className="text-lg font-bold mb-2">Upgrade to Pro</h4>
              <p className="text-emerald-100/70 text-sm mb-6 leading-relaxed">Unlock advanced investment tracking and personalized AI portfolio rebalancing.</p>
              <button className="w-full py-3 bg-emerald-400 text-primary-dark font-black rounded-xl hover:bg-emerald-300 transition-colors">
                Learn More
              </button>
            </div>
          </div>
        </div>
      </main>

      <footer className="mt-auto px-10 py-8 border-t border-primary/5 text-center bg-white">
        <p className="text-primary/40 text-[10px] font-bold uppercase tracking-[0.2em]">WealthWise Financial Systems • Secure 256-bit Encrypted</p>
      </footer>

      {/* Modal Mockup */}
      {showAddModal && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-primary/40 backdrop-blur-sm">
          <div className="bg-white rounded-2xl w-full max-w-md p-8 shadow-2xl scale-in-center">
            <div className="flex justify-between items-center mb-6">
              <h3 className="text-xl font-bold text-primary">Add Transaction</h3>
              <button onClick={() => setShowAddModal(false)} className="text-primary/40 hover:text-primary">
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>
            <div className="space-y-4">
              <div className="space-y-1">
                <label className="text-xs font-bold text-primary/60 uppercase">Amount</label>
                <input type="text" className="w-full p-3 bg-primary/5 border-none rounded-xl focus:ring-2 focus:ring-primary outline-none" placeholder="$ 0.00" />
              </div>
              <div className="space-y-1">
                <label className="text-xs font-bold text-primary/60 uppercase">Description</label>
                <input type="text" className="w-full p-3 bg-primary/5 border-none rounded-xl focus:ring-2 focus:ring-primary outline-none" placeholder="What did you spend on?" />
              </div>
              <button className="w-full py-4 bg-primary text-white font-bold rounded-xl mt-4">Save Transaction</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const HeaderButton: React.FC<{ icon: string; size?: 'sm' | 'md' }> = ({ icon, size = 'md' }) => (
  <button className={`flex ${size === 'md' ? 'size-10' : 'size-9'} cursor-pointer items-center justify-center rounded-lg bg-primary/5 text-primary hover:bg-primary/10 transition-colors`}>
    <span className={`material-symbols-outlined ${size === 'md' ? 'text-[24px]' : 'text-[20px]'}`}>{icon}</span>
  </button>
);

const SummaryCard: React.FC<{ title: string; amount: string; trend: string; trendColor: string; progress?: number; subtext?: string; icon: string; iconColor?: string }> = ({ 
  title, amount, trend, trendColor, progress, subtext, icon, iconColor = 'bg-primary/10 text-primary' 
}) => (
  <div className="flex flex-col gap-4 rounded-xl p-6 bg-white border border-primary/5 shadow-sm hover:shadow-md transition-shadow">
    <div className="flex items-center justify-between">
      <div className={`size-10 rounded-xl flex items-center justify-center ${iconColor}`}>
        <span className="material-symbols-outlined">{icon}</span>
      </div>
      <span className={`text-[10px] font-black px-2 py-1 rounded-full ${trendColor} uppercase tracking-wider`}>{trend}</span>
    </div>
    <div>
      <p className="text-primary/60 text-xs font-bold uppercase tracking-widest mb-1">{title}</p>
      <p className="text-primary text-3xl font-black leading-tight tracking-tight">{amount}</p>
    </div>
    {progress !== undefined ? (
      <div className="h-1 w-full bg-primary/5 rounded-full overflow-hidden">
        <div className="h-full bg-primary" style={{ width: `${progress}%` }}></div>
      </div>
    ) : subtext ? (
      <p className="text-[10px] text-primary/40 italic font-medium">{subtext}</p>
    ) : null}
  </div>
);

export default DashboardPage;
