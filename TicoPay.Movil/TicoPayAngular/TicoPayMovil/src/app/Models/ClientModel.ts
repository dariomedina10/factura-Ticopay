export interface IClient {
    name: string; // Max 200 NN
    lastName:  string; // Max 80 NN
    identificationType: IdentificationType; // Int NN
    identification: string; // Max 50 NN
    identificacionExtranjero: string; // Max 20
    nameComercial: string; // Max 80
    code: number; // Int Identity NN
    birthday: string; // datetime
    gender: number; // Int NN
    phoneNumber: string; // Max 15
    mobilNumber: string; // Max 15
    fax: string; // Max 23
    email: string; // Max 100
    webSite: string; // Max 1000
    barrioId: number; // Int
    postalCode: string; // Max 15
    address: string; // Max 200
    contactName: string; // Max 80
    contactMobilNumber: string; // Max 23
    contactPhoneNumber: string; // Max 23
    contactEmail: string; // Max 60
    balance: number; // Decimal (18,2) NN
    note: string; // Max 250
    state: number; // Int NN
    creditDays: number; // Int
    id: string; // Unique Identifier NN
}

export enum IdentificationType {
    CedulaFisica = 0,
    CedulaJuridica = 1,
    DIMEX = 2,
    NITE = 3,
    NoAsiganda = 4,
}

export class Client implements IClient {
    public name: string; // Max 200 NN
    public lastName:  string; // Max 80 NN
    public identificationType: number; // Int NN
    public identification: string; // Max 50 NN
    public identificacionExtranjero: string; // Max 20
    public nameComercial: string; // Max 80
    public code: number; // Int Identity NN
    public birthday: string; // datetime
    public gender: number; // Int NN
    public phoneNumber: string; // Max 15
    public mobilNumber: string; // Max 15
    public fax: string; // Max 23
    public email: string; // Max 100
    public webSite: string; // Max 1000
    public barrioId: number; // Int
    public postalCode: string; // Max 15
    public address: string; // Max 200
    public contactName: string; // Max 80
    public contactMobilNumber: string; // Max 23
    public contactPhoneNumber: string; // Max 23
    public contactEmail: string; // Max 60
    public balance: number; // Decimal (18,2) NN
    public note: string; // Max 250
    public state: number; // Int NN
    public creditDays: number; // Int
    public id: string; // Unique Identifier NN
}
