export class AddressModel {

  addressId: number;
  countryId: number;
  stateId: number;
  cityId: number;
  zipCode: number;

  countryName: string;
  stateName: string;
  cityName: string;
}
export class Country {
  countryId: number;
  countryName: string;
}
export class State {
 stateId: number;
 stateName: string;
  countryId: number;
}
export class City {
  cityId: number;
  cityName: string;
  stateId: number;
}
