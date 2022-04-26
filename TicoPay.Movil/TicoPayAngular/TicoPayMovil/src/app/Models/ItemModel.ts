import { IServiceTax } from './TaxesModel';
import { UnitMeasurement } from './ServiceModel';

export interface IItem {
    Id: string;
    Name: string;
    TaxId: string;
    Tax: IServiceTax;
    Price: number;
    UnidadMedidaType: UnitMeasurement;
    UnidadMedidaOtra: string;
    Type: ItemType;
}

export class Item {
    Id: string;
    Name: string;
    TaxId: string;
    Tax: IServiceTax;
    Price: number;
    UnidadMedidaType: UnitMeasurement;
    UnidadMedidaOtra: string;
    Type: ItemType;
}

export enum ItemType {
    Producto,
    Servicio,
}
