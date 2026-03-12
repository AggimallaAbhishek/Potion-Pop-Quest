
import React, { useState, useEffect } from 'react';
import { GoogleGenAI } from "@google/genai";
import { Transaction } from '../types';

interface AIInsightsProps {
  transactions: Transaction[];
}

const AIInsights: React.FC<AIInsightsProps> = ({ transactions }) => {
  const [insight, setInsight] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const generateInsights = async () => {
    setLoading(true);
    setError(null);
    try {
      const ai = new GoogleGenAI({ apiKey: process.env.API_KEY });
      const transactionSummary = transactions.map(t => `${t.date}: ${t.name} (${t.category}) - ${t.type === 'debit' ? '-' : '+'}$${t.amount}`).join('\n');
      
      const response = await ai.models.generateContent({
        model: 'gemini-3-flash-preview',
        contents: `Analyze these recent transactions and provide 3 concise, actionable financial insights and a "Financial Health Tip". Keep it encouraging and professional.\n\nTransactions:\n${transactionSummary}`,
        config: {
          systemInstruction: "You are a world-class financial advisor AI. Your advice is data-driven, concise, and helps users save money while achieving long-term wealth.",
          temperature: 0.7,
        }
      });

      setInsight(response.text || "I couldn't generate insights at this moment. Try again later.");
    } catch (err) {
      console.error("AI Insight Error:", err);
      setError("Failed to connect to the financial brain. Please check your connection.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    generateInsights();
  }, []);

  return (
    <div className="relative overflow-hidden rounded-xl bg-white border border-primary/10 shadow-lg shadow-primary/5 p-6">
      <div className="absolute top-0 right-0 p-4">
        <span className="material-symbols-outlined text-primary/20 text-4xl select-none">auto_awesome</span>
      </div>
      
      <div className="flex items-center gap-2 mb-4">
        <div className="bg-primary/10 p-2 rounded-lg text-primary">
          <span className="material-symbols-outlined text-xl">psychology</span>
        </div>
        <h3 className="text-lg font-bold text-primary">WealthWise AI Insights</h3>
      </div>

      {loading ? (
        <div className="space-y-3 animate-pulse">
          <div className="h-4 bg-primary/5 rounded w-3/4"></div>
          <div className="h-4 bg-primary/5 rounded w-full"></div>
          <div className="h-4 bg-primary/5 rounded w-5/6"></div>
          <div className="h-20 bg-primary/5 rounded w-full mt-4"></div>
        </div>
      ) : error ? (
        <div className="text-center py-6">
          <p className="text-rose-500 text-sm mb-4">{error}</p>
          <button 
            onClick={generateInsights}
            className="text-xs font-bold text-primary hover:underline flex items-center justify-center gap-1 mx-auto"
          >
            <span className="material-symbols-outlined text-sm">refresh</span> Retry Analysis
          </button>
        </div>
      ) : (
        <div className="prose prose-sm prose-slate max-w-none">
          <div className="text-primary/80 text-sm leading-relaxed whitespace-pre-line">
            {insight}
          </div>
          <div className="mt-6 pt-4 border-t border-primary/5 flex items-center justify-between">
            <span className="text-[10px] font-bold text-primary/40 uppercase tracking-widest">Powered by Gemini 3</span>
            <button 
              onClick={generateInsights}
              className="text-[10px] font-bold text-primary bg-primary/5 px-3 py-1.5 rounded-full hover:bg-primary/10 transition-colors"
            >
              Refresh Analysis
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default AIInsights;
