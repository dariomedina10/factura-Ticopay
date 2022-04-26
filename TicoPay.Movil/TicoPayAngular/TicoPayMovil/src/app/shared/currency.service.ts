import { Injectable } from '@angular/core';

import * as currencies from '../../assets/Currencies.json';
import { Currencies } from '../Models/Authentificatiomodel';

@Injectable()
export class CurrencyService {

  private _currencies: Currencies[];
  constructor() { }

  public getCurrencyName(currencyNumber: number): string {
    this._currencies = (<any>currencies).currencies;
    const currencyN = currencyNumber.toString().padStart(3, '0');
    for (const currency of this._currencies) {
      if (currency.number === currencyN) {
        return currency.code;
      }
    }
    return null;
  }

}
