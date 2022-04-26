export interface IServiceTax {
    name: string; // Nvarchar NN
    rate: number; // Decimal (4,2) NN
    id: string; // UniqueIdentifier NN
}

export interface ITaxes {
    name: string;
    rate: number;
    taxTypes: number;
    id: string;
}
