import { Component, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-registersecond',
  templateUrl: './registersecond.component.html',
  styleUrl: './registersecond.component.css'
})
export class RegistersecondComponent {
  constructor(private modalService: NgbModal) {}

  openModal(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }
  

}
