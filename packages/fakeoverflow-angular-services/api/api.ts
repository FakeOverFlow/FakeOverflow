export * from './auth.service';
import { AuthService } from './auth.service';
export * from './me.service';
import { MeService } from './me.service';
export const APIS = [AuthService, MeService];
