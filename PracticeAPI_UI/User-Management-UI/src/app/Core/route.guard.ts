import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { BsModalService } from 'ngx-bootstrap/modal';
import { Observable, Subject } from 'rxjs';
import { CommonModalComponent } from '../Views/Shared/common-modal/common-modal.component';

export interface CanComponentDeactivate {
  canDeactivate: () => Observable<boolean> | Promise<boolean> | boolean;
}

export const RouteGuard: CanDeactivateFn<CanComponentDeactivate> = (component: CanComponentDeactivate) => {
  const modalService = inject(BsModalService);
  if (component.canDeactivate()) {
    const subject = new Subject<boolean>();
    const modal = modalService.show(CommonModalComponent, { 'class': 'modal-dialog-primary' });
    modal.content.subject = subject;
    return subject.asObservable();
  }
  return true;
};
