//admin.component.ts
import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { Dropdown } from 'primeng/dropdown';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { PaymentModel, UserDetailModel } from '../userprofile/models/UpdateUserModel';
import { AdminService } from './services/admin.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
interface DropDownData {
  name: string,
  code: string
}
interface ChartValue {
  name: string,
  count: number
}
const DEFAULT_COLORS = ['#3366CC', '#DC3912', '#FF9900', '#109618', '#990099',
  '#3B3EAC', '#0099C6', '#DD4477', '#66AA00', '#B82E2E',
  '#316395', '#994499', '#22AA99', '#AAAA11', '#6633CC',
  '#E67300', '#8B0707', '#329262', '#5574A6', '#3B3EAC']
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styles: [`
  :host ::ng-deep .p-button {
      margin: 0 .5rem 0 0;
      min-width: 10rem;
  }

  p {
      margin: 0;
  }

  .confirmation-content {
      display: flex;
      align-items: center;
      justify-content: center;
  }

  :host ::ng-deep .p-dialog .p-button {
      min-width: 6rem;
  }
`]
})
export class AdminComponent implements OnInit {
  subscriptions: Subscription[] = [];
  userDetail: UserDetailModel[] = [];
  copyUserDetails!: UserDetailModel[]
  filteredChartDetails: UserDetailModel[] = [];
  filteredExpiryDetails: UserDetailModel[] = [];
  chartBool: boolean = false;
  loading: boolean = true;
  displayModal: boolean = false;
  displayBasic: boolean = false;
  displayBasic2: boolean = false;
  displayMaximizable: boolean = false;
  displayPosition: boolean = false;
  position: string = "";
  paymentDetails: PaymentModel[] = [];
  cars!: SelectItem[];
  memberId: string = "";
  chapterId: string = "";
  customerId: number = 0;
  paymentData: string[] = []
  data: any;
  userDetail1!: UserDetailModel[]
  chapterData!: DropDownData[];
  selectedChapterData!: DropDownData;
  organizationData!: DropDownData[];
  selectedOrganizationData!: DropDownData;
  @ViewChild('dropDownThing')
  dropDownThing!: Dropdown;
  chartDataSetValue: number[] = [];
  chartLableData: string[] = [];
  multiAxisData: any;
  multiAxisOptions: any;
  representatives: any;
  expiresIn: any;
  newData: ChartValue[] = [];
  barchartColors:any
  constructor(private adminService: AdminService, private messageService: MessageService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.GetAllUsers();
    this.getAllUsersDetails();
    this.getChapters();
    this.getOrganizations();
    this.expiresIn = [
      { name: 45, id: '1' },
      { name: 30, id: '2' },
      { name: 15, id: '3' },
      { name: 10, id: '4' },
    ];
  }
  clear(table: Table) {
    table.clear();
    this.chartBool = false
    this.filteredChartDetails = [];
    this.userDetail = this.copyUserDetails;
  }
  private configureDefaultColours(data: number[]): string[] {
    debugger
    let customColours: string[] = []
    if (data.length) {
      customColours = data.map((element, idx) => {
        return DEFAULT_COLORS[idx % DEFAULT_COLORS.length];
      });
    }
    return customColours;
  }
  
  getOrganizations() {
    this.subscriptions.push(
      this.adminService
        .getOrganizations().subscribe(
          {
            next: (res) => {
              this.organizationData = res;
            },
            error: () => {

            }
          },
        )
    )
  }
  getChapters() {
    this.subscriptions.push(
      this.adminService
        .getChapters().subscribe(
          {
            next: (res) => {
              debugger
              this.chapterData = res;
            },
            error: () => {

            }
          },
        )
    )
  }
  selectData(e: any) {
    this.chartBool = true;
    console.log("this.chartLableData[e.index]", this.chartLableData[e.index]);
    this.filteredChartDetails = this.userDetail.filter(x => x.country == this.chartLableData[e.element.index]);
    console.log(e.element.index);
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Mail sent Successfully' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Something Went Wrong!' });
  }
  showInfo() {
    this.messageService.add({ severity: 'info', summary: 'Info', detail: 'Mail sending' });
  }

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  setOrgChapter(id: any) {
    debugger
    this.displayBasic = true;
    this.customerId = id;
    var res = this.userDetail.find(x => x.userId.toString() == id);
    this.memberId = res ? res.memberId : "";

  }
  viewReminder(id: any) {

  }
 
  showModalDialog(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
    this.displayModal = true;
  }

  onFilter(event: any, dt: any) {
    event.filteredValue.forEach((element: { userId: UserDetailModel }) => {
      this.paymentData.push(element?.userId.toString())
    });
  }
  showMaximizableDialog() {
    this.displayMaximizable = true;
  }
  massPaymentRem() {
    if (this.paymentData.length == 0) {
      this.userDetail.forEach((element) => {
        this.paymentData.push(element.userId.toString());
      });
    }
    this.showInfo()
    this.subscriptions.push(
      this.adminService
        .massPaymentRem(this.paymentData).subscribe(
          {
            next: (res) => {
              if (res.statusCode == 200) {
                this.onReject();
                this.showSuccess();
              }
            },
            error: () => {
              this.onReject();
              this.showError();
            }
          },
        )
    );
  }
  charUpdate() {
    this.multiAxisData = {
      labels: this.chartLableData,
      datasets: [{
        label: 'Peak Value',
        backgroundColor: this.barchartColors,
        yAxisID: 'y',
        data:this.chartDataSetValue
      }]
    };
    this.multiAxisOptions = {
      plugins: {
        legend: {
          labels: {
            color: '#495057'
          }
        },
        tooltips: {
          mode: 'index',
          intersect: true
        }
      },
      scales: {
        x: {
          ticks: {
            color: '#495057'
          },
          grid: {
            color: '#ebedef'
          }
        },
        y: {
          type: 'linear',
          display: true,
          position: 'left',
          ticks: {
            min: 0,
            max: 100,
            color: '#495057'
          },
          grid: {
            color: '#ebedef'
          }
        },
      }
    }
  }
  getChartData() {
    var temp: string[] = [];
    this.userDetail1.forEach(element => {
      temp.push(element.country.toString());
    });
    var mySet = new Set(temp);
    this.chartLableData = [...mySet]
    for (let i = 0; i < this.chartLableData.length; i++) {
      this.newData.push({ name: this.chartLableData[i], count: 0 })
      for (let j = i; j < this.userDetail1.length; j++) {
        if (this.chartLableData[i] == this.userDetail1[j].country) {
          if (this.newData.length > 0) {
            var r = this.newData.findIndex(x => x.name == this.userDetail1[i].country)
            this.newData[i].count = this.newData[i].count + 1
            //this.chartDataSetValue.push(this.newData[i].count)
            this.chartDataSetValue[i]=this.newData[i].count
          }
        }
      }
    }
    this.barchartColors = this.configureDefaultColours(this.chartDataSetValue);
    console.log(this.newData)
  }
  getAllUsersDetails() {
    this.subscriptions.push(
      this.adminService
        .getAllUserDetails().subscribe(
          {
            next: (res) => {
              this.userDetail1 = res.userslist;
              this.getChartData()
              this.loading = false;
              this.charUpdate();
            },
            error: () => {

            }
          },
        )
    );
  }
  expireFilter(e: any) {
    if (e.value.name) {
      this.userDetail.forEach(element => {
        let currentDate = new Date();
        if (element.paymentDate != null) {
          var newDate = new Date(element.paymentDate);
          var res = Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(newDate.getFullYear(), newDate.getMonth(), newDate.getDate())) / (1000 * 60 * 60 * 24));
          if (res <= e.value.name) {
            this.filteredExpiryDetails.push(element);
          }
        }
      });
    }
    this.userDetail = this.filteredExpiryDetails;

  }
  saveMember() {
    debugger
    this.showInfo()
    let organizationId = this.selectedOrganizationData.code;
    let chapterId = this.selectedChapterData.code;
    if (organizationId && chapterId) {
      this.subscriptions.push(
        this.adminService
          .saveMember(this.customerId, this.memberId, organizationId, chapterId).subscribe(
            {
              next: (res) => {
                if (res.statusCode == 200) {
                  this.onReject();
                  this.showSuccess();
                }
                this.dropDownThing.clear
                // this.dropDownThing.value = "";
              },
              error: () => {
                this.onReject();
                this.showError();
              }
            },
          )
      );
    }
  }
  showPositionDialog(position: string) {
    this.position = position;
    this.displayPosition = true;
  }
  paymentReminder(id: any) {
    this.showInfo()
    this.subscriptions.push(
      this.adminService
        .paymentReminder(id).subscribe(
          {
            next: (res) => {
              if (res.statusCode == 200) {
                this.onReject();
                this.showSuccess();
              }
            },
            error: () => {
              this.onReject();
              this.showError();
            }
          },
        )
    );
  }
  getPaymentDetails(Id: any) {
    debugger

    this.subscriptions.push(
      this.adminService
        .getPaymentDetails(Id).subscribe(
          {
            next: (res) => {
              this.displayBasic2 = true;
              this.paymentDetails = res;
            },
            error: () => {
            }
          },
        )
    );
  }
  GetAllUsers() {
    this.subscriptions.push(
      this.adminService
        .getAllUsers().subscribe(
          {
            next: (res) => {
              debugger
              this.userDetail = res.userslist;
              this.copyUserDetails = res.userslist;
              this.loading = false;
            },
            error: () => {

            }
          },
        )
    );
  }

}
