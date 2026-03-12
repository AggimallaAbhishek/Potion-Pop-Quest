
import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

const LoginPage: React.FC = () => {
  const navigate = useNavigate();

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();
    navigate('/dashboard');
  };

  return (
    <div className="font-sans bg-primary/5 min-h-screen flex flex-col relative overflow-hidden">
      {/* Background Pattern */}
      <div className="absolute inset-0 z-0 opacity-[0.03] pointer-events-none">
        <svg height="100%" width="100%" xmlns="http://www.w3.org/2000/svg">
          <defs>
            <pattern height="40" id="grid" patternUnits="userSpaceOnUse" width="40">
              <path d="M 40 0 L 0 0 0 40" fill="none" stroke="currentColor" strokeWidth="1"></path>
            </pattern>
          </defs>
          <rect fill="url(#grid)" height="100%" width="100%"></rect>
        </svg>
      </div>

      <header className="relative z-10 w-full px-6 lg:px-40 py-5">
        <div className="max-w-[1200px] mx-auto flex items-center justify-between">
          <Link to="/" className="flex items-center gap-2 text-primary">
            <span className="material-symbols-outlined text-3xl font-bold">account_balance_wallet</span>
            <h1 className="text-xl font-bold tracking-tight">WealthWise</h1>
          </Link>
          <a className="text-sm font-semibold text-primary hover:text-primary/80 transition-colors" href="#">Support</a>
        </div>
      </header>

      <main className="relative z-10 flex-1 flex items-center justify-center px-6 py-12">
        <div className="w-full max-w-[440px] bg-white rounded-xl shadow-xl shadow-primary/5 border border-primary/10 p-8 lg:p-10">
          <div className="text-center mb-10">
            <h2 className="text-2xl lg:text-3xl font-bold text-primary mb-2">Welcome back</h2>
            <p className="text-primary/60 text-sm">Please enter your details to access your dashboard.</p>
          </div>

          <form onSubmit={handleLogin} className="space-y-6">
            <div className="space-y-2">
              <label className="block text-sm font-medium text-primary" htmlFor="email">Email address</label>
              <input 
                className="w-full px-4 py-3 bg-slate-50 border border-primary/10 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all placeholder:text-primary/30 text-primary" 
                id="email" 
                placeholder="name@company.com" 
                required 
                type="email"
              />
            </div>

            <div className="space-y-2">
              <div className="flex justify-between items-center">
                <label className="block text-sm font-medium text-primary" htmlFor="password">Password</label>
              </div>
              <div className="relative flex items-center">
                <input 
                  className="w-full px-4 py-3 bg-slate-50 border border-primary/10 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all placeholder:text-primary/30 text-primary" 
                  id="password" 
                  placeholder="••••••••" 
                  required 
                  type="password"
                />
                <button className="absolute right-3 text-primary/40 hover:text-primary transition-colors" type="button">
                  <span className="material-symbols-outlined text-xl">visibility</span>
                </button>
              </div>
            </div>

            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <input className="h-4 w-4 text-primary focus:ring-primary border-primary/20 rounded cursor-pointer" id="remember-me" type="checkbox"/>
                <label className="ml-2 block text-sm text-primary/70 cursor-pointer" htmlFor="remember-me">Remember me</label>
              </div>
              <a className="text-sm font-semibold text-primary hover:underline" href="#">Forgot password?</a>
            </div>

            <button className="w-full bg-primary text-white py-3.5 px-4 rounded-lg font-bold text-sm hover:bg-primary/90 transition-all shadow-lg shadow-primary/10 flex items-center justify-center gap-2" type="submit">
              <span>Login to Dashboard</span>
              <span className="material-symbols-outlined text-sm">arrow_forward</span>
            </button>

            <div className="text-center mt-6">
              <p className="text-sm text-primary/60">
                Don't have an account? 
                <Link className="text-primary font-bold hover:underline ml-1" to="/register">Sign up for free</Link>
              </p>
            </div>
          </form>
        </div>
      </main>

      <footer className="relative z-10 py-8 text-center">
        <div className="flex items-center justify-center gap-2 text-primary/40 text-xs font-medium uppercase tracking-widest">
          <span className="material-symbols-outlined text-base">lock</span>
          <span>Secure SSL Encrypted Connection</span>
        </div>
        <div className="mt-4 text-primary/30 text-[10px]">
          © 2024 WealthWise Financial Systems. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default LoginPage;
