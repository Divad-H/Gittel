import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { MessageService } from '../services/message.service';
import { SampleClient } from '../generated-client/sample-client';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [
    MessageService,
    SampleClient
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
