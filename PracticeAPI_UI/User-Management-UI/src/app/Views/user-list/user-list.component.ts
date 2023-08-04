import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { LocationEnum, ModalParams, ModalTypeEnum, SortingClassEnum } from '../../Model/CommonModel';
import { DataTableParams, OrderByEnum, PaginationParams } from '../../Model/DataTableModel';
import { UserDetailEnum, UserDetailsModel } from '../../Model/UserDetailsModel';
import { CommonService } from '../../Services/common.service';
import { UserService } from '../../Services/user.service';
import { City, Country, State } from '../../Model/AddressModel';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  @ViewChild('content') content: ElementRef;  //to export data in pdf

  userId: number;
  modalParams = new ModalParams();
  dataTableParams: DataTableParams = new DataTableParams();
  paginationParams: PaginationParams = new PaginationParams();
  lstUserDetails: UserDetailsModel[] = [];

  currentSortProperty = "";

  search: string = "";
  searchIn: string = UserDetailEnum.userName;
  fromDate: Date = null;
  toDate: Date = null;

  entriesLabel: string;
  ddlSeacrchIn: string[] = [];
  countries: Country[] = [];
  //states: State[] = [];
  //cities: City[] = [];
  selectedCountries: number[] = [];
  selectedStates: number[] = [];
  selectedCities: number[] = [];

  states: { countryId: number, countryName: string, stateData: State[] }[] = []
  cities: { stateId: number, stateName: string, cityData: City[] }[] = []

  locationIds: string = null;
  locationBy: string = null;
  isSearched: boolean = false;
  constructor(private userService: UserService, private commonService: CommonService) {
    this.ddlSeacrchIn.push(UserDetailEnum.userName)
    this.ddlSeacrchIn.push(UserDetailEnum.email)
  }

  ngOnInit() {
    this.getUserList()
    this.getCountryList();
    this.userService.registerSubject.subscribe(val => {
      if (val)
        this.getUserList();
      this.modalParams = new ModalParams()
    })

  }


  getUserList(fromDate = null, toDate = null, locationIds: string = null, locationBy: string = null) {
    this.userService.getUserList(this.dataTableParams, fromDate, toDate, locationIds, locationBy, response => {
      this.paginationParams.currentPage = response.currentPage;
      this.paginationParams.totalPages = response.totalPages;
      this.paginationParams.totalCount = response.totalCount;
      this.paginationParams.pageSize = response.pageSize;
      this.lstUserDetails = response.lstUserDetails;

      this.getUserCountLabel()
    })
  }


  getCountryList() {
    this.userService.getCountry(res => {
      this.countries = res
    });
  }
  getStateList(countryId: number) {
    this.userService.getStateByCountryId(countryId).then((res: State[]) => {
      //this.states.push(...res)
      let countryName: string = this.countries.find(x => x.countryId == countryId).countryName
      this.states.push({ countryId: countryId, countryName: countryName, stateData: res })
    }).catch((error) => {
      console.error("Error", error);
    });
  }

  getCityList(stateId: number) {
    this.userService.getCityByStateId(stateId).then((res: City[]) => {
      let stateName: string = "";
      this.states.forEach(x => {
        let name: string = x.stateData.find(d => d.stateId == stateId)?.stateName
        if (name)
          stateName = name;
      })
      this.cities.push({ stateId: stateId, stateName: stateName, cityData: res })
    }).catch((error) => {
      console.error("Error", error);
    });
  }
  onDDLSelect(id, ddlName) {
    if (ddlName == LocationEnum.Country)
      this.getStateList(id)
    else if (ddlName == LocationEnum.State)
      this.getCityList(id)
  }
  onDDLRemove(id, ddlName) {
    if (ddlName == LocationEnum.Country)
      this.states = this.states.filter(x => x.countryId != id)
    else if (ddlName == LocationEnum.State)
      this.cities = this.cities.filter(x => x.stateId != id)
  }
  getUserCountLabel() {
    const fromCount = this.paginationParams.currentPage > 1 ? ((this.paginationParams.currentPage - 1) * this.paginationParams.pageSize) + 1 : this.paginationParams.currentPage
    const toCount = (fromCount - 1) + this.lstUserDetails.length;

    this.entriesLabel = `Showing ${fromCount} to ${toCount} out of ${this.paginationParams.totalCount} entries `
  }
  pageChanged(event) {
    this.dataTableParams.CurrentPage = event.page
    this.getUserList()
  }
  changeSorting(sortByProperty: string) {
    this.lstUserDetails = [];
    this.dataTableParams.SortColumn = this.currentSortProperty = sortByProperty;

    //toggle property on click
    if (this.dataTableParams.SortColumn == this.currentSortProperty) {
      if (this.dataTableParams.SortColumnDir == OrderByEnum.Descending)
        this.dataTableParams.SortColumnDir = OrderByEnum.Ascending
      else
        this.dataTableParams.SortColumnDir = OrderByEnum.Descending
    }
    else {
      this.dataTableParams.SortColumnDir = OrderByEnum.Ascending
    }

    this.dataTableParams.CurrentPage = 1;
    this.getUserList();
  }
  setSortingIcon(columnName: string) {
    if (columnName == this.dataTableParams.SortColumn) {
      if (this.dataTableParams.SortColumnDir == OrderByEnum.Ascending)
        return SortingClassEnum.SortUp
      else
        return SortingClassEnum.SortDown
    }
    else
      return SortingClassEnum.DefaultSort
  }

  onFilter() {
    if (this.selectedCities?.length > 0) {
      this.locationBy = LocationEnum.City
      this.locationIds = this.selectedCities.join(",")
    } else if (this.selectedStates?.length > 0) {
      this.locationBy = LocationEnum.State
      this.locationIds = this.selectedStates.join(",")
    }
    else if (this.selectedCountries?.length > 0) {
      this.locationBy = LocationEnum.Country
      this.locationIds = this.selectedCountries.join(",")
    }
    else
      this.locationIds = this.locationBy = null

    if ((this.fromDate || this.toDate) && (!this.fromDate || !this.toDate)) {
      this.commonService.showWarning("Date Range Required", "Alert")
      return
    }


    this.getUserList(this.fromDate?.toLocaleString(), this.toDate?.toLocaleString(), this.locationIds, this.locationBy)
  }
  onReset() {

    this.resetDataTableParams()
    this.fromDate = this.toDate = this.locationIds = this.locationBy = null
    this.selectedCities = this.selectedStates = this.selectedCountries = [];
    this.clearSearch(true)
  }
  onSearch() {
    if (this.search.trim().length < 3) {
      this.commonService.showWarning("Please enter atleast 3 characters to search", "Search Criteria!");
      return;
    }
    if (this.search == this.dataTableParams.SearchValue)
      return;

    this.lstUserDetails = [];
    this.resetDataTableParams()

    this.dataTableParams.SearchIn = this.searchIn;
    this.dataTableParams.SearchValue = this.search.trim();
    this.getUserList();
    this.isSearched = this.dataTableParams.SearchValue.trim() == "" ? false : true;
  }
  clearSearch(isFromReset = false) {
    if (!this.isSearched && !isFromReset) {
      this.isSearched = false;
      this.search = ""
      return;
    }
    this.resetDataTableParams();
    this.dataTableParams.SearchValue = this.search = "";
    this.isSearched = false;
    this.getUserList();
  }
  resetDataTableParams() {
    this.dataTableParams.CurrentPage = 1;
    this.dataTableParams.SearchIn = "";
    this.dataTableParams.SearchValue = "";
  }


  onDelete(userId: number) {
    this.userId = userId;
    this.modalParams = new ModalParams(ModalTypeEnum.Delete, "Are You Sure?", "This process cannot be undone");
  }

  onAddEditUser(userId: number = 0) {
    this.modalParams = new ModalParams(ModalTypeEnum.Register, "Add/Update User", "", userId);
  }

  modalEvent(event) {
    this.modalParams = new ModalParams()
    if (event) {
      this.userService.deleteUserById(this.userId, response => {
        if (response) {
          this.commonService.showSuccess("User has been deleted successfully", "Success")
          this.getUserList();
        }
        else
          this.commonService.showError("Failed to delete user", "Internal Server Error")
      })
    }
  }


  exportToExcel() {
    let data = this.lstUserDetails.map(user => ({
      Id: user.userId,
      UserName: user.userName,
      Gender: user.gender,
      Email: user.email,
      DateOfBirth: formatDate(user.birthDate, 'yyyy-MM-dd', 'en'),
      Mobile: user.mobileNo,
      Country: user.address.countryName,
      State: user.address.stateName,
      City: user.address.cityName,
    }));
    this.commonService.exportToExcel(data, "UserDetails")
  }
  exportToExcelAll() {
    this.userService.exportAllUserToExcel().subscribe(res => {
      this.commonService.saveExcelWithBlob(res, "UserDetails")
    });
  }
  exportToPdfAll() {
    this.userService.exportAllUserToPdf().subscribe(res => {
      this.commonService.savePdfWithBlob(res, "UserDetails")
    });
  }
  exportToPdf() {
    this.commonService.exportToPdf(this.content, "UserDetails")
  }
}
