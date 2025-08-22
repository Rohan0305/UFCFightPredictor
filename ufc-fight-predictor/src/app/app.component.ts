import { Component, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule]
})
export class AppComponent {
  fighter1: any = null;
  fighter2: any = null;
  fighter1Name: string = '';
  fighter2Name: string = '';
  prediction: any = null;
  error: string = '';
  isLoading: boolean = false;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  searchFighter1() {
    if (!this.fighter1Name.trim()) return;
    
    this.http.get<any>(`http://localhost:5294/api/Fighter/search?name=${this.fighter1Name}`)
      .subscribe({
        next: (data) => {
          if (data && data.length > 0) {
            this.fighter1 = data[0];
            this.error = '';
          } else {
            this.error = 'Fighter 1 not found';
          }
        },
        error: (err) => {
          this.error = `Error searching Fighter 1: ${err.message}`;
        }
      });
  }

  searchFighter2() {
    if (!this.fighter2Name.trim()) return;
    
    this.http.get<any>(`http://localhost:5294/api/Fighter/search?name=${this.fighter2Name}`)
      .subscribe({
        next: (data) => {
          if (data && data.length > 0) {
            this.fighter2 = data[0];
            this.error = '';
          } else {
            this.error = 'Fighter 2 not found';
          }
        },
        error: (err) => {
          this.error = `Error searching Fighter 2: ${err.message}`;
        }
      });
  }

  async predictWinner() {
    if (!this.fighter1 || !this.fighter2) return;
    
    this.isLoading = true;
    this.error = '';
    
    try {
      // Create a detailed prompt with both fighters' stats
      const prompt = this.createGPTPrompt();
      
      // Call your GPT API endpoint (you'll need to create this)
      const response = await this.callGPTAPI(prompt);
      
      this.prediction = {
        Winner: response.winner,
        Confidence: response.confidence,
        Reason: response.reasoning
      };
      
    } catch (err: any) {
      this.error = `Prediction error: ${err.message}`;
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  private createGPTPrompt(): string {
    const f1 = this.fighter1;
    const f2 = this.fighter2;
    
    return `Analyze this UFC fight matchup and predict the winner:

FIGHTER 1: ${f1.Name} (${f1.Nickname || 'No nickname'})
- Record: ${f1['Division Body']?.Wins || '0'}-${f1['Division Body']?.Losses || '0'}-${f1['Division Body']?.Draws || '0'}
- Age: ${f1['Fighter Bio']?.Age || 'Unknown'}
- Height: ${f1['Fighter Bio']?.Height || 'Unknown'}"
- Weight: ${f1['Fighter Bio']?.Weight || 'Unknown'} lbs
- Reach: ${f1['Fighter Bio']?.Reach || 'Unknown'}"
- Striking Accuracy: ${f1.Records?.['Striking accuracy'] || 'Unknown'}
- Strikes Landed per Min: ${f1.Records?.['Sig. Str. Landed'] || 'Unknown'}
- Strikes Absorbed per Min: ${f1.Records?.['Sig. Str. Absorbed'] || 'Unknown'}
- Striking Defense: ${f1.Records?.['Sig. Str. Defense'] || 'Unknown'}
- Takedown Avg: ${f1.Records?.['Takedown avg'] || 'Unknown'}
- Takedown Accuracy: ${f1.Records?.['Takedown Accuracy'] || 'Unknown'}
- Takedown Defense: ${f1.Records?.['Takedown Defense'] || 'Unknown'}
- KO Wins: ${f1['Win Stats']?.['Wins by Knockout'] || 'Unknown'}
- Submission Wins: ${f1['Win Stats']?.['Wins by Submission'] || 'Unknown'}
- Win Distribution: Standing ${f1['Win by Method']?.Standing || 'Unknown'}, Clinch ${f1['Win by Method']?.Clinch || 'Unknown'}, Ground ${f1['Win by Method']?.Ground || 'Unknown'}

FIGHTER 2: ${f2.Name} (${f2.Nickname || 'No nickname'})
- Record: ${f2['Division Body']?.Wins || '0'}-${f2['Division Body']?.Losses || '0'}-${f2['Division Body']?.Draws || '0'}
- Age: ${f2['Fighter Bio']?.Age || 'Unknown'}
- Height: ${f2['Fighter Bio']?.Height || 'Unknown'}"
- Weight: ${f2['Fighter Bio']?.Weight || 'Unknown'} lbs
- Reach: ${f2['Fighter Bio']?.Reach || 'Unknown'}"
- Striking Accuracy: ${f2.Records?.['Striking accuracy'] || 'Unknown'}
- Strikes Landed per Min: ${f2.Records?.['Sig. Str. Landed'] || 'Unknown'}
- Strikes Absorbed per Min: ${f2.Records?.['Sig. Str. Absorbed'] || 'Unknown'}
- Striking Defense: ${f2.Records?.['Sig. Str. Defense'] || 'Unknown'}
- Takedown Avg: ${f2.Records?.['Takedown avg'] || 'Unknown'}
- Takedown Accuracy: ${f2.Records?.['Takedown Accuracy'] || 'Unknown'}
- Takedown Defense: ${f2.Records?.['Takedown Defense'] || 'Unknown'}
- KO Wins: ${f2['Win Stats']?.['Wins by Knockout'] || 'Unknown'}
- Submission Wins: ${f2['Win Stats']?.['Wins by Submission'] || 'Unknown'}
- Win Distribution: Standing ${f2['Win by Method']?.Standing || 'Unknown'}, Clinch ${f2['Win by Method']?.Clinch || 'Unknown'}, Ground ${f2['Win by Method']?.Ground || 'Unknown'}

Based on these stats, analyze the matchup and provide:
1. Winner prediction (just the fighter's name)
2. Confidence level (0.0 to 1.0)
3. Detailed reasoning for your prediction

Format your response as JSON:
{
  "winner": "Fighter Name",
  "confidence": 0.85,
  "reasoning": "Detailed analysis..."
}`;
  }

  private async callGPTAPI(prompt: string): Promise<any> {
    // REMOVE THE API KEY - just put a placeholder
    //const apiKey = / We'll add the real one back later
    
    const apiUrl = 'https://api.openai.com/v1/chat/completions';
    
    const response = await fetch(apiUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${apiKey}`
      },
      body: JSON.stringify({
        model: 'gpt-3.5-turbo',
        messages: [
          {
            role: 'system',
            content: 'You are a UFC fight analyst. Analyze the provided fighter stats and predict the winner with confidence and reasoning.'
          },
          {
            role: 'user',
            content: prompt
          }
        ],
        temperature: 0.7,
        max_tokens: 500
      })
    });

    if (!response.ok) {
      throw new Error(`GPT API error: ${response.status}`);
    }

    const data = await response.json();
    const content = data.choices[0].message.content;
    
    try {
      return JSON.parse(content);
    } catch (e) {
      // If GPT doesn't return valid JSON, extract info manually
      const lines = content.split('\n');
      const winnerLine = lines.find((line: string) => line.toLowerCase().includes('winner:'));
      const confidenceLine = lines.find((line: string) => line.toLowerCase().includes('confidence:'));
      const reasoningLine = lines.find((line: string) => line.toLowerCase().includes('reasoning:'));

      const winner = winnerLine ? winnerLine.split(':').slice(1).join(':').trim() : 'Unknown';
      const confidence = confidenceLine ? parseFloat(confidenceLine.split(':').slice(1).join(':').trim()) : 0.5;
      const reasoning = reasoningLine ? reasoningLine.split(':').slice(1).join(':').trim() : content;

      return { winner, confidence, reasoning };
    }
  }
}
