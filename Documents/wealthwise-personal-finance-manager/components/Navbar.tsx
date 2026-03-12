
import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Navbar: React.FC = () => {
  const navigate = useNavigate();

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-primary/10 bg-white/80 backdrop-blur-md">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <Link to="/" className="flex items-center gap-2">
            <div className="bg-primary text-white p-1.5 rounded-lg flex items-center justify-center">
              <span className="material-symbols-outlined text-xl">account_balance_wallet</span>
            </div>
            <span className="text-xl font-bold tracking-tight text-primary">WealthWise</span>
          </Link>
          
          <div className="hidden md:flex items-center space-x-8">
            <a href="#features" className="text-sm font-medium text-slate-600 hover:text-primary transition-colors">Features</a>
            <a href="#" className="text-sm font-medium text-slate-600 hover:text-primary transition-colors">Pricing</a>
            <a href="#" className="text-sm font-medium text-slate-600 hover:text-primary transition-colors">About</a>
          </div>

          <div className="flex items-center gap-3">
            <button 
              onClick={() => navigate('/login')}
              className="hidden sm:block px-4 py-2 text-sm font-semibold text-primary hover:bg-primary/5 rounded-lg transition-all"
            >
              Login
            </button>
            <button 
              onClick={() => navigate('/register')}
              className="px-5 py-2.5 bg-primary text-white text-sm font-bold rounded-lg hover:shadow-lg hover:shadow-primary/20 transition-all"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
