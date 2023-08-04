import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { take } from 'rxjs';
import { CommonService } from '../../../Services/common.service';

@Component({
  selector: 'app-validation-summary',
  templateUrl: './validation-summary.component.html',
  styleUrls: ['./validation-summary.component.css']
})
export class ValidationSummaryComponent implements OnInit {
  @Input() form: FormGroup;
  @Input() isSubmitClick: boolean;
  errors: string[] = [];
  isSubmitClicked = false;

  constructor(private commonService: CommonService) { }

  ngOnInit() {
    if (this.form instanceof FormGroup === false) {
      throw new Error('You must supply the validation summary with an NgForm.');
    }
    this.form.statusChanges.subscribe(status => {
      if (this.isSubmitClicked) {
        this.resetErrorMessages();
        this.generateErrorMessages(this.form);
      }
    });
    this.commonService.isSubmitClick.pipe(take(2)).subscribe(value => {
      if (value) {
        this.generateErrorMessages(this.form);
        this.isSubmitClicked = true;
      }
    })
  }

  resetErrorMessages() {
    this.errors.length = 0;
  }

  generateErrorMessages(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(controlName => {
      let control = formGroup.controls[controlName];
      let errors = control.errors;
      if (errors === null || errors.count === 0) {
        return;
      }
      // Handle the 'required' case
      if (errors.required) {
        this.errors.push(`${controlName} is required`);
      }
      // Handle 'minlength' case
      if (errors.minlength) {
        this.errors.push(`${controlName} minimum length is ${errors.minlength.requiredLength}.`);
      }
      if (errors.duplicate) {
        this.errors.push(`${controlName} is already exist.`);
      }
      if (errors.compareValidation) {
        this.errors.push(`${controlName} does not match.`);
      }
      if (errors.pattern) {
        //this.errors.push(`${controlName} suppport ${errors.pattern.requiredPattern}.`);
        this.errors.push(`${controlName} invalid formate.`);
      }
      if (errors.whiteSpace) {
        this.errors.push(`${controlName} does not allow whitespace.`);
      }
      if (errors.fileSize) {
        this.errors.push(`${controlName} file size is greater then expected.`);
      }
      if (errors.email) {
        this.errors.push(`${controlName} invalid.`);
      }

      // Handle custom messages.
      if (errors.message) {
        this.errors.push(`${controlName} ${errors.message}`);
      }
    });
  }
}
