import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogPayload } from '../../models/confirm-dialog-payload.model';
@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  @Input()
  confirmDialogPayload!: ConfirmDialogPayload;
  @Output() confirm = new EventEmitter();
  @Output() cancel = new EventEmitter();

  constructor(
    private confirmationService: ConfirmationService) { }

  ngOnInit(): void {
    this.confirmationService.confirm({
      message: this.confirmDialogPayload.message,
      header: this.confirmDialogPayload.header,
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.confirm.emit();
      },
      reject: () => {
        this.cancel.emit();
      }
    });
  }
}
