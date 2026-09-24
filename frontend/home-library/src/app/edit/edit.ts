import { Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthorDto, BookDto, ChapterDto } from '../apiClient';
import { MatChipsModule } from '@angular/material/chips';
import { BookService } from '../services/book-service';
import { MatDialog } from '@angular/material/dialog';
import { AuthorsModal } from '../authors-modal/authors-modal';
import { Observable, of } from 'rxjs';

@Component({
  imports: [MatCardModule, MatButtonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatChipsModule],
  selector: 'app-edit',
  styleUrl: './edit.scss',
  templateUrl: './edit.html',
})
export class Edit implements OnInit {
  private readonly router = inject(Router);

  private readonly bookService = inject(BookService);

  private readonly dialog = inject(MatDialog);

  private route = inject(ActivatedRoute);

  formGroup!: FormGroup;

  authors = signal<AuthorDto[]>([]);

  newAuthorEditing = signal(false);

  isInit = signal(false);
  
  get titleFormControl() { return this.formGroup!.get('titleFormControl')!; }
  get yearFormControl() { return this.formGroup!.get('yearFormControl')!; }
  get lastNameFormControl() { return this.formGroup!.get('lastNameFormControl')!; }
  get firstNameFormControl() { return this.formGroup!.get('firstNameFormControl')!; }
  get contentFormControl() { return this.formGroup!.get('contentFormControl')!; }

  bookId: number | undefined;

  ngOnInit(): void {
    this.bookId = Number(this.route.snapshot.paramMap.get('id'));

    this.init();
  }

  onSave() {
    if (this.formGroup?.valid && this.authors().length >0) {
      let dto = new BookDto();

      if (this.bookId) {
        dto.id = this.bookId;
      }
      
      dto.title = this.titleFormControl.value;
      dto.publishYear = this.yearFormControl.value;
      dto.authors = this.authors();
      
      const contentLines = this.contentFormControl.value
        .split(/\r?\n/)
        .map((line: string) => line.trim())
        .filter((line: string) => line.length > 0);
      
      let chapters: ChapterDto[] = [];

      contentLines.forEach((element: string) => {
        const chap = new ChapterDto();
        chap.name = element

        chapters.push(chap);
      });

      dto.chapters = chapters;

      this.bookService.saveBook(dto)
        .subscribe(result => {
          this.router.navigate(["/list"]);
        })
    }
  }

  onCancel() {
    this.router.navigate(['/list']);
  }

  onNewAuthor() {
    this.newAuthorEditing.set(true);
  }

  onSelectAuthor() {
    const dialogRef = this.dialog.open(AuthorsModal, {width: '800px'});

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.authors.update(list => [...list, result]);
      }
    });
  }

  onAddNewAuthor() {
    if (this.formGroup?.controls['lastNameFormControl'].value && this.formGroup?.controls['lastNameFormControl'].valid && 
      this.formGroup?.controls['firstNameFormControl'].value && this.formGroup?.controls['firstNameFormControl'].valid) {
        
        let author = new AuthorDto();
        author.firstName = this.formGroup?.controls['firstNameFormControl'].value;
        author.lastName = this.formGroup?.controls['lastNameFormControl'].value;

        let index: number = this.authors().findIndex(item => item.id === author.id && item.firstName === author.firstName && item.lastName === author.lastName);

        if (index === -1) {
          this.authors.update(list => [...list, author]);

          this.formGroup?.controls['lastNameFormControl'].setValue(null);
          this.formGroup?.controls['firstNameFormControl'].setValue(null);

          this.newAuthorEditing.set(false);
        }
    }
    
  }

  onCancelNewAuthor() {
    this.newAuthorEditing.set(false);
  }

  onDeleteAuthor(index: number) {
    this.authors.update(list => list.filter((_, i) => i !== index));
  }

  private loadBook(): Observable<BookDto> {
    
    let emptyBook = new BookDto();

    return this.bookId ? this.bookService.getById(this.bookId) : of(emptyBook);
  }

  private init() {
    this.loadBook()
      .subscribe(result => {

        let content:string = '';

        result.chapters?.forEach(item => {
          content += item.name + '\r\n';
        })

        this.formGroup = new FormGroup({
          titleFormControl: new FormControl(result?.title, [Validators.required, Validators.maxLength(255)]),
          yearFormControl: new FormControl(result?.publishYear, [Validators.required, Validators.min(1900), Validators.max(2026)]),
          lastNameFormControl: new FormControl(null, [Validators.maxLength(50)]),
          firstNameFormControl: new FormControl(null, [Validators.maxLength(50)]),
          contentFormControl:  new FormControl(content, [Validators.required]),
        })

        if (result.authors) {
          this.authors.update(list => result.authors!);
        }

      

      this.isInit.set(true);
    });
  }
}

