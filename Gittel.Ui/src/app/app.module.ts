import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { MessageService } from '../services/message.service';
import { clientApiProviders } from '../generated-client/injection-tokens';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [
    MessageService,
    ...clientApiProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
