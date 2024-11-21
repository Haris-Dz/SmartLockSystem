import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {RouterModule, RouterOutlet} from "@angular/router";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import {LocationStrategy, HashLocationStrategy, NgOptimizedImage} from '@angular/common';
import { HomeComponent } from './components/home/home.component';
import { LockLogsComponent } from './components/lock-logs/lock-logs.component';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LockLogsComponent,

  ],
  imports: [
    BrowserModule,
    FormsModule,

    HttpClientModule,
    RouterOutlet,
    RouterModule.forRoot([
      {path: 'home', component: HomeComponent},
      {path: '', redirectTo: '/home', pathMatch: 'full'},
      {path: 'logs', component: LockLogsComponent},
    ]),
    NgbModule,
    ReactiveFormsModule,
    NgOptimizedImage
  ],
  providers: [{provide: LocationStrategy, useClass: HashLocationStrategy}],
  bootstrap: [AppComponent]
})
export class AppModule { }
