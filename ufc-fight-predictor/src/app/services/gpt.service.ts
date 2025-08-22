export interface GPTResponse {
  winner: string;
  confidence: number;
  reasoning: string;
}

export class GPTService {
  private readonly apiKey = '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';
  private readonly apiUrl = 'https://api.openai.com/v1/chat/completions';

  async predictWinner(fighter1: any, fighter2: any): Promise<GPTResponse> {
    const prompt = this.createGPTPrompt(fighter1, fighter2);
    return await this.callGPTAPI(prompt);
  }

  private createGPTPrompt(fighter1: any, fighter2: any): string {
    const f1 = fighter1;
    const f2 = fighter2;
    
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

  private async callGPTAPI(prompt: string): Promise<GPTResponse> {
    const response = await fetch(this.apiUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.apiKey}`
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