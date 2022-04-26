import { IClient, IdentificationType } from './ClientModel';
import { IService, UnitMeasurement } from './ServiceModel';

export interface IInvoiceLines {
    IdService: string;
    IdProduct: string;
    Cantidad: number;
    Descuento: number;
    idImpuesto: string;
    Impuesto: number;
    Precio: number;
    Servicio: string;
    Total: number;
    Note: string;
    UnidadMedida: UnitMeasurement;
    UnidadMedidaOtra: string;
    Tipo: LineType;
    // TotalDescuento: number;
    // TotalImpuesto: number;
    // ID: number;
}

export class InvoiceLines {
    IdService: string;
    IdProduct: string;
    Cantidad: number;
    Descuento: number;
    idImpuesto: string;
    Impuesto: number;
    Precio: number;
    Servicio: string;
    Total: number;
    Note: string;
    UnidadMedida: UnitMeasurement;
    UnidadMedidaOtra: string;
    Tipo: LineType;
}

export class ListPaymentType {
    TypePayment: PaymentType;
    Trans: string; // Max 150
    Balance: number;
}

export interface IListPaymentType {
    TypePayment: PaymentType;
    Trans: string; // Max 150
    Balance: number;
}

export class Invoice {
    ClientId: string; // UniqueIdentifier
    ClientName: string;
    ClientIdentificationType: IdentificationType;
    ClientIdentification: string;
    ClientPhoneNumber: string;
    ClientMobilNumber: string;
    ClientEmail: string;
    ListPaymentType: ListPaymentType[];
    InvoiceLines: InvoiceLines[];
    DiscountGeneral: number; // Decimal
    TypeDiscountGeneral: DiscountType; // integer
    CreditTerm: number;
    TipoFirma: FirmType;
    CodigoMoneda: Moneda;
    TypeDocument: DocumentType;
}

export interface IInvoice {
    ClientId: string; // UniqueIdentifier
    ClientName: string;
    ClientIdentificationType: IdentificationType;
    ClientIdentification: string;
    ClientPhoneNumber: string;
    ClientMobilNumber: string;
    ClientEmail: string;
    ListPaymentType: IListPaymentType[];
    InvoiceLines: IInvoiceLines[];
    DiscountGeneral: number; // Decimal
    TypeDiscountGeneral: DiscountType; // integer
    CreditTerm: number;
    TipoFirma: FirmType;
    CodigoMoneda: Moneda;
    TypeDocument: DocumentType;
}

export enum DiscountType {
    Percentage = 1,
    Amount = 2,
}

export enum PaymentType {
    Cash = 0,
    CreditCard = 1,
    Check = 2,
    Deposit = 3,
}

export class InvoiceSearchConfiguration {
    StartDueDate: string;
    EndDueDate: string;
    ClientId: string;
    InvoiceId: string;
    Status: InvoiceStatus;
}

export enum InvoiceStatus {
    Pagada = 0,
    Provisional = 1,
    Contabilizada = 2,
    Reversada = 3,
    Pendiente = 4,
    Anulada = 5,
}

export enum ConditionSaleType {
    Contado = 1,
    Credito = 2,
    Consignacion = 3,
    Apartado = 4,
    ArrendamientoOpcionDeCompra = 5,
    ArrendamientoFuncionFinanciera = 6,
    Otros = 99,
}

export interface ICompleteInvoicePaymentType {
    amount: number; // Decimal (18.2) NN
    paymentDate: Date; // Datetime NN
    invoiceId: string; // Unique Identifier NN
    exchangeRateId: string; // UniqueIdentifier
    exchangeRate: number; // Sin saber que es
    codigoMoneda: number; // Int NN
    paymentInvoiceType: number; // int NN
    paymetnMethodType: PaymentType; // int NN
    transaction: string; // Nvarchar
    reference: string; // Nvarchar
}

export interface ICompleteInvoiceLines {
    tenantId: number;  // int NN
    pricePerUnit: number; // Decimal (18.5) NN
    taxAmount: number; // Decimal (18.5) NN
    total: number; // Decimal (18.5) NN
    discountPercentage: number; // Decimal (18.5) NN
    note: string; // Max 200
    title: string; // Max 50
    quantity: number; // Decimal (16.3) NN
    invoiceId: string; // Unique Identifier NN
    lineType: number; // int NN ?
    serviceId: string; // Unique Identifier
    productId: string; // Unique Identifier
    lineNumber: number; // int NN
    codeTypes: number; // int NN
    descriptionDiscount: string; // max 20
    subTotal: number; // Decimal (18.5) NN
    lineTotal: number; // Decimal (18.5) NN
    service: IService;
}

export interface ICompleteInvoice {
    tenantName: string;
    comercialName: string;
    clientId: string;
    client: IClient;
    ClientName: string;
    ClientIdentificationType: IdentificationType;
    ClientIdentification: string;
    ClientPhoneNumber: string;
    ClientMobilNumber: string;
    ClientEmail: string;
    number: number; // Int NN
    alphanumeric: string; // Max 50
    note: string; // Max 500
    consecutiveNumber: string; // Max 20
    qrCode: any;
    subTotal: number; // Decimal (18.5) NN
    discountPercentaje: number; // Decimal (18.2) NN
    paymentDate: any; // not from invoice
    transaction: any;
    discountAmount: number; // Decimal (18.5) NN
    totalTax: number; // Decimal (18.5) NN
    total: number; // Decimal (18.5) NN
    balance: number; // Decimal (18.2) NN
    dueDate: Date; // Datetime NN
    status: InvoiceStatus; // int NN
    invoiceLines: ICompleteInvoiceLines[];
    invoicePaymentTypes: ICompleteInvoicePaymentType[];
    notes: any;
    creditTerm: number;
    expirationDate: Date;
    ConditionSaleType: ConditionSaleType;
    tipoFirma: FirmType;
    codigoMoneda: Moneda;
    simboloMoneda: string;
    typeDocument: DocumentType;
    voucherKey: string;
    id: string; // Unique Identifier NN
}

export enum FirmType {
    Llave = 0,
    /// Llave Criptográfica
    Firma,
    /// Firma Digital
    Todos
    /// Ambas (Solo aplica para la configuración del Tenant)
}

export enum Moneda {
    /// Colones
    CRC = 36,
    /// Dolares
    USD = 148,
}

export enum LineType {
    Service,
    Product
}

export enum DocumentType {
    Invoice = 1,
    Ticket = 4
}

