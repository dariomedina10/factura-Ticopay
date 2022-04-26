import { IServiceTax } from './TaxesModel';

export interface IService {
    name: string; // Max 160
    price: number; // Decimal (18,2) NN
    taxId:  string; // Unique Identifier
    tax: IServiceTax; // small Tax entity from Taxes table
    unitMeasurement: number; // int NN
    unitMeasurementOthers: string; // Nvarchar
    isRecurrent: boolean; // bit NN
    cronExpression: string; // Nvarchar
    id: string; // UniqueIdentifier NN
    quantity: number; // Decimal 1 , 99999
}

export enum UnitMeasurement {
    ServiciosProfesionales = 0,
    Metro,
    Kilogramo,
    Segundo,
    Amperio,
    GradosKelvin,
    Moles,
    Candela,
    MetroCuadrado,
    MetroCubico,
    MetrosXSegundo,
    MetrosXSegundoCuadrado,
    NumeroOndas,
    KilogramoMetroCubico,
    AmperioXMetro,
    AmperioXMetroCuadrado,
    MolXMetroCubico,
    CandelaXMetroCuadrado,
    Uno,
    Radian,
    EsteRadian,
    Hercio,
    Newton,
    Pascal,
    Joule,
    Vatios,
    Coulomb,
    Voltios,
    Faradios,
    Ohmios,
    Siemens,
    Weber,
    Tesla,
    Henry,
    GradosCentigrados,
    Lumen,
    Lux,
    Becquere,
    Gray,
    Sievert,
    Katal,
    PascalSegundo,
    NewtonMetro,
    NewtonXMetro,
    RadianXSegundo,
    RadianXSegundoCuadrado,
    VatiosXMetroCuadrado,
    JouleXKelvin,
    JouleXKilogramoKelvin,
    JouleXKilogramo,
    VatioXMetroKelvin,
    JouleXMetroCubico,
    VoltioXMetro,
    CoulombXMetroCubico,
    CoulombXMetroCuadrado,
    FaradioXMetro,
    HenryXMetro,
    JouleXMol,
    JouleXMolKelvin,
    CoulombXKilogramo,
    GrayXSegundo,
    VatioXEsteRadian,
    VatioXEsteRadianMetroCuadrado,
    KatalXMetroCubico,
    Minuto,
    Hora,
    Dia,
    AnguloGrado,
    AnguloMinuto,
    AnguloSegundo,
    Litro,
    Tonelada,
    Decibel,
    Barn,
    ElectronVoltio,
    UnidadMasaAtomica,
    UnidadAstronomica,
    Unidad,
    Gal,
    Gramos,
    Kilometro,
    Pulgada,
    Centimetro,
    MiliLitro,
    Milimetro,
    Onza,
    Otros,
}

export class Service {
    name: string; // Max 160
    price: number; // Decimal (18,2) NN
    taxId:  string; // Unique Identifier
    unitMeasurement: UnitMeasurement; // int NN
    unitMeasurementOthers: string; // Nvarchar
    isRecurrent: boolean; // bit NN
    cronExpression: string; // Nvarchar
    id: string; // UniqueIdentifier NN
    quantity: number;
}
