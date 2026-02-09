import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroqService {
  private readonly API_KEY = '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';
  private readonly ENDPOINT = 'https://api.groq.com/openai/v1/chat/completions';

  constructor(private http: HttpClient) {}

  async generate(prompt: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.API_KEY}`
    });

    const body = {
      model: 'llama3-8b-8192',
      messages: [
        {
          role: 'system',
          content: `Sen bir kredi danışmanısın. Kullanıcılara kredi konularında yardımcı oluyorsun. 
          
          Optimal vade hesaplama için kullanıcıdan şu bilgileri iste:
          - Aylık gelir miktarı
          - Çekmek istediği kredi tutarı
          - Kredi türü (Konut kredisi, İhtiyaç kredisi veya Taşıt kredisi)
          
          Örnek: "Aylık 8000 TL gelirim var, 500000 TL konut kredisi çekmek istiyorum"
          
          Eğer kullanıcı bu bilgileri verirse, sistem otomatik olarak ödeme gücüne göre optimal vade süresini hesaplayacak. Aksi takdirde kullanıcıya bu bilgileri vermesini söyle.
          
          Kısa, sade ve net cevaplar ver. Türkçe konuş. Kullanıcıya yardımcı ol.`
        },
        {
          role: 'user',
          content: prompt
        }
      ],
      temperature: 0.7,
      max_tokens: 500
    };

    try {
      const response: any = await firstValueFrom(
        this.http.post(this.ENDPOINT, body, { headers })
      );
      
      if (response?.choices?.[0]?.message?.content) {
        return response.choices[0].message.content.trim();
      } else {
        console.error('Groq API yanıtı geçersiz:', response);
        return 'Yanıt alınamadı. Lütfen tekrar deneyin.';
      }
    } catch (error: any) {
      console.error('Groq API Hatası:', error);
      
      if (error.status === 401) {
        return 'API anahtarı geçersiz. Lütfen sistem yöneticisi ile iletişime geçin.';
      } else if (error.status === 429) {
        return 'API istek limiti aşıldı. Lütfen biraz bekleyip tekrar deneyin.';
      } else if (error.status >= 500) {
        return 'Sunucu hatası. Lütfen daha sonra tekrar deneyin.';
      } else {
        return 'AI yanıtı alınamadı. Kredi hesaplama için aylık gelirinizi, kredi tutarınızı ve kredi türünüzü belirtiniz.';
      }
    }
  }
}
