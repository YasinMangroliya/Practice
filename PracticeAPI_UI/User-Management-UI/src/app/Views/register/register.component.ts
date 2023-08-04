import { DatePipe, formatDate } from '@angular/common';
import { Component, HostListener, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { City, Country, State } from '../../Model/AddressModel';
import { RoleEnum } from '../../Model/CommonModel';
import { SessionEnum } from '../../Model/EndpointEnum';
import { UserDetailEnum, UserDetailsModel } from '../../Model/UserDetailsModel';
import { CommonService } from '../../Services/common.service';
import { UserService } from '../../Services/user.service';
import { CustomValidators } from '../../Utilities/CustomValidators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  encapsulation: ViewEncapsulation.Emulated,
})
export class RegisterComponent implements OnInit {



  url: any = '/assets/Img/user.png';
  selectedFileName: string = "Profile Image"
  selectedFile = null;
  isLoading = false;

  @Input() userId = 0;
  userDetailForm: FormGroup;
  userDetailModel: UserDetailsModel = new UserDetailsModel();
  countries: Country[] = [];
  states: State[] = [];
  cities: City[] = [];

  activeRole = sessionStorage.getItem(SessionEnum.Role)
  ddlRoles: { roleId: number, roleName: string }[] = []


  constructor(public userService: UserService, public commonService: CommonService, private route: ActivatedRoute, private formBuilder: FormBuilder, private datePipe: DatePipe, private sanitizer: DomSanitizer) {
    this.ddlRoles = [{ roleId: 1, roleName: "Admin" },
    { roleId: 2, roleName: "Customer" },
    { roleId: 3, roleName: "User" },
    { roleId: 4, roleName: "Restricted" }]
  }
  ngOnInit() {
    this.userId = this.userId == 0 ? +this.route.snapshot.paramMap.get('userId') : this.userId

    this.getCountryList();
    this.setFormBuilder();
    this.setValidators();
    if (this.userId > 0) {
      this.getUserById()
    }
  }
  @HostListener("window:beforeunload", ["$event"])
  canDeactivate(event: Event) {
    //event?.preventDefault();
    //event?.stopPropagation();
    if (this.userDetailForm.dirty && event)
      event.returnValue = true;

    if (this.userDetailForm.dirty) {
      return true;
    }
    return false;
  }

  setFormBuilder() {
    this.userDetailForm = this.formBuilder.group({
      userId: [0],
      userName: [null, [Validators.required, Validators.minLength(4), Validators.maxLength(20)]],
      password: [null, [Validators.required, CustomValidators.noWhitespaceValidator, Validators.minLength(4), Validators.maxLength(20)]],
      confirmPassword: [null, [Validators.required]],
      email: [null, [Validators.required, Validators.email]],
      mobileNo: ['', [Validators.required, Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      gender: ['M', [Validators.required]],
      birthDate: [null, [Validators.required]],
      profileImageFile: [null],
      isActive: [true],//for admin
      roleId: [3],  //for admin
      addressId: [0],
      countryId: [0, [Validators.min(1)]],
      stateId: [0, [Validators.min(1)]],
      cityId: [0, [Validators.min(1)]],
      zipCode: [null, [Validators.required]],
    }
      , {
        validators:
          [CustomValidators.comparePasswords(UserDetailEnum.password, UserDetailEnum.confirmPassword)]
      })
  }
  setEditForm() {
    this.userDetailForm.patchValue({
      userId: this.userDetailModel.userId,
      userName: this.userDetailModel.userName,
      password: this.userDetailModel.password,
      confirmPassword: this.userDetailModel.password,
      email: this.userDetailModel.email,
      mobileNo: this.userDetailModel.mobileNo,
      gender: this.userDetailModel.gender,
      //birthDate: this.datePipe.transform(this.userDetailModel.birthDate, 'dd/MM/yyyy'),
      birthDate: formatDate(this.userDetailModel.birthDate, 'yyyy-MM-dd', 'en'),

      isActive: this.userDetailModel.isActive,
      roleId: this.userDetailModel.roleId,
      addressId: this.userDetailModel.address.addressId,
      countryId: this.userDetailModel.address.countryId,
      stateId: this.userDetailModel.address.stateId,
      cityId: this.userDetailModel.address.cityId,
      zipCode: this.userDetailModel.address.zipCode,
    })
  }
  getUserById() {
    this.userService.getUserById(this.userId).then((response: UserDetailsModel) => {
      this.userDetailModel = response;
      this.url = 'data:image/png;base64,' + this.userDetailModel.profileImageBlob

      const getState = this.userService.getStateByCountryId(this.userDetailModel.address.countryId)
      const getCity = this.userService.getCityByStateId(this.userDetailModel.address.stateId)

      combineLatest([getState, getCity]).subscribe(([resState, resCity]) => {
        this.states = resState as State[]
        this.cities = resCity as City[]
        this.setEditForm()
      })
    }).catch((error) => {
      console.error("Error", error);
    });

  }
  getCountryList() {
    this.userService.getCountry(res => {
      this.countries = res
    });
  }
  getStateList(countryId: number) {
    this.setFormControlValue(UserDetailEnum.stateId, 0)
    this.setFormControlValue(UserDetailEnum.cityId, 0)
    this.states = []
    this.cities = []

    this.userService.getStateByCountryId(countryId).then((res: State[]) => {
      this.states = res
    }).catch((error) => {
      console.error("Error", error);
    });;
  }
  getCityList(stateId: number) {
    this.setFormControlValue(UserDetailEnum.cityId, 0)
    this.cities = []
    this.userService.getCityByStateId(stateId).then((res: City[]) => {
      this.cities = res
    }).catch((error) => {
      console.error("Error", error);
    });;
  }
  checkUniqueUserName() {
    if (this.userDetailForm.controls[UserDetailEnum.userName].valid) {
      this.isLoading = true;
      this.userService.checkUniqueUserName(this.userId, this.getFormControlValue(UserDetailEnum.userName), response => {
        if (!response) {
          this.userDetailForm.controls[UserDetailEnum.userName].setErrors({ 'duplicate': true });
          this.commonService.showError('Already Exist', "Username Already Exist...")
        }
        this.isLoading = false;
      })
    }
  }

  onSubmit() {
    this.userDetailForm.markAllAsTouched();
    this.commonService.isSubmitClick.next(true)

    if (this.userDetailForm.valid) {
      const formData = this.setModelBeforeSubmit()
      this.userService.saveUser(formData, response => {
        if (response > 0) {
          this.commonService.showSuccess('User save successfully', "Success")
          this.setFormBuilder();
          if (this.activeRole == RoleEnum.Admin)
            this.userService.registerSubject.next(true)
        }

      })
    }
    else
      this.commonService.showError('Please validate required fields', "Validation Alert")
  }
  setModelBeforeSubmit() {
    let formData = new FormData();
    if (this.userId > 0) {
      formData.append(UserDetailEnum.userId, this.getFormControlValue(UserDetailEnum.userId));
      formData.append(UserDetailEnum.profileImageName, this.userDetailModel.profileImageName);
      formData.append(UserDetailEnum.ProfileImageBlob, this.userDetailModel.profileImageBlob);
    }
    formData.append(UserDetailEnum.userName, this.getFormControlValue(UserDetailEnum.userName));
    formData.append(UserDetailEnum.password, this.getFormControlValue(UserDetailEnum.password));
    formData.append(UserDetailEnum.email, this.getFormControlValue(UserDetailEnum.email));
    formData.append(UserDetailEnum.mobileNo, this.getFormControlValue(UserDetailEnum.mobileNo));
    formData.append(UserDetailEnum.gender, this.getFormControlValue(UserDetailEnum.gender));
    formData.append(UserDetailEnum.birthDate, this.getFormControlValue(UserDetailEnum.birthDate));
    formData.append(UserDetailEnum.profileImageFile, this.selectedFile);
    formData.append(UserDetailEnum.isActive, this.getFormControlValue(UserDetailEnum.isActive));
    formData.append(UserDetailEnum.roleId, this.getFormControlValue(UserDetailEnum.roleId));
    formData.append(UserDetailEnum.addressId, this.getFormControlValue(UserDetailEnum.addressId));
    formData.append(`address[${UserDetailEnum.countryId}]`, this.getFormControlValue(UserDetailEnum.countryId));
    formData.append(`address[${UserDetailEnum.stateId}]`, this.getFormControlValue(UserDetailEnum.stateId));
    formData.append(`address[${UserDetailEnum.cityId}]`, this.getFormControlValue(UserDetailEnum.cityId));
    formData.append(`address[${UserDetailEnum.zipCode}]`, this.getFormControlValue(UserDetailEnum.zipCode));

    return formData;
  }
  onFileSelect(event) {
    this.selectedFile = event.target.files[0];
    if (this.commonService.convertToMB(this.selectedFile?.size) < 3) {
      let reader = new FileReader();
      this.selectedFileName = this.selectedFile?.name
      reader.readAsDataURL(event.target.files[0]); // read file as data url

      reader.onload = (event) => { // called once readAsDataURL is completed
        this.url = event.target.result;
      }
    }
    else {
      this.userDetailForm.controls[UserDetailEnum.profileImageFile].setErrors({ 'fileSize': true });
      this.commonService.showError("Less then 3MB file should be allowed", "Large File Size!!")
    }
  }

  setValidators() {
    if (this.userId > 0) {
      this.userDetailForm.get(UserDetailEnum.profileImageFile).setValidators([Validators.pattern("(.*?)\.(jpg|png|jpeg|PNG|JPG|JPEG)$")])
    } else {
      this.userDetailForm.get(UserDetailEnum.profileImageFile).setValidators([Validators.required, Validators.pattern("(.*?)\.(jpg|png|jpeg|PNG|JPG|JPEG)$")])
    }
  }

  cssValidation(field: string) {
    return {
      'is-invalid': (this.userDetailForm.get(field).invalid && (this.userDetailForm.get(field).dirty || this.userDetailForm.get(field).touched))
    };
  }
  setFormControlValue(control, value) {
    if (this.userDetailForm?.get(control))
      this.userDetailForm.get(control).setValue(value);
  }

  getFormControlValue(control) {
    if (this.userDetailForm?.get(control))
      return this.userDetailForm.get(control).value;
  }

  checkAdminRole() {
    if (this.activeRole == RoleEnum.Admin)
      return true
    else
      return false
  }
}
