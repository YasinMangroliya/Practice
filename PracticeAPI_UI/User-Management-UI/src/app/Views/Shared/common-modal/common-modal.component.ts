import { Component, ElementRef, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import { ModalParams } from '../../../Model/CommonModel';
import { UserService } from '../../../Services/user.service';

@Component({
  selector: 'app-common-modal',
  templateUrl: './common-modal.component.html',
  styleUrls: ['./common-modal.component.css']
})

export class CommonModalComponent implements OnInit {
  @Output() confirmationModalEmitter: EventEmitter<boolean> = new EventEmitter();
  @Input() modalParams = new ModalParams();

  subject: Subject<boolean>;
  constructor(public modalRef: BsModalRef, public userService: UserService) { }
  ngOnInit() {
  }

  closeModal(isConfirmed: boolean) {
    this.confirmationModalEmitter.emit(isConfirmed);
  }

  action(value: boolean) {
    this.modalRef.hide();
    this.subject.next(value);
    this.subject.complete();
  }
}
