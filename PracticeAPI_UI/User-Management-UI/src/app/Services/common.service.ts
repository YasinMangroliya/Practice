import { ElementRef, Injectable } from '@angular/core';
import { Workbook } from 'exceljs';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, catchError } from 'rxjs';
import * as filesaver from 'file-saver';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  previousPath: string = null
  isSubmitClick = new BehaviorSubject<boolean>(false);
  constructor(private toastr: ToastrService,) {
  }
  showSuccess(message, title) {
    this.toastr.success(message, title, {
      timeOut: 5000,
      closeButton: true,
      progressBar: true,
      progressAnimation: "increasing",
      positionClass: "toast-top-right",
      extendedTimeOut: 2500,
      tapToDismiss: false
    });
  }

  showError(message, title) {
    this.toastr.error(message, title, {
      timeOut: 5000,
      closeButton: true,
      progressBar: true,
      positionClass: "toast-top-right",
      extendedTimeOut: 2500,
      tapToDismiss: false,
      enableHtml: true
    });
  }

  showInfo(message, title) {
    this.toastr.info(message, title, {
      timeOut: 5000,
      closeButton: true,
      progressBar: true,
      positionClass: "toast-top-right",
      extendedTimeOut: 2500,
      tapToDismiss: false
    });
  }

  showWarning(message, title) {
    this.toastr.warning(message, title, {
      timeOut: 5000,
      closeButton: true,
      progressBar: true,
      positionClass: "toast-top-right",
      extendedTimeOut: 2500,
      tapToDismiss: false,
      enableHtml: true
    });
  }

  convertToMB(size: number) {
    let sizeInKB = 1024 * 1024;
    return size / sizeInKB
  }

  exportToExcel(data: any[], fileName: string = "Data") {
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet(fileName);



    const headerRow = worksheet.addRow(Object.keys(data[0]));  //for all model fields

    headerRow.eachCell(cell => {
      cell.fill = {
        type: 'pattern',
        pattern: 'solid',
        fgColor: { argb: '007bff' }
      };
      cell.font = {
        bold: true,
        color: { argb: 'FFFFFF' }
      };
      cell.alignment = { horizontal: 'center', vertical: 'middle' };
    });

    worksheet.columns.forEach(column => {
      column.style = { alignment: { horizontal: 'left', vertical: 'middle' } };
    })
    data.forEach(item => {
      worksheet.addRow(Object.values(item));
    });

    // Auto-fit column widths
    worksheet.columns.forEach(column => {
      const values = column?.values?.filter(val => val); // Filter out undefined and null values
      const maxLength = values ? Math.max(...values.map(value => value?.toString().length)) : 0;
      column.width = maxLength + 2;
    })

    workbook.xlsx.writeBuffer().then((data) => {
      this.saveExcelWithBlob(data, fileName)
    }, catchError((err => { throw err })))
  }
  saveExcelWithBlob(blobData,fileName) {
    let blob = new Blob([blobData], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    filesaver.saveAs(blob, fileName + '.xlsx');
  }
  savePdfWithBlob(blobData, fileName) {
    let blob = new Blob([blobData], { type: 'application/pdf' });
    filesaver.saveAs(blob, fileName + '.pdf');
  }
  exportToPdf(htmlContent: ElementRef, fileName: string) {
    const content = htmlContent.nativeElement;

    // Create a new PDF document
    const doc = new jsPDF();

    // Define the PDF properties
    const pdfOptions = {
      margin: {
        top: 20,
        bottom: 20,
        left: 10,
        right: 10,
      },
    };

    html2canvas(content).then(canvas => {
      const imgData = canvas.toDataURL('image/png');
      const imgWidth = doc.internal.pageSize.getWidth() - pdfOptions.margin.left - pdfOptions.margin.right;
      const imgHeight = (canvas.height * imgWidth) / canvas.width;

      // Add the image to the PDF document
      doc.addImage(imgData, 'PNG', pdfOptions.margin.left, pdfOptions.margin.top, imgWidth, imgHeight);

      // Save the PDF with a specific name
      doc.save(fileName + ".pdf");
    }).catch(err => { console.error(err) });
  }
}
