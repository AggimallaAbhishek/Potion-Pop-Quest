
import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault();
    navigate('/dashboard');
  };

  return (
    <div className="bg-primary/5 min-h-screen flex flex-col font-sans">
      <nav className="w-full px-6 py-4 flex items-center justify-between border-b border-primary/10 bg-white">
        <Link to="/" className="flex items-center gap-2 text-primary">
          <div className="size-8 bg-primary rounded-lg flex items-center justify-center text-white">
            <span className="material-symbols-outlined text-2xl">account_balance_wallet</span>
          </div>
          <h1 className="text-xl font-bold tracking-tight">WealthWise</h1>
        </Link>
        <div className="hidden md:block">
          <p className="text-sm text-gray-500">Secure 256-bit encryption</p>
        </div>
      </nav>

      <main className="flex-grow flex items-center justify-center p-4 sm:p-8">
        <div className="w-full max-w-[480px] bg-white rounded-xl shadow-xl shadow-primary/5 border border-primary/5 p-8 md:p-10">
          <div className="text-center mb-8">
            <h2 className="text-3xl font-black text-primary tracking-tight mb-2">Create your account</h2>
            <p className="text-gray-500">Join WealthWise to start managing your future.</p>
          </div>

          <form onSubmit={handleRegister} className="space-y-5">
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-semibold text-primary">Full Name</label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-xl">person</span>
                <input className="w-full pl-10 pr-4 py-3 bg-slate-50 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all text-primary placeholder:text-gray-400" placeholder="Enter your full name" type="text" required />
              </div>
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-semibold text-primary">Email Address</label>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-xl">mail</span>
                <input className="w-full pl-10 pr-4 py-3 bg-slate-50 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all text-primary placeholder:text-gray-400" placeholder="email@example.com" type="email" required />
              </div>
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-semibold text-primary">Password</label>
              <div className="relative flex items-center">
                <span className="material-symbols-outlined absolute left-3 text-gray-400 text-xl">lock</span>
                <input className="w-full pl-10 pr-12 py-3 bg-slate-50 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all text-primary placeholder:text-gray-400" placeholder="At least 8 characters" type="password" required />
                <button className="absolute right-3 text-gray-400 hover:text-primary transition-colors" type="button">
                  <span className="material-symbols-outlined text-xl">visibility</span>
                </button>
              </div>
              <p className="text-[11px] text-gray-400 mt-1">Must include a mix of letters, numbers & symbols.</p>
            </div>

            <button className="w-full bg-primary hover:bg-primary/90 text-white font-bold py-4 rounded-lg shadow-lg shadow-primary/20 transition-all flex items-center justify-center gap-2 mt-2" type="submit">
              <span>Create Account</span>
              <span className="material-symbols-outlined text-lg">arrow_forward</span>
            </button>
          </form>

          <div className="relative my-8">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-gray-200"></div>
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-white px-3 text-gray-400 font-medium">Or sign up with</span>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <SocialAuthButton platform="Google" iconUrl="https://lh3.googleusercontent.com/aida-public/AB6AXuCMX11iCN0TBv6d3YIY76i4XrLuL7E-od8AK4pRC0VAPejHg3tHSIx3ZaGRfkID4bSPUXxPu6v4RrcFwKUaRABhjpc9F68wGu74kH3swCVPjXcX9qczNYwq16JcL6laWKjpEv3QticdTyHwlKgOft0q7o0O9O_uK4yPeVeUmTdqYqJJXo2K8rILYqD2pX_2aqUcpNcZhLcxT-ya-DiGpiRFmi4dQkqP2ovl5rTTyiFrS3Vd_WxdKQTSbUrl2C-3en4EQyMNnl53d1U" />
            <SocialAuthButton platform="Apple" iconUrl="https://lh3.googleusercontent.com/aida-public/AB6AXuBUgV-QTWWI7aI0mJVDSBeyMNpSZpvdLye9MpArGqS60QXKtH5p3x-V8FZnxj30oYmf8d-EFTtKKJasVU_OOgsJxCgmzAHpdSxqoI2WWkc81vqwgn1Y-IBlfsXyHPkZvAElNN0n2foBeP29YQIpMnIfWjSAeF-vR7BOurmzpbK1zDee0bWbyNrW4VmFNFZlEz0-i39CN991n2shwZeEPtaIquDLKEJgkjCDFka_prqld6ypzmXqziFgbcI1xh5qAEHBaplDHRJ_Xb4" />
          </div>

          <div className="mt-8 text-center">
            <p className="text-gray-500 text-sm">
              Already have an account? 
              <Link className="text-primary font-bold hover:underline ml-1" to="/login">Log in</Link>
            </p>
          </div>
        </div>
      </main>

      <footer className="w-full py-8 px-6 border-t border-primary/5">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4 text-xs text-gray-400 font-medium">
          <div className="flex items-center gap-6">
            <a className="hover:text-primary transition-colors" href="#">Terms of Service</a>
            <a className="hover:text-primary transition-colors" href="#">Privacy Policy</a>
            <a className="hover:text-primary transition-colors" href="#">Cookie Policy</a>
          </div>
          <p>© 2024 WealthWise Financial Corp. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
};

const SocialAuthButton: React.FC<{ platform: string; iconUrl: string }> = ({ platform, iconUrl }) => (
  <button className="flex items-center justify-center gap-2 px-4 py-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
    <img alt={platform} className="size-5" src={iconUrl} />
    <span className="text-sm font-semibold text-gray-700">{platform}</span>
  </button>
);

export default RegisterPage;
