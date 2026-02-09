import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GroqService } from '../services/groq';
import { KrediHesaplamaService, VadeHesaplamaRequest } from '../services/kredi-hesaplama';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './gemini-chat.html',
  styleUrls: ['./gemini-chat.scss']
})
export class Chatbot {
  isOpen = false;
  newMessage = '';
  messages: { sender: 'user' | 'ai', text: string, isKrediHesaplama?: boolean, krediData?: any }[] = [];

  constructor(
    private groqService: GroqService,
    private krediHesaplamaService: KrediHesaplamaService
  ) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  async sendMessage() {
    const prompt = this.newMessage.trim();
    if (!prompt) return;

    this.messages.push({ sender: 'user', text: prompt });
    this.newMessage = '';

    // Kredi hesaplama isteği olup olmadığını kontrol et
    const krediBilgileri = this.krediBilgileriniCikar(prompt);
    
    if (krediBilgileri) {
      await this.krediHesapla(krediBilgileri);
    } else {
      // Normal Groq yanıtı
      try {
        const response = await this.groqService.generate(prompt);
        this.messages.push({ sender: 'ai', text: response });
      } catch (error) {
        console.error('Groq API Hatası:', error);
        this.messages.push({ 
          sender: 'ai', 
          text: 'Üzgünüm, şu anda AI yanıtı alamıyorum. Kredi hesaplama için aylık gelirinizi, kredi tutarınızı ve kredi türünüzü belirtiniz.' 
        });
      }
    }
  }

  private krediBilgileriniCikar(prompt: string): VadeHesaplamaRequest | null {
    const lowerPrompt = prompt.toLowerCase();
    
    // Kredi türünü belirle
    let krediTipi = '';
    if (lowerPrompt.includes('konut') || lowerPrompt.includes('ev') || lowerPrompt.includes('mortgage')) {
      krediTipi = 'konut';
    } else if (lowerPrompt.includes('ihtiyaç') || lowerPrompt.includes('ihtiyac') || lowerPrompt.includes('kişisel') || lowerPrompt.includes('kisisel')) {
      krediTipi = 'ihtiyaç';
    } else if (lowerPrompt.includes('taşıt') || lowerPrompt.includes('tasit') || lowerPrompt.includes('araba') || lowerPrompt.includes('otomobil') || lowerPrompt.includes('araç')) {
      krediTipi = 'taşıt';
    } else {
      return null; // Kredi türü belirlenemedi
    }

    // Tüm sayıları bul
    const numberMatches = prompt.match(/(\d+(?:\.\d+)?)/g);
    if (!numberMatches || numberMatches.length < 2) {
      return null;
    }

    const numbers = numberMatches.map(match => parseFloat(match));
    
    // Gelir ve kredi tutarını belirle
    let aylikGelir = 0;
    let krediTutari = 0;

    // "Aylık" kelimesinden sonra gelen sayı gelir olabilir
    const aylikIndex = lowerPrompt.indexOf('aylık');
    if (aylikIndex !== -1) {
      // "Aylık" kelimesinden sonraki ilk sayıyı bul
      const afterAylik = prompt.substring(aylikIndex);
      const gelirMatch = afterAylik.match(/(\d+(?:\.\d+)?)/);
      if (gelirMatch) {
        aylikGelir = parseFloat(gelirMatch[1]);
      }
    }

    // "Kredi" kelimesinden önce gelen sayı kredi tutarı olabilir
    const krediIndex = lowerPrompt.indexOf('kredi');
    if (krediIndex !== -1) {
      // "Kredi" kelimesinden önceki son sayıyı bul
      const beforeKredi = prompt.substring(0, krediIndex);
      const krediMatches = beforeKredi.match(/(\d+(?:\.\d+)?)/g);
      if (krediMatches && krediMatches.length > 0) {
        krediTutari = parseFloat(krediMatches[krediMatches.length - 1]);
      }
    }

    // Eğer yukarıdaki yöntemler çalışmadıysa, sayıları sırayla dene
    if (aylikGelir === 0 || krediTutari === 0) {
      // En küçük sayı genellikle gelir, en büyük sayı kredi tutarı
      const sortedNumbers = [...numbers].sort((a, b) => a - b);
      
      if (aylikGelir === 0) {
        aylikGelir = sortedNumbers[0]; // En küçük sayı gelir
      }
      
      if (krediTutari === 0) {
        krediTutari = sortedNumbers[sortedNumbers.length - 1]; // En büyük sayı kredi tutarı
      }
    }

    // Mantıklı kontrol: Gelir genellikle kredi tutarından küçük olur
    if (aylikGelir >= krediTutari && aylikGelir > 0 && krediTutari > 0) {
      // Sayıları değiştir
      const temp = aylikGelir;
      aylikGelir = krediTutari;
      krediTutari = temp;
    }

    // Son kontrol: Gelir ve kredi tutarı mantıklı mı?
    if (aylikGelir <= 0 || krediTutari <= 0 || aylikGelir >= krediTutari) {
      return null;
    }

    console.log('Çıkarılan bilgiler:', { krediTipi, aylikGelir, krediTutari });

    return {
      krediTipi,
      aylikGelir,
      krediTutari
    };
  }

  private async krediHesapla(request: VadeHesaplamaRequest) {
    try {
      console.log('Kredi hesaplama isteği:', request);
      
      const response = await this.krediHesaplamaService.hesaplaOptimalVade(request).toPromise();
      
      if (response && response.basarili) {
        this.messages.push({ 
          sender: 'ai', 
          text: response.mesaj,
          isKrediHesaplama: true,
          krediData: response
        });
      } else {
        const hataMesaji = response?.mesaj || 'Kredi hesaplama sırasında bir hata oluştu. Lütfen tekrar deneyin.';
        this.messages.push({ 
          sender: 'ai', 
          text: hataMesaji
        });
      }
    } catch (error) {
      console.error('Kredi hesaplama hatası:', error);
      this.messages.push({ 
        sender: 'ai', 
        text: 'Kredi hesaplama servisi şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin veya farklı bir kredi türü deneyin.' 
      });
    }
  }
}

