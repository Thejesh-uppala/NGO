import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ToastPayload } from '../../models/toast-payload.model';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss']
})
export class ToastComponent implements OnInit {

  //toastPayload: ToastPayload;
  constructor(private toastService: ToastService, private messageService: MessageService) { }

  ngOnInit(): void {
    // this.toastService.toast.subscribe(payload => {
    //   if (payload) {
    //     this.toastPayload = payload;
    //     this.messageService.clear();
    //     this.messageService.add({ severity: payload.severity, summary: payload.title, detail: payload.content });
    //   }
    // });
  }
}
