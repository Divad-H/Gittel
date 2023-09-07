import { Component } from '@angular/core';
import { SampleRequestDto } from '../generated-client/sample-request-dto';
import { SampleResultDto } from '../generated-client/sample-result-dto';
import { Observable } from 'rxjs';
import { SampleRequestDto2 } from '../generated-client/sample-request-dto2';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private readonly messageService: MessageService) {
  }

  callSampleFunction(data: SampleRequestDto): Observable<SampleResultDto> {
    return this.messageService.callNativeFunction("Sample", "SampleFunction", [data]);
  }

  callSampleFunction2(data: SampleRequestDto, data2: SampleRequestDto2): Observable<null> {
    return this.messageService.callNativeFunction("Sample", "ReturnVoid", [data, data2]);
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
