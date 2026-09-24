import { Component, OnInit, inject  } from '@angular/core';
import { BookDto } from './apiClient';
import { BookService } from './services/book-service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  imports: [ MatTableModule, MatButtonModule, MatCardModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App  implements OnInit {
  displayedColumns =  [
    'id',
    'title',
    'year',
    'authors',
    'content',
    'actions'
  ]
  dataSource = new MatTableDataSource<BookDto>([]);

  private readonly bookService = inject(BookService);
  private readonly router = inject(Router);
  
  formGroup: FormGroup = new FormGroup({
          titleFormControl: new FormControl(null, [Validators.maxLength(255)]),
          authorFormControl: new FormControl(null, [Validators.maxLength(50)]),
          contentFormControl:  new FormControl(),
  })

    
  get titleFormControl() { return this.formGroup!.get('titleFormControl')!; }
  get authorFormControl() { return this.formGroup!.get('authorFormControl')!; }
  get contentControl () { return this.formGroup.get('contentFormControl')! };


  ngOnInit(): void {
    this.loadBooks(undefined, undefined, undefined);
  }
  

  onEdit(book: BookDto) {
    this.router.navigate(['/edit', book.id]);
  }

  onSearch() {
    let title: string | undefined = undefined;
    let author: string | undefined = undefined;
    let content: string | undefined = undefined;

    if (this.titleFormControl.valid && this.titleFormControl.value) {
      title = this.titleFormControl.value;
    }

    if (this.authorFormControl.valid && this.authorFormControl.value) {
      author = this.authorFormControl.value;
    }

    

    if (this.contentControl?.value) {
      content = this.contentControl.value;
    }

    this.loadBooks(title, author, content);
  }

  onNewBook() {
    this.router.navigate(['/edit']);
  }

  onClearFilters() {
    this.titleFormControl.setValue(null);
    this.authorFormControl.setValue(null);
    this.contentControl.setValue(null);
  }

  private loadBooks(title: string | undefined, author: string | undefined, content: string | undefined) {
    
    this.bookService.findBooks(title, author, content)
      .subscribe(result => {
        this.dataSource.data = result;
      })
  }
}
