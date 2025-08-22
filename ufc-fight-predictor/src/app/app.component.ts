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

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  searchFighter1() {
    if (!this.fighter1Name.trim()) return;
    
    console.log('=== SEARCH FIGHTER 1 START ===');
    console.log('Searching for:', this.fighter1Name);
    console.log('Current fighter1 before search:', this.fighter1);
    
    this.http.get<any[]>(`http://localhost:5294/api/Fighter/search?name=${this.fighter1Name}`)
      .subscribe({
        next: (fighters) => {
          console.log('=== API RESPONSE RECEIVED ===');
          console.log('Raw fighters array:', fighters);
          console.log('Fighters type:', typeof fighters);
          console.log('Fighters length:', fighters?.length);
          
          if (fighters && fighters.length > 0) {
            console.log('=== SETTING FIGHTER 1 ===');
            console.log('First fighter object:', fighters[0]);
            console.log('First fighter type:', typeof fighters[0]);
            console.log('First fighter keys:', Object.keys(fighters[0]));
            
            this.fighter1 = fighters[0];
            
            // Force change detection
            this.cdr.detectChanges();
            
            console.log('Change detection triggered');
            
            console.log('=== AFTER SETTING FIGHTER 1 ===');
            console.log('this.fighter1:', this.fighter1);
            console.log('this.fighter1.Name:', this.fighter1?.Name);
            console.log('this.fighter1.Nickname:', this.fighter1?.Nickname);
            console.log('this.fighter1[\'Division Body\']:', this.fighter1?.['Division Body']);
            
            this.error = '';
          } else {
            console.log('No fighters found in response');
            this.error = 'No fighters found';
          }
        },
        error: (err) => {
          console.error('=== API ERROR ===');
          console.error('Error object:', err);
          console.error('Error message:', err.message);
          this.error = `Error: ${err.message}`;
        }
      });
  }

  searchFighter2() {
    if (!this.fighter2Name.trim()) return;
    
    console.log('Searching for fighter 2:', this.fighter2Name);
    
    this.http.get<any[]>(`http://localhost:5294/api/Fighter/search?name=${this.fighter2Name}`)
      .subscribe({
        next: (fighters) => {
          console.log('API response received:', fighters);
          if (fighters && fighters.length > 0) {
            this.fighter2 = fighters[0];
            console.log('Fighter 2 set to:', this.fighter2);
            console.log('Fighter 2 name:', this.fighter2.Name);
            console.log('Fighter 2 nickname:', this.fighter2.Nickname);
            this.error = '';
          } else {
            console.log('No fighters found');
            this.error = 'No fighters found';
          }
        },
        error: (err) => {
          console.error('API error:', err);
          this.error = `Error: ${err.message}`;
        }
      });
  }

  predictWinner() {
    if (!this.fighter1 || !this.fighter2) return;
    
    this.http.post<any>('http://localhost:5294/api/Fighter/predict', {
      Fighter1: this.fighter1,
      Fighter2: this.fighter2
    }).subscribe({
      next: (result) => {
        this.prediction = result;
        this.error = '';
      },
      error: (err) => {
        this.error = `Prediction error: ${err.message}`;
      }
    });
  }
}
