import { AbstractControl,  ValidationErrors, ValidatorFn } from "@angular/forms";

export class CustomValidators {

  static comparePasswords(controlName: string, matchingControlName: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const control = formGroup.get(controlName);
      const matchingControl = formGroup.get(matchingControlName);

      if (control && matchingControl && control.value !== matchingControl.value) {
        matchingControl.setErrors({ compareValidation: true });
      } else {
        matchingControl.setErrors(null);
      }

      return null;
    };
  }

  static comparedValidation(controlName: string, match: string): any {

    if (controlName !== match) {
      return { compareValidation: true }
    } else {
      return null;
    }
  }
  // Validates Space values
  static noWhitespaceValidator(str): any {
    if (str.pristine) {
      return null;
    }
    str.markAsTouched();
    console.log("cus",/\s/.test(String(str.value)));
    if (/\s/.test(String(str.value))) {
      return {
        whiteSpace: true
      };
    }
      return null;
    
  }
}
