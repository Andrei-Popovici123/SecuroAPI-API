import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Scans } from './scans';

describe('Scans', () => {
  let component: Scans;
  let fixture: ComponentFixture<Scans>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Scans],
    }).compileComponents();

    fixture = TestBed.createComponent(Scans);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
