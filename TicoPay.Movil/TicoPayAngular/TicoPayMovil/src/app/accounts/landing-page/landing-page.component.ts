import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../shared/dataServices/auth-service.service';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent implements OnInit {

  private _error: string;
  private _comments: string;

  constructor(private _router: Router, private route: ActivatedRoute, private authService: AuthService) { }

  private redirection() {
    this.authService.logout();
    const currentTenant = JSON.parse(localStorage.getItem('tenancyName'));
    this._router.navigateByUrl('/login?tenancyName=' + currentTenant.tenancyName);
  }

  ngOnInit() {
    this._comments = 'Si el problema persiste contacte a nuestro equipo de soporte';
    this.route.params.subscribe(params => {
      const error = params.error;
      if (error) {
        this._error = error;
      }
    });
  }

}
