import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom, forkJoin } from 'rxjs';
import { ApiService } from './api.service';
import { Incident, IncidentStatus, Machine, Overview, Reading, Severity } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html'
})
export class App implements OnInit, OnDestroy {
  private readonly api = inject(ApiService);
  private refreshTimer?: ReturnType<typeof setInterval>;

  readonly overview = signal<Overview | null>(null);
  readonly machines = signal<Machine[]>([]);
  readonly incidents = signal<Incident[]>([]);
  readonly readings = signal<Reading[]>([]);
  readonly selectedMachineId = signal<string | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly toast = signal('');
  readonly showIncidentForm = signal(false);

  formMachineId = '';
  formTitle = '';
  formDescription = '';
  formSeverity: Severity = 'Medium';

  readonly selectedMachine = computed(() =>
    this.machines().find(m => m.id === this.selectedMachineId()) ?? null);
  readonly activeIncidents = computed(() =>
    this.incidents().filter(i => i.status !== 'Resolved').length);

  readonly chartLine = computed(() => {
    const rows = this.readings();
    if (rows.length === 0) return '';
    return rows.map((r, i) => {
      const x = rows.length === 1 ? 0 : (i / (rows.length - 1)) * 680;
      const y = 146 - Math.max(0, Math.min(100, r.temperatureC)) * 1.25;
      return `${i === 0 ? 'M' : 'L'} ${x.toFixed(1)} ${y.toFixed(1)}`;
    }).join(' ');
  });
  readonly chartArea = computed(() => this.chartLine() ? `${this.chartLine()} L 680 150 L 0 150 Z` : '');

  ngOnInit(): void {
    void this.refresh();
    this.refreshTimer = setInterval(() => void this.refresh(), 15000);
  }
  ngOnDestroy(): void { if (this.refreshTimer) clearInterval(this.refreshTimer); }

  async refresh(): Promise<void> {
    try {
      const result = await firstValueFrom(forkJoin({
        overview: this.api.overview(), machines: this.api.machines(), incidents: this.api.incidents()
      }));
      this.overview.set(result.overview);
      this.machines.set(result.machines);
      this.incidents.set(result.incidents);
      this.error.set('');
      const selected = this.selectedMachineId();
      if (!selected && result.machines.length) {
        await this.selectMachine(result.machines[0].id);
      } else if (selected) {
        await this.loadReadings(selected);
      }
    } catch {
      this.error.set('Cannot reach the API. Check that the .NET backend is running.');
    } finally {
      this.loading.set(false);
    }
  }

  async selectMachine(id: string): Promise<void> {
    this.selectedMachineId.set(id);
    this.formMachineId = id;
    await this.loadReadings(id);
  }
  private async loadReadings(id: string): Promise<void> {
    try { this.readings.set(await firstValueFrom(this.api.readings(id))); }
    catch { this.readings.set([]); }
  }

  openIncidentForm(): void {
    this.formMachineId = this.selectedMachineId() ?? this.machines()[0]?.id ?? '';
    this.showIncidentForm.set(true);
  }

  async createIncident(event: Event): Promise<void> {
    event.preventDefault();
    if (!this.formMachineId || !this.formTitle.trim() || !this.formDescription.trim()) {
      this.toast.set('Please complete all incident fields.'); return;
    }
    try {
      await firstValueFrom(this.api.createIncident({
        machineId: this.formMachineId,
        title: this.formTitle.trim(),
        description: this.formDescription.trim(),
        severity: this.formSeverity
      }));
      this.showIncidentForm.set(false);
      this.formTitle = ''; this.formDescription = '';
      this.toast.set('Incident created.');
      await this.refresh();
    } catch { this.toast.set('Could not create incident.'); }
  }

  async advanceIncident(incident: Incident): Promise<void> {
    const next: IncidentStatus = incident.status === 'Open' ? 'Acknowledged' : 'Resolved';
    try {
      await firstValueFrom(this.api.changeIncidentStatus(incident.id, next));
      this.toast.set(`Incident changed to ${next}.`);
      await this.refresh();
    } catch { this.toast.set('Status update failed.'); }
  }
}
