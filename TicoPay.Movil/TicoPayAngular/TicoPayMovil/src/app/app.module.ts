import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { Routes, RouterModule} from '@angular/router';
import { AuthGuardService } from './shared/guardServices/auth-guard.service';
import { LoginComponent } from './accounts/login/login.component';
import {HomeComponent} from './accounts/home/home.component';
import { FormsModule } from '@angular/forms';
import { AuthService } from './shared/dataServices/auth-service.service';
import { HttpModule } from '@angular/http';
import { InvoicesModule } from './Invoices/invoices.module';
import { ClientsModule } from './clients/clients.module';
import { ServicesModule } from './services/services.module';
import { SharedModule } from './shared/shared.module';
import { LandingPageComponent } from './accounts/landing-page/landing-page.component';
import { ProductsModule } from './products/products.module';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    LandingPageComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    HttpModule,
    SharedModule,
    RouterModule.forRoot([
    {path: 'home', canActivate: [AuthGuardService], component : HomeComponent},
    {path: 'login', component : LoginComponent},
    {path: 'landing/:error', component : LandingPageComponent},
    {path: '', redirectTo: 'login', pathMatch: 'full'},
    {path: '**', redirectTo: 'home', pathMatch: 'full'}
    ],
    // { enableTracing: true }
    ),
    InvoicesModule,
    ClientsModule,
    ServicesModule,
    ProductsModule
  ],
  exports: [
    RouterModule
  ],
  providers: [AuthGuardService, AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
