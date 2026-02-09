import { Routes } from '@angular/router';
import { HesaplamaFormComponent } from './components/hesaplama-form/hesaplama-form';

export const routes: Routes = [
  { path: '', component: HesaplamaFormComponent },
  { path: '**', redirectTo: '' }
];
