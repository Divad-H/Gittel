import { Component } from '@angular/core';
import { SampleClient } from '../generated-client/sample-client';
import { take } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private readonly sampleClient: SampleClient) {
  }

  public foo() {

    this.sampleClient.sampleFunction({
      text: "TestString"
    }).subscribe(res => {
      console.log(res);
    })

  }

  public foo2() {

    this.sampleClient.returnVoid({
      text: "TestString"
    }, {
      number: 5
    }).subscribe(res => {
      console.log(res);
    })

  }

  public foo3() {

    this.sampleClient.sampleEvent({
      text: "TestString"
    }).pipe(take(5)
    ).subscribe(
      res => { console.log(res); },
      err => { console.log(err); },
      () => console.log('completed')
    );
  }

  public foo4() {

    this.sampleClient.sampleEventWithMultipleParameters({
      number: 5
    },
    {
      text: "TestString"
    }).pipe(take(5)
    ).subscribe(
      res => { console.log(res); },
      err => { console.log(err); },
      () => console.log('completed')
    );
    
  }
}
