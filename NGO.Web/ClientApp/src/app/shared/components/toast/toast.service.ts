import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { ToastPayload } from '../../models/toast-payload.model';

@Injectable({ providedIn: 'root' })
export class ToastService {
   // public toast: BehaviorSubject<ToastPayload> = new BehaviorSubject<ToastPayload>(null);

    raiseWarning(payload: ToastPayload): void {
        payload.severity = 'warn';
        //this.toast.next(payload);
    }

    raiseSuccess(payload: ToastPayload): void {
        payload.severity = 'success';
        //this.toast.next(payload);
    }

    raiseError(payload: ToastPayload): void {
        payload.severity = 'error';
        //this.toast.next(payload);
    }

    raiseInfo(payload: ToastPayload): void {
        payload.severity = 'info';
        //this.toast.next(payload);
    }
}
