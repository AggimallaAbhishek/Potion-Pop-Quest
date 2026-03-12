
import React from 'react';
import Navbar from '../components/Navbar';
import { Link } from 'react-router-dom';

const LandingPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-secondary">
      <Navbar />

      {/* Hero Section */}
      <section className="relative overflow-hidden pt-16 pb-20 lg:pt-24 lg:pb-32">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col lg:flex-row items-center gap-12 lg:gap-20">
          <div className="flex-1 text-center lg:text-left">
            <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-bold uppercase tracking-wider mb-6">
              <span className="flex h-2 w-2 rounded-full bg-primary animate-pulse"></span>
              Trusted by 50,000+ users
            </div>
            <h1 className="text-4xl sm:text-5xl lg:text-6xl font-black text-slate-900 leading-[1.1] mb-6 tracking-tight">
              Take Control of Your <span className="text-primary">Financial Future</span>
            </h1>
            <p className="text-lg text-slate-600 mb-10 max-w-2xl mx-auto lg:mx-0 leading-relaxed">
              Track expenses, grow investments, and achieve your goals with the all-in-one personal finance manager designed for growth.
            </p>
            <div className="flex flex-col sm:flex-row items-center justify-center lg:justify-start gap-4">
              <Link to="/register" className="w-full sm:w-auto px-8 py-4 bg-primary text-white font-bold rounded-xl shadow-xl shadow-primary/20 hover:scale-[1.02] active:scale-95 transition-all text-center">
                Get Started for Free
              </Link>
              <button className="w-full sm:w-auto px-8 py-4 bg-white border border-slate-200 text-slate-700 font-bold rounded-xl hover:bg-slate-50 transition-all flex items-center justify-center gap-2">
                <span className="material-symbols-outlined text-xl">play_circle</span>
                See how it works
              </button>
            </div>
            <div className="mt-10 flex items-center justify-center lg:justify-start gap-6 opacity-60">
              <span className="text-sm font-medium">Compatible with</span>
              <div className="flex gap-4">
                <span className="material-symbols-outlined">payments</span>
                <span className="material-symbols-outlined">account_balance</span>
                <span className="material-symbols-outlined">credit_card</span>
              </div>
            </div>
          </div>
          
          <div className="flex-1 w-full max-w-[600px] lg:max-w-none">
            <div className="relative rounded-2xl overflow-hidden shadow-2xl border border-white/20 bg-primary/5 p-4 lg:p-8">
              <div className="aspect-video bg-white rounded-xl shadow-sm overflow-hidden flex flex-col">
                <div className="h-12 border-b flex items-center px-4 justify-between bg-slate-50/50">
                  <div className="flex gap-1.5">
                    <div className="w-2.5 h-2.5 rounded-full bg-slate-300"></div>
                    <div className="w-2.5 h-2.5 rounded-full bg-slate-300"></div>
                    <div className="w-2.5 h-2.5 rounded-full bg-slate-300"></div>
                  </div>
                  <div className="h-4 w-32 bg-slate-200 rounded-full"></div>
                </div>
                <div className="flex-1 p-6 grid grid-cols-3 gap-4">
                  <div className="col-span-2 space-y-4">
                    <div className="h-24 bg-primary/10 rounded-lg flex items-end p-4 gap-2">
                      <div className="w-full h-1/2 bg-primary/40 rounded-t-sm"></div>
                      <div className="w-full h-3/4 bg-primary/60 rounded-t-sm"></div>
                      <div className="w-full h-full bg-primary rounded-t-sm"></div>
                      <div className="w-full h-2/3 bg-primary/50 rounded-t-sm"></div>
                    </div>
                    <div className="space-y-2">
                      <div className="h-3 w-1/2 bg-slate-100 rounded"></div>
                      <div className="h-3 w-3/4 bg-slate-100 rounded"></div>
                    </div>
                  </div>
                  <div className="space-y-4">
                    <div className="h-full bg-slate-50 rounded-lg border border-dashed border-slate-200 flex flex-col items-center justify-center p-4">
                      <div className="w-12 h-12 rounded-full border-4 border-primary border-t-transparent animate-spin mb-2"></div>
                      <div className="h-2 w-full bg-slate-200 rounded"></div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="absolute -top-10 -right-10 w-40 h-40 bg-primary/10 rounded-full blur-3xl -z-10"></div>
              <div className="absolute -bottom-10 -left-10 w-40 h-40 bg-primary/10 rounded-full blur-3xl -z-10"></div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-24 bg-white" id="features">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center max-w-3xl mx-auto mb-16">
            <h2 className="text-primary font-bold text-sm uppercase tracking-widest mb-3">Features</h2>
            <h3 className="text-3xl sm:text-4xl font-extrabold text-slate-900 mb-5 tracking-tight">Everything you need to grow your wealth</h3>
            <p className="text-slate-600 text-lg">Powerful tools for your money, simplified for your everyday life. Secure, intuitive, and always in sync.</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <FeatureCard 
              icon="account_balance_wallet"
              title="Smart Budgeting"
              description="Automated categorizing and limit alerts to keep your spending on track without the manual effort."
              benefits={["AI categorization", "Real-time alerts"]}
            />
            <FeatureCard 
              icon="trending_up"
              title="Investment Tracking"
              description="Real-time portfolio performance and diversification insights. Track stocks, crypto, and real estate."
              benefits={["Global market data", "Asset allocation views"]}
            />
            <FeatureCard 
              icon="verified_user"
              title="Secure Analytics"
              description="Bank-grade encryption with visual spending trends and deep data insights to protect your privacy."
              benefits={["256-bit encryption", "Zero-knowledge privacy"]}
            />
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="bg-primary rounded-3xl p-8 md:p-16 relative overflow-hidden text-center md:text-left flex flex-col md:flex-row items-center justify-between gap-10">
            <div className="relative z-10 max-w-xl text-white">
              <h2 className="text-3xl sm:text-4xl font-black mb-6">Ready to grow your wealth?</h2>
              <p className="opacity-80 text-lg leading-relaxed mb-0">
                Join thousands of users who have transformed their financial habits with WealthWise. Start your 14-day free trial today.
              </p>
            </div>
            <div className="relative z-10 shrink-0">
              <Link to="/register" className="inline-block px-10 py-5 bg-white text-primary font-black text-lg rounded-xl hover:bg-slate-50 hover:scale-105 transition-all shadow-2xl">
                Get Started Now
              </Link>
            </div>
            <div className="absolute top-0 right-0 -translate-y-1/2 translate-x-1/2 w-96 h-96 bg-white/5 rounded-full blur-3xl"></div>
            <div className="absolute bottom-0 left-0 translate-y-1/2 -translate-x-1/2 w-64 h-64 bg-white/5 rounded-full blur-2xl"></div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-slate-50 border-t border-slate-200 pt-20 pb-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-10 mb-16">
            <div className="col-span-2 lg:col-span-2">
              <div className="flex items-center gap-2 mb-6 text-primary">
                <span className="material-symbols-outlined text-2xl font-bold">account_balance_wallet</span>
                <span className="text-lg font-bold tracking-tight">WealthWise</span>
              </div>
              <p className="text-slate-500 text-sm max-w-xs leading-relaxed mb-6">
                The modern way to manage your personal finances. We help you make smarter decisions with your money.
              </p>
              <div className="flex gap-4">
                <SocialLink icon="public" />
                <SocialLink icon="share" />
                <SocialLink icon="mail" />
              </div>
            </div>
            <div>
              <FooterHeader title="Product" />
              <FooterList items={["Features", "Pricing", "Security", "Updates"]} />
            </div>
            <div>
              <FooterHeader title="Company" />
              <FooterList items={["About", "Careers", "Blog", "Contact"]} />
            </div>
            <div>
              <FooterHeader title="Legal" />
              <FooterList items={["Privacy Policy", "Terms of Service", "Cookie Policy"]} />
            </div>
          </div>
          <div className="border-t border-slate-200 pt-8 flex flex-col md:flex-row justify-between items-center gap-4">
            <p className="text-slate-500 text-xs">© 2024 WealthWise Inc. All rights reserved.</p>
            <p className="text-slate-400 text-xs flex items-center gap-1">
              Made with <span className="material-symbols-outlined text-red-400 text-xs fill-1">favorite</span> for financial freedom.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
};

const FeatureCard: React.FC<{ icon: string; title: string; description: string; benefits: string[] }> = ({ icon, title, description, benefits }) => (
  <div className="group p-8 rounded-2xl border border-slate-100 bg-slate-50/30 hover:bg-white hover:shadow-xl hover:shadow-primary/5 hover:border-primary/10 transition-all duration-300">
    <div className="w-14 h-14 bg-primary/10 text-primary rounded-xl flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
      <span className="material-symbols-outlined text-3xl font-light">{icon}</span>
    </div>
    <h4 className="text-xl font-bold text-slate-900 mb-3">{title}</h4>
    <p className="text-slate-600 leading-relaxed mb-6">{description}</p>
    <ul className="space-y-3">
      {benefits.map((benefit, i) => (
        <li key={i} className="flex items-center gap-2 text-sm text-slate-500">
          <span className="material-symbols-outlined text-primary text-lg">check_circle</span>
          {benefit}
        </li>
      ))}
    </ul>
  </div>
);

const SocialLink: React.FC<{ icon: string }> = ({ icon }) => (
  <a href="#" className="w-10 h-10 rounded-full bg-slate-200 flex items-center justify-center text-slate-600 hover:bg-primary hover:text-white transition-all">
    <span className="material-symbols-outlined text-lg">{icon}</span>
  </a>
);

const FooterHeader: React.FC<{ title: string }> = ({ title }) => (
  <h5 className="font-bold text-slate-900 mb-6 uppercase text-xs tracking-widest">{title}</h5>
);

const FooterList: React.FC<{ items: string[] }> = ({ items }) => (
  <ul className="space-y-4 text-sm text-slate-500">
    {items.map((item, i) => (
      <li key={i}><a href="#" className="hover:text-primary transition-colors">{item}</a></li>
    ))}
  </ul>
);

export default LandingPage;
