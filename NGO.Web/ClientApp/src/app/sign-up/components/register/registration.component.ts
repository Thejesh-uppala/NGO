import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ICreateOrderRequest, IPayPalConfig } from 'ngx-paypal';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { CityInterface, CountryInterface, StateInterface } from 'src/app/userprofile/models/UpdateUserModel';
import { RegistrationModel, SpouseDetails } from './models/UserRegistrationModel';
import { RegistrationService } from './services/registrationService';
import { Country, State, City } from 'country-state-city';

let payemetDate="",paymentId="",payerId="";
export class CountyData {
  constructor(public name: string, public id: string) {
  }
}
export class StateData {
  constructor(public name: string, public id: string) {
  }
}
export class CityData {
  constructor(public name: string, public id: string) {
  }
}
@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})

export class RegistrationComponent implements OnInit {
  signUpForm!: FormGroup;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: Boolean = false;
  errorMessage: string = ""
  error = "";
  currentUser: any;
  subscriptions: Subscription[] = [];
  selectedFile!: File;
  public payPalConfig?: IPayPalConfig;
  registrationModel = {} as RegistrationModel;
  spouseDetails = {} as SpouseDetails;
  paypalSubmitted: boolean = true;
  countries: any
  states: any
  cities: any
  spouseCountries: any
  spouseStates: any
  spouseCities: CityInterface[] = [];
  childCountries: any
  childStates: any
  childCities: CityInterface[] = [];
  countryName!: CountryInterface
  stateName!: StateInterface;
  cityName!: CityInterface;
  childCountryName!: CountryInterface
  childStateName!: StateInterface;
  childCityName!: CityInterface;
  spouseCountryName: string = "";
  spousestateName: string = "";
  disableSpouseFlag: boolean = false;
  disableChildFlag: boolean[] = [];
  cities1!: CityInterface[];
  emailPattern:string = "^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$";
  @Input() display!: boolean;
  @Input() email!: string;
  @Input() phNumber!: string;
  @Input() password!: string;
  @Input() userName!: string;
  @ViewChild('paypalRef', { static: true })
  private paypalRef!: ElementRef;
  constructor(private formBuilder: FormBuilder,
    private router: Router,
    private registrationService: RegistrationService,
    private messageService: MessageService,) { }

  ngOnInit(): void {
    this.signUpForm = this.formBuilder.group({
      //username: ["", Validators.required],
      familyName: ["", Validators.required],
      firstName: ["", Validators.required],
      emailAddress: ["", [Validators.required,Validators.pattern(this.emailPattern)]],
      lastName: ["", Validators.required],
      dOB: ["", Validators.required],
      address: ["", Validators.required],
      memberShipType: ["", Validators.required],
      city: ["", Validators.required],
      country: ["", Validators.required],
      state: ["", Validators.required],
      postalCode: ["", Validators.required],
      whatsAppNumber: ["", Validators.required],
      phoneNumber: ["", [Validators.required, Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      homeTown: ["", Validators.required],
      refference: ["", Validators.required],
      Photo: [""],
      reason: ["", Validators.required],
      spouseFirstName: ["", Validators.required],
      spouseEmail: ["", [Validators.required,Validators.pattern(this.emailPattern)]],
      spouseLastName: ["", Validators.required],
      spousePhoneNumber: ["", [Validators.required, Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      spouseDob: ["", Validators.required],
      spouseState: ["", Validators.required],
      spouseCity: ["", Validators.required],
      spouseCountry: ["", Validators.required],
      spouseHometown: ["", Validators.required],
      childrensDetails: this.formBuilder.array([])
    });
    this.paypalConfig();
    this.GetAllCoutries();
   // this.initConfig();
    this.cities1 = [
      { name: 'New York', id: 'NY' },
      { name: 'Rome', id: 'RM' },
      { name: 'London', id: 'LDN' },
      { name: 'Istanbul', id: 'IST' },
      { name: 'Paris', id: 'PRS' }
    ];
    console.log(this.cities);

  }
  get f() {
    return this.signUpForm.controls;
  }
  get hasDropDownError() {
    return (
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.touched &&
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.errors &&
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.invalid
    )
  }
  paypalConfig() {
    window.paypal.Buttons({
      style: {
        layout: 'horizontal',
        shape: 'rect',
        label: 'paypal',
        width: '10px'
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
          payemetDate=x.create_time;
          paymentId=x.id;
          payerId=x.payer.payer_id;
          console.log("onApprove",x)
          this.signUp();
        });
      },
      // onApprove: function (data:any, actions:any) { 
      //  return actions.order.capture().then((x: any)=>{
      //    debugger
      //   payemetDate=x.create_time;
      //   paymentId=x.id;
      //   payerId=x.payer.payer_id;
      //   console.log("onApprove",x)
      //   this.save()
      //  })
      // },  
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
  spouseSameAddress(e: any) {
    debugger

    if (e.currentTarget.checked) {
      this.disableSpouseFlag = e.currentTarget.checked;
      this.spouseStates = this.states
      this.spouseCities = this.cities
      this.cityName = this.signUpForm.value.city
      this.stateName = this.signUpForm.value.state
      this.countryName = this.signUpForm.value.country
    } else {
      this.disableSpouseFlag = e.currentTarget.checked;
      this.spouseStates = []
      this.spouseCities = []
    }
  }
  childSameAddress(e: any, i: number) {
    if (e.currentTarget.checked) {
      this.disableChildFlag[i] = e.currentTarget.checked;
      this.childStates = this.states
      this.childCities = this.cities
      this.childCityName = this.signUpForm.value.city
      this.childStateName = this.signUpForm.value.state
      this.childCountryName = this.signUpForm.value.country
    } else {
      this.disableChildFlag[i] = e.currentTarget.checked;
      this.childStates = []
      this.childCities = []
    }
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
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Registered Successfully' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Something Went Wrong!' });
  }
  showInfo() {
    this.messageService.add({ severity: 'info', summary: 'Info', detail: 'Registration in progress' });
  }
  onReject() {
    this.messageService.clear('c');
  }
  onConfirm() {
    this.messageService.clear('c');
  }
  removeChild(i: number) {
    this.childrensDetails.removeAt(i);
  }
  get childrensDetails(): FormArray {
    return this.signUpForm.controls["childrensDetails"] as FormArray;
  }
  processFile(_event: any): void {
    const reader = new FileReader();
    if (_event.target.files && _event.target.files.length) {
      const [file] = _event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.signUpForm.patchValue({
          Photo: reader.result
        });
      }
    }
    this.selectedFile = <File>_event.target.files[0];
  }
  addChildrens() {
    this.childrensDetails.push(this.newChild());
  }
  newChild(): FormGroup {
    return this.formBuilder.group({
      childUniqueId:[""],
      childFirstName: ["", Validators.required],
      childEmailAddress: ["", [Validators.required,Validators.pattern(this.emailPattern)]],
      childLastName: ["", Validators.required],
      childPhoneNumber: ["", [Validators.required, Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      childState: ["", Validators.required],
      childCity: ["", Validators.required],
      childCountry: ["", Validators.required],
      childDOB: ["", Validators.required],
    })
  }
  composeModel(): void {
    this.registrationModel.firstName = this.signUpForm.value.firstName;
    this.registrationModel.lastName = this.signUpForm.value.lastName;
    this.registrationModel.socialMedia = this.signUpForm.value.firstName;
    this.registrationModel.state = this.stateName.name ? this.stateName.name : "";
    this.registrationModel.dob = this.signUpForm.value.dOB;
    this.registrationModel.city = this.signUpForm.value.city;
    this.registrationModel.country = this.countryName.name ? this.countryName.name : "";
    this.registrationModel.homeTown = this.signUpForm.value.homeTown;
    this.registrationModel.whatsAppNumber = this.signUpForm.value.whatsAppNumber;
    this.registrationModel.phoneNumber = this.signUpForm.value.phoneNumber;
    this.registrationModel.reason = this.signUpForm.value.reason;
    this.registrationModel.emailAddress = this.signUpForm.value.emailAddress;
    this.registrationModel.address = this.signUpForm.value.address;
    this.registrationModel.refference = this.signUpForm.value.refference;
    this.registrationModel.postalCode = this.signUpForm.value.postalCode;
    this.registrationModel.paymentId = paymentId
    this.registrationModel.paymentDate =  payemetDate
    this.registrationModel.payer_id =  payerId;
    
    this.registrationModel.memberShipType = this.signUpForm.value.memberShipType;
    this.registrationModel.password = this.password;
    //Spouse Details
    this.spouseDetails.spouseFirstName = this.signUpForm.value.spouseFirstName;
    this.spouseDetails.spouseCountry = this.signUpForm.value.spouseCountry.name;
    this.spouseDetails.spouseCity = this.signUpForm.value.spouseCity;
    this.spouseDetails.spouseDob = this.signUpForm.value.spouseDob;
    this.spouseDetails.spouseEmail = this.signUpForm.value.spouseEmail;
    this.spouseDetails.spouseHometown = this.signUpForm.value.spouseHometown;
    this.spouseDetails.spouseLastName = this.signUpForm.value.spouseLastName;
    this.spouseDetails.spousePhoneNumber = this.signUpForm.value.spousePhoneNumber;
    this.spouseDetails.spouseState = this.signUpForm.value.spouseState.name;
    this.registrationModel.spouseDetails = this.spouseDetails;
    //this.registrationModel.photoData=this.selectedFile;
    for (let index = 0; index < this.signUpForm.value.childrensDetails.length; index++) {
      this.signUpForm.value.childrensDetails[index].childCountry = this.signUpForm.value.childrensDetails[index].childCountry.name
      this.signUpForm.value.childrensDetails[index].childCity = this.signUpForm.value.childrensDetails[index].childCity.name
      this.signUpForm.value.childrensDetails[index].childState = this.signUpForm.value.childrensDetails[index].childState.name
      this.signUpForm.value.childrensDetails[index].childUniqueId="c"+index
    }
    this.registrationModel.childrensDetails = this.signUpForm.value.childrensDetails;
  }
  GetAllCoutries() {
    this.countries = Country.getAllCountries()
    this.spouseCountries = Country.getAllCountries()
    this.childCountries = Country.getAllCountries()
  }
  onChangeCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode)
      this.countryName = e.value
      this.states = states

    } else {
      this.states = [];
      this.cities = [];
    }
  }

  onChangeState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode)
      this.stateName = e.value
      this.cities = cities
    } else {
      this.cities = [];
    }
  }
  onChangeCity(e: any) {
    if (e.value) {
      this.cityName = e.value
      this.spouseCities = this.cities
    } else {
      this.cityName = this.cityName
    }
  }
  onChangeSpouseCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode)
      this.spouseStates = states
    } else {
      this.spouseStates = [];
      this.spouseCities = [];
    }
  }
  onChangeSpouseState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode)
      this.spouseCities = cities;
    } else {
      this.spouseCities = [];
    }
  }
  onChangeChildCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode)
      this.childCountryName = e.value;
      this.childStates = states
    } else {
      this.childStates = [];
      this.childCities = [];
    }
  }
  onChangeChildState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode)
      this.childStateName = e.value
      this.childCities = cities
    } else {
      this.childCities = [];
    }
  }
  UploadPhoto(id: any) {
    let formData: FormData = new FormData();
    formData.append('Photo', this.selectedFile);
    this.subscriptions.push(this.registrationService.
      uploadPhoto(formData, id).
      subscribe({
        next: (_res) => {
          this.onReject();
        },
        error: () => {
          this.onReject();
          this.showError()
        }
      }))
  }
  signUp(): void {
    if (this.signUpForm.invalid)
      return;
    this.showInfo();
    this.composeModel();
    this.subscriptions.push(
      this.registrationService
        .register(this.registrationModel).subscribe(
          {
            next: (res) => {
              if (res) {
                this.onReject();
                this.showSuccess();
                this.UploadPhoto(res);
                this.router.navigate(["/login"]);
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
}

