import { Component, inject, OnInit } from '@angular/core';
import { AuthorService } from '../services/author-service';
import { AuthorDto } from '../apiClient';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  imports: [MatTableModule, MatButtonModule, MatDialogModule],
  selector: 'app-authors-modal',
  styleUrl: './authors-modal.scss',
  templateUrl: './authors-modal.html',
})
export class AuthorsModal implements OnInit {
  displayedColumns =  [
    'id',
    'lastName',
    'firstName'
  ]
  
  dataSource = new MatTableDataSource<AuthorDto>([]);
  
  private readonly authorService = inject(AuthorService);
  private readonly dialogRef = inject(MatDialogRef<AuthorsModal>)


  ngOnInit(): void {
    this.loadData();
  }

  onSelectAuthor(author: AuthorDto) {
    this.dialogRef.close(author);
  }
  
  private loadData() {
    this.authorService.getAll()
      .subscribe(result => {
        this.dataSource.data = result;
      })
  }
}
