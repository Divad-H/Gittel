import { Component } from '@angular/core';
import { SampleRequestDto } from '../generated-client/sample-request-dto';
import { SampleResultDto } from '../generated-client/sample-result-dto';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Gittel.Ui';

  constructor() {
    (window as any).chrome.webview.addEventListener('message', (arg: any) => {
      console.log(arg);
    })
  }

  //callSampleFunction(data: SampleRequestDto): Observable<SampleResultDto> {
  //
  //}

  public foo() {
    (window as any).chrome.webview.postMessage({
      controller: "sample",
      function: "sampleFunction",
      requestId: "asdf-1234",
      data: JSON.stringify({
        text: "TestString"
      })
    })
  }
}
