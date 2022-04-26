import { TestBed, inject } from '@angular/core/testing';

import { TaxesService } from './taxes.service';

describe('TaxesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TaxesService]
    });
  });

  it('should be created', inject([TaxesService], (service: TaxesService) => {
    expect(service).toBeTruthy();
  }));
});
