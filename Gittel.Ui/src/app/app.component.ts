import { Component } from '@angular/core';
import { SampleRequestDto } from '../generated-client/sample-request-dto';
import { SampleResultDto } from '../generated-client/sample-result-dto';
import { EMPTY, Observable, Subject, defer, filter, map, merge, mergeWith, of, startWith, take, tap, withLatestFrom } from 'rxjs';
import { ResponseDto } from '../generated-client/response-dto';
import { RequestDto } from '../generated-client/request-dto';
import { RequestType } from '../generated-client/request-type';
import { makeid } from '../utilities/makeid';
import { SampleRequestDto2 } from '../generated-client/sample-request-dto2';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  private readonly receivedMessage = new Subject<ResponseDto>();

  constructor() {
    (window as any).chrome.webview.addEventListener('message', (arg: any) => {
      this.receivedMessage.next(arg.data);
    })
  }

  callSampleFunction(data: SampleRequestDto): Observable<SampleResultDto> {

    const requestData: RequestDto = {
      controller: "Sample",
      function: "SampleFunction",
      requestId: makeid(22),
      requestType: RequestType.FunctionCall,
      data: [JSON.stringify(data)]
    };

    return this.receivedMessage.pipe(
      mergeWith(defer(() => {
        (window as any).chrome.webview.postMessage(requestData);
        return EMPTY;
      })),
      filter(data => data.requestId === requestData.requestId),
      map(data => JSON.parse(data.data!)),
      take(1)
    );
  }

  callSampleFunction2(data: SampleRequestDto, data2: SampleRequestDto2): Observable<SampleResultDto> {

    const requestData: RequestDto = {
      controller: "Sample",
      function: "ReturnVoid",
      requestId: makeid(22),
      requestType: RequestType.FunctionCall,
      data: [JSON.stringify(data), JSON.stringify(data2)]
    };

    (window as any).chrome.webview.postMessage(requestData);

    return this.receivedMessage.pipe(
      filter(data => data.requestId === requestData.requestId),
      tap(data => console.log(data)),
      map(data => JSON.parse(data.data!)),
      take(1)
    );
  }

  public foo() {

    this.callSampleFunction({
      text: "TestString"
    }).subscribe(res => {
      console.log(res);
    })

  }

  public foo2() {

    this.callSampleFunction2({
      text: "TestString"
    }, {
      number: 5
    }).subscribe(res => {
      console.log(res);
    })

  }
}
