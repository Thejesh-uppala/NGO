import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ICreateOrderRequest, IPayPalConfig } from 'ngx-paypal';
import { Subscription } from 'rxjs';
import { MessageService } from "primeng/api";
import { UserDetailModel } from '../userprofile/models/UpdateUserModel';
import { DashBoardService } from './services/dashBoardService';
import { Customer, Representative } from './services/product';
import { Table } from 'primeng/table';

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
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashBoardComponent implements OnInit {
  public datasets: any;
  public salesChart: any;
  public chartOrders: any;
  public chartSales: any;
  public userId: any;
  public clicked: boolean = true;
  public clicked1: boolean = false;
  public image: any;
  subscriptions: Subscription[] = [];
  leads!: UserDetailModel[];
  public payPalConfig?: IPayPalConfig;
  userDetail: UserDetailModel = new UserDetailModel();
  filteredChartDetails: UserDetailModel[] = [];
  chartBool: boolean = false;
  currentUser: any;
  totalRecords: number = 0;
  multiAxisData: any;
  basicData: any;
  multiAxisOptions: any;

  //test
  customers!: Customer[];
  userDetail1!: UserDetailModel[];
  copyUserDetails!: UserDetailModel[]
  representatives!: Representative[];
  statuses!: any[];
  loading: boolean = true;
  activityValues: number[] = [0, 100];
  displayBasic: boolean = false;
  displayBasic2: boolean = false;
  data: any;
  chartDataSetValue: number[] = [];
  chartLableData: string[] = [];
  organizationData!: DropDownData[];
  warningColor:string="";
  newData: ChartValue[] = [];
  barchartColors:any
  payemetDate!:string 
  paymentId!:string
  payerId!:string;
  @ViewChild('paypalRef', { static: true })
  private paypalRef!: ElementRef;
  constructor(private router: Router,
    private messageService: MessageService,
    private dashBoradService: DashBoardService) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (this.currentUser.userRole == "Administrator") {
      this.router.navigateByUrl("/admin");
    }
  }

  ngOnInit(): void {
    this.getAllUsersDetails();
    this.getUsersDetails();
    this.getOrganizations();
    //this.initConfig();
    this.paypalConfig();
    
  }
  colorShow(){
      let currentDate = new Date();
      if(this.userDetail.paymentDate!=null){
      var newDate = new Date(this.userDetail.paymentDate);
      var res=Math.floor((Date.UTC(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate()) - Date.UTC(newDate.getFullYear(), newDate.getMonth(), newDate.getDate()) ) /(1000 * 60 * 60 * 24));
      if(res<=45){
        this.warningColor="pink"
      }else if(res<=30){
        this.warningColor="yellow"
      }else if(res<=15){
        this.warningColor="green"
      }else if(res<=10){
        this.warningColor="#DC3912"
      }
    }
  }
  clear(table: Table) {
    table.clear();
    this.chartBool = false
    this.filteredChartDetails=[]
    this.userDetail1=[]
    this.userDetail1=this.copyUserDetails;
  }
  private configureDefaultColours(data: number[]): string[] {
    let customColours: string[] = []
    if (data.length) {

      customColours = data.map((element, idx) => {
        return DEFAULT_COLORS[idx % DEFAULT_COLORS.length];
      });
    }

    return customColours;
  }
  
  selectData(e: any) {
    this.chartBool = true;
    this.filteredChartDetails = this.userDetail1.filter(x => x.country == this.chartLableData[e.element.index]);
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
  edit() {
    this.router.navigate(["/user-profile"]);
  }
  renew() {
    this.displayBasic = true;
  }
  paypalConfig() {
    console.log("window",window)
    window.paypal.Buttons({
      style: {
        layout: 'horizontal',
        shape: 'rect',
        label: 'paypal',
        size: 'responsive',
        height: 40,
      },
      createOrder: function(data:any, actions:any) {
        return actions.order.create({
          purchase_units: [{
            amount: {
              value: "1.00",
              currency_code: "USD"
            }
          }]
        });
      },
      onApprove: (data:any, actions:any) => {
        actions.order.capture().then((x: any) => {
          this.payemetDate=x.create_time;
          this.paymentId=x.id;
          this.payerId=x.payer.payer_id;
        });
      }, 
      onCancel: function (data:any) {  
        // Show a cancel page, or return to cart  
        console.log("onCancel",data);  
      },  
      onError: function (err:any) {  
        // Show an error page here, when an error occurs  
        console.log("onError",err);  
      }  
    }).render(this.paypalRef.nativeElement);

  }
  charUpdate() {
    this.multiAxisData = {
      labels: this.chartLableData,
      datasets: [{
        label: 'Peak Value',
        backgroundColor:  this.barchartColors,
        yAxisID: 'y',
        data: this.chartDataSetValue
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
        // y1: {
        //   type: 'linear',
        //   display: true,
        //   position: 'right',
        //   grid: {
        //     drawOnChartArea: false,
        //     color: '#ebedef'
        //   },
        //   ticks: {
        //     min: 0,
        //     max: 100,
        //     color: '#495057'
        //   }
        // }
      }
    };
  }
  getOrganizations() {
    this.subscriptions.push(
      this.dashBoradService
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
  getAllUsersDetails() {
    this.subscriptions.push(
      this.dashBoradService
        .getAllUserDetails().subscribe(
          {
            next: (res) => {
              this.userDetail1 = res.userslist;
              this.copyUserDetails=res.userslist
              this.loading = false;
              this.getChartData()
              this.charUpdate();
            },
            error: () => {
              this.showError();
            }
          },
        )
    );
  }

  private initConfig(): void {
    this.payPalConfig = {
      currency: 'EUR',
      clientId: 'sb',
      createOrderOnClient: (data) => <ICreateOrderRequest>{
        intent: 'CAPTURE',
        purchase_units: [{
          amount: {
            currency_code: 'EUR',
            value: '9.99',
            breakdown: {
              item_total: {
                currency_code: 'EUR',
                value: '9.99'
              }
            }
          },
          items: [{
            name: 'Enterprise Subscription',
            quantity: '1',
            category: 'DIGITAL_GOODS',
            unit_amount: {
              currency_code: 'EUR',
              value: '9.99',
            },
          }]
        }]
      },
      advanced: {
        commit: 'true'
      },
      style: {
        label: 'paypal',
        layout: 'vertical'
      },
      onApprove: (data, actions) => {
        console.log('onApprove - transaction was approved, but not authorized', data, actions);
        actions.order.get().then((details: any) => {
          console.log('onApprove - you can get full order details inside onApprove: ', details);
        });

      },
      onClientAuthorization: (data) => {
        console.log('onClientAuthorization - you should probably inform your server about completed transaction at this point', data);
        // this.showSuccess = true;
      },
      onCancel: (data, actions) => {
        console.log('OnCancel', data, actions);
        //this.showCancel = true;

      },
      onError: err => {
        console.log('OnError', err);
        // this.showError = true;
      },
      onClick: (data, actions) => {
        console.log('onClick', data, actions);
        //this.resetStatus();
      }
    };
  }
  contactMe(event: any) {
    debugger
    this.showInfo();
    this.subscriptions.push(
      this.dashBoradService
        .contactMe(event, this.currentUser.userId).subscribe(
          {
            next: (res) => {
              this.onReject();
              this.showSuccess();
            },
            error: () => {
              this.onReject();
              this.showError();
            }
          },
        )
    );
  }
  onLazyLoad(event: any) {
    debugger
    //this.chartBool=false; 
  }
  getChartData() {
    debugger
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
            var r = this.newData.findIndex(x => x.name == this.userDetail1[j].country)
            this.newData[i].count = this.newData[i].count + 1
            //this.chartDataSetValue.push(this.newData[i].count)
            this.chartDataSetValue[i]=this.newData[i].count
          }
        }
      }
    }
    console.log(this.chartDataSetValue)
    this.barchartColors = this.configureDefaultColours(this.chartDataSetValue);
  }
  getUsersDetails() {
    this.subscriptions.push(
      this.dashBoradService
        .getUserDetails(this.currentUser.userId).subscribe(
          {
            next: (res) => {
              this.userDetail = res.userslist;
              this.image = 'data:image/png;base64,' + res.userslist.photo;
              this.colorShow();
            },
            error: () => {

            }
          },
        )
    );
  }

}
