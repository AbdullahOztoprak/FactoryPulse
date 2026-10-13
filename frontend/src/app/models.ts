export type MachineStatus = 'Running' | 'Idle' | 'Fault';
export type IncidentStatus = 'Open' | 'Acknowledged' | 'Resolved';
export type Severity = 'Low' | 'Medium' | 'High';

export interface Overview {
  machineCount: number;
  runningCount: number;
  idleCount: number;
  faultCount: number;
  availabilityPercent: number;
  unitsToday: number;
  openIncidents: number;
  updatedAtUtc: string;
  dataSource: string;
}
export interface Machine {
  id: string;
  code: string;
  name: string;
  line: string;
  status: MachineStatus;
  updatedAtUtc: string;
  latestTemperatureC: number | null;
  latestVibrationMmS: number | null;
  lastReadingAtUtc: string | null;
}
export interface Reading {
  recordedAtUtc: string;
  temperatureC: number;
  vibrationMmS: number;
  unitsProduced: number;
}
export interface Incident {
  id: string;
  machineId: string;
  machineCode: string;
  title: string;
  description: string;
  severity: Severity;
  status: IncidentStatus;
  createdAtUtc: string;
  resolvedAtUtc: string | null;
}
export interface CreateIncident {
  machineId: string;
  title: string;
  description: string;
  severity: Severity;
}
