import { ComponentFixture, TestBed } from '@angular/core/testing';

import { sidebarComponent } from './sidebar.component';

describe('sidebarComponent', () => {
  let component: sidebarComponent;
  let fixture: ComponentFixture<sidebarComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [sidebarComponent]
    });
    fixture = TestBed.createComponent(sidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
