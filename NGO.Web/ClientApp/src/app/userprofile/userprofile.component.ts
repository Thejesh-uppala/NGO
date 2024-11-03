import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { ICreateOrderRequest, IPayPalConfig } from 'ngx-paypal';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ToastService } from '../shared/components/toast/toast.service';
import { ChildrensDetailsModel, CityInterface, CountryInterface, SpouseDetails, StateInterface, UserDetailModel, UserModel } from './models/UpdateUserModel';
import { UserProfileService } from './services/userprofile.service';
import { Country, State, City }  from 'country-state-city';
import { DatePipe } from '@angular/common';

interface Reason {
  name: string,
  code: string
}
@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss']
})
export class UserprofileComponent implements OnInit {
  profileForm!: FormGroup;
  returnUrl: string = '';
  currentUser: any;
  subscriptions: Subscription[] = [];
  selectedFile!: File;
  updateUserModel = {} as UserDetailModel;
  spouseDetails = {} as SpouseDetails;
  userDetail: UserDetailModel = new UserDetailModel();
  public payPalConfig?: IPayPalConfig;
  image: any;
  countries: any
  states: any
  cities:any
  spouseCountries: any
  spouseStates:any
  spouseCities: CityInterface[]=[];
  childCountries: any
  childStates: any
  childCities: any
  countryName!:CountryInterface
  stateName!:StateInterface;
  cityName!:CityInterface;
  childCountryName:string[]=[]
  childStateName:string[]=[]
  childCityName:string[]=[];
  spouseCountryName:string="";
  spousestateName:string="";
  //childstateName:string="";
  disableSpouseFlag:boolean=false;
  disableChildFlag:boolean[]=[];
  chartDataSetValue: number[] = [10,20];
  reason:Reason[]=[];
  selectedReason!:Reason
  dateOfBirth:any
  spouseDob:any
  constructor(private router: Router,
    private toastService: ToastService,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
    private userprofileService: UserProfileService) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.profileForm = this.formBuilder.group({
      firstName: ["", Validators.required],
      emailAddress: ["", Validators.required],
      lastName: ["", Validators.required],
      dOB: ["", Validators.required],
      address: ["", Validators.required],
      city: ["", Validators.required],
      country: ["", Validators.required],
      state: ["", Validators.required],
      postalCode: ["", Validators.required],
      whatsAppNumber: ["", Validators.required],
      phoneNumber: ["",[Validators.required,  Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      homeTown: ["", Validators.required],
      familyName: ["", Validators.required],
      refference: ["", Validators.required],
      Photo: ["", Validators.required],
      reason: ["", Validators.required],
      spouseFirstName: ["", Validators.required],
      spouseEmail: ["", Validators.required],
      spouseLastName: ["", Validators.required],
      spousePhoneNumber: ["", Validators.required],
      spouseDob: ["", Validators.required],
      spouseState: ["", Validators.required],
      spouseCity: ["", Validators.required],
      spouseCountry: ["", Validators.required],
      spouseHometown: ["", Validators.required],
      childrensDetails: this.formBuilder.array([])
    });
    this.getUserDetaisl();
    this.initConfig();
    this.GetAllCoutries();
    this.reason = [
      {name: 'Option 1', code: 'op1'},
      {name: 'Option 2', code: 'op2'},
      {name: 'Option 3', code: 'op3'},
      {name: 'Option 4', code: 'op4'},
      {name: 'Option 5', code: 'op5'}
  ];

  }
  spouseSameAddress(e:any){
    
    if(e.currentTarget.checked){
      this.disableSpouseFlag=e.currentTarget.checked;
      this.spouseStates=this.states
      this.spouseCities=this.cities
      this.cityName=this.profileForm.value.city
      this.stateName=this.profileForm.value.state
      this.countryName=this.profileForm.value.country
    }else{
      this.disableSpouseFlag=e.currentTarget.checked;
      this.spouseStates=[]
      this.spouseCities=[]
    }
  }
  childSameAddress(e:any,i:number){
    if(e.currentTarget.checked){
      this.disableChildFlag[i]=e.currentTarget.checked;
      this.childStates=this.states
      this.childCities=this.cities
      this.childCityName[i]=this.profileForm.value.city
      this.childStateName[i]=this.profileForm.value.state
      this.childCountryName[i]=this.profileForm.value.country
    }else{
      this.disableChildFlag[i]=e.currentTarget.checked;
      this.childStates=[]
      this.childCities=[]
    }
  } 
  get f(){  
    return this.profileForm.controls;  
  } 
  
  get childrensDetails(): FormArray {
    return this.profileForm.controls["childrensDetails"] as FormArray;
  }
  
  addChildrens() {
    this.childrensDetails.push(this.newChild());
  }
  newChild(): FormGroup {
    return this.formBuilder.group({
      childFirstName: ["", Validators.required],
      childUniqueId:[""],
      childEmailAddress: ["", Validators.required],
      childLastName: ["", Validators.required],
      childPhoneNumber: ["", Validators.required],
      childState: ["", Validators.required],
      childCity: ["", Validators.required],
      childCountry: ["", Validators.required],
      childDOB: ["", Validators.required],
    })
  }
  removeChild(i: number) {
    this.childrensDetails.removeAt(i);
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Profile Updated Successfully' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Something Went Wrong!' });
  }
  showInfo() {
    this.messageService.add({ severity: 'info', summary: 'Info', detail: 'Profile Saving in Progress' });
  }
  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  private generateDatumFormGroup(child: any) {
    return this.formBuilder.group({
      childFirstName: this.formBuilder.control({ value: child.childFirstName }),
      // subCategory: this.formBuilder.control({ value: datum.SubCategory, disabled: true }),
      // skill: this.formBuilder.control({ value: datum.Skill, disabled: true }),
      // skillId: this.formBuilder.control({ value: datum.SkillID, disabled: true })
    });
  }
  setExistingChild(child: any): FormArray {
    const formArray = new FormArray<FormGroup>([]);
    child.forEach((s:any) => {
      //this.childCountryName= s.childCountry;
      formArray.push(this.formBuilder.group({
        childFirstName: s.childFirstName,
        childEmailAddress: s.childEmailAddress,
        childLastName: s.childLastName,
        childPhoneNumber: s.childPhoneNumber,
        childState: s.childState,
        childCity: s.childCity,
        childCountry: s.childCountry,
        childDOB:this.formatDate(s.childDOB)
      }));
    });
  
    return formArray;
  }
  private formatDate(date:any) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }
  getUserDetaisl() {
    this.subscriptions.push(
      this.userprofileService
        .getCurrentUserProfile(this.currentUser.userId).subscribe(
          {
            next: (res) => {
              this.profileForm.patchValue(
                res.userslist
              );
              debugger
              this.profileForm.patchValue({
                spouseFirstName: res.userslist.spouseDetails.spouseFirstName,
                spouseCountry: res.userslist.spouseDetails.spouseCountry,
                spouseCity: res.userslist.spouseDetails.spouseCity,
                spouseDob: res.userslist.spouseDetails.spouseDob,
                spouseEmail: res.userslist.spouseDetails.spouseEmail,
                spouseHometown: res.userslist.spouseDetails.spouseHometown,
                spouseLastName: res.userslist.spouseDetails.spouseLastName,
                spousePhoneNumber: res.userslist.spouseDetails.spousePhoneNumber,
                spouseState: res.userslist.spouseDetails.spouseState,
              })
              var date=this.formatDate(res.userslist.dob);
              var spouseDob=this.formatDate(res.userslist.spouseDetails.spouseDob);
              this.dateOfBirth=date
              this.spouseDob=spouseDob
              //this.profileForm.controls['spouseCountry'].setValue(res.userslist.spouseDetails.spouseCountry);
             // this.profileForm.controls['spouseCity'].setValue(res.userslist.spouseDetails.spouseCity);

              // this.profileForm = this.formBuilder.group({
              //   childrensDetails: this.formBuilder.array( res.userslist.childrensDetails.map((child: any) => this.generateDatumFormGroup(child)))
              // });
             this.profileForm.setControl('childrensDetails', this.setExistingChild(res.userslist.childrensDetails)); 
             this.userDetail = res.userslist;
              this.image = 'data:image/png;base64,' + res.userslist.photoData;
            },
            error: () => {

            }
          },
        )
    );
  }
  composeModel(): void {
    this.updateUserModel.firstName = this.profileForm.value.firstName;
    this.updateUserModel.lastName = this.profileForm.value.lastName;
    this.updateUserModel.socialMedia = this.profileForm.value.socialMedia;
    this.updateUserModel.state = this.stateName.name? this.stateName.name:"";
    this.updateUserModel.dob = this.profileForm.value.dOB;
    this.updateUserModel.city = this.profileForm.value.city.name;
    this.updateUserModel.country = this.countryName.name? this.countryName.name:"";
    this.updateUserModel.homeTown = this.profileForm.value.homeTown;
    this.updateUserModel.whatsAppNumber = this.profileForm.value.whatsAppNumber;
    this.updateUserModel.phoneNumber = this.profileForm.value.phoneNumber;
    this.updateUserModel.reason = this.profileForm.value.reason.name;
    this.updateUserModel.email = this.profileForm.value.emailAddress;
    this.updateUserModel.userId = this.currentUser.userId;
    this.updateUserModel.address = this.profileForm.value.address;
    this.updateUserModel.refference = this.profileForm.value.refference;
    this.updateUserModel.postalCode = this.profileForm.value.postalCode;
    this.updateUserModel.emailAddress = this.profileForm.value.emailAddress;
    this.updateUserModel.familyName = this.profileForm.value.familyName;
    //Spouse Details
    this.spouseDetails.spouseFirstName = this.profileForm.value.spouseFirstName;
    this.spouseDetails.spouseCountry = this.profileForm.value.spouseCountry.name;
    this.spouseDetails.spouseCity = this.profileForm.value.spouseCity.name;
    this.spouseDetails.spouseDob = this.profileForm.value.spouseDob;
    this.spouseDetails.spouseEmail = this.profileForm.value.spouseEmail;
    this.spouseDetails.spouseHometown = this.profileForm.value.spouseHometown;
    this.spouseDetails.spouseLastName = this.profileForm.value.spouseLastName;
    this.spouseDetails.spousePhoneNumber = this.profileForm.value.spousePhoneNumber;
    this.spouseDetails.spouseState = this.profileForm.value.spouseState.name;
    this.updateUserModel.spouseDetails = this.spouseDetails;
    //this.updateUserModel.photo=this.selectedFile;
    for (let index = 0; index < this.profileForm.value.childrensDetails.length; index++) {
      this.profileForm.value.childrensDetails[index].childCountry=this.profileForm.value.childrensDetails[index].childCountry.name
      this.profileForm.value.childrensDetails[index].childCity=this.profileForm.value.childrensDetails[index].childCity.name
      this.profileForm.value.childrensDetails[index].childState=this.profileForm.value.childrensDetails[index].childState.name
      this.profileForm.value.childrensDetails[index].childUniqueId=this.currentUser.userId+"c"+index
    }
    this.updateUserModel.childrensDetails=this.profileForm.value.childrensDetails;
  }
  GetAllCoutries(){
        this.countries= Country.getAllCountries()
        this.spouseCountries=Country.getAllCountries()
        this.childCountries=Country.getAllCountries()
    }
  
  
  onChangeCountry(e: any) {
    if (e.value) {
      let states=State.getStatesOfCountry(e.value.isoCode)
      this.countryName=e.value
      this.states=states
      
    } else {
      this.states = [];
      this.cities = [];
    }
  }

  onChangeState(e: any) {
    if (e.value) {
      let cities=City.getCitiesOfState(e.value.countryCode,e.value.isoCode)
      this.stateName=e.value
      this.cities=cities
      this.spouseStates=this.states
      } else {
        this.cities = [];
      }
  }
  onChangeCity(e: any) {
    if (e.value) {
      this.cityName=e.value
     this.spouseCities=this.cities
     } else {
       this.cityName= this.cityName
     }
  }
  onChangeSpouseCountry(e:any){
    if (e.value) {
      let states=State.getStatesOfCountry(e.value.isoCode)
      this.spouseStates=states
     } else {
       this.spouseStates = [];
       this.spouseCities = [];
     }
  }
  onChangeSpouseState(e:any){
    if (e.value) {
      let cities=City.getCitiesOfState(e.value.countryCode,e.value.isoCode)
      this.spouseCities=cities;
      } else {
        this.spouseCities = [];
      }
  }
  onChangeChildCountry(e:any){
    if (e.value) {
      let states=State.getStatesOfCountry(e.value.isoCode)
      //this.childCountryName=e.value;
      this.childStates=states
       } else {
         this.childStates = [];
         this.childCities = [];
       }
  }
  onChangeChildState(e:any){
    if (e.value) {
      let cities=City.getCitiesOfState(e.value.countryCode,e.value.isoCode)
      //this.childStateName=e.value
      this.childCities=cities
      } else {
        this.childCities = [];
      }
  }
  processFile(_event: any): void {
    const reader = new FileReader();
    if (_event.target.files && _event.target.files.length) {
      const [file] = _event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.profileForm.patchValue({
          Photo: reader.result
        });
      }
    }
    this.selectedFile = <File>_event.target.files[0];
  }
  UploadPhoto(id:any){
    let formData: FormData = new FormData();
    formData.append('Photo', this.selectedFile);
    this.subscriptions.push(this.userprofileService.
      uploadPhoto(formData,id.userslist).
      subscribe({
        next: (res) => {
          this.onReject();
          this.image = 'data:image/png;base64,' + res;
        },
        error: () => {
          this.onReject();
          this.showError()
        }
      }))
  }
  updateDetails() {
    if (this.profileForm.invalid)
      return;
    this.showInfo();
    let formData: FormData = new FormData();
    //for (let i = 0; i < this.profileForm.value.childrensDetails.length; i++) {
      // for (let key of myFormValue) {
      //   formData.append(key, myFormValue[key]);
      // }
     // formData.append('ChildrensDetails', this.profileForm.value[i]);
    ///}
    formData.append('Photo', this.selectedFile);
    this.composeModel();
    //this.updateUserModel.photoData=this.selectedFile;
    //formData.append('ChildrensDetails', this.profileForm.value.childrensDetails);
    // formData.append('Photo', this.selectedFile);
    // formData.append('FirstName', this.profileForm.value.firstName);
    // formData.append('EmailAddress', this.profileForm.value.emailAddress);
    // formData.append('Address', this.profileForm.value.address);
    // formData.append('DOB', this.profileForm.value.dOB);
    // formData.append('LastName', this.profileForm.value.lastName);
    // formData.append('PostalCode', this.profileForm.value.postalCode);
    // formData.append('City', this.profileForm.value.city);
    // formData.append('State', this.profileForm.value.state);
    // formData.append('Country', this.profileForm.value.country);
    // formData.append('Reason', this.profileForm.value.reason);
    // formData.append('Refference', this.profileForm.value.refference);
    // formData.append('HomeTown', this.profileForm.value.homeTown);
    // formData.append('PhoneNumber', this.profileForm.value.phoneNumber);
    // formData.append('WhatsAppNumber', this.profileForm.value.whatsAppNumber);
    // formData.append('UserId', this.currentUser.userId);
    //Spouse
    // formData.append('SpouseFirstName', this.profileForm.value.spouseFirstName);
    // formData.append('SpouseEmail', this.profileForm.value.spouseEmail);
    // formData.append('SpouseLastName', this.profileForm.value.spouseLastName);
    // formData.append('SpousePhoneNumber', this.profileForm.value.spousePhoneNumber);
    // formData.append('SpouseDOB', this.profileForm.value.spouseDob);
    // formData.append('SpouseState', this.profileForm.value.spouseState);
    // formData.append('SpouseCity', this.profileForm.value.spouseCity);
    // formData.append('SpouseCountry', this.profileForm.value.spouseCountry);
    // formData.append('SpouseHometown', this.profileForm.value.spouseHometown);
    //Childrens
    // for (let index = 0; index <  this.profileForm.value.childrensDetails.length; index++) {
    //   formData.append('ChildrensDetails', JSON.stringify(this.profileForm.value.childrensDetails[index]).toString())
    // }
  //   for(let i = 0; i < this.chartDataSetValue.length; i++) {
  //     formData.append("Ids", this.chartDataSetValue[i].toString());
  // }

    this.subscriptions.push(this.userprofileService.
      updateUser(this.updateUserModel).
      subscribe({
        next: (res) => {
          this.onReject();
          this.showSuccess()
          this.UploadPhoto(res)
        },
        error: () => {
          this.onReject();
          this.showError()
        }
      }))
  }
  private initConfig(): void {
    this.payPalConfig = {
      currency: 'EUR',
      clientId: 'sb',
      createOrderOnClient: (_data) => <ICreateOrderRequest>{
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
}
