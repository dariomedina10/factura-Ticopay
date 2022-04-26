import { IdentificationType } from "./ClientModel";
import { UnitMeasurement } from "./ServiceModel";
import { FirmType } from "./InvoiceModel";

export class UserCredentials {
  constructor(
    public userName: string,
    public token: string
  ) { }
}

export class BodyInformation {
  constructor(
    public tenancyName: string,
    public usernameOrEmailAddress: string,
    public password: string
  ) { }
}

export class RefreshCredentials {
    constructor(
      public Token: string,
      public AdditionalTimeType: TimeLapsType,
      public AdditionalTimeAmount: number
    ) {}
}

export enum TimeLapsType {
    Minutes = 0,
    Hours = 1,
    Days = 2,
}

export class Login {
  constructor(
    public userName: string,
    public password: string
  ) {
  }
}

export interface ITenant {
  id: number;
  tenancyName: string;
  name: string;
  bussinesName: string;
  comercialName: string;
  identificationType: IdentificationType;
  identificationNumber: string;
  phoneNumber: string;
  fax: string;
  email: string;
  alternativeEmail: string;
  codigoMoneda: number;
  currencyCode: string;
  barrioId: number;
  address: string;
  countryID: number;
  editionId: number;
  registerID: string;
  lastInvoiceNumber: number;
  lastNoteDebitNumber: number;
  lastNoteCreditNumber: number;
  unitMeasurementDefault: UnitMeasurement;
  unitMeasurementOthersDefault: string;
  tipoFirma: FirmType;
  isPos: boolean;
}

export class Currencies {
  code: string;
  number: string;
  digits: number;
  currency: string;
  countries: string[];
}
