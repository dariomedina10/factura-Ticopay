import { IServiceTax } from './TaxesModel';
import { UnitMeasurement } from './ServiceModel';

export interface IProduct {
    name: string;
    note: string;
    retailPrice: number;
    supplyPrice: number;
    estado: Estatus;
    markup: number;
    totalInStock: number;
    taxId: string;
    unitMeasurement: UnitMeasurement;
    tax: IServiceTax;
    supplierCode: string;
    salesAccountCode: string;
    canBeSold: boolean;
    id: string;
}

export class Product {
    name: string;
    note: string;
    retailPrice: number;
    supplyPrice: number;
    estado: Estatus;
    markup: number;
    totalInStock: number;
    taxId: string;
    unitMeasurement: UnitMeasurement;
    tax: IServiceTax;
    supplierCode: string;
    salesAccountCode: string;
    canBeSold: boolean;
    id: string;
}

export enum Estatus {
    Activo,
    Inactivo
}
