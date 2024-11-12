import { Component, OnInit, } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Table } from 'primeng/table';
interface Customer {
  id: number;
  name: string;
  country: {
    name: string;
    code: string;
  };
  company: string;
  status: string;
  verified: boolean;
  activity: number;
  representative: {
    name: string;
    image: string;
  };
  balance: number;
}

interface Representative {
  name: string;
  image: string;
}
@Component({
  selector: 'app-registersecond',
  templateUrl: './registersecond.component.html',
  styleUrl: './registersecond.component.css'
})
export class RegistersecondComponent implements OnInit{
  customers!: Customer[];

  representatives!: Representative[];

  statuses!: any[];

  loading: boolean = true;

  activityValues: number[] = [0, 100];

  searchValue: string | undefined;

  constructor(private modalService: NgbModal) {}
  ngOnInit() {

  }

 members: any[] = [
    { name: 'John Doe', membershipExpiryDate: '2024-12-31', email: 'john.doe@example.com', currentPlan: 'Premium', status: 'Active' },
    { name: 'Jane Smith', membershipExpiryDate: '2025-06-30', email: 'jane.smith@example.com', currentPlan: 'Standard', status: 'Active' },
    { name: 'Sam Wilson', membershipExpiryDate: '2024-05-15', email: 'sam.wilson@example.com', currentPlan: 'Basic', status: 'Inactive' },
    { name: 'Sara Conner', membershipExpiryDate: '2024-11-20', email: 'sara.conner@example.com', currentPlan: 'Premium', status: 'Active' }
  ];
  openModal(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }
  

}