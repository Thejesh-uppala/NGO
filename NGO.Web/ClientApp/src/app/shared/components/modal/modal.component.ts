import { Component, OnInit, Input } from '@angular/core';
 
import { BootstrapNgModalService } from 'bootstrap-ng-modal';
 
@Component({
    selector: 'app-modal',
    templateUrl: './modal.component.html',
    styleUrls: ['./modal.component.css']
})
export class ModalOneComponent implements OnInit {
 
    @Input() user: any;
 
    constructor(public modalService: BootstrapNgModalService) { }
 
    ngOnInit() {
    }
 
}