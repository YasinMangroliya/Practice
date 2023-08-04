import { AddressModel } from "./AddressModel";

export class UserDetailsModel {
  constructor() {
    this.address = new AddressModel();
  }
  userId: number;
  userName: string;
  password: string;
  email: string;
  mobileNo: number;
  gender: string
  birthDate: Date;
  addressId: number;
  profileImageName: string;
  profileImageBlob: string;
  createdDate: Date;
  createdBy: number;
  modifiedDate: Date;
  modifiedBy: number;
  isActive: boolean
  roleId: number;
  roleName: string;
  refreshToken: string;
  jwtToken: string;
  loginAttempt: string;
  loginAttemptDateTime: string;

  address: AddressModel;

}
export class LoginModel {
  UserName: string;
  Password: string;
}

export enum UserDetailEnum {
  userId = "userId",
  userName = "userName",
  password = "password",
  confirmPassword = "confirmPassword",
  email = "email",
  mobileNo = "mobileNo",
  gender = "gender",
  birthDate = "birthDate",
  profileImageName = "profileImageName",
  profileImageFile = "profileImageFile",
  ProfileImageBlob ="ProfileImageBlob",
  isActive = "isActive",
  roleId = "roleId",
  addressId = "addressId",
  countryId = "countryId",
  stateId = "stateId",
  cityId = "cityId",
  zipCode = "zipCode",
}
