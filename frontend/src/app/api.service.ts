import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateIncident, Incident, IncidentStatus, Machine, Overview, Reading } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  overview(): Observable<Overview> { return this.http.get<Overview>(`${this.base}/overview`); }
  machines(): Observable<Machine[]> { return this.http.get<Machine[]>(`${this.base}/machines`); }
  readings(machineId: string): Observable<Reading[]> {
    return this.http.get<Reading[]>(`${this.base}/machines/${machineId}/readings?limit=30`);
  }
  incidents(): Observable<Incident[]> { return this.http.get<Incident[]>(`${this.base}/incidents`); }
  createIncident(payload: CreateIncident): Observable<{ id: string; status: IncidentStatus }> {
    return this.http.post<{ id: string; status: IncidentStatus }>(`${this.base}/incidents`, payload);
  }
  changeIncidentStatus(id: string, status: IncidentStatus): Observable<{ id: string; status: IncidentStatus }> {
    return this.http.patch<{ id: string; status: IncidentStatus }>(`${this.base}/incidents/${id}/status`, { status });
  }
}
