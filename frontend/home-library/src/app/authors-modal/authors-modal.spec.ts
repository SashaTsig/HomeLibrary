import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthorsModal } from './authors-modal';

describe('AuthorsModal', () => {
  let component: AuthorsModal;
  let fixture: ComponentFixture<AuthorsModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthorsModal],
    }).compileComponents();

    fixture = TestBed.createComponent(AuthorsModal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
