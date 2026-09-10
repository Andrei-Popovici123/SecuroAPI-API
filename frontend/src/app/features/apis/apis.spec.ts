import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Apis } from './apis';

describe('Apis', () => {
  let component: Apis;
  let fixture: ComponentFixture<Apis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Apis],
    }).compileComponents();

    fixture = TestBed.createComponent(Apis);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
